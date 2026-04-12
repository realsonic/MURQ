using MURQ.Common.Exceptions;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.URQL;
using MURQ.Domain.URQL.Interpretation;
using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Locations;

using System.Text;
using System.Text.RegularExpressions;

namespace MURQ.Domain.Games;

public class Game(Quest quest) : IGameContext
{
    public event EventHandler<OnTextPrintedEventArgs>? OnTextPrinted;

    public event Action? OnScreenCleared;

    public event EventHandler<OnErrorEventArgs>? OnUrqlError;

    public Quest Quest { get; } = quest;

    public CurrentLocationView CurrentLocation => new()
    {
        Name = currentLocationName,
        Text = currentLocationText.ToString(),
        Buttons = currentScreenButtons
    };

    /// <summary>
    /// Цвета текста и фона кнопок. Работает через переменную <c>style_dos_buttoncolor</c>.
    /// </summary>
    public (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) ButtonColors => GetDosButtonColor();

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        ClearCurrentView();
        await InterpretQuestLinesAsync(cancellationToken);
    }

    private async Task InterpretQuestLinesAsync(CancellationToken cancellationToken)
    {
        gameState = GameState.RunningStatements;

        while (gameState == GameState.RunningStatements && !cancellationToken.IsCancellationRequested)
        {
            await InterpretCurrentQuestLineAsync(cancellationToken);
            Quest.NextLine();
        }
    }

    private async Task InterpretCurrentQuestLineAsync(CancellationToken cancellationToken)
    {
        if (Quest.CurrentLine is null)
        {
            gameState = GameState.WaitingUserInput;
            return;
        }

        QuestLine questLine = Quest.CurrentLine;
        if (questLine is not CodeLine codeLine)
        {
            return;
        }

        IEnumerable<OriginatedCharacter> code = codeLine.ToCode(this);
        UrqlLexer lexer = new(code);
        UrqlInterpreter interpreter = new(lexer, this);

        try
        {
            await interpreter.InterpretStatementLineAsync(cancellationToken);
        }
        catch (UrqlException ex)
        {
            ReportCodeLineError(codeLine, ex);
        }
    }

    /// <inheritdoc/>
    void IGameContext.Print(string? text)
    {
        currentLocationText.Append(text);
        (InterfaceColor foregroundColor, InterfaceColor backgroundColor) = GetDosTextColor();
        OnTextPrinted?.Invoke(this, new OnTextPrintedEventArgs(text, false, foregroundColor, backgroundColor));
    }

    /// <inheritdoc/>
    void IGameContext.PrintLine(string? text)
    {
        currentLocationText.AppendLine(text);
        (InterfaceColor foregroundColor, InterfaceColor backgroundColor) = GetDosTextColor();
        OnTextPrinted?.Invoke(this, new OnTextPrintedEventArgs(text, true, foregroundColor, backgroundColor));
    }

    /// <inheritdoc/>
    void IGameContext.AddButton(string caption, string label) => currentScreenButtons.Add(new Button
    {
        Caption = caption,
        IsPhantom = Quest.IsLabelPhantom(label),
        OnButtonPressedAsync = (cancellationToken) => JumpByButtonAsync(label, cancellationToken)
    });

    /// <inheritdoc/>
    void IGameContext.EndLocation()
    {
        Quest.ClearCurrentLine();
        gameState = GameState.WaitingUserInput;
    }

    /// <inheritdoc/>
    void IGameContext.ClearScreen()
    {
        ClearCurrentView();
        OnScreenCleared?.Invoke();
    }

    /// <inheritdoc/>
    void IGameContext.AssignVariable(string name, Value value)
    {
        switch (name.ToLowerInvariant())
        {
            case StyleDosTextColorVarName or StyleDosButtonColorVarName:
                if (value is NumberValue numberValue)
                    _userVariables[name] = numberValue;
                break;

            default:
                _userVariables[name] = value;
                break;
        }
    }

    /// <inheritdoc/>
    public Value? GetVariableValue(string variableName) => variableName.ToLowerInvariant() switch
    {
        "current_loc" => new StringValue(currentLocationName ?? string.Empty),
        "previous_loc" => new StringValue(previousLocationName ?? string.Empty),
        StyleDosTextColorVarName => GetUserVariableValue(variableName) ?? new NumberValue(DefaultStyleDosTextColor),
        StyleDosButtonColorVarName => GetUserVariableValue(variableName) ?? new NumberValue(DefaultStyleDosButtonColor),
        _ when rndRegex.IsMatch(variableName) => GetRandom(variableName),
        _ => GetUserVariableValue(variableName)
    };

    /// <inheritdoc/>
    void IGameContext.Goto(string label) => JumpByGoto(label);

    /// <inheritdoc/>
    void IGameContext.Perkill() => _userVariables.Clear();

    private async Task JumpByButtonAsync(string label, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(label))
            return;

        ClearCurrentView();

        if (Quest.TryGoToLabel(label, out string? resultLabel))
        {
            ChangeCurrentLocationName(resultLabel);
            await InterpretQuestLinesAsync(cancellationToken);
        }
    }

    private void JumpByGoto(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return;

        Quest.TryGoToLabel(label, out _);
    }

    private void ClearCurrentView()
    {
        currentLocationText.Clear();
        currentScreenButtons.Clear();
    }

    private void ChangeCurrentLocationName(string name)
    {
        previousLocationName = currentLocationName;
        currentLocationName = name;
    }

    private (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) GetDosTextColor()
    {
        Value colorValue = GetUserVariableValue(StyleDosTextColorVarName) ?? new NumberValue(DefaultStyleDosTextColor);
        NumberValue numberValue = colorValue as NumberValue ?? throw new MurqException($"Системная переменная {StyleDosTextColorVarName} не числового типа.");

        return ExtractColorsFromNumber(numberValue);
    }

    private (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) GetDosButtonColor()
    {
        Value colorValue = GetUserVariableValue(StyleDosButtonColorVarName) ?? new NumberValue(DefaultStyleDosButtonColor);
        NumberValue numberValue = colorValue as NumberValue ?? throw new MurqException($"Системная переменная {StyleDosButtonColorVarName} не числового типа.");

        return ExtractColorsFromNumber(numberValue);
    }

    private static (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) ExtractColorsFromNumber(NumberValue numberValue)
    {
        decimal value = numberValue.AsDecimal;

        byte colorCode = value switch
        {
            > byte.MaxValue => 128,
            < byte.MinValue => 128,
            _ => (byte)value
        };

        return DecodeDosTextColor(colorCode);
    }

    private static (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) DecodeDosTextColor(byte value)
    {
        byte foreground = (byte)((value & 0x0F));
        byte background = (byte)((value & 0xF0) >> 0x4);

        return (ForegroundColor: (InterfaceColor)foreground, BackgroundColor: (InterfaceColor)background);
    }

    private NumberValue GetRandom(string variableName)
    {
        string rndLimitString = rndRegex.Match(variableName).Groups["number"].Value;

        if (rndLimitString == string.Empty)
        {
            float randomFloatNumber = random.NextSingle();
            return new NumberValue(Convert.ToDecimal(randomFloatNumber));
        }

        int rndLimit = Convert.ToInt32(rndLimitString);
        int randomIntNumber = random.Next(1, rndLimit + 1);

        return new NumberValue(randomIntNumber);
    }

    private Value? GetUserVariableValue(string variableName) => _userVariables.TryGetValue(variableName, out Value? value) ? value : null;

    private void ReportCodeLineError(CodeLine codeLine, UrqlException ex) => OnUrqlError?.Invoke(this, new OnErrorEventArgs(ex, codeLine.Location));

    private bool IsStarted => gameState is not GameState.InitialState;

    public class CurrentLocationView
    {
        public string? Name { get; init; }
        public string? Text { get; init; }
        public IReadOnlyList<Button> Buttons { get; init; } = [];
    }

    public class Button
    {
        public required string Caption { get; init; }
        public required bool IsPhantom { get; init; }
        public required Func<CancellationToken, Task> OnButtonPressedAsync { get; init; }

        public async Task PressAsync(CancellationToken cancellationToken = default) => await OnButtonPressedAsync(cancellationToken);
    }

    private GameState gameState = GameState.InitialState;
    private readonly StringBuilder currentLocationText = new();
    private readonly List<Button> currentScreenButtons = [];
    private string? currentLocationName;
    private string? previousLocationName;
    private readonly Dictionary<string, Value> _userVariables = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Regex rndRegex = new(@"^rnd(?<number>\d*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly Random random = new();

    private const string StyleDosTextColorVarName = "style_dos_textcolor";
    private const int DefaultStyleDosTextColor = 7;
    private const string StyleDosButtonColorVarName = "style_dos_buttoncolor";
    private const int DefaultStyleDosButtonColor = 15;

    public class OnTextPrintedEventArgs(string? text, bool isNewLineAtEnd, InterfaceColor foregroundColor, InterfaceColor backgroundColor) : EventArgs
    {
        public string? Text { get; } = text;
        public bool IsNewLineAtEnd { get; } = isNewLineAtEnd;
        public InterfaceColor ForegroundColor { get; } = foregroundColor;
        public InterfaceColor BackgroundColor { get; } = backgroundColor;
    }

    public class OnErrorEventArgs(Exception exception, Location location) : EventArgs
    {
        public Exception Exception { get; } = exception;
        public Location Location { get; } = location;
    }
}
using MURQ.Common.Exceptions;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
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
    /// Цвет текста. Работает через переменную <c>style_dos_textcolor</c>.
    /// </summary>
    public InterfaceColor TextForegroundColor => ExtractColorsFromVariable(StyleDosTextColorVarName).ForegroundColor;

    /// <summary>
    /// Цвет фона текста. Работает через переменную <c>style_dos_textcolor</c>.
    /// </summary>
    public InterfaceColor TextBackgroundColor => ExtractColorsFromVariable(StyleDosTextColorVarName).BackgroundColor;

    /// <summary>
    /// Цвет текста кнопок. Работает через переменную <c>style_dos_buttoncolor</c>.
    /// </summary>
    public InterfaceColor ButtonForegroundColor => ExtractColorsFromVariable(StyleDosButtonColorVarName).ForegroundColor;

    /// <summary>
    /// Цвет фона кнопок. Работает через переменную <c>style_dos_buttoncolor</c>.
    /// </summary>
    public InterfaceColor ButtonBackgroundColor => ExtractColorsFromVariable(StyleDosButtonColorVarName).BackgroundColor;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        SeedSystemVariables(); //todo заменить не фоллбеки
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
        OnTextPrinted?.Invoke(this, new OnTextPrintedEventArgs(text, false, TextForegroundColor, TextBackgroundColor));
    }

    /// <inheritdoc/>
    void IGameContext.PrintLine(string? text)
    {
        currentLocationText.AppendLine(text);
        OnTextPrinted?.Invoke(this, new OnTextPrintedEventArgs(text, true, TextForegroundColor, TextBackgroundColor));
    }

    /// <inheritdoc/>
    void IGameContext.AddButton(string caption, string label) => currentScreenButtons.Add(new Button
    {
        Caption = caption,
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

    public void AssignVariable(string name, Value value)
    {
        if (TryAssignSystemVariable(name, value)) return;

        _variables[name] = new Variable(name, value);
    }

    public Variable? GetVariable(string variableName) => GetPseudoVariable(variableName) ?? GetTrueVariable(variableName);

    void IGameContext.Goto(string label) => JumpByGoto(label);

    void IGameContext.Perkill()
    {
        _variables.Clear();
        SeedSystemVariables();
    }

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

    private void SeedSystemVariables()
    {
        _variables[StyleDosTextColorVarName] = new Variable(StyleDosTextColorVarName, new NumberValue(7));
        _variables[StyleDosButtonColorVarName] = new Variable(StyleDosButtonColorVarName, new NumberValue(15));
    }

    private Variable? GetPseudoVariable(string name) => name.ToLower() switch
    {
        CurrentLocVarName => new Variable(CurrentLocVarName, new StringValue(currentLocationName ?? string.Empty)),
        PreviousLocVarName => new Variable(PreviousLocVarName, new StringValue(previousLocationName ?? string.Empty)),
        _ when rndRegex.IsMatch(name) => GetRandom(name),
        _ => null
    };

    private Variable? GetTrueVariable(string variableName) => _variables.TryGetValue(variableName, out Variable? variable) ? variable : null;

    private bool TryAssignSystemVariable(string name, Value value)
    {
        switch (name.ToLower())
        {
            case StyleDosTextColorVarName or StyleDosButtonColorVarName:
                if (value is NumberValue numberValue)
                    _variables[name] = new Variable(name, numberValue);
                return true;

            default:
                return false;
        }
    }

    private (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) ExtractColorsFromVariable(string variableName)
    {
        Variable colorVariable = GetTrueVariable(variableName) ?? throw new MurqException($"Системная переменная {variableName} не задана.");
        NumberValue numberValue = colorVariable.Value as NumberValue ?? throw new MurqException($"Системная переменная {variableName} не числового типа.");
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

    private Variable GetRandom(string variableName)
    {
        string rndLimitString = rndRegex.Match(variableName).Groups["number"].Value;

        if (rndLimitString == string.Empty)
        {
            float randomFloatNumber = random.NextSingle();
            return new Variable(variableName, new NumberValue(Convert.ToDecimal(randomFloatNumber)));
        }

        int rndLimit = Convert.ToInt32(rndLimitString);
        int randomIntNumber = random.Next(1, rndLimit + 1);
        return new Variable(variableName, new NumberValue(randomIntNumber));
    }

    private void ReportCodeLineError(CodeLine codeLine, UrqlException ex)
    {
        OnUrqlError?.Invoke(this, new OnErrorEventArgs(ex, codeLine.Location));
    }

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
        public required Func<CancellationToken, Task> OnButtonPressedAsync { get; init; }

        public async Task PressAsync(CancellationToken cancellationToken = default) => await OnButtonPressedAsync(cancellationToken);
    }

    private GameState gameState = GameState.InitialState;
    private readonly StringBuilder currentLocationText = new();
    private readonly List<Button> currentScreenButtons = [];
    private string? currentLocationName;
    private string? previousLocationName;
    private readonly Dictionary<string, Variable> _variables = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Regex rndRegex = new(@"^rnd(?<number>\d*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly Random random = new();

    private const string CurrentLocVarName = "current_loc";
    private const string PreviousLocVarName = "previous_loc";
    private const string StyleDosTextColorVarName = "style_dos_textcolor";
    private const string StyleDosButtonColorVarName = "style_dos_buttoncolor";

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
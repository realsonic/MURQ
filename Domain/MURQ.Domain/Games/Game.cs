using MURQ.Common.Exceptions;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

using System.Text;
using System.Text.RegularExpressions;

namespace MURQ.Domain.Games;

public class Game(Quest quest) : IGameContext
{
    public event EventHandler<OnTextPrintedEventArgs>? OnTextPrinted;

    public event Action? OnScreenCleared;

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

        SeedSystemVariables();
        ClearCurrentView();
        SetNextStatementToStarting();
        await RunStatementsAsync(cancellationToken);
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
    void IGameContext.AddButton(string caption, LabelStatement? labelStatement) => currentScreenButtons.Add(new Button
    {
        Caption = caption,
        OnButtonPressedAsync = (cancellationToken) => JumpByButtonAsync(labelStatement, cancellationToken)
    });

    /// <inheritdoc/>
    void IGameContext.EndLocation() => SetModeWaitingUserInput();

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

    void IGameContext.Goto(LabelStatement? labelStatement) => JumpByGoto(labelStatement);

    void IGameContext.Perkill() => _variables.Clear();

    private async Task JumpByButtonAsync(LabelStatement? labelStatement, CancellationToken cancellationToken)
    {
        if (labelStatement is null) return;

        ClearCurrentView();
        SetCurrentLabel(labelStatement);
        UpdateCurrentLocationName(labelStatement.Label);

        await RunStatementsAsync(cancellationToken);
    }

    private void JumpByGoto(LabelStatement? labelStatement)
    {
        if (labelStatement is null) return;

        SetCurrentLabel(labelStatement);
    }

    private void ClearCurrentView()
    {
        currentLocationText.Clear();
        currentScreenButtons.Clear();
    }

    private void UpdateCurrentLocationName(string name)
    {
        previousLocationName = currentLocationName;
        currentLocationName = name;
    }

    private async Task RunStatementsAsync(CancellationToken cancellationToken)
    {
        SetModeRunningStatements();
        while (IsRunningStatements && !cancellationToken.IsCancellationRequested)
        {
            await RunCurrentStatementAsync(cancellationToken);
            PromoteNextStatement();
        }
    }

    private async Task RunCurrentStatementAsync(CancellationToken cancellationToken)
    {
        if (currentStatement is null)
        {
            SetModeWaitingUserInput();
            return;
        }

        await currentStatement.RunAsync(this, cancellationToken);
    }

    private void SetCurrentLabel(LabelStatement? labelStatement)
    {
        if (labelStatement is not null)
        {
            SetCurrentStatement(labelStatement);
        }
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
        Variable colorVariable = _variables[variableName] ?? throw new MurqException($"Системная переменная {variableName} не задана.");
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

    private void PromoteNextStatement() => currentStatement = Quest.GetNextStatement(currentStatement);

    private void SetNextStatementToStarting() => currentStatement = Quest.StartingStatement;
    private void SetModeRunningStatements() => gameState = GameState.RunningStatements;
    private void SetModeWaitingUserInput() => gameState = GameState.WaitingUserInput;
    private void SetCurrentStatement(Statement statement) => currentStatement = statement;

    private bool IsStarted => gameState is not GameState.InitialState;
    private bool IsRunningStatements => gameState == GameState.RunningStatements;

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
    private Statement? currentStatement;
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
}
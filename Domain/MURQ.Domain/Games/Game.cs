using MURQ.Common.Exceptions;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

using System.Text.RegularExpressions;

namespace MURQ.Domain.Games;

public class Game(Quest quest) : IGameContext
{
    public event Action<string, InterfaceColor, InterfaceColor>? OnTextPrinted;
    public event Action? OnScreenCleared;

    public Quest Quest { get; } = quest;

    public CurrentLocationView CurrentLocation => new()
    {
        Name = _currentLocationName,
        Buttons = _currentScreenButtons
    };

    /// <summary>
    /// Цвет текста.
    /// </summary>
    public InterfaceColor ForegroundColor { get; private set; } = InterfaceColor.Gray;

    /// <summary>
    /// Цвет фона текста.
    /// </summary>
    public InterfaceColor BackgroundColor { get; set; } = InterfaceColor.Black;

    public void Start()
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        ClearCurrentView();
        SetNextStatementToStarting();
        RunStatements();
    }

    void IGameContext.PrintText(string? text)
    {
        if (text is not null)
            OnTextPrinted?.Invoke(text, ForegroundColor, BackgroundColor);
    }

    void IGameContext.AddButton(string caption, LabelStatement? labelStatement) => _currentScreenButtons.Add(new Button
    {
        Caption = caption,
        OnButtonPressed = () => JumpByButton(labelStatement)
    });

    void IGameContext.EndLocation() => SetModeWaitingUserInput();

    void IGameContext.ClearScreen()
    {
        ClearCurrentView();
        OnScreenCleared?.Invoke();
    }

    public void AssignVariable(string name, Value value)
    {
        if (TryAssignSystemVariable(name, value)) return;

        _userVariables[name] = new Variable(name, value);
    }

    public Variable? GetVariable(string variableName) => GetSystemVariable(variableName) ?? GetUserVariable(variableName);

    void IGameContext.Goto(LabelStatement? labelStatement) => JumpByGoto(labelStatement);

    void IGameContext.Perkill() => _userVariables.Clear();

    private void JumpByButton(LabelStatement? labelStatement)
    {
        if (labelStatement is null) return;

        ClearCurrentView();
        SetCurrentLabel(labelStatement);
        UpdateCurrentLocationName(labelStatement.Label);

        RunStatements();
    }

    private void JumpByGoto(LabelStatement? labelStatement)
    {
        if (labelStatement is null) return;

        SetCurrentLabel(labelStatement);
    }

    private void ClearCurrentView()
    {
        _currentScreenButtons.Clear();
    }

    private void UpdateCurrentLocationName(string currentLocationName)
    {
        _previousLocationName = _currentLocationName;
        _currentLocationName = currentLocationName;
    }

    private void RunStatements()
    {
        SetModeRunningStatements();
        while (IsRunningStatements)
        {
            RunCurrentStatement();
            PromoteNextStatement();
        }
    }

    private void RunCurrentStatement()
    {
        if (_currentStatement is null)
        {
            SetModeWaitingUserInput();
            return;
        }

        _currentStatement.Run(this);
    }

    private void SetCurrentLabel(LabelStatement? labelStatement)
    {
        if (labelStatement is not null)
        {
            SetCurrentStatement(labelStatement);
        }
    }

    private Variable? GetSystemVariable(string name) => name.ToLower() switch
    {
        "current_loc" => new Variable("current_loc", new StringValue(_currentLocationName ?? string.Empty)),
        "previous_loc" => new Variable("previous_loc", new StringValue(_previousLocationName ?? string.Empty)),
        "style_dos_textcolor" => new Variable("style_dos_textcolor", new NumberValue(EncodeDosTextColor(ForegroundColor, BackgroundColor))),
        _ when rndRegex.IsMatch(name) => GetRandom(name),
        _ => null
    };

    private Variable? GetUserVariable(string variableName) => _userVariables.TryGetValue(variableName, out Variable? variable) ? variable : null;

    private bool TryAssignSystemVariable(string name, Value value)
    {
        switch (name.ToLower())
        {
            case "style_dos_textcolor":
                (ForegroundColor, BackgroundColor) = DecodeDosTextColor(Convert.ToByte(value.AsDecimal));
                return true;

            default:
                return false;
        }
    }

    private static byte EncodeDosTextColor(InterfaceColor foregroundColor, InterfaceColor backgroundColor)
    {
        byte foreground = (byte)foregroundColor;
        byte background = (byte)((byte)backgroundColor << 0x4);
        return (byte)(background + foreground);
    }

    private static (InterfaceColor ForegroundColor, InterfaceColor BackgroundColor) DecodeDosTextColor(byte value)
    {
        byte foreground = (byte)((value & 0x0F));
        byte background = (byte)((value & 0xF0) >> 0x4);
        return (ForegroundColor: (InterfaceColor)foreground, BackgroundColor: (InterfaceColor)background);
    }

    private void PromoteNextStatement() => _currentStatement = Quest.GetNextStatement(_currentStatement);

    private void SetNextStatementToStarting() => _currentStatement = Quest.StartingStatement;
    private void SetModeRunningStatements() => _gameState = GameState.RunningStatements;
    private void SetModeWaitingUserInput() => _gameState = GameState.WaitingUserInput;
    private void SetCurrentStatement(Statement statement) => _currentStatement = statement;

    private bool IsStarted => _gameState is not GameState.InitialState;
    private bool IsRunningStatements => _gameState == GameState.RunningStatements;

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
        public required Action OnButtonPressed { get; init; }

        public void Press() => OnButtonPressed();
    }

    private GameState _gameState = GameState.InitialState;
    private Statement? _currentStatement;
    private readonly List<Button> _currentScreenButtons = [];
    private string? _currentLocationName;
    private string? _previousLocationName;
    private readonly Dictionary<string, Variable> _userVariables = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Regex rndRegex = new(@"^rnd(?<number>\d*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly Random random = new();
}
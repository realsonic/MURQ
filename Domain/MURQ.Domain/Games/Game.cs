using MURQ.Common.Exceptions;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

using System.Text;

namespace MURQ.Domain.Games;

public class Game
{
    public Game(Quest quest)
    {
        Quest = quest;

        _globalGameContext = new GameContext();
        SubscribeToRunningContextEvents();
    }

    public event Action? OnLocationEntered;
    public event Action? OnScreenCleared;

    public Quest Quest { get; }

    public CurrentLocationView CurrentLocation => new()
    {
        Name = _currentLocationName,
        Text = _currentScreenText.ToString(),
        Buttons = _currentScreenButtons
    };

    public void Start()
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        ClearCurrentView();
        SetNextStatementToStarting();
        RunStatements();
    }

    public Variable? GetVariable(string name) => _variables.TryGetValue(name, out Variable? variable) ? variable : null;

    private void SubscribeToRunningContextEvents()
    {
        _globalGameContext.OnLocationEntered += locationName =>
        {
            _currentLocationName = locationName;
            OnLocationEntered?.Invoke();
        };

        _globalGameContext.OnTextPrinted += text => _currentScreenText.Append(text);

        _globalGameContext.OnButtonAdded += (caption, labelStatement) => _currentScreenButtons.Add(new Button
        {
            Caption = caption,
            OnButtonPressed = () => GoToNewLocation(labelStatement)
        });

        _globalGameContext.OnEnd += StopAndWaitUser;

        _globalGameContext.OnClearScreen += () =>
        {
            ClearCurrentView();
            OnScreenCleared?.Invoke();
        };

        _globalGameContext.OnVariableAssignment += AssignVariable;
    }

    private void GoToNewLocation(LabelStatement? labelStatement)
    {
        if (labelStatement is null) return;

        ClearCurrentView();
        GoToLabel(labelStatement);
        RunStatements();
    }

    private void StopAndWaitUser()
    {
        SetModeWaitingUserInput();
    }

    private void ClearCurrentView()
    {
        _currentScreenText.Clear();
        _currentScreenButtons.Clear();
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

        _currentStatement.Run(_globalGameContext);
    }

    private void GoToLabel(LabelStatement? labelStatement)
    {
        if (labelStatement is not null)
        {
            SetCurrentStatement(labelStatement);
        }
    }

    private void AssignVariable(string name, decimal value)
    {
        _variables[name] = new DecimalVariable(name, value);
    }

    private void PromoteNextStatement() => _currentStatement = Quest.GetNextStatement(_currentStatement);

    private void SetNextStatementToStarting() => _currentStatement = Quest.StartingStatement;
    private void SetModeRunningStatements() => _gameState = GameState.RunningStatements;
    private void SetModeWaitingUserInput() => _gameState = GameState.WaitingUserInput;
    private void SetCurrentStatement(Statement statement) => _currentStatement = statement;

    private bool IsStarted => _gameState is not GameState.InitialState;
    private bool IsRunningStatements => _gameState == GameState.RunningStatements;

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
    private readonly GameContext _globalGameContext;
    private readonly StringBuilder _currentScreenText = new();
    private readonly List<Button> _currentScreenButtons = [];
    private string? _currentLocationName;
    private readonly Dictionary<string, Variable> _variables = new(StringComparer.InvariantCultureIgnoreCase);
}
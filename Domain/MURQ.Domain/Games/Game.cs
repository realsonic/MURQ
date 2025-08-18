using MURQ.Common.Exceptions;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

using System.Text;

namespace MURQ.Domain.Games;

public class Game(Quest quest) : IGameContext
{
    public event Action? OnLocationEntered;
    public event Action? OnScreenCleared;

    public Quest Quest { get; } = quest;

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

    void IGameContext.PrintText(string? text) => _currentScreenText.Append(text);

    void IGameContext.AddButton(string caption, LabelStatement? labelStatement) => _currentScreenButtons.Add(new Button
    {
        Caption = caption,
        OnButtonPressed = () => GoByButton(labelStatement)
    });

    void IGameContext.EnterLocation(string locationName)
    {
        _previousLocationName = _currentLocationName;
        _currentLocationName = locationName;
        OnLocationEntered?.Invoke();
    }

    void IGameContext.EndLocation() => SetModeWaitingUserInput();

    void IGameContext.ClearScreen()
    {
        ClearCurrentView();
        OnScreenCleared?.Invoke();
    }

    void IGameContext.AssignVariable(string VariableName, Value Value)
    {
        _variables[VariableName] = new Variable(VariableName, Value);
    }

    public Variable? GetVariable(string variableName) => GetSystemVariable(variableName) ?? GetGameVariable(variableName);

    void IGameContext.GoToLabel(LabelStatement? labelStatement) => GoByGoto(labelStatement);

    private void GoByButton(LabelStatement? labelStatement)
    {
        if (labelStatement is null) return;

        ClearCurrentView();
        SetCurrentLabel(labelStatement);
        RunStatements();
    }

    private void GoByGoto(LabelStatement? labelStatement)
    {
        if (labelStatement is null) return;

        SetCurrentLabel(labelStatement);
        RunStatements();
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

        _currentStatement.Run(this);
    }

    private void SetCurrentLabel(LabelStatement? labelStatement)
    {
        if (labelStatement is not null)
        {
            SetCurrentStatement(labelStatement);
        }
    }

    private Variable? GetSystemVariable(string variableName) => variableName.ToLower() switch
    {
        "current_loc" => new Variable("current_loc", new StringValue(_currentLocationName ?? string.Empty)),
        "previous_loc" => new Variable("previous_loc", new StringValue(_previousLocationName ?? string.Empty)),
        _ => null
    };

    private Variable? GetGameVariable(string variableName) => _variables.TryGetValue(variableName, out Variable? variable) ? variable : null;

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
    private readonly StringBuilder _currentScreenText = new();
    private readonly List<Button> _currentScreenButtons = [];
    private string? _currentLocationName;
    private string? _previousLocationName;
    private readonly Dictionary<string, Variable> _variables = new(StringComparer.InvariantCultureIgnoreCase);
}
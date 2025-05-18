using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public class GameContext : IGameContext
{
    public event Action<string>? OnLocationEntered;
    public event Action<string?>? OnTextPrinted;
    public event Action<string, LabelStatement?>? OnButtonAdded;
    public event Action? OnEnd;
    public event Action? OnClearScreen;
    public event Action<string, decimal>? OnVariableAssignment;

    public void EnterLocation(string label) => OnLocationEntered?.Invoke(label);

    public void PrintText(string? text) => OnTextPrinted?.Invoke(text);

    public void AddButton(string caption, LabelStatement? labelStatement) => OnButtonAdded?.Invoke(caption, labelStatement);

    public void End() => OnEnd?.Invoke();

    public void ClearScreen() => OnClearScreen?.Invoke();

    public void AssignVariable(string VariableName, decimal Value) => OnVariableAssignment?.Invoke(VariableName, Value);
}
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public class GameContext : IGameContext
{
    public event Action<string?>? OnTextPrinted;
    public event Action<string, LabelStatement?>? OnButtonAdded;
    public event Action? OnEnd;
    public event Action<string>? OnLocationChanged;

    public void PrintText(string? text) => OnTextPrinted?.Invoke(text);

    public void AddButton(string caption, LabelStatement? labelStatement) =>
        OnButtonAdded?.Invoke(caption, labelStatement);

    public void End() => OnEnd?.Invoke();
    
    public void ChangeLocation(string label) => OnLocationChanged?.Invoke(label);
}
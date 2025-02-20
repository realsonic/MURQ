using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Games;

public class GameContext : IGameContext
{
    public event Action<string?>? OnTextPrinted;
    public event Action<string, LabelInstruction?>? OnButtonAdded;
    public event Action? OnEnd;
    public event Action<string>? OnLocationChanged;

    public void PrintText(string? text) => OnTextPrinted?.Invoke(text);

    public void AddButton(string caption, LabelInstruction? labelInstruction) =>
        OnButtonAdded?.Invoke(caption, labelInstruction);

    public void End() => OnEnd?.Invoke();
    
    public void ChangeLocation(string label) => OnLocationChanged?.Invoke(label);
}
using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Games;

public class RunningContext
{
    public event Action<string?>? OnTextPrinted;
    public event Action<string, LabelInstruction?>? OnButtonAdded;

    public void PrintText(string? text) => OnTextPrinted?.Invoke(text);

    public void AddButton(string caption, LabelInstruction? labelInstruction) =>
        OnButtonAdded?.Invoke(caption, labelInstruction);
}
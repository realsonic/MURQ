using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Games;

public interface IGameContext
{
    void PrintText(string? text);
    void AddButton(string caption, LabelInstruction? labelInstruction);
    void ChangeLocation(string label);
    void End();
}
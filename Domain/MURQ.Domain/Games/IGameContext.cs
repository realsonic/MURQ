using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public interface IGameContext
{
    void PrintText(string? text);
    void AddButton(string caption, LabelStatement? labelInstruction);
    void ChangeLocation(string label);
    void End();
    void ClearScreen();
}
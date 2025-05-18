using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public interface IGameContext
{
    void PrintText(string? text);
    void AddButton(string caption, LabelStatement? labelStatement);
    void EnterLocation(string label);
    void End();
    void ClearScreen();
    void AssignVariable(string VariableName, decimal Value);
}
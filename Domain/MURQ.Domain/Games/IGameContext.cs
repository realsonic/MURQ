using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public interface IGameContext
{
    void PrintText(string? text);
    void AddButton(string caption, LabelStatement? labelStatement);
    void EndLocation();
    void ClearScreen();
    void AssignVariable(string variableName, Value value);
    Variable? GetVariable(string variableName);
    void Goto(LabelStatement? labelStatement);
    void Perkill();
}
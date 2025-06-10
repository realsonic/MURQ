using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public interface IGameContext
{
    void PrintText(string? text);
    void AddButton(string caption, LabelStatement? labelStatement);
    void EnterLocation(string locationName);
    void EndLocation();
    void ClearScreen();
    void AssignVariable(string variableName, decimal value);
    Variable? GetVariable(string variableName);
}
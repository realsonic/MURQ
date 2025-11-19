using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Games;

public interface IGameContext
{
    /// <summary>
    /// Напечатать текст для игрока.
    /// </summary>
    /// <param name="text">Текст.</param>
    void Print(string? text);

    /// <summary>
    /// Напечатать текст для игрока и новую строку в конце.
    /// </summary>
    /// <param name="text">Текст.</param>
    void PrintLine(string? text = null);

    void AddButton(string caption, LabelStatement? labelStatement);
    void EndLocation();
    void ClearScreen();
    void AssignVariable(string variableName, Value value);
    Variable? GetVariable(string variableName);
    void Goto(LabelStatement? labelStatement);
    void Perkill();
}
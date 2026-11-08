using MURQ.Domain.Games.Values;

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

    void AddButton(string caption, string label);
    void EndLocation();
    void ClearScreen();
    void AssignVariable(string variableName, Value value);
    Value? GetVariableValue(string variableName);
    
    /// <summary>
    /// Перейти на метку. Если такой метки нет, переход не выполняется.
    /// </summary>
    /// <param name="label">Метка.</param>
    void Goto(string label);

    /// <summary>
    /// Выполнить процедуру. Если такой метки нет, переход не выполняется.
    /// </summary>
    /// <param name="label">Метка процедуры.</param>
    void Proc(string label);

    void Perkill();
}
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

class GameContextEmulation : IGameContext
{
    public event Action<string>? OnCommandExecuted;

    public void AddButton(string caption, string label)
    {
        throw new NotImplementedException();
    }

    public void AssignVariable(string variableName, Value value)
    {
        throw new NotImplementedException();
    }

    public void ClearScreen()
    {
        throw new NotImplementedException();
    }

    public void EndLocation()
    {
        throw new NotImplementedException();
    }

    public Value? GetVariableValue(string variableName)
    {
        return new StringValue($"<Строковая переменная {variableName}>");
    }

    public void Goto(string label)
    {
        throw new NotImplementedException();
    }

    public void Perkill()
    {
        throw new NotImplementedException();
    }

    public void Print(string? text)
    {
        OnCommandExecuted?.Invoke($"Print(\"{text}\"); ");
    }

    public void PrintLine(string? text = null)
    {
        OnCommandExecuted?.Invoke($"PrintLine(\"{text}\"); ");
    }
}
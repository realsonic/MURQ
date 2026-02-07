using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests.Statements;

class GameContextEmulation : IGameContext
{
    public void AddButton(string caption, LabelStatement? labelStatement)
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

    public Variable? GetVariable(string variableName)
    {
        return new Variable(variableName, new StringValue($"<Строковая переменная {variableName}>"));
    }

    public void Goto(LabelStatement? labelStatement)
    {
        throw new NotImplementedException();
    }

    public void Perkill()
    {
        throw new NotImplementedException();
    }

    public void Print(string? text)
    {
        throw new NotImplementedException();
    }

    public void PrintLine(string? text = null)
    {
        throw new NotImplementedException();
    }
}
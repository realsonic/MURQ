namespace MURQ.Domain.Games.Values;

public record StringValue(string Value) : Value
{
    public override decimal AsDecimal => Value.Length;

    public override string AsString => Value;
}

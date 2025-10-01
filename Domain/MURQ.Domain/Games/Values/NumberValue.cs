namespace MURQ.Domain.Games.Values;

public record NumberValue(decimal Value) : Value
{
    public override decimal AsDecimal => Value;

    public override string AsString => string.Empty;
}

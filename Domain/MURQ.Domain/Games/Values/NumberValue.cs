namespace MURQ.Domain.Games.Values;

public record NumberValue(decimal Value) : Value
{
    public override decimal DecimalValue => Value;

    public override string TextValue => Value.ToString();
}

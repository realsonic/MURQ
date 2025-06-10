namespace MURQ.Domain.Games.Values;

public record DecimalValue(decimal Value) : Value
{
    public static implicit operator DecimalValue(decimal value) => new(value);
}
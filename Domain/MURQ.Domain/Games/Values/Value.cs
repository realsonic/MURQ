namespace MURQ.Domain.Games.Values;

public abstract record Value
{
    public static implicit operator decimal(Value value)
    {
        if (value is DecimalValue decimalValue) return decimalValue.Value;

        throw new NotImplementedException("для строковых значений возврат длины строки"); //todo
    }

    public static implicit operator Value(decimal value) => new DecimalValue(value);
}

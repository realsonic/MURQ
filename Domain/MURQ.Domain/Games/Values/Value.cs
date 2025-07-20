namespace MURQ.Domain.Games.Values;

public abstract record Value
{
    public abstract decimal DecimalValue { get; }

    public abstract string TextValue { get; }
}

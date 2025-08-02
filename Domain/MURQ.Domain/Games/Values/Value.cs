namespace MURQ.Domain.Games.Values;

public abstract record Value
{
    public abstract decimal AsDecimal { get; }

    public abstract string AsString { get; }
}

namespace MURQ.Domain.Games.Values;

public abstract record Value
{
    public abstract decimal AsDecimal { get; }

    public abstract string AsString { get; }

    public bool IsLogicalTrue => AsDecimal != 0;

    public static Value FromBool(bool value) => new NumberValue(value ? 1 : 0);
}

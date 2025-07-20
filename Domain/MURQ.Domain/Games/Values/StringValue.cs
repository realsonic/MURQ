namespace MURQ.Domain.Games.Values;

public record StringValue(string Text) : Value
{
    public override decimal DecimalValue => Text.Length;

    public override string TextValue => Text;
}

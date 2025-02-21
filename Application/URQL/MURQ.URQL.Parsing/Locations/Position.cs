namespace MURQ.URQL.Parsing.Locations;

public record Position(int Line, int Column)
{
    public static Position Initial { get; } = new Position(1, 1);

    public Position AddColumn() => new(Line, Column + 1);

    public Position NewLine() => new(Line + 1, 1);

    public override string? ToString() => $"[{Line}, {Column}]";
}

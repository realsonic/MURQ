namespace MURQ.URQL.Locations;

public record Position(int Line, int Column)
{
    public static Position Initial { get; } = new Position(1, 1);

    public Position AddColumn() => this with { Column = Column + 1 };

    public Position NewLine() => this with { Line = Line + 1, Column = 1 };

    public override string? ToString() => $"[{Line}, {Column}]";
}

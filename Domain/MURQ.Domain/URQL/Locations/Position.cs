namespace MURQ.Domain.URQL.Locations;

public record struct Position(int Line, int Column)
{
    public static Position Initial { get; } = new Position(1, 1);

    public Position AddColumn() => this with { Column = Column + 1 };

    public Position NewLine() => this with { Line = Line + 1, Column = 1 };

    public override readonly string? ToString() => $"[{Line}, {Column}]";
}

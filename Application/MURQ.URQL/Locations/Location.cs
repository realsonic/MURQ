namespace MURQ.URQL.Locations;

public record Location(Position Start, Position End)
{
    public static Location StartAt(Position position) => new(Start: position, End: position);

    public Location EndAt(Position position) => this with { End = position };

    public static implicit operator Location(((int Line, int Column) Start, (int Line, int Column) End) coordinates)
        => new(new Position(coordinates.Start.Line, coordinates.Start.Column), new Position(coordinates.End.Line, coordinates.End.Column));

    public static implicit operator Location((Position Start, Position End) coordinates)
        => new(coordinates.Start, coordinates.End);

    public static implicit operator Location((Location Start, Location End) coordinates)
        => new(coordinates.Start.Start, coordinates.End.End);

    public override string? ToString() => $"{Start}–{End}";
}

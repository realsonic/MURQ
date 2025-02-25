namespace MURQ.URQL.Locations;

public record Location(Position Start, Position End)
{
    public static Location StartAt(Position position) => new(Start: position, End: position);

    public Location EndAt(Position position) => this with { End = position };

    public static Location Create((int Line, int Column) start, (int Line, int Column) end)
        => new(new Position(start.Line, start.Column), new Position(end.Line, end.Column));

    public static implicit operator Location(((int Line, int Column) start, (int Line, int Column) end) coordinates)
        => Create(coordinates.start, coordinates.end);

    public override string? ToString() => $"{Start}–{End}";
}

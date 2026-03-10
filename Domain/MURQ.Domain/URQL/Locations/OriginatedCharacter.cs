namespace MURQ.Domain.URQL.Locations;

public record struct OriginatedCharacter(char Character, Origin Origin)
{
    public static implicit operator OriginatedCharacter(PositionedCharacter positionedCharacter) 
        => new(positionedCharacter.Character, new PositionOrigin(positionedCharacter.Position));
}

public abstract record Origin;
public record PositionOrigin(Position Position) : Origin;
public record LocationOrigin(Location Location) : Origin;
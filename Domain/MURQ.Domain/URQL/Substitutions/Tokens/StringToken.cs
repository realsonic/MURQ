using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Substitutions.Tokens;

public record StringToken(List<PositionedCharacter> SourceCharacters, Location Location) : Token(Location);

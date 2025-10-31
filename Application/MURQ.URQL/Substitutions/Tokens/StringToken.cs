using MURQ.URQL.Locations;

namespace MURQ.URQL.Substitutions.Tokens;

public record StringToken(string Value, Location Location) : Token(Location);

using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Substitutions.Tokens;

public record StringToken(string Value, Location Location) : Token(Location);

using MURQ.Domain.URQL.Locations;

namespace MURQ.URQL.Substitutions.Tokens;

public record SubstitutionStopToken(Location Location) : Token(Location);
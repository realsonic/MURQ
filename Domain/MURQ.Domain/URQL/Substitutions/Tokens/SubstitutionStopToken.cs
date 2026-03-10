using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Substitutions.Tokens;

public record SubstitutionStopToken(Location Location) : Token(Location);
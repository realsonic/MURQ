using MURQ.Domain.Quests.Locations;

namespace MURQ.URQL.Substitutions.Tokens;

public record SubstitutionStopToken(Location Location) : Token(Location);
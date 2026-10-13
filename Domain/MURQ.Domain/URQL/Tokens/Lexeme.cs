using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Tokens;

public record Lexeme(string Literal, Location Location);
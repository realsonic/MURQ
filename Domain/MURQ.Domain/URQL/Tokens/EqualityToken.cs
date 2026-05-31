using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description(@"равно ""=""")]
public record EqualityToken(string Lexeme, Location Location) : Token(Lexeme, Location);
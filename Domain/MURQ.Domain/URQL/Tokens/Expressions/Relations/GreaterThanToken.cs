using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"больше "">""")]
public record GreaterThanToken(string Lexeme, Location Location) : Token(Lexeme, Location);
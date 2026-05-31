using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"минус ""-""")]
public record MinusToken(string Lexeme, Location Location) : Token(Lexeme, Location);
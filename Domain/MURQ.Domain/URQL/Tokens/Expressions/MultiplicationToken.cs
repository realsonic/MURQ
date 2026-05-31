using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"умножение ""*""")]
public record MultiplicationToken(string Lexeme, Location Location) : Token(Lexeme, Location);
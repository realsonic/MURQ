using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"правая скобка "")""")]
public record RightBraceToken(string Lexeme, Location Location) : Token(Lexeme, Location);
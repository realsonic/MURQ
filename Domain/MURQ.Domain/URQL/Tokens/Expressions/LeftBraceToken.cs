using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"левая скобка ""(""")]
public record LeftBraceToken(string Lexeme, Location Location) : Token(Lexeme, Location);
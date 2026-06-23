using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"плюс ""+""")]
public record PlusToken(string Lexeme, Location Location) : Token(Lexeme, Location);

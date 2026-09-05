using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Logic;

[Description(@"логическое ИЛИ ""or""")]
public record OrToken(string Lexeme, Location Location) : Token(Lexeme, Location);
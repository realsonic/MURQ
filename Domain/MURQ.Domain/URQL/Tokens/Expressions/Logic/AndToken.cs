using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Logic;

[Description(@"логическое И ""and""")]
public record AndToken(string Lexeme, Location Location) : Token(Lexeme, Location);
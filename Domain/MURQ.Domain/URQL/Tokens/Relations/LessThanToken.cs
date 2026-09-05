using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Relations;

[Description(@"меньше ""<""")]
public record LessThanToken(string Lexeme, Location Location) : Token(Lexeme, Location);
using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"деление ""/""")]
public record DivisionToken(string Lexeme, Location Location) : Token(Lexeme, Location);
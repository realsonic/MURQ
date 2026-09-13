using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"не равно ""<>""")]
public record NotEqualToken(string Lexeme, Location Location) : RelationToken(Lexeme, Location);
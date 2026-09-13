using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"меньше или равно ""<=""")]
public record LessThanOrEqualToken(string Lexeme, Location Location) : RelationToken(Lexeme, Location);
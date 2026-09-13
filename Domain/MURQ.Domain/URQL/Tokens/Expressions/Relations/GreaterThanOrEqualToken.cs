using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"больше или равно "">=""")]
public record GreaterThanOrEqualToken(string Lexeme, Location Location) : RelationToken(Lexeme, Location);
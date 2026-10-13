using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"больше или равно "">=""")]
public record GreaterThanOrEqualToken(Lexeme Lexeme) : RelationToken(Lexeme);
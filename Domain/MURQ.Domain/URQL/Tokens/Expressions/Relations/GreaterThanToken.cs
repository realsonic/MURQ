using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"больше "">""")]
public record GreaterThanToken(Lexeme Lexeme) : RelationToken(Lexeme);
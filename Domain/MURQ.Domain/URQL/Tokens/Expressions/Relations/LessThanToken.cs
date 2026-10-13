using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"меньше ""<""")]
public record LessThanToken(Lexeme Lexeme) : RelationToken(Lexeme);
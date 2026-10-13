using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"меньше или равно ""<=""")]
public record LessThanOrEqualToken(Lexeme Lexeme) : RelationToken(Lexeme);
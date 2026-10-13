using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

[Description(@"не равно ""<>""")]
public record NotEqualToken(Lexeme Lexeme) : RelationToken(Lexeme);
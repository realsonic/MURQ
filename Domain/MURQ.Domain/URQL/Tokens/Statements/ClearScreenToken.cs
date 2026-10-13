using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор очистка экрана (cls)")]
public record ClearScreenToken(Lexeme Lexeme) : StatementToken(Lexeme);
using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор удаление переменных (perkill)")]
public record PerkillToken(Lexeme Lexeme) : StatementToken(Lexeme);
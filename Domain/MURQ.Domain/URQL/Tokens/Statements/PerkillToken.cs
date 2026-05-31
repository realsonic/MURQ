using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор удаление переменных (perkill)")]
public record PerkillToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location);
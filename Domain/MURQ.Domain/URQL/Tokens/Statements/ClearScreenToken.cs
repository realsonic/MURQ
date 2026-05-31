using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор очистка экрана (cls)")]
public record ClearScreenToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location);
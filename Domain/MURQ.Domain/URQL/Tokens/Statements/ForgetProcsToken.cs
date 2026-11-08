using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор забыть стек proc (forget_procs)")]
public record ForgetProcsToken(Lexeme Lexeme) : StatementToken(Lexeme);

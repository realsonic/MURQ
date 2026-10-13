namespace MURQ.Domain.URQL.Tokens.Statements;

public abstract record StatementToken(Lexeme Lexeme) : Token(Lexeme);
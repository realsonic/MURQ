namespace MURQ.URQL.SyntaxTree.Statements;
public record PrintStatementSto(string Text, bool IsNewLineAtEnd) : StatementSto;
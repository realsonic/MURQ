using MURQ.Common.Exceptions;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Lexers;
using MURQ.URQL.Parsers;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Statements;

namespace MURQ.Application;

public class UrqLoader(IEnumerable<char> source)
{
    public Quest LoadQuest()
    {
        UrqlLexer lexer = new(source);
        UrqlParser parser = new(lexer.Scan());

        QuestSto questSto = parser.ParseQuest();

        Quest quest = new(statements: questSto.Statements.Select<StatementSto, Statement>(statementSto => statementSto switch
        {
            LabelStatementSto labelStatementSto => new LabelStatement() { Label = labelStatementSto.Label },
            PrintStatementSto printStatementSto => new PrintStatement() { Text = printStatementSto.Text, IsNewLineAtEnd = printStatementSto.IsNewLineAtEnd },
            _ => throw new MurqException($"Неизвестный тип инструкции {statementSto}.")
        }));

        return quest;
    }
}

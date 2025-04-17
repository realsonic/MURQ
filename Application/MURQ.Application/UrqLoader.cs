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

        Dictionary<LabelStatementSto, LabelStatement> labelStatementDictionaryBySto = questSto.Statements
            .OfType<LabelStatementSto>()
            .ToDictionary(
                labelStatementSto => labelStatementSto,
                labelStatementSto => new LabelStatement { Label = labelStatementSto.Label });

        Dictionary<string, LabelStatement> labelStatementDictionaryByLowerLabel = labelStatementDictionaryBySto.Values
            .ToDictionary(
                labelStatement => labelStatement.Label.ToLower(),
                LabelStatement => LabelStatement);

        Quest quest = new(statements: questSto.Statements.Select<StatementSto, Statement>(statementSto => statementSto switch
        {
            LabelStatementSto labelStatementSto => labelStatementDictionaryBySto[labelStatementSto],
            PrintStatementSto printStatementSto => new PrintStatement { Text = printStatementSto.Text, IsNewLineAtEnd = printStatementSto.IsNewLineAtEnd },
            ButtonStatementSto buttonStatementSto => new ButtonStatement
            {
                LabelStatement = labelStatementDictionaryByLowerLabel.TryGetValue(buttonStatementSto.Label.ToLower(), out LabelStatement? labelStatement) ?
                    labelStatement : null,
                Caption = buttonStatementSto.Caption
            },
            EndStatementSto endStatementSto => new EndStatement(),
            ClearScreenStatementSto clearScreenStatementSto => new ClearScreenStatement(),
            _ => throw new MurqException($"Неизвестный тип инструкции {statementSto}.")
        }));

        return quest;
    }
}

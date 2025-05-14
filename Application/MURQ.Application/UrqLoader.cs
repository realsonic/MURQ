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
        Quest quest = ProduceQuest(questSto);
        return quest;
    }

    private static Quest ProduceQuest(QuestSto questSto)
    {
        Dictionary<LabelStatementSto, LabelStatement> labelsBySto = questSto.Statements.OfType<LabelStatementSto>()
            .ToDictionary(
                labelStatementSto => labelStatementSto,
                labelStatementSto => new LabelStatement { Label = labelStatementSto.Label }
            );

        Dictionary<string, LabelStatement> labelsByLowers = labelsBySto.Values.ToDictionary(
            labelStatement => labelStatement.Label.ToLower(),
            LabelStatement => LabelStatement
        );

        IEnumerable<Statement> statements = questSto.Statements.Select(statementSto => ProduceStatement(statementSto, labelsBySto, labelsByLowers));
        Quest quest = new(statements);
        return quest;
    }

    private static Statement ProduceStatement(StatementSto statementSto, Dictionary<LabelStatementSto, LabelStatement> labelsBySto, Dictionary<string, LabelStatement> labelsByLowers) => statementSto switch
    {
        LabelStatementSto labelStatementSto => labelsBySto[labelStatementSto],
        PrintStatementSto printStatementSto => ProducePrintStatement(printStatementSto),
        ButtonStatementSto buttonStatementSto => ProduceButtonStatement(buttonStatementSto, labelsByLowers),
        EndStatementSto => new EndStatement(),
        ClearScreenStatementSto => new ClearScreenStatement(),
        _ => throw new MurqException($"Неизвестный тип инструкции {statementSto}.")
    };

    private static PrintStatement ProducePrintStatement(PrintStatementSto printStatementSto) =>
        new() { Text = printStatementSto.Text, IsNewLineAtEnd = printStatementSto.IsNewLineAtEnd };

    private static ButtonStatement ProduceButtonStatement(ButtonStatementSto buttonStatementSto, Dictionary<string, LabelStatement> labelsBySto) => new()
    {
        LabelStatement = labelsBySto.TryGetValue(buttonStatementSto.Label.ToLower(), out LabelStatement? labelStatement) ? labelStatement : null,
        Caption = buttonStatementSto.Caption
    };
}

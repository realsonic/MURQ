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

        _questSto = parser.ParseQuest();

        Quest quest = ProduceQuest();
        return quest;
    }

    private Quest ProduceQuest()
    {
        if (_questSto is null)
            throw new MurqException("Не загружено синтаксическое дерево квеста.");

        IEnumerable<Statement> statements = _questSto.Statements.Select(ProduceStatement);
        Quest quest = new(statements);
        return quest;
    }

    private Statement ProduceStatement(StatementSto statementSto) => statementSto switch
    {
        LabelStatementSto labelStatementSto => GetLabelStatementByLabel(labelStatementSto.Label) ?? throw new MurqException("Не найдена инструкция метки"),
        PrintStatementSto printStatementSto => ProducePrintStatement(printStatementSto),
        ButtonStatementSto buttonStatementSto => ProduceButtonStatement(buttonStatementSto),
        EndStatementSto => new EndStatement(),
        ClearScreenStatementSto => new ClearScreenStatement(),
        AssignVariableStatementSto assignVariableStatementSto => ProduceAssignVariableStatement(assignVariableStatementSto),
        _ => throw new MurqException($"Неизвестный тип инструкции {statementSto}.")
        _ => throw new NotImplementedException($"Инструкция ({statementSto}) ещё не реализована.")
    };

    private static PrintStatement ProducePrintStatement(PrintStatementSto printStatementSto) => new()
    {
        Text = printStatementSto.Text,
        IsNewLineAtEnd = printStatementSto.IsNewLineAtEnd
    };

    private ButtonStatement ProduceButtonStatement(ButtonStatementSto buttonStatementSto) => new()
    {
        LabelStatement = GetLabelStatementByLabel(buttonStatementSto.Label),
        Caption = buttonStatementSto.Caption
    };

    private static AssignVariableStatement ProduceAssignVariableStatement(AssignVariableStatementSto assignVariableStatementSto) => new()
    {
        VariableName = assignVariableStatementSto.VariableName,
        Value = assignVariableStatementSto.Value
    };


    private LabelStatement? GetLabelStatementByLabel(string label)
    {
        if (_labelStatementsByLabel.TryGetValue(label, out LabelStatement? foundLabelStatement))
        {
            return foundLabelStatement;
        }

        if (_questSto is null)
            throw new MurqException("Не загружено синтаксическое дерево квеста.");

        LabelStatementSto? labelStatementSto = _questSto.Statements
            .OfType<LabelStatementSto>()
            .FirstOrDefault(labelStatement => labelStatement.Label.Equals(label, StringComparison.InvariantCultureIgnoreCase));
        
        if (labelStatementSto is not null)
        {
            LabelStatement labelStatement = new() { Label = labelStatementSto.Label };
            _labelStatementsByLabel[labelStatement.Label] = labelStatement;
            return labelStatement;
        }

        return null;
    }

    private QuestSto? _questSto;
    private Dictionary<string, LabelStatement> _labelStatementsByLabel = new(StringComparer.InvariantCultureIgnoreCase);
}

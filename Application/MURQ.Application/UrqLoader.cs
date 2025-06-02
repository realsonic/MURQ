using MURQ.Common.Exceptions;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Lexing;
using MURQ.URQL.Parsing;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Expressions;
using MURQ.URQL.SyntaxTree.Statements;

namespace MURQ.Application;

public class UrqLoader(IEnumerable<char> source)
{
    public Quest LoadQuest()
    {
        UrqlLexer lexer = new(source);
        UrqlParser parser = new(lexer.Scan());

        _questSto = parser.ParseQuest();

        _labelStatementsByLabel.Clear();
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
        LabelStatementSto labelStatementSto => GetOrCreateLabelStatementByLabel(labelStatementSto.Label) ?? throw new MurqException("Не найдена инструкция метки"),
        PrintStatementSto printStatementSto => ProducePrintStatement(printStatementSto),
        ButtonStatementSto buttonStatementSto => ProduceButtonStatement(buttonStatementSto),
        EndStatementSto => new EndStatement(),
        ClearScreenStatementSto => new ClearScreenStatement(),
        AssignVariableStatementSto assignVariableStatementSto => ProduceAssignVariableStatement(assignVariableStatementSto),
        IfStatementSto ifStatementSto => ProduceIfStatement(ifStatementSto),
        _ => throw new NotImplementedException($"Инструкция ({statementSto}) ещё не реализована.")
    };

    private static PrintStatement ProducePrintStatement(PrintStatementSto printStatementSto) => new()
    {
        Text = printStatementSto.Text,
        IsNewLineAtEnd = printStatementSto.IsNewLineAtEnd
    };

    private ButtonStatement ProduceButtonStatement(ButtonStatementSto buttonStatementSto) => new()
    {
        LabelStatement = GetOrCreateLabelStatementByLabel(buttonStatementSto.Label),
        Caption = buttonStatementSto.Caption
    };

    private static AssignVariableStatement ProduceAssignVariableStatement(AssignVariableStatementSto assignVariableStatementSto) => new()
    {
        VariableName = assignVariableStatementSto.VariableName,
        Value = assignVariableStatementSto.Value
    };

    private IfStatement ProduceIfStatement(IfStatementSto ifStatementSto) => new()
    {
        Condition = ProduceExpression(ifStatementSto.Condition),
        ThenStatement = ProduceStatement(ifStatementSto.ThenStatement)
    };

    private Expression ProduceExpression(ExpressionSto expressionSto) => expressionSto switch
    {
        RelationExpressionSto relationExpressionSto => ProduceRelationExpression(relationExpressionSto),
        VariableExpressionSto variableExpressionSto => new VariableExpression { VariableName = variableExpressionSto.VariableName },
        DecimalConstantExpressionSto decimalConstantExpressionSto => new DecimalConstantExpression { Value = decimalConstantExpressionSto.Value },
        _ => throw new NotImplementedException($"Выражение типа ({expressionSto}) ещё не реализовано.")
    };

    private RelationExpression ProduceRelationExpression(RelationExpressionSto relationExpressionSto) => new()
    {
        LeftExpression = ProduceExpression(relationExpressionSto.LeftExpression),
        RightExpression = ProduceExpression(relationExpressionSto.RightExpression)
    };

    private LabelStatement? GetOrCreateLabelStatementByLabel(string label)
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
    private readonly Dictionary<string, LabelStatement> _labelStatementsByLabel = new(StringComparer.InvariantCultureIgnoreCase);
}

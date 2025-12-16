using MURQ.Application.UrqLoaders.UrqStrings;
using MURQ.Common.Exceptions;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Lexing;
using MURQ.URQL.Parsing;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Expressions;
using MURQ.URQL.SyntaxTree.Statements;

namespace MURQ.Application.UrqLoaders;

public class UrqLoader(UrqStringLoader urqStringLoader)
{
    public Quest LoadQuest(IEnumerable<char> source)
    {
        UrqlLexer lexer = new(source);
        UrqlParser parser = new(lexer.Scan());

        _questSto = parser.ParseQuest();
        ProduceAndCacheLabelStatements();

        Quest quest = ProduceQuest();
        return quest;
    }

    private void ProduceAndCacheLabelStatements()
    {
        _labelStatementPairs.Clear();

        var labelStatementStos = _questSto!.Statements.OfType<LabelStatementSto>();

        foreach (LabelStatementSto labelStatementSto in labelStatementStos)
        {
            LabelStatement labelStatement = new() { Label = labelStatementSto.Label };
            _labelStatementPairs.Add((labelStatementSto, labelStatement));
        }
    }

    private Quest ProduceQuest()
    {
        IEnumerable<Statement> statements = _questSto!.Statements.Select(ProduceStatement);
        Quest quest = new(statements);
        return quest;
    }

    private Statement ProduceStatement(StatementSto statementSto) => statementSto switch
    {
        LabelStatementSto labelStatementSto => GetLabelStatement(labelStatementSto),
        PrintStatementSto printStatementSto => ProducePrintStatement(printStatementSto),
        ButtonStatementSto buttonStatementSto => ProduceButtonStatement(buttonStatementSto),
        EndStatementSto => new EndStatement(),
        ClearScreenStatementSto => new ClearScreenStatement(),
        AssignVariableStatementSto assignVariableStatementSto => ProduceAssignVariableStatement(assignVariableStatementSto),
        IfStatementSto ifStatementSto => ProduceIfStatement(ifStatementSto),
        GotoStatementSto gotoStatementSto => ProduceGotoStatement(gotoStatementSto),
        PerkillStatementSto => new PerkillStatement(),
        PauseStatementSto pauseStatementSto => ProducePauseStatement(pauseStatementSto),
        _ => throw new NotImplementedException($"Инструкция ({statementSto}) ещё не реализована.")
    };

    private LabelStatement GetLabelStatement(LabelStatementSto labelStatementSto)
        => _labelStatementPairs.Exists(p => p.LabelStatementSto == labelStatementSto)
            ? _labelStatementPairs.Find(p => p.LabelStatementSto == labelStatementSto).LabelStatement
            : throw new MurqException($"Неожиданно в списке закэшированных меток не оказалось метки для {labelStatementSto}");

    private PrintStatement ProducePrintStatement(PrintStatementSto printStatementSto) => new()
    {
        UrqString = urqStringLoader.LoadFromString(printStatementSto.Text),
        IsNewLineAtEnd = printStatementSto.IsNewLineAtEnd
    };

    private ButtonStatement ProduceButtonStatement(ButtonStatementSto buttonStatementSto) => new()
    {
        LabelStatement = TryGetLabelStatementByLabel(buttonStatementSto.Label),
        Caption = buttonStatementSto.Caption
    };

    private AssignVariableStatement ProduceAssignVariableStatement(AssignVariableStatementSto assignVariableStatementSto) => new()
    {
        VariableName = assignVariableStatementSto.VariableName,
        Expression = ProduceExpression(assignVariableStatementSto.ExpressionSto)
    };

    private IfStatement ProduceIfStatement(IfStatementSto ifStatementSto) => new()
    {
        Condition = ProduceExpression(ifStatementSto.Condition),
        ThenStatement = ProduceStatement(ifStatementSto.ThenStatement)
    };

    private GotoStatement ProduceGotoStatement(GotoStatementSto gotoStatementSto) => new()
    {
        LabelStatement = TryGetLabelStatementByLabel(gotoStatementSto.Label)
    };

    private static PauseStatement ProducePauseStatement(PauseStatementSto pauseStatementSto) => new()
    {
        Duration = pauseStatementSto.Duration
    };

    private Expression ProduceExpression(ExpressionSto expressionSto) => expressionSto switch
    {
        RelationExpressionSto relationExpressionSto => ProduceRelationExpression(relationExpressionSto),
        VariableExpressionSto variableExpressionSto => new VariableExpression { Name = variableExpressionSto.VariableName },
        DecimalConstantExpressionSto decimalConstantExpressionSto => new DecimalConstantExpression { Value = decimalConstantExpressionSto.Value },
        StringLiteralExpressionSto stringLiteralExpressionSto => new StringLiteralExpression { Text = stringLiteralExpressionSto.Text },
        _ => throw new NotImplementedException($"Выражение типа ({expressionSto}) ещё не реализовано.")
    };

    private RelationExpression ProduceRelationExpression(RelationExpressionSto relationExpressionSto) => new()
    {
        LeftExpression = ProduceExpression(relationExpressionSto.LeftExpression),
        RightExpression = ProduceExpression(relationExpressionSto.RightExpression)
    };

    private LabelStatement? TryGetLabelStatementByLabel(string label)
        => _labelStatementPairs.Exists(p => label.Equals(p.LabelStatement.Label, StringComparison.InvariantCultureIgnoreCase))
            ? _labelStatementPairs.Find(p => label.Equals(p.LabelStatement.Label, StringComparison.InvariantCultureIgnoreCase)).LabelStatement
            : null;

    private QuestSto? _questSto;
    private readonly List<(LabelStatementSto LabelStatementSto, LabelStatement LabelStatement)> _labelStatementPairs = [];
}

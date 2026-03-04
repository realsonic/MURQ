using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Interpretation.Exceptions;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;
using MURQ.URQL.Tokens.Statements.If;

namespace MURQ.URQL.Interpretation;

public class UrqlInterpreter(IEnumerable<Token> tokens, IGameContext gameContext) : UrqlParser(tokens)
{
    /// <summary>
    /// Грамматика:
    /// <code>
    /// statementLine = [ joinedStatements ];
    /// </code>
    /// </summary>
    public async Task RunStatementLineAsync(CancellationToken cancellationToken)
    {
        MoveToNextTerminal();

        if (Lookahead.IsStartOfStatement()) // 2ая линия
        {
            await RunJoinedStatementsAdaptedAsync(cancellationToken);
        }
        else if (Lookahead is null) // ϵ-продукция
        {
            return;
        }

        if (Lookahead is not null)
            throw new UnexpectedElementException("Ожидался конец строки", Lookahead);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// joinedStatementsAdapted = statement, joinedStatementsRest;
    /// </code>
    /// </summary>
    private async Task RunJoinedStatementsAdaptedAsync(CancellationToken cancellationToken)
    {
        await RunStatementAsync(cancellationToken);
        await RunJoinedStatementsRestAsync(cancellationToken);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// joinedStatementsRest = [? & ?, statement, joinedStatementsRest];
    /// </code>
    /// </summary>
    private async Task RunJoinedStatementsRestAsync(CancellationToken cancellationToken)
    {
        if (Lookahead is StatementJoinToken) // 2ая ветка
        {
            Match<StatementJoinToken>();
            await RunStatementAsync(cancellationToken);
            await RunJoinedStatementsRestAsync(cancellationToken);
        }
        else
        {
            return; // ϵ-продукция
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// statement = assignVariableStatement | ifStatement | ? Label ? | ? Print ? | ? Button ? | ? End ? | ? ClearScreen ?;
    /// </code>
    /// </summary>
    private async Task RunStatementAsync(CancellationToken cancellationToken)
    {
        switch (Lookahead)
        {
            case PrintToken:
                await RunPrintStatementAsync(cancellationToken);
                break;

            case Token when Lookahead.IsStartOfAssignVariableStatement():
                await RunAssignVariableStatementAsync(cancellationToken);
                break;

            case Token when Lookahead.IsStartOfIfStatement():
                await RunIfThenStatement(cancellationToken);
                break;

            default:
                throw new UnexpectedElementException("Ожидалась инструкция", Lookahead);
        }
    }

    private async Task RunPrintStatementAsync(CancellationToken cancellationToken)
    {
        PrintStatement printStatement = ParsePrintTerminal();
        await printStatement.RunAsync(gameContext, cancellationToken);
    }

    private async Task RunAssignVariableStatementAsync(CancellationToken cancellationToken)
    {
        AssignVariableStatement assignVariableStatement = ParseAssignVariableStatement();
        await assignVariableStatement.RunAsync(gameContext, cancellationToken);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// ifStatement = ? If ?, relationExpression, ? Then ?, joinedStatements;
    /// </code>
    /// </summary>
    /// <returns>Условный оператор <c>if-else</c></returns>
    private async Task RunIfThenStatement(CancellationToken cancellationToken)
    {
        Match<IfToken>();

        RelationExpression relationExpression = ParseRelationExpression();
        Value relationResult = relationExpression.Calculate(gameContext);
        if (relationResult.AsDecimal != 0)
        {
            Match<ThenToken>("в ветвлении if-then");
            await RunJoinedStatementsAdaptedAsync(cancellationToken);
        }
    }
}
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.Domain.URQL.Interpretation.Exceptions;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Statements;
using MURQ.Domain.URQL.Tokens.Statements.If;

namespace MURQ.Domain.URQL.Interpretation;

public class UrqlInterpreter(IEnumerable<Token> tokens, IGameContext gameContext) : UrqlParser(tokens)
{
    /// <summary>
    /// Грамматика:
    /// <code>
    /// statementLine = [ joinedStatements ];
    /// </code>
    /// </summary>
    public async Task InterpretStatementLineAsync(CancellationToken cancellationToken)
    {
        MoveToNextTerminal();

        if (Lookahead.IsStartOfStatement()) // 2ая линия
        {
            await InterpretJoinedStatementsAdaptedAsync(toRun: true, cancellationToken);
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
    private async Task InterpretJoinedStatementsAdaptedAsync(bool toRun, CancellationToken cancellationToken)
    {
        await InterpretStatementAsync(toRun, cancellationToken);
        await InterpretJoinedStatementsRestAsync(toRun, cancellationToken);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// joinedStatementsRest = [? & ?, statement, joinedStatementsRest];
    /// </code>
    /// </summary>
    private async Task InterpretJoinedStatementsRestAsync(bool toRun, CancellationToken cancellationToken)
    {
        if (Lookahead is StatementJoinToken) // 2ая ветка
        {
            Match<StatementJoinToken>();
            await InterpretStatementAsync(toRun, cancellationToken);
            await InterpretJoinedStatementsRestAsync(toRun, cancellationToken);
        }
        else
        {
            return; // ϵ-продукция
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// statement = assignVariableStatement | ifStatement | ? Print ? | ? Button ? | ? End ? | ? ClearScreen ? | ? Goto ? | ? Perkill ? | ? Pause ?;
    /// </code>
    /// </summary>
    private async Task InterpretStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        switch (Lookahead)
        {
            case Token when Lookahead.IsStartOfIfStatement():
                await InterpretIfThenStatement(toRun, cancellationToken);
                break;
            
            case Token when Lookahead.IsStartOfAssignVariableStatement():
                await InterpretAssignVariableStatementAsync(toRun, cancellationToken);
                break;

            case PrintToken:
                await InterpretPrintStatementAsync(toRun, cancellationToken);
                break;

            case ButtonToken:
                await InterpretButtonStatementAsync(toRun, cancellationToken);
                break;

            case EndToken:
                await InterpretEndStatementAsync(toRun, cancellationToken);
                break;

            case ClearScreenToken:
                await InterpretClearScreenStatementAdync(toRun, cancellationToken);
                break;

            case GotoToken:
                await InterpretGotoStatementAsync(toRun, cancellationToken);
                break;

            case PerkillToken:
                await InterpretPerkillStatementAsync(toRun, cancellationToken);
                break;

            case PauseToken:
                await InterpretPauseStatementAsync(toRun, cancellationToken);
                break;

            default:
                throw new UnexpectedElementException("Ожидалась инструкция", Lookahead);
        }
    }

    private async Task InterpretAssignVariableStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        AssignVariableStatement assignVariableStatement = ParseAssignVariableStatement();

        if (toRun)
        {
            await assignVariableStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// ifStatement = ? If ?, relationExpression, ? Then ?, joinedStatements;
    /// </code>
    /// </summary>
    private async Task InterpretIfThenStatement(bool toRun, CancellationToken cancellationToken)
    {
        Match<IfToken>();

        RelationExpression relationExpression = ParseRelationExpression();
        Value relationResult = relationExpression.Calculate(gameContext);

        Match<ThenToken>("в ветвлении if-then");

        bool isConditionTrue = relationResult.AsDecimal != 0;

        if (isConditionTrue)
        {
            await InterpretJoinedStatementsAdaptedAsync(toRun, cancellationToken);
            //todo skip else
        }
        else
        {
            await InterpretJoinedStatementsAdaptedAsync(toRun: false, cancellationToken);
            //todo run else
        }
    }

    private async Task InterpretPrintStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        PrintStatement printStatement = ParsePrintTerminal();

        if (toRun)
        {
            await printStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretButtonStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        ButtonStatement buttonStatement = ParseButtonTerminal();

        if (toRun)
        {
            await buttonStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretEndStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        EndStatement endStatement = ParseEndTerminal();

        if (toRun)
        {
            await endStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretClearScreenStatementAdync(bool toRun, CancellationToken cancellationToken)
    {
        ClearScreenStatement clearScreenStatement = ParseClearScreenTerminal();

        if (toRun)
        {
            await clearScreenStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretGotoStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        GotoStatement gotoStatement = ParseGotoTerminal();

        if (toRun)
        {
            await gotoStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretPerkillStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        PerkillStatement perkillStatement = ParsePerkillTerminal();

        if (toRun)
        {
            await perkillStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretPauseStatementAsync(bool toRun, CancellationToken cancellationToken)
    {
        PauseStatement pauseStatement = ParsePauseTerminal();

        if (toRun)
        {
            await pauseStatement.RunAsync(gameContext, cancellationToken);
        }
    }
}
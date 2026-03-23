using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.Domain.URQL.Interpretation.Exceptions;
using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Statements;
using MURQ.Domain.URQL.Tokens.Statements.If;

namespace MURQ.Domain.URQL.Interpretation;

public class UrqlInterpreter(UrqlLexer urqlLexer, IGameContext gameContext) : UrqlParser(urqlLexer)
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
            await InterpretJoinedStatementsAdaptedAsync(InterpretationMode.Run, cancellationToken);
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
    private async Task InterpretJoinedStatementsAdaptedAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        await InterpretStatementAsync(interpretationMode, cancellationToken);
        await InterpretJoinedStatementsRestAsync(interpretationMode, cancellationToken);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// joinedStatementsRest = [? & ?, statement, joinedStatementsRest];
    /// </code>
    /// </summary>
    private async Task InterpretJoinedStatementsRestAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        if (Lookahead is StatementJoinToken) // 2ая ветка
        {
            Match<StatementJoinToken>();
            await InterpretStatementAsync(interpretationMode, cancellationToken);
            await InterpretJoinedStatementsRestAsync(interpretationMode, cancellationToken);
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
    private async Task InterpretStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        switch (Lookahead)
        {
            case Token when Lookahead.IsStartOfIfStatement():
                await InterpretIfThenStatement(interpretationMode, cancellationToken);
                break;

            case Token when Lookahead.IsStartOfAssignVariableStatement():
                await InterpretAssignVariableStatementAsync(interpretationMode, cancellationToken);
                break;

            case PrintToken:
                await InterpretPrintStatementAsync(interpretationMode, cancellationToken);
                break;

            case ButtonToken:
                await InterpretButtonStatementAsync(interpretationMode, cancellationToken);
                break;

            case EndToken:
                await InterpretEndStatementAsync(interpretationMode, cancellationToken);
                break;

            case ClearScreenToken:
                await InterpretClearScreenStatementAdync(interpretationMode, cancellationToken);
                break;

            case GotoToken:
                await InterpretGotoStatementAsync(interpretationMode, cancellationToken);
                break;

            case PerkillToken:
                await InterpretPerkillStatementAsync(interpretationMode, cancellationToken);
                break;

            case PauseToken:
                await InterpretPauseStatementAsync(interpretationMode, cancellationToken);
                break;

            default:
                throw new UnexpectedElementException("Ожидалась инструкция", Lookahead);
        }
    }

    private async Task InterpretAssignVariableStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        AssignVariableStatement assignVariableStatement = ParseAssignVariableStatement();

        if (interpretationMode is InterpretationMode.Run)
        {
            await assignVariableStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// ifStatement = ? If ?, relationExpression, ? Then ?, joinedStatements, [ ? Else ?, joinedStatements ];
    /// </code>
    /// </summary>
    private async Task InterpretIfThenStatement(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        Match<IfToken>();

        RelationExpression relationExpression = ParseRelationExpression();
        Value relationResult = relationExpression.Calculate(gameContext);

        bool isConditionTrue = relationResult.AsDecimal != 0;

        // then
        EnterThen();
        try
        {
            Match<ThenToken>("в ветвлении if-then");
            await InterpretJoinedStatementsAdaptedAsync(isConditionTrue ? interpretationMode : InterpretationMode.ParseNotRun, cancellationToken);
        }
        finally
        {
            ExitThen();
        }

        // else
        if (Lookahead is ElseToken)
        {
            Match<ElseToken>();
            await InterpretJoinedStatementsAdaptedAsync(isConditionTrue ? InterpretationMode.ParseNotRun : interpretationMode, cancellationToken);
        }
    }

    private async Task InterpretPrintStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        PrintStatement printStatement = ParsePrintTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await printStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretButtonStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        ButtonStatement buttonStatement = ParseButtonTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await buttonStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretEndStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        EndStatement endStatement = ParseEndTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await endStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretClearScreenStatementAdync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        ClearScreenStatement clearScreenStatement = ParseClearScreenTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await clearScreenStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretGotoStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        GotoStatement gotoStatement = ParseGotoTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await gotoStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretPerkillStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        PerkillStatement perkillStatement = ParsePerkillTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await perkillStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private async Task InterpretPauseStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        PauseStatement pauseStatement = ParsePauseTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await pauseStatement.RunAsync(gameContext, cancellationToken);
        }
    }

    private enum InterpretationMode
    {
        Run,
        ParseNotRun
    }
}
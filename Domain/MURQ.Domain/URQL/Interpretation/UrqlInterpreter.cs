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
    private async Task<InterpretationResult> InterpretJoinedStatementsAdaptedAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        InterpretationResult interpretationResult = await InterpretStatementAsync(interpretationMode, cancellationToken);
        if (interpretationResult is InterpretationResult.ImmediateStop)
        {
            // т.к. нужно немедленно остановиться, то остаток кода мы только парсим, но не выполняем
            interpretationMode = InterpretationMode.JustParse;
        }

        return await InterpretJoinedStatementsRestAsync(interpretationMode, cancellationToken);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// joinedStatementsRest = [? & ?, statement, joinedStatementsRest];
    /// </code>
    /// </summary>
    private async Task<InterpretationResult> InterpretJoinedStatementsRestAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        if (Lookahead is StatementJoinToken) // 2ая ветка
        {
            Match<StatementJoinToken>();

            InterpretationResult interpretationResult = await InterpretStatementAsync(interpretationMode, cancellationToken);
            if (interpretationResult is InterpretationResult.ImmediateStop)
            {
                // т.к. нужно немедленно остановиться, то остаток кода мы только парсим, но не выполняем
                interpretationMode = InterpretationMode.JustParse;
            }

            return await InterpretJoinedStatementsRestAsync(interpretationMode, cancellationToken);
        }
        else
        {
            return InterpretationResult.Normal; // ϵ-продукция
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// statement = assignVariableStatement | ifStatement | ? Print ? | ? Button ? | ? End ? | ? ClearScreen ? | ? Goto ? | ? Perkill ? | ? Pause ?;
    /// </code>
    /// </summary>
    private async Task<InterpretationResult> InterpretStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken) => Lookahead switch
    {
        Token when Lookahead.IsStartOfIfStatement() => await InterpretIfThenStatement(interpretationMode, cancellationToken),
        Token when Lookahead.IsStartOfAssignVariableStatement() => await InterpretAssignVariableStatementAsync(interpretationMode, cancellationToken),
        PrintToken => await InterpretPrintStatementAsync(interpretationMode, cancellationToken),
        ButtonToken => await InterpretButtonStatementAsync(interpretationMode, cancellationToken),
        EndToken => await InterpretEndStatementAsync(interpretationMode, cancellationToken),
        ClearScreenToken => await InterpretClearScreenStatementAdync(interpretationMode, cancellationToken),
        GotoToken => await InterpretGotoStatementAsync(interpretationMode, cancellationToken),
        PerkillToken => await InterpretPerkillStatementAsync(interpretationMode, cancellationToken),
        PauseToken => await InterpretPauseStatementAsync(interpretationMode, cancellationToken),
        _ => throw new UnexpectedElementException("Ожидалась инструкция", Lookahead),
    };

    private async Task<InterpretationResult> InterpretAssignVariableStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        AssignVariableStatement assignVariableStatement = ParseAssignVariableStatement();

        if (interpretationMode is InterpretationMode.Run)
        {
            await assignVariableStatement.RunAsync(gameContext, cancellationToken);
        }

        return InterpretationResult.Normal;
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// ifStatement = ? If ?, relationExpression, ? Then ?, joinedStatements, [ ? Else ?, joinedStatements ];
    /// </code>
    /// </summary>
    private async Task<InterpretationResult> InterpretIfThenStatement(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        Match<IfToken>();

        RelationExpression relationExpression = ParseRelationExpression();
        Value relationResult = relationExpression.Calculate(gameContext);

        bool isConditionTrue = relationResult.AsDecimal != 0;

        InterpretationResult interpretationResult = InterpretationResult.Normal;

        // парсим [и выполняем] then
        EnterThen();
        try
        {
            Match<ThenToken>("в ветвлении if-then");

            InterpretationResult thenInterpretationResult = await InterpretJoinedStatementsAdaptedAsync(isConditionTrue ? interpretationMode : InterpretationMode.JustParse, cancellationToken);
            if (thenInterpretationResult is InterpretationResult.ImmediateStop)
            {
                interpretationResult = InterpretationResult.ImmediateStop;
            }
        }
        finally
        {
            ExitThen();
        }

        // парсим [и выполняем] else
        if (Lookahead is ElseToken)
        {
            Match<ElseToken>();

            InterpretationResult elseInterpretationResult = await InterpretJoinedStatementsAdaptedAsync(isConditionTrue ? InterpretationMode.JustParse : interpretationMode, cancellationToken);
            if (elseInterpretationResult is InterpretationResult.ImmediateStop)
            {
                interpretationResult = InterpretationResult.ImmediateStop;
            }
        }

        return interpretationResult;
    }

    private async Task<InterpretationResult> InterpretPrintStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        PrintStatement printStatement = ParsePrintTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await printStatement.RunAsync(gameContext, cancellationToken);
        }

        return InterpretationResult.Normal;
    }

    private async Task<InterpretationResult> InterpretButtonStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        ButtonStatement buttonStatement = ParseButtonTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await buttonStatement.RunAsync(gameContext, cancellationToken);
        }

        return InterpretationResult.Normal;
    }

    private async Task<InterpretationResult> InterpretEndStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        EndStatement endStatement = ParseEndTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await endStatement.RunAsync(gameContext, cancellationToken);
            return InterpretationResult.ImmediateStop;
        }

        return InterpretationResult.Normal;
    }

    private async Task<InterpretationResult> InterpretClearScreenStatementAdync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        ClearScreenStatement clearScreenStatement = ParseClearScreenTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await clearScreenStatement.RunAsync(gameContext, cancellationToken);
        }

        return InterpretationResult.Normal;
    }

    private async Task<InterpretationResult> InterpretGotoStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        GotoStatement gotoStatement = ParseGotoTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await gotoStatement.RunAsync(gameContext, cancellationToken);
            return InterpretationResult.ImmediateStop;
        }

        return InterpretationResult.Normal;
    }

    private async Task<InterpretationResult> InterpretPerkillStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        PerkillStatement perkillStatement = ParsePerkillTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await perkillStatement.RunAsync(gameContext, cancellationToken);
        }

        return InterpretationResult.Normal;
    }

    private async Task<InterpretationResult> InterpretPauseStatementAsync(InterpretationMode interpretationMode, CancellationToken cancellationToken)
    {
        PauseStatement pauseStatement = ParsePauseTerminal();

        if (interpretationMode is InterpretationMode.Run)
        {
            await pauseStatement.RunAsync(gameContext, cancellationToken);
        }

        return InterpretationResult.Normal;
    }

    private enum InterpretationMode
    {
        Run,
        JustParse
    }

    private enum InterpretationResult
    {
        /// <summary>
        /// Нормальный режим (продолжаем)
        /// </summary>
        Normal,

        /// <summary>
        /// Немедленная остановка выполнения
        /// </summary>
        ImmediateStop
    }
}
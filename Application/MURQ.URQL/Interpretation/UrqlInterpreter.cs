using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.Domain.Quests.UrqStrings;
using MURQ.URQL.Interpretation.Exceptions;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;
using MURQ.URQL.Tokens.Statements.If;

namespace MURQ.URQL.Interpretation;

public class UrqlInterpreter(IEnumerable<Token> tokens, IGameContext gameContext)
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

        if (_lookahead.IsStartOfStatement()) // 2ая линия
        {
            await RunJoinedStatementsAdaptedAsync(cancellationToken);
        }
        else if (_lookahead is null) // ϵ-продукция
        {
            return;
        }

        if (_lookahead is not null)
            throw new UnexpectedElementException("Ожидался конец строки", _lookahead);
    }

    #region Run statements

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
        if (_lookahead is StatementJoinToken) // 2ая ветка
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
        switch (_lookahead)
        {
            case PrintToken:
                await RunPrintStatementAsync(cancellationToken);
                break;

            case Token when _lookahead.IsStartOfAssignVariableStatement():
                await RunAssignVariableStatementAsync(cancellationToken);
                break;

            case Token when _lookahead.IsStartOfIfStatement():
                await RunIfThenStatement(cancellationToken);
                break;

            default:
                throw new UnexpectedElementException("Ожидалась инструкция", _lookahead);
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

    #endregion

    #region Parse

    #region Parse statements

    private PrintStatement ParsePrintTerminal()
    {
        PrintToken printToken = Match<PrintToken>();

        return new PrintStatement
        {
            UrqString = new([new UrqStringTextPart(printToken.Text)]), // todo убрать UrqString из PrintStatement
            IsNewLineAtEnd = printToken.IsNewLineAtEnd
        };
    }

    private AssignVariableStatement ParseAssignVariableStatement()
    {
        VariableToken variableToken = Match<VariableToken>("в левой части присвоения значения переменной");
        Match<EqualityToken>($"при присвоении значения переменной {variableToken.Name}");
        Expression expression = ParseValueExpression();

        return new AssignVariableStatement
        {
            VariableName = variableToken.Name,
            Expression = expression
        };
    }

    #endregion

    #region Parse expressions

    private RelationExpression ParseRelationExpression()
    {
        Expression leftExpression = ParseValueExpression();
        Match<EqualityToken>("в выражении сравнения значений");
        Expression rightExpression = ParseValueExpression();

        return new RelationExpression { 
            LeftExpression = leftExpression, 
            RightExpression = rightExpression
        };
    }

    private Expression ParseValueExpression() => _lookahead switch
    {
        VariableToken => ParseVariableExpressionTerminal(),
        NumberToken => ParseNumberExpressionTerminal(),
        StringLiteralToken => ParseStringLiteralExpressionTerminal(),
        _ => throw new UnexpectedElementException("Ожидалась переменная, число или строка в кавычках", _lookahead)
    };

    private VariableExpression ParseVariableExpressionTerminal()
    {
        VariableToken variableToken = Match<VariableToken>();
        return new VariableExpression { Name = variableToken.Name };
    }

    private DecimalConstantExpression ParseNumberExpressionTerminal()
    {
        NumberToken numberToken = Match<NumberToken>();
        return new DecimalConstantExpression { Value = numberToken.Value };
    }

    private StringLiteralExpression ParseStringLiteralExpressionTerminal()
    {
        StringLiteralToken stringLiteralToken = Match<StringLiteralToken>();
        return new StringLiteralExpression { Text = stringLiteralToken.Text };
    }

    #endregion 
    
    #endregion

    private TToken Match<TToken>(string? context = null) where TToken : Token
    {
        if (_lookahead is TToken token)
        {
            MoveToNextTerminal();
            return token;
        }
        else throw new UnexpectedTokenException<TToken>(_lookahead, context);
    }

    private void MoveToNextTerminal() => _lookahead = _tokenEnumerator!.MoveNext() ? _tokenEnumerator.Current : null;

    private IEnumerator<Token> _tokenEnumerator = tokens.GetEnumerator();
    private Token? _lookahead;
}

public static class TerminalStartExtensions
{
    public static bool IsStartOfStatement(this Token? token)
        => token.IsStartOfAssignVariableStatement()
        || token.IsStartOfIfStatement()
        || token is StatementToken;

    public static bool IsStartOfAssignVariableStatement(this Token? token) => token is VariableToken;
    public static bool IsStartOfIfStatement(this Token? token) => token is IfToken;
}
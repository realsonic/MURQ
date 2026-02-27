using MURQ.Domain.Games;
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
    public async Task RunUrqlLineAsync(CancellationToken cancellationToken)
    {
        MoveToNextTerminal();

        await RunStatementAsync(cancellationToken);
    }

    #region Run statements

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

    #endregion

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
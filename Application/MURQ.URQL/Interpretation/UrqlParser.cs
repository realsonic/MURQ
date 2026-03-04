using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.Domain.Quests.UrqStrings;
using MURQ.URQL.Interpretation.Exceptions;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;
using MURQ.URQL.Tokens.Statements.If;

namespace MURQ.URQL.Interpretation;

public class UrqlParser(IEnumerable<Token> tokens)
{
    protected AssignVariableStatement ParseAssignVariableStatement()
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

    protected DecimalConstantExpression ParseNumberExpressionTerminal()
    {
        NumberToken numberToken = Match<NumberToken>();
        return new DecimalConstantExpression { Value = numberToken.Value };
    }

    protected PrintStatement ParsePrintTerminal()
    {
        PrintToken printToken = Match<PrintToken>();

        return new PrintStatement
        {
            UrqString = new([new UrqStringTextPart(printToken.Text)]), // todo убрать UrqString из PrintStatement
            IsNewLineAtEnd = printToken.IsNewLineAtEnd
        };
    }

    protected RelationExpression ParseRelationExpression()
    {
        Expression leftExpression = ParseValueExpression();
        Match<EqualityToken>("в выражении сравнения значений");
        Expression rightExpression = ParseValueExpression();

        return new RelationExpression
        {
            LeftExpression = leftExpression,
            RightExpression = rightExpression
        };
    }

    protected StringLiteralExpression ParseStringLiteralExpressionTerminal()
    {
        StringLiteralToken stringLiteralToken = Match<StringLiteralToken>();
        return new StringLiteralExpression { Text = stringLiteralToken.Text };
    }

    protected Expression ParseValueExpression() => Lookahead switch
    {
        VariableToken => ParseVariableExpressionTerminal(),
        NumberToken => ParseNumberExpressionTerminal(),
        StringLiteralToken => ParseStringLiteralExpressionTerminal(),
        _ => throw new UnexpectedElementException("Ожидалась переменная, число или строка в кавычках", Lookahead)
    };

    protected VariableExpression ParseVariableExpressionTerminal()
    {
        VariableToken variableToken = Match<VariableToken>();
        return new VariableExpression { Name = variableToken.Name };
    }

    protected TToken Match<TToken>(string? context = null) where TToken : Token
    {
        if (Lookahead is TToken token)
        {
            MoveToNextTerminal();
            return token;
        }
        else throw new UnexpectedTokenException<TToken>(Lookahead, context);
    }

    protected void MoveToNextTerminal() => Lookahead = _tokenEnumerator.MoveNext() ? _tokenEnumerator.Current : null;

    protected Token? Lookahead { get; private set; }

    private readonly IEnumerator<Token> _tokenEnumerator = tokens.GetEnumerator();
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
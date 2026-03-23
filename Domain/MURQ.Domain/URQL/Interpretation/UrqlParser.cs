using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;
using MURQ.Domain.URQL.Interpretation.Exceptions;
using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Statements;
using MURQ.Domain.URQL.Tokens.Statements.If;

namespace MURQ.Domain.URQL.Interpretation;

public class UrqlParser(UrqlLexer urqlLexer)
{
    #region Statements

    /// <summary>
    /// Грамматика:
    /// <code>
    /// assignVariableStatement = ? Variable ?, ? Equality ? (*""=""*), valueExpression;
    /// </code>
    /// </summary>
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

    protected PrintStatement ParsePrintTerminal()
    {
        PrintToken printToken = Match<PrintToken>();

        return new PrintStatement
        {
            Text = printToken.Text,
            IsNewLineAtEnd = printToken.IsNewLineAtEnd
        };
    }

    protected ButtonStatement ParseButtonTerminal()
    {
        ButtonToken buttonToken = Match<ButtonToken>();

        string label = buttonToken.Label.Trim();
        if (label == string.Empty)
        {
            throw new ParseException($"Метка кнопки пустая: {buttonToken}");
        }

        string caption = buttonToken.Caption.Trim();

        return new ButtonStatement
        {
            Label = label,
            Caption = caption
        };
    }

    protected EndStatement ParseEndTerminal()
    {
        Match<EndToken>();
        return new EndStatement();
    }

    protected ClearScreenStatement ParseClearScreenTerminal()
    {
        Match<ClearScreenToken>();
        return new ClearScreenStatement();
    }

    protected GotoStatement ParseGotoTerminal()
    {
        GotoToken gotoToken = Match<GotoToken>();

        string label = gotoToken.Label.Trim();
        if (label == string.Empty)
        {
            throw new ParseException($"Метка безусловного перехода пустая: {gotoToken}");
        }

        return new GotoStatement { Label = label };
    }

    protected PerkillStatement ParsePerkillTerminal()
    {
        Match<PerkillToken>();
        return new PerkillStatement();
    }

    protected PauseStatement ParsePauseTerminal()
    {
        PauseToken pauseToken = Match<PauseToken>();
        return new PauseStatement { Duration = pauseToken.Duration };
    }

    #endregion

    #region Expressions

    protected DecimalConstantExpression ParseNumberExpressionTerminal()
    {
        NumberToken numberToken = Match<NumberToken>();
        return new DecimalConstantExpression { Value = numberToken.Value };
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// relationExpression = valueExpression, ? Equality ? (*""=""*), valueExpression;
    /// </code>
    /// </summary>
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

    /// <summary>
    /// Грамматика:
    /// <code>
    /// valueExpression = ? Variable ? | ? Number ? | ? StringLiteral ?;
    /// </code>
    /// </summary>
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

    #endregion

    protected void EnterThen()
    {
        _thenCount++;
        urqlLexer.IsElseOpenTextTerminator = _thenCount > 0;
    }

    protected void ExitThen()
    {
        _thenCount--;
        urqlLexer.IsElseOpenTextTerminator = _thenCount > 0;
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

    private readonly IEnumerator<Token> _tokenEnumerator = urqlLexer.Scan().GetEnumerator();

    /// <summary>
    /// Счётчик вложенностей <c>then</c>.
    /// Служит для определения <c>else</c> как терминатора открытого текста.
    /// </summary>
    private int _thenCount = 0;
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
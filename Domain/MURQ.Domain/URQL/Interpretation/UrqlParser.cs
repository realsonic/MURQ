using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Expressions.Logic;
using MURQ.Domain.Quests.Statements;
using MURQ.Domain.URQL.Interpretation.Exceptions;
using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Expressions;
using MURQ.Domain.URQL.Tokens.Expressions.Logic;
using MURQ.Domain.URQL.Tokens.Expressions.Relations;
using MURQ.Domain.URQL.Tokens.Statements;
using MURQ.Domain.URQL.Tokens.Statements.If;

namespace MURQ.Domain.URQL.Interpretation;

public class UrqlParser(UrqlLexer urqlLexer)
{
    #region Statements

    /// <summary>
    /// Грамматика:
    /// <code>
    /// assignVariableStatement = ?Variable?, '=', expression;
    /// </code>
    /// </summary>
    protected AssignVariableStatement ParseAssignVariableStatement()
    {
        VariableToken variableToken = Match<VariableToken>("в левой части присвоения значения переменной");
        Match<EqualityToken>($"при присвоении значения переменной {variableToken.Name}");
        Expression expression = ParseExpression();

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

    /// <summary>
    /// Грамматика:
    /// <code>
    /// expression = orExpression;
    /// </code>
    /// </summary>
    /// <returns><see cref="Expression"/></returns>
    protected Expression ParseExpression() => ParseOrExpression();

    /// <summary>
    /// Грамматика:
    /// <code>
    /// orExpression =
    ///     orExpression, 'or', andExpression
    ///   | andExpression;
    /// </code>
    /// Адаптация через цикл:
    /// <code>
    /// orExpression = andExpression, { 'or', andExpression };
    /// </code>
    /// </summary>
    protected Expression ParseOrExpression()
    {
        Expression resultExpression = ParseAndExpression();

        while (Lookahead is OrToken)
        {
            Match<OrToken>();
            
            Expression rightAndExpression = ParseAndExpression();

            resultExpression = new OrExpression
            {
                LeftExpression = resultExpression,
                RightExpression = rightAndExpression
            };
        }

        return resultExpression;
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// andExpression =
    ///     andExpression, 'and', relationExpression
    ///   | relationExpression;
    /// </code>
    /// Адаптация через цикл:
    /// <code>
    /// andExpression = relationExpression, { 'and', relationExpression };
    /// </code>
    /// </summary>
    private Expression ParseAndExpression()
    {
        Expression resultExpression = ParseRelationExpression();

        while (Lookahead is AndToken)
        {
            Match<AndToken>();

            Expression relationExpression = ParseRelationExpression();

            resultExpression = new AndExpression
            {
                LeftExpression = resultExpression,
                RightExpression = relationExpression
            };
        }

        return resultExpression;
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// relationExpression = additionExpression, [ ( '=' | '<' | '<=' | '>' | '>=' | '<>' ), additionExpression ];
    /// </code>
    /// </summary>
    protected Expression ParseRelationExpression()
    {
        Expression leftExpression = ParseAdditionExpression();

        RelationExpression.RelationKind relationKind;
        switch (Lookahead)
        {
            case EqualityToken:
                Match<EqualityToken>("в выражении сравнения значений");
                relationKind = RelationExpression.RelationKind.Equal;
                break;

            case LessThanToken:
                Match<LessThanToken>("в выражении сравнения значений");
                relationKind = RelationExpression.RelationKind.LessThan;
                break;

            case LessThanOrEqualToken:
                Match<LessThanOrEqualToken>("в выражении сравнения значений");
                relationKind = RelationExpression.RelationKind.LessThanOrEqual;
                break;

            case GreaterThanToken:
                Match<GreaterThanToken>("в выражении сравнения значений");
                relationKind = RelationExpression.RelationKind.GreaterThan;
                break;

            case GreaterThanOrEqualToken:
                Match<GreaterThanOrEqualToken>("в выражении сравнения значений");
                relationKind = RelationExpression.RelationKind.GreaterThanOrEqual;
                break;

            case NotEqualToken:
                Match<NotEqualToken>("в выражении сравнения значений");
                relationKind = RelationExpression.RelationKind.NotEqual;
                break;

            default:
                return leftExpression;
        }

        Expression rightExpression = ParseAdditionExpression();

        return new RelationExpression
        {
            Kind = relationKind,
            LeftExpression = leftExpression,
            RightExpression = rightExpression
        };
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// additionExpression =
    ///     additionExpression, ( '+' | '-' ), multiplicationExpression
    ///   | multiplicationExpression;
    /// </code>
    /// Адаптация через цикл:
    /// <code>
    /// additionExpression = multiplicationExpression, { ( '+' | '-' ), multiplicationExpression };
    /// </code>
    /// </summary>
    /// <returns><see cref="Expression"/></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected Expression ParseAdditionExpression()
    {
        Expression resultExpression = ParseMultiplicationExpression();

        while (Lookahead is PlusToken or MinusToken)
        {
            switch (Lookahead)
            {
                case PlusToken:
                    Match<PlusToken>();
                    resultExpression = new AdditionExpression { LeftExpression = resultExpression, RightExpression = ParseMultiplicationExpression() };
                    break;

                case MinusToken:
                    Match<MinusToken>();
                    resultExpression = new SubstractionExpression { LeftExpression = resultExpression, RightExpression = ParseMultiplicationExpression() };
                    break;

                default:
                    throw new NotImplementedException($"Тип операции {Lookahead} ещё не обрабатывается.");
            }
        }

        return resultExpression;
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// multiplicationExpression =
    ///     multiplicationExpression, ( '*' | "/" ), signExpression
    ///   | signExpression;
    /// </code>
    /// Адаптация через цикл:
    /// <code>
    /// multiplicationExpression = signExpression, { ( '*' | '/' ), signExpression };
    /// </code>
    /// </summary>
    /// <returns><see cref="Expression"/></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected Expression ParseMultiplicationExpression()
    {
        Expression resultExpression = ParseSignExpression();

        while (Lookahead is MultiplicationToken or DivisionToken)
        {
            switch (Lookahead)
            {
                case MultiplicationToken:
                    Match<MultiplicationToken>();
                    resultExpression = new MultiplicationExpression { LeftExpression = resultExpression, RightExpression = ParseFactorExpression() };
                    break;

                case DivisionToken:
                    Match<DivisionToken>();
                    resultExpression = new DivisionExpression { LeftExpression = resultExpression, RightExpression = ParseFactorExpression() };
                    break;

                default:
                    throw new NotImplementedException($"Тип операции {Lookahead} ещё не обрабатывается.");
            }
        }

        return resultExpression;
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// signExpression =
    ///     ( '+' | '-' ), signExpression
    ///   | factorExpression;
    /// </code>
    /// </summary>
    /// <returns><see cref="Expression"/></returns>
    protected Expression ParseSignExpression()
    {
        switch (Lookahead)
        {
            case PlusToken:
                Match<PlusToken>();
                return new UnaryPlusExpression { Expression = ParseSignExpression() };

            case MinusToken:
                Match<MinusToken>();
                return new UnaryMinusExpression { Expression = ParseSignExpression() };

            default:
                return ParseFactorExpression();
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// factor = ('+' | '-'), factor | ?(?, expression, ?)? | ?Variable? | ?Number? | ?StringLiteral?;
    /// </code>
    /// </summary>
    protected Expression ParseFactorExpression()
    {
        switch (Lookahead)
        {
            case LeftBraceToken:
                Match<LeftBraceToken>();
                Expression expression = ParseExpression();
                Match<RightBraceToken>("после открывающей скобки");
                return expression;

            case VariableToken:
                return ParseVariableExpressionTerminal();

            case NumberToken:
                return ParseNumberExpressionTerminal();

            case StringLiteralToken:
                return ParseStringLiteralExpressionTerminal();

            default:
                throw new UnexpectedElementException("Ожидалось одно из: выражение в скобках, переменная, число, строка в кавычках", Lookahead, "в выражении");
        }
    }

    protected VariableExpression ParseVariableExpressionTerminal()
    {
        VariableToken variableToken = Match<VariableToken>();
        return new VariableExpression { Name = variableToken.Name };
    }

    protected DecimalConstantExpression ParseNumberExpressionTerminal()
    {
        NumberToken numberToken = Match<NumberToken>();
        return new DecimalConstantExpression { Value = numberToken.Value };
    }

    protected StringLiteralExpression ParseStringLiteralExpressionTerminal()
    {
        StringLiteralToken stringLiteralToken = Match<StringLiteralToken>();
        return new StringLiteralExpression { Text = stringLiteralToken.Text };
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
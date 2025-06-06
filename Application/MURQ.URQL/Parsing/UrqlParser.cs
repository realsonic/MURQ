﻿using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Expressions;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;
using MURQ.URQL.Tokens.Statements.If;

namespace MURQ.URQL.Parsing;

/// <summary>
/// Парсер URQL по методу рекурсивного спуска.
/// </summary>
/// <remarks>
///     <para>Описание грамматики можно найти в файле <c>URQL Syntax.md</c> в папке документов <c>docs</c>.</para>
///     <para>Грамматики в комментариях методов нетерминалов (<c>Parse*</c>) приведены в нотации EBNF от PlantUML: https://plantuml.com/en/ebnf.</para>
/// </remarks>
public class UrqlParser
{
    public UrqlParser(IEnumerable<Token> tokens)
    {
        enumerator = tokens.GetEnumerator();
        lookahead = NextTerminal();
    }

    public QuestSto ParseQuest()
    {
        List<StatementSto> statements = [.. ParseStatementsAdapted()];

        // обязательно пройти IEnumerable, иначе lookahead не обнулится и проверка ниже упадёт 

        if (lookahead is not null)
        {
            throw new ParseException($"Ожидался конец, а встретился: {lookahead}");
        }

        return new QuestSto(statements);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// statementsAdapted = [statement], statementsRest;
    /// </code>
    /// </summary>
    /// <returns>Перечисление инструкций</returns>
    private IEnumerable<StatementSto> ParseStatementsAdapted()
    {
        if (lookahead.IsStartOfStatement())
        {
            yield return ParseStatement();
        }

        foreach (var statementSto in ParseStatementsRest())
        {
            yield return statementSto;
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// statementsRest = [? New line ?, [statement], statementsRest];
    /// </code>
    /// </summary>
    /// <returns>Перечисление инструкций</returns>
    private IEnumerable<StatementSto> ParseStatementsRest()
    {
        if (lookahead is NewLineToken) // 2ая ветка
        {
            Match<NewLineToken>();

            if (lookahead.IsStartOfStatement())
            {
                yield return ParseStatement();
            }

            foreach (var statementSto in ParseStatementsRest())
            {
                yield return statementSto;
            }
        }
        else
        {
            yield break; // ϵ-продукция
        }
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// statement = assignVariableStatement | ifStatement | ? Label ? | ? Print ? | ? Button ? | ? End ? | ? ClearScreen ?;
    /// </code>
    /// </summary>
    /// <returns>Инструкция</returns>
    private StatementSto ParseStatement() => lookahead switch
    {
        LabelToken => ParseLabel(),
        PrintToken => ParsePrint(),
        ButtonToken => ParseButton(),
        EndToken => ParseEnd(),
        ClearScreenToken => ParseClearScreen(),
        _ when lookahead.IsStartOfAssignVariableStatement() => ParseAssignVariableStatement(),
        _ when lookahead.IsStartOfIfStatement() => ParseIf(),
        _ => throw new ParseException($"Ожидалась инструкция, а встретился {lookahead}.")
    };

    private LabelStatementSto ParseLabel()
    {
        LabelToken labelToken = Match<LabelToken>();

        string label = labelToken.Label.Trim();

        if (label == string.Empty)
        {
            throw new ParseException($"Метка пустая: {labelToken}");
        }

        return new LabelStatementSto(label, labelToken.Location);
    }

    private PrintStatementSto ParsePrint()
    {
        PrintToken printToken = Match<PrintToken>();
        return new PrintStatementSto(printToken.Text, printToken.IsNewLineAtEnd, printToken.Location);
    }

    private ButtonStatementSto ParseButton()
    {
        ButtonToken buttonToken = Match<ButtonToken>();

        string label = buttonToken.Label.Trim();
        if (label == string.Empty)
        {
            throw new ParseException($"Метка кнопки пустая: {buttonToken}");
        }

        string caption = buttonToken.Caption.Trim();

        return new ButtonStatementSto(label, caption, buttonToken.Location);
    }

    private EndStatementSto ParseEnd()
    {
        EndToken endToken = Match<EndToken>();
        return new EndStatementSto(endToken.Location);
    }

    private ClearScreenStatementSto ParseClearScreen()
    {
        ClearScreenToken clearScreenToken = Match<ClearScreenToken>();
        return new ClearScreenStatementSto(clearScreenToken.Location);
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// assignVariableStatement = ? Variable ?,  ? Equality ? (*""=""*), ? Number ?;
    /// </code>
    /// </summary>
    /// <returns>Присвение значения переменной</returns>
    private AssignVariableStatementSto ParseAssignVariableStatement()
    {
        VariableToken variableToken = Match<VariableToken>();
        Match<EqualityToken>();
        NumberToken numberToken = Match<NumberToken>();

        return new AssignVariableStatementSto(variableToken.Name, numberToken.Value, (variableToken.Location, numberToken.Location));
    }

    /// <summary>
    /// Грамматика:
    /// <code>
    /// ifStatement = ? If ?, relationExpression, ? Then ?, statement;
    /// </code>
    /// </summary>
    /// <returns>Условный оператор <c>if-else</c></returns>
    private IfStatementSto ParseIf()
    {
        IfToken ifToken = Match<IfToken>();
        RelationExpressionSto relationExpressionSto = ParseRelationExpression();
        Match<ThenToken>();
        StatementSto thenStatementSto = ParseStatement();

        return new IfStatementSto(relationExpressionSto, thenStatementSto, ifToken.Location.Start);
    }
    
    /// <summary>
    /// Грамматика:
    /// <code>
    /// relationExpression = valueExpression, ? Equality ? (*""=""*), valueExpression;
    /// </code>
    /// </summary>
    /// <returns>Выражение сравнения</returns>
    private RelationExpressionSto ParseRelationExpression()
    {
        ExpressionSto leftExpression = ParseValueExpression();
        Match<EqualityToken>();
        ExpressionSto rightExpression = ParseValueExpression();

        return new RelationExpressionSto(leftExpression, rightExpression);
    }
        
    /// <summary>
    /// Грамматика:
    /// <code>
    /// valueExpression = ? Variable ? | ? Number ?;
    /// </code>
    /// </summary>
    /// <returns>Выражение-значение (переменна или число)</returns>
    private ExpressionSto ParseValueExpression() => lookahead switch
    {
        VariableToken => ParseVariableExpression(),
        NumberToken => ParseNumberExpression(),
        _ => throw new ParseException($"Ожидались переменная или число, а встретился {lookahead}.")
    };

    private VariableExpressionSto ParseVariableExpression()
    {
        VariableToken variableToken = Match<VariableToken>();
        return new VariableExpressionSto(variableToken.Name, variableToken.Location);
    }

    private DecimalConstantExpressionSto ParseNumberExpression()
    {
        NumberToken numberToken = Match<NumberToken>();
        return new DecimalConstantExpressionSto(numberToken.Value, numberToken.Location);
    }

    private TToken Match<TToken>()
    {
        if (lookahead is TToken token)
        {
            lookahead = NextTerminal();
            return token;
        }
        else throw new ParseException($"Ожидался токен {typeof(TToken)}, а встретился {lookahead}");
    }

    private Token? NextTerminal() => enumerator.MoveNext() ? enumerator.Current : null;

    private readonly IEnumerator<Token> enumerator;
    private Token? lookahead;
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

using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Parsers;

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

    private StatementSto ParseStatement()
    {
        return lookahead switch
        {
            LabelToken => ParseLabel(),
            PrintToken => ParsePrint(),
            ButtonToken => ParseButton(),
            EndToken => ParseEnd(),
            _ => throw new ParseException($"Ожидалась инструкция, а встретился {lookahead}."),
        };
    }

    private LabelStatementSto ParseLabel()
    {
        LabelToken labelToken = Match<LabelToken>();
        
        if(labelToken.Label.Length is 0)
        {
            throw new ParseException($"Метка пустая: {labelToken}");
        }
        
        return new LabelStatementSto(labelToken.Label.TrimEnd());
    }

    private PrintStatementSto ParsePrint()
    {
        PrintToken printToken = Match<PrintToken>();
        return new PrintStatementSto(printToken.Text, printToken.IsNewLineAtEnd);
    }

    private ButtonStatementSto ParseButton()
    {
        ButtonToken buttonToken = Match<ButtonToken>();

        if (buttonToken.Label.Length is 0)
        {
            throw new ParseException($"Метка кнопки пустая: {buttonToken}");
        }

        return new ButtonStatementSto(buttonToken.Label.TrimStart(), buttonToken.Caption);
    }

    private EndStatementSto ParseEnd()
    {
        _ = Match<EndToken>();
        return new EndStatementSto();
    }

    private TToken Match<TToken>()
    {
        if (lookahead is TToken token)
        {
            lookahead = NextTerminal();
            return token;
        }
        else throw new ParseException($"Ожидался токен {typeof(TToken)}, а встретился {lookahead}.");
    }

    private Token? NextTerminal() => enumerator.MoveNext() ? enumerator.Current : null;

    private readonly IEnumerator<Token> enumerator;
    private Token? lookahead;
}

public static class TerminalStartExtendsions
{
    public static bool IsStartOfStatement(this Token? token)
    {
        return token is StatementToken; // работает, пока statement - только инструкция без конструкций
    }
}

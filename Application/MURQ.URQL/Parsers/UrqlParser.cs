using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Parsers;

public class UrqlParser
{
    public UrqlParser(IEnumerable<Token> tokens)
    {
        enumerator = tokens.GetEnumerator();
        lookahead = NextTerminal();
    }

    public QuestSto ParseQuest()
    {
        IEnumerable<StatementSto> statements = ParseStatementsAdapted();
        return new QuestSto(statements.ToList());
    }

    private IEnumerable<StatementSto> ParseStatementsAdapted()
    {
        return ParseStatementsRest();
    }

    private IEnumerable<StatementSto> ParseStatementsRest()
    {
        if (lookahead is StatementToken)
        {
            List<StatementSto> statements = [ParseStatement()];
            statements.AddRange(ParseStatementsRest());
            return statements;
        }
        else return [];
    }

    private StatementSto ParseStatement()
    {
        return lookahead switch
        {
            LabelToken => ParseLabel(),
            PrintToken => ParsePrint(),
            _ => throw new ParseException($"Ожидалась инструкция, а встретился {lookahead}."),
        };
    }

    private LabelStatementSto ParseLabel()
    {
        LabelToken labelToken = Match<LabelToken>();
        return new LabelStatementSto(labelToken.Label);
    }

    private PrintStatementSto ParsePrint()
    {
        PrintToken printToken = Match<PrintToken>();
        return new PrintStatementSto(printToken.Text, printToken.IsNewLineAtEnd);
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

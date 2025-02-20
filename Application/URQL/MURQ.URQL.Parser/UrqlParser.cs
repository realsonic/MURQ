using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Tokens.Tokens;
using MURQ.URQL.Tokens.Tokens.Statements;

namespace MURQ.URQL.Parser;

public class UrqlParser
{
    public UrqlParser(IEnumerable<Token> tokens)
    {
        enumerator = tokens.GetEnumerator();
        lookahead = NextTerminal();
    }

    public Quest ParseQuest()
    {
        var statements = ParseStatementsAdapted();
        return new Quest(statements);
    }

    private IEnumerable<Statement> ParseStatementsAdapted()
    {
        return ParseStatementsRest();
    }

    private IEnumerable<Statement> ParseStatementsRest()
    {
        if (lookahead is StatementToken)
        {
            List<Statement> statements = [ParseStatement()];
            statements.AddRange(ParseStatementsRest());
            return statements;
        }
        else return [];
    }

    private Statement ParseStatement()
    {
        return ParsePrint();
    }

    private PrintStatement ParsePrint()
    {
        PrintToken printToken = Match<PrintToken>();
        return new PrintStatement { Text = printToken.Text };
    }

    private TToken Match<TToken>()
    {
        if(lookahead is TToken token)
        {
            lookahead = NextTerminal();
            return token;
        }
        else throw new ParseException($"Ожидался токен {typeof(TToken)}, а встретился {lookahead}.");
    }

    //private void Match(Token token)
    //{
    //    if (lookahead?.GetType() == token.GetType()) lookahead = NextTerminal();
    //    else throw new Exception($"Syntax error: expected '{token}', but met '{lookahead}'");
    //}

    private Token? NextTerminal() => enumerator.MoveNext() ? enumerator.Current : null;

    private readonly IEnumerator<Token> enumerator;
    private Token? lookahead;
}

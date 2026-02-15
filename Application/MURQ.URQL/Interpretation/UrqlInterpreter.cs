using MURQ.Domain.Games;
using MURQ.URQL.Interpretation.Exceptions;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;
using MURQ.URQL.Tokens.Statements.If;

namespace MURQ.URQL.Interpretation;

public class UrqlInterpreter
{
    public UrqlInterpreter(IEnumerable<Token> tokens, IGameContext gameContext)
    {
        enumerator = tokens.GetEnumerator();
        lookahead = NextTerminal();
        this.gameContext = gameContext;
    }

    public async Task RunUrqlLineAsync(CancellationToken cancellationToken)
    {
        await RunStatementAsync(cancellationToken);
    }

    private async Task RunStatementAsync(CancellationToken cancellationToken)
    {
        switch (lookahead)
        {
            case PrintToken:
                await RunPrintTerminalAsync(cancellationToken);
                break;

            default:
                throw new UnexpectedElementException("Ожидалась инструкция", lookahead);
        }
    }

    private async Task RunPrintTerminalAsync(CancellationToken cancellationToken)
    {
        PrintToken printToken = Match<PrintToken>();

        if (printToken.IsNewLineAtEnd)
            gameContext.PrintLine(printToken.Text);
        else
            gameContext.Print(printToken.Text);
    }

    private TToken Match<TToken>(string? context = null) where TToken : Token
    {
        if (lookahead is TToken token)
        {
            lookahead = NextTerminal();
            return token;
        }
        else throw new UnexpectedTokenException<TToken>(lookahead, context);
    }

    private Token? NextTerminal() => enumerator!.MoveNext() ? enumerator.Current : null;

    private readonly IEnumerator<Token> enumerator;
    private Token? lookahead;
    private readonly IGameContext gameContext;
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
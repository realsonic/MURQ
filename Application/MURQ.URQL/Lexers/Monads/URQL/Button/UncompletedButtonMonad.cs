using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexers.Monads.URQL.Button;

public record UncompletedButtonMonad(bool IsCommaMet, string Label, string Caption, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ',' when !IsCommaMet && Label.Length > 0 => new UncompletedButtonMonad(true, Label, string.Empty, Lexeme + character, Location.EndAt(position)),
        '\n' when IsCommaMet && Caption.Length > 0 => new CompletedLexemeMonad(new ButtonToken(Label, Caption, Lexeme, Location), null),
        not '\n' when !IsCommaMet => new UncompletedButtonMonad(false, Label + character, string.Empty, Lexeme + character, Location.EndAt(position)),
        not '\n' when IsCommaMet => new UncompletedButtonMonad(true, Label, Caption + character, Lexeme + character, Location.EndAt(position)),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position)),
    };

    public override LexemeMonad Finalize() => IsCommaMet && Caption.Length > 0
            ? new CompletedLexemeMonad(new ButtonToken(Label, Caption, Lexeme, Location), null)
            : new UnknownLexemeMonad(Lexeme, Location);
}

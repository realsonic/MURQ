using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
using MURQ.Domain.URQL.Lexing.Exceptions;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Statements;

using System.Text;

namespace MURQ.Domain.URQL.Lexing;

public class UrqlLexer(IEnumerable<OriginatedCharacter> source)
{
    public IEnumerable<Token> Scan()
    {
        MoveToNextCharacter();

        while (Lookahead is not null)
        {
            switch (Lookahead.Value.Character)
            {
                case ' ' or '\t':
                    Match();
                    break;

                case char when char.IsLetter(Lookahead.Value.Character):
                    yield return ParseWord();
                    break;

                case '_':
                    yield return ParseVariable(string.Empty);
                    break;

                case '=':
                    yield return ParseEquality();
                    break;

                /*  TODO
                        '"' => StringLiteralMonad.StartAfterOpeningQuote(character, position),
                        '&' => new StatementJoinToken(character.ToString(), Location.StartAt(position)).AsMonad(),
                        _ when char.IsDigit(character) => NumberMonad.Start(character, position),

                 */

                default: throw new UnknownLexemeException(GetLexemeData());
            }

            _lexeme.Clear();
        }

        if (_lexeme.Count > 0)
            throw new UnknownLexemeException(GetLexemeData());
    }

    private Token ParseWord()
    {
        StringBuilder wordBuilder = new();

        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            switch (Lookahead.Value.Character)
            {
                case char when char.IsLetter(Lookahead.Value.Character):
                    wordBuilder.Append(Lookahead.Value.Character);
                    Match();
                    continue;

                case '_':
                case char when char.IsDigit(Lookahead.Value.Character):
                    return ParseVariable(wordBuilder.ToString());

                default:
                    isTerminated = true;
                    break;
            }
        }

        if (_lexeme.Count is 0)
            throw new InvalidOperationException($"Лексема оказалась пустой!");

        string collectedWord = wordBuilder.ToString();
        return collectedWord.ToLower() switch
        {
            "p" => ParsePrintAfterKeyword(false),
            "pln" => ParsePrintAfterKeyword(true),
            "btn" => ParseButtonAfterBtn(),
            "end" => ParseEndAfterEnd(),
            //TODO
            //"cls" =>
            //"if" => n
            //"then" =>
            //"else" =>
            "goto" => ParseGotoAfterGoto(),
            //"perkill"
            //"pause" =
            _ => ParseVariable(collectedWord)
        };
    }

    private PrintToken ParsePrintAfterKeyword(bool isNewLine)
    {
        // После p/pln разрешён пробел либо конец
        if (Lookahead?.Character is ' ' or '\t')
        {
            Match();
        }
        else if (Lookahead is not null)
            throw new LexingException($"После команды p/pln ожидался пробел или табуляция.", GetLexemeData());

        // Собираем текст
        StringBuilder textBuilder = new();
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            switch (Lookahead.Value.Character)
            {
                case '&':
                    isTerminated = true;
                    break;

                // todo добавить else как терминатор, если мы внутри if - или(!) в парсере

                default:
                    textBuilder.Append(Lookahead.Value.Character);
                    Match();
                    break;
            }
        }

        (string lexeme, Location location) = GetLexemeData();
        return new PrintToken(textBuilder.ToString(), isNewLine, lexeme, location);
    }

    private ButtonToken ParseButtonAfterBtn()
    {
        // После btn разрешён пробел или табуляция
        if (Lookahead?.Character is ' ' or '\t')
            Match();
        else
            throw new LexingException($"После btn ожидался пробел или табуляция.", GetLexemeData());

        // Собираем метку
        StringBuilder labelBuilder = new();
        bool isCommaMet = false;
        while (Lookahead is not null && !isCommaMet)
        {
            if (Lookahead.Value.Character is ',')
            {
                isCommaMet = true;
            }
            else
            {
                labelBuilder.Append(Lookahead.Value.Character);
            }

            Match();
        }

        if (!isCommaMet)
            throw new LexingException($"В команде btn после метки ожидалась запятая.", GetLexemeData());

        // Собираем надпись
        StringBuilder captionBuilder = new();
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            switch (Lookahead.Value.Character)
            {
                case '&':
                    isTerminated = true;
                    break;

                // todo добавить else как терминатор, если мы внутри if - или(!) в парсере

                default:
                    captionBuilder.Append(Lookahead.Value.Character);
                    Match();
                    break;
            }
        }

        (string lexeme, Location location) = GetLexemeData();
        return new ButtonToken(labelBuilder.ToString().Trim(), captionBuilder.ToString(), lexeme, location);
    }

    private EndToken ParseEndAfterEnd()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new EndToken(lexeme, location);
    }

    private GotoToken ParseGotoAfterGoto()
    {
        // После goto разрешён пробел или табуляция
        if (Lookahead?.Character is ' ' or '\t')
            Match();
        else
            throw new LexingException($"После goto ожидался пробел или табуляция.", GetLexemeData());

        StringBuilder labelBuilder = new();
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            switch (Lookahead.Value.Character)
            {
                case '&':
                    isTerminated = true;
                    break;

                // todo добавить else как терминатор, если мы внутри if - или(!) в парсере

                default:
                    labelBuilder.Append(Lookahead.Value.Character);
                    Match();
                    break;
            }
        }

        (string lexeme, Location location) = GetLexemeData();
        return new GotoToken(labelBuilder.ToString().Trim(), lexeme, location);
    }

    private VariableToken ParseVariable(string collectedName)
    {
        throw new NotImplementedException();
    }

    private EqualityToken ParseEquality()
    {
        throw new NotImplementedException();
    }

    private (string Lexeme, Location Location)? TryGetLexemeData()
    {
        if (_lexeme.Count is 0)
            return null;

        string lexemeString = _lexeme.ToPlainString();

        Location location = Location
            .StartAt(_lexeme.First().Origin.GetLocation().Start)
            .EndAt(_lexeme.Last().Origin.GetLocation().End);

        return (Lexeme: lexemeString, Location: location);
    }

    private (string Lexeme, Location Location) GetLexemeData()
        => TryGetLexemeData() ?? throw new InvalidOperationException("Лексема оказалось пустой!");

    private void Match()
    {
        if (Lookahead is null)
            throw new InvalidOperationException($"Следующий символ ({nameof(Lookahead)}) оказался пустой!");

        _lexeme.Add(Lookahead.Value);
        MoveToNextCharacter();
    }

    private void MoveToNextCharacter() => Lookahead = _characterEnumerator.MoveNext() ? _characterEnumerator.Current : null;

    private OriginatedCharacter? Lookahead { get; set; }

    private readonly List<OriginatedCharacter> _lexeme = [];
    private readonly IEnumerator<OriginatedCharacter> _characterEnumerator = source.GetEnumerator();
}

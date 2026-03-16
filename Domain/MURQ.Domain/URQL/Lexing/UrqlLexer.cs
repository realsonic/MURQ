using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
using MURQ.Domain.URQL.Lexing.Exceptions;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Statements;
using MURQ.Domain.URQL.Tokens.Statements.If;

using System.Text;

namespace MURQ.Domain.URQL.Lexing;

public class UrqlLexer(IEnumerable<OriginatedCharacter> source)
{
    public IEnumerable<Token> Scan()
    {
        MoveToNextCharacter();

        while (Lookahead is not null)
        {
            char character = Lookahead.Value.Character;
            switch (character)
            {
                case ' ' or '\t':
                    Match(character);
                    break;

                case char when char.IsLetter(character):
                    yield return ParseWord();
                    break;

                case char when char.IsDigit(character):
                    yield return ParseNumberLiteral();
                    break;

                case '"':
                    yield return ParseStringLiteral();
                    break;

                case '_':
                    yield return ParseVariable(string.Empty);
                    break;

                case '=':
                    yield return ParseEquality();
                    break;

                case '&':
                    yield return ParseStatementJoin();
                    break;

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
            char character = Lookahead.Value.Character;
            switch (character)
            {
                case char when char.IsLetter(character):
                    wordBuilder.Append(character);
                    Match(character);
                    continue;

                case '_':
                case char when char.IsDigit(character):
                    return ParseVariable(wordBuilder.ToString());

                default:
                    isTerminated = true;
                    break;
            }
        }

        string collectedWord = wordBuilder.ToString();
        return collectedWord.ToLower() switch
        {
            "p" => ParsePrintAfterKeyword(false),
            "pln" => ParsePrintAfterKeyword(true),
            "btn" => ParseButtonAfterBtn(),
            "end" => ParseEndAfterEnd(),
            "cls" => ParseClearScreenAfterCls(),
            "if" => ParseIfAfterIf(),
            "then" => ParseThenAfterThen(),
            "else" => ParseElseAfterThen(),
            "goto" => ParseGotoAfterGoto(),
            "perkill" => ParsePerkillAfterPerkill(),
            "pause" => ParsePauseAfterPause(),
            _ => ParseVariable(collectedWord)
        };
    }

    private PrintToken ParsePrintAfterKeyword(bool isNewLine)
    {
        // После p/pln разрешён пробел либо конец
        AllowSpaceOrTab("p/pln");

        // Собираем текст
        string text = ParseTextTillTermination();

        (string lexeme, Location location) = GetLexemeData();
        return new PrintToken(text, isNewLine, lexeme, location);
    }

    private ButtonToken ParseButtonAfterBtn()
    {
        // После btn разрешён пробел или табуляция
        AllowSpaceOrTab("btn");

        // Собираем метку
        StringBuilder labelBuilder = new();
        bool isCommaMet = false;
        while (Lookahead is not null && !isCommaMet)
        {
            char character = Lookahead.Value.Character;
            if (character is ',')
            {
                isCommaMet = true;
            }
            else
            {
                labelBuilder.Append(character);
            }

            Match(character);
        }

        if (!isCommaMet)
            throw new LexingException($"В команде btn после метки ожидалась запятая", GetLexemeData());

        // Собираем надпись
        string caption = ParseTextTillTermination();

        (string lexeme, Location location) = GetLexemeData();
        return new ButtonToken(labelBuilder.ToString().Trim(), caption, lexeme, location);
    }

    private EndToken ParseEndAfterEnd()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new EndToken(lexeme, location);
    }

    private ClearScreenToken ParseClearScreenAfterCls()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new ClearScreenToken(lexeme, location);
    }

    private IfToken ParseIfAfterIf()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new IfToken(lexeme, location);
    }

    private ThenToken ParseThenAfterThen()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new ThenToken(lexeme, location);
    }

    private ElseToken ParseElseAfterThen()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new ElseToken(lexeme, location);
    }

    private GotoToken ParseGotoAfterGoto()
    {
        // После goto разрешён пробел или табуляция
        AllowSpaceOrTab("goto");

        // Собираем метку
        var label = ParseTextTillTermination();

        (string lexeme, Location location) = GetLexemeData();
        return new GotoToken(label.Trim(), lexeme, location);
    }

    private PerkillToken ParsePerkillAfterPerkill()
    {
        (string lexeme, Location location) = GetLexemeData();
        return new PerkillToken(lexeme, location);
    }

    private PauseToken ParsePauseAfterPause()
    {
        AllowSpaceOrTab("pause");

        decimal duration = ParseNumber();

        (string lexeme, Location location) = GetLexemeData();
        return new PauseToken((int)duration, lexeme, location);
    }

    private VariableToken ParseVariable(string collectedName)
    {
        StringBuilder nameBuilder = new(collectedName);
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            char character = Lookahead.Value.Character;
            switch (character)
            {
                case char when char.IsLetterOrDigit(character):
                case '_':
                    nameBuilder.Append(character);
                    Match(character);
                    continue;

                default:
                    isTerminated = true;
                    break;
            }
        }

        (string lexeme, Location location) = GetLexemeData();
        return new VariableToken(nameBuilder.ToString(), lexeme, location);
    }

    private EqualityToken ParseEquality()
    {
        Match('=');

        (string lexeme, Location location) = GetLexemeData();
        return new EqualityToken(lexeme, location);
    }

    private StatementJoinToken ParseStatementJoin()
    {
        Match('&');

        (string lexeme, Location location) = GetLexemeData();
        return new StatementJoinToken(lexeme, location);
    }

    private StringLiteralToken ParseStringLiteral()
    {
        Match('"');

        // Собираем текст внутри кавычек
        StringBuilder textBuilder = new();
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            char character = Lookahead.Value.Character;
            switch (character)
            {
                case '"':
                    isTerminated = true;
                    break;

                default:
                    textBuilder.Append(character);
                    Match(character);
                    break;
            }
        }

        Match('"');

        (string lexeme, Location location) = GetLexemeData();
        return new StringLiteralToken(textBuilder.ToString(), lexeme, location);
    }

    private NumberToken ParseNumberLiteral()
    {
        decimal number = ParseNumber();
        (string lexeme, Location location) = GetLexemeData();
        return new NumberToken(number, lexeme, location);
    }

    private void AllowSpaceOrTab(string commandName)
    {
        switch (Lookahead?.Character)
        {
            case ' ' or '\t':
                Match(Lookahead.Value.Character);
                break;

            default:
                if (Lookahead is not null)
                    throw new LexingException($"После команды {commandName} ожидался пробел или табуляция", GetLexemeData());
                break;
        }
    }

    private string ParseTextTillTermination()
    {
        StringBuilder textBuilder = new();
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            char character = Lookahead.Value.Character;
            switch (character)
            {
                case '&':
                    isTerminated = true;
                    break;

                // todo добавить else как терминатор, если мы внутри if - или(!) в парсере

                default:
                    textBuilder.Append(character);
                    Match(character);
                    break;
            }
        }
        return textBuilder.ToString();
    }

    private decimal ParseNumber()
    {
        // Собираем число    
        StringBuilder numberBuilder = new();
        bool isTerminated = false;
        while (Lookahead is not null && !isTerminated)
        {
            char character = Lookahead.Value.Character;
            switch (character)
            {
                case char when char.IsDigit(character):
                    numberBuilder.Append(character);
                    Match(character);
                    break;

                default:
                    isTerminated = true;
                    break;
            }
        }

        // Превращаем в число
        string numberString = numberBuilder.ToString();
        return decimal.TryParse(numberString, out decimal number)
            ? number
            : throw new LexingException($"{numberString} не подходит как число", GetLexemeData());
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

    private void Match(char expectedCharacter)
    {
        if (Lookahead is null)
            throw new LexingException($"Ожидался '{expectedCharacter}', а встретился конец строки", GetLexemeData());

        if (Lookahead.Value.Character != expectedCharacter)
            throw new LexingException($"Ожидался '{expectedCharacter}', а встретился '{Lookahead.Value.Character}'", GetLexemeData());

        _lexeme.Add(Lookahead.Value);
        MoveToNextCharacter();
    }

    private void MoveToNextCharacter() => Lookahead = _characterEnumerator.MoveNext() ? _characterEnumerator.Current : null;

    private OriginatedCharacter? Lookahead { get; set; }

    private readonly List<OriginatedCharacter> _lexeme = [];
    private readonly IEnumerator<OriginatedCharacter> _characterEnumerator = source.GetEnumerator();
}

using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
using MURQ.Domain.URQL.Lexing.Exceptions;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Expressions;
using MURQ.Domain.URQL.Tokens.Expressions.Logic;
using MURQ.Domain.URQL.Tokens.Expressions.Relations;
using MURQ.Domain.URQL.Tokens.Statements;
using MURQ.Domain.URQL.Tokens.Statements.If;

using System.Text;

namespace MURQ.Domain.URQL.Lexing;

public class UrqlLexer(IEnumerable<OriginatedCharacter> source)
{
    /// <summary>
    /// Является ли <c>else</c> терминатором для открытого текста.
    /// </summary>
    /// <remarks>
    ///     Примеры открытого текста:
    ///     <list type="bullet">
    ///         <item><c>pln Текст</c></item>
    ///         <item><c>btn Метка,Надпись</c></item>
    ///         <item><c>goto Метка</c></item>
    ///     </list>
    /// </remarks>
    public bool IsElseOpenTextTerminator { get; set; } = false;

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

                case '<':
                    yield return ParseLessAndNotEqualRelations();
                    break;

                case '>':
                    yield return ParseGreaterThan();
                    break;

                case '+':
                    yield return ParsePlus();
                    break;

                case '-':
                    yield return ParseMinus();
                    break;

                case '*':
                    yield return ParseMultiplication();
                    break;

                case '/':
                    yield return ParseDivision();
                    break;

                case '(':
                    yield return ParseLeftBrace();
                    break;

                case ')':
                    yield return ParseRightBrace();
                    break;

                case '&':
                    yield return ParseStatementJoin();
                    break;

                default: throw new UnknownLexemeException(new Lexeme(Lookahead.Value.Character.ToString(), Lookahead.Value.Origin.GetLocation()));
            }

            _lexemeCharacters.Clear();

            while (_postponedTokens.Count > 0)
            {
                Token token = _postponedTokens.Dequeue();
                yield return token;
            }
        }

        if (_lexemeCharacters.Count > 0)
            throw new UnknownLexemeException(GetLexeme());
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
            "and" => ParseAndAfterAnd(),
            "or" => ParseOrAfterOr(),
            _ => ParseVariable(collectedWord)
        };
    }

    private PrintToken ParsePrintAfterKeyword(bool isNewLine)
    {
        // После p/pln разрешён пробел либо конец
        AllowSpaceOrTab("p/pln");

        // Собираем текст
        string text = ParseTextTillTermination();

        return new PrintToken(text, isNewLine, GetLexeme());
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
            throw new LexingException($"В команде btn после метки ожидалась запятая", GetLexeme());

        // Собираем надпись
        string caption = ParseTextTillTermination();

        return new ButtonToken(labelBuilder.ToString().Trim(), caption, GetLexeme());
    }

    private EndToken ParseEndAfterEnd() => new(GetLexeme());

    private ClearScreenToken ParseClearScreenAfterCls() => new(GetLexeme());

    private IfToken ParseIfAfterIf() => new(GetLexeme());

    private ThenToken ParseThenAfterThen() => new(GetLexeme());

    private ElseToken ParseElseAfterThen() => new(GetLexeme());

    private GotoToken ParseGotoAfterGoto()
    {
        // После goto разрешён пробел или табуляция
        AllowSpaceOrTab("goto");

        // Собираем метку
        var label = ParseTextTillTermination();

        return new GotoToken(label.Trim(), GetLexeme());
    }

    private PerkillToken ParsePerkillAfterPerkill() => new(GetLexeme());

    private PauseToken ParsePauseAfterPause()
    {
        AllowSpaceOrTab("pause");

        decimal duration = ParseNumber();

        return new PauseToken((int)duration, GetLexeme());
    }

    private AndToken ParseAndAfterAnd() => new(GetLexeme());

    private OrToken ParseOrAfterOr() => new(GetLexeme());

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

        return new VariableToken(nameBuilder.ToString(), GetLexeme());
    }

    private EqualityToken ParseEquality()
    {
        Match('=');

        return new EqualityToken(GetLexeme());
    }

    private RelationToken ParseLessAndNotEqualRelations()
    {
        Match('<');

        switch (Lookahead?.Character)
        {
            case '=':
                Match('=');
                return new LessThanOrEqualToken(GetLexeme());

            case '>':
                Match('>');
                return new NotEqualToken(GetLexeme());

            default:
                return new LessThanToken(GetLexeme());
        }
    }

    private RelationToken ParseGreaterThan()
    {
        Match('>');

        if (Lookahead?.Character == '=')
        {
            Match('=');
            return new GreaterThanOrEqualToken(GetLexeme());
        }

        return new GreaterThanToken(GetLexeme());
    }

    private PlusToken ParsePlus()
    {
        Match('+');

        return new PlusToken(GetLexeme());
    }

    private MinusToken ParseMinus()
    {
        Match('-');

        return new MinusToken(GetLexeme());
    }

    private MultiplicationToken ParseMultiplication()
    {
        Match('*');

        return new MultiplicationToken(GetLexeme());
    }

    private DivisionToken ParseDivision()
    {
        Match('/');

        return new DivisionToken(GetLexeme());
    }

    private LeftBraceToken ParseLeftBrace()
    {
        Match('(');

        return new LeftBraceToken(GetLexeme());
    }

    private RightBraceToken ParseRightBrace()
    {
        Match(')');

        return new RightBraceToken(GetLexeme());
    }

    private StatementJoinToken ParseStatementJoin()
    {
        Match('&');

        return new StatementJoinToken(GetLexeme());
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

        return new StringLiteralToken(textBuilder.ToString(), GetLexeme());
    }

    private NumberToken ParseNumberLiteral()
    {
        decimal number = ParseNumber();
        return new NumberToken(number, GetLexeme());
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
                    throw new LexingException($"После команды {commandName} ожидался пробел или табуляция", GetLexeme());
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

                // else как терминатор
                case ' ' or '\t' when IsElseOpenTextTerminator && textBuilder.Length >= 5 && textBuilder[^5] is ' ' or '\t':
                    string lastChars = textBuilder.ToString(textBuilder.Length - 4, 4); // пробел впереди уже проверили
                    if (lastChars.Equals("else", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // вырезаем else как терминатор

                        textBuilder.Remove(textBuilder.Length - 5, 5); // убираем с пробелом впереди

                        List<OriginatedCharacter> elseTail = _lexemeCharacters.GetRange(_lexemeCharacters.Count - 4, 4); // берём без пробела впереди
                        Location elseLocation = Location
                            .StartAt(elseTail.First().Origin.GetLocation().Start)
                            .EndAt(elseTail.Last().Origin.GetLocation().End);
                        _postponedTokens.Enqueue(new ElseToken(new Lexeme(elseTail.ToLiteral(), elseLocation)));

                        _lexemeCharacters.RemoveRange(_lexemeCharacters.Count - 5, 5); // убираем с пробелом впереди

                        isTerminated = true;
                    }
                    else
                    {
                        // поглощаем в текст
                        textBuilder.Append(character);
                        Match(character);
                    }
                    break;

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
            : throw new LexingException($"{numberString} не подходит как число", GetLexeme());
    }

    private Lexeme? TryGetLexeme()
    {
        if (_lexemeCharacters.Count is 0)
            return null;

        string literal = _lexemeCharacters.ToLiteral();

        Location location = Location
            .StartAt(_lexemeCharacters.First().Origin.GetLocation().Start)
            .EndAt(_lexemeCharacters.Last().Origin.GetLocation().End);

        return new Lexeme(literal, location);
    }

    private Lexeme GetLexeme()
        => TryGetLexeme() ?? throw new InvalidOperationException("Лексема оказалось пустой");

    private void Match(char expectedCharacter)
    {
        if (Lookahead is null)
            throw new LexingException($"Ожидался '{expectedCharacter}', а встретился конец строки", GetLexeme());

        if (Lookahead.Value.Character != expectedCharacter)
            throw new LexingException($"Ожидался '{expectedCharacter}', а встретился '{Lookahead.Value.Character}'", GetLexeme());

        _lexemeCharacters.Add(Lookahead.Value);
        MoveToNextCharacter();
    }

    private void MoveToNextCharacter() => Lookahead = _characterEnumerator.MoveNext() ? _characterEnumerator.Current : null;

    private OriginatedCharacter? Lookahead { get; set; }

    private readonly List<OriginatedCharacter> _lexemeCharacters = [];
    private readonly IEnumerator<OriginatedCharacter> _characterEnumerator = source.GetEnumerator();
    private readonly Queue<Token> _postponedTokens = [];
}

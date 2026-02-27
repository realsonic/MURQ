using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Interpretation;
using MURQ.URQL.Lexing;
using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions;
using MURQ.URQL.Tokens;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 5";

IEnumerable<List<(char Character, Position Position)>> sourceLines = ReadFile(@"Demo5.qst")
    .ToEnumerableWithoutCarriageReturn()
    .ToPositionedEnumerable()
    .ToEnumerableWithoutComments()
    .ToEnumerableWithoutLineContinuations()
    .SplitByLineBreaks();

List<SubstitutionTree> substitutionTrees = [.. ConvertToSubstitutionTrees(sourceLines)];

Demo5GameContext gameContext = new();

foreach (var substitutionTree in substitutionTrees)
{
    IEnumerable<char> sourceLine = substitutionTree.ToRawUrql(gameContext).Select(element => element.Character);
    UrqlLexer urqlLexer = new(sourceLine);
    IEnumerable<Token> lineTokens = urqlLexer.Scan();
    UrqlInterpreter urqlInterpreter = new(lineTokens, gameContext);
    await urqlInterpreter.RunUrqlLineAsync(default);
}

Console.Write("\n>> Нажмите любую клавишу для выхода. <<");
Console.ReadKey(true);


static IEnumerable<char> ReadFile(string filePath)
{
    using StreamReader streamReader = File.OpenText(filePath);
    const int bufferSize = 1024 * 8; // 8 KB — хороший компромисс
    char[] buffer = new char[bufferSize];

    int charsRead;
    while ((charsRead = streamReader.ReadBlock(buffer, 0, bufferSize)) > 0)
    {
        for (int i = 0; i < charsRead; i++)
            yield return buffer[i];
    }
}

static IEnumerable<SubstitutionTree> ConvertToSubstitutionTrees(IEnumerable<IEnumerable<(char Character, Position Position)>> lines)
{
    foreach (var line in lines)
    {
        SubstitutionParser substitutionParser = new(new SubstitutionLexer());
        yield return substitutionParser.ParseLine(line);
    }
}

class Demo5GameContext : IGameContext
{
    public void AddButton(string caption, LabelStatement? labelStatement)
    {
        throw new NotImplementedException();
    }

    public void AssignVariable(string name, Value value)
    {
        _variables[name] = new Variable(name, value);
    }

    public void ClearScreen()
    {
        throw new NotImplementedException();
    }

    public void EndLocation()
    {
        throw new NotImplementedException();
    }

    public Variable? GetVariable(string variableName) => _variables.TryGetValue(variableName, out Variable? variable) ? variable : null;

    public void Goto(LabelStatement? labelStatement)
    {
        throw new NotImplementedException();
    }

    public void Perkill()
    {
        throw new NotImplementedException();
    }

    public void Print(string? text)
    {
        throw new NotImplementedException();
    }

    public void PrintLine(string? text = null) => Console.WriteLine(text);

    private Dictionary<string, Variable> _variables = new(StringComparer.InvariantCultureIgnoreCase);
}
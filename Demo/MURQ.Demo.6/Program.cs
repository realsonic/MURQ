using MURQ.Application.UrqLoaders;
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.URQL.Interpretation;
using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;
using MURQ.URQL.Substitutions;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 6";

UrqLoader urqLoader = new(new SubstitutionParser(new SubstitutionLexer()));
Quest quest = urqLoader.LoadQuest(ReadFile(@"Demo6.qst"));

Demo6GameContext gameContext = new();

foreach (CodeLine codeLine in quest.QuestLines.OfType<CodeLine>())
{
    IEnumerable<(char Character, Position Position)> sourceLine = codeLine.ToCode(gameContext);
    UrqlLexer urqlLexer = new(sourceLine);
    IEnumerable<Token> lineTokens = urqlLexer.Scan();
    UrqlInterpreter urqlInterpreter = new(lineTokens, gameContext);
    await urqlInterpreter.InterpretStatementLineAsync(default);
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

class Demo6GameContext : IGameContext
{
    public void AddButton(string caption, string label)
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

    public void Goto(string label)
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

    private readonly Dictionary<string, Variable> _variables = new(StringComparer.InvariantCultureIgnoreCase);
}
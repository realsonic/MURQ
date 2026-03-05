using MURQ.Domain.Quests.Locations;
using MURQ.Domain.Quests.QuestLines;
using MURQ.URQL.Interpretation;
using MURQ.URQL.Lexing;
using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Processor.Json;
using MURQ.URQL.Substitutions;
using MURQ.URQL.Tokens;

using System.Diagnostics;

Console.WriteLine("MURQ. Утилита обработки URQL. v.0.1\n");

if (args is not [string urqlFilePath])
{
    Console.WriteLine("Нет обязательного параметра - пути к URQL-файлу.");
    return;
}

TimeSpan totalTime = default;
Stopwatch stopwatch = Stopwatch.StartNew();

List<char> source = [.. Utilities.ReadFile(urqlFilePath)];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Исходный файл ----------------------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {source.ToJoinedString()}
    ----------------------------------------------------------------------

    """);

stopwatch.Restart();
List<char> trimmedSource = [.. source.ToEnumerableWithoutCarriageReturn()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 1. Удаление ненужных символов -------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {trimmedSource.ToJoinedString()}
    ----------------------------------------------------------------------
        
    """);

stopwatch.Restart();
List<(char Character, Position Position)> positionedSource = [.. trimmedSource.ToPositionedEnumerable()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 2. Добавление координат -------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {string.Join(", ", positionedSource.Select(pc => $"{pc.Character.ToPrintableChar()}{pc.Position}"))}
    ----------------------------------------------------------------------
        
    """);

stopwatch.Restart();
List<(char Character, Position Position)> uncommentedSource = [.. positionedSource.ToEnumerableWithoutComments()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 3. Удаление комментариев ------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {uncommentedSource.ToJoinedString()}
    ----------------------------------------------------------------------
        
    """);

stopwatch.Restart();
List<(char Character, Position Position)> uncontinuedSource = [.. uncommentedSource.ToEnumerableWithoutLineContinuations()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 4. Схлопывание переносов ------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {uncontinuedSource.ToJoinedString()}
    ----------------------------------------------------------------------

    """);

stopwatch.Restart();
List<List<(char Character, Position Position)>> lines = [.. uncontinuedSource.SplitByLineBreaks()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 5. Разбивка на строки ---------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {lines.ToJoinedNumberedLines(line => line.ToJoinedString())}
    ----------------------------------------------------------------------

    """);

stopwatch.Restart();
List<CodeLine> codeLines = [.. lines.ToCodeLines()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 6. Распознавание подстановок --------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {codeLines.ToJoinedNumberedLines(Serializer.Serialize)}
    ----------------------------------------------------------------------

    """);

Console.WriteLine($"Сумма времени всех этапов: \t{totalTime}");

// тест цепочки
Console.Write("Общее время единой цепочкой: \t");
stopwatch.Restart();
List<CodeLine> result = Utilities.ReadFile(urqlFilePath)
    .ToEnumerableWithoutCarriageReturn()
    .ToPositionedEnumerable()
    .ToEnumerableWithoutComments()
    .ToEnumerableWithoutLineContinuations()
    .SplitByLineBreaks()
    .ToCodeLines()
    .ToList();
stopwatch.Stop();
Console.WriteLine($"{stopwatch.Elapsed}\n");

stopwatch.Restart();
List<List<(char Character, Position Position)>> urqlLines = [.. codeLines.Select(substitutionTree => substitutionTree.ToCode(new GameContextEmulation()).ToList())];
stopwatch.Stop();
Utilities.WriteBlock(
    "Шаг 1. Эмуляция раскрытия подстановок",
    urqlLines.Select(line => line.ToJoinedString()),
    stopwatch);

stopwatch.Restart();
List<List<Token>> tokenLines = [.. urqlLines.Select(line => new UrqlLexer(line.Select(element => element.Character)).Scan().ToList())];
stopwatch.Stop();
Utilities.WriteBlock(
    "Шаг 2. Эмуляция получения токенов",
    tokenLines.Select(line => line.ToJoinedString()),
    stopwatch);

Console.WriteLine("-->> Шаг 3. Эмуляция интерпретации ------------------------------------------");
GameContextEmulation gameContextEmulation = new();
gameContextEmulation.OnCommandExecuted += command => Console.Write(command);
stopwatch.Restart();
int lineNo = 0;
foreach (List<Token> tokenLine in tokenLines)
{
    Console.Write($"[{++lineNo}] ");
    UrqlInterpreter urqlInterpreter = new(tokenLine, gameContextEmulation);
    await urqlInterpreter.InterpretStatementLineAsync(default);
    Console.WriteLine();
}
stopwatch.Stop();
Console.WriteLine($"--<<------------------------------------------------------------- ({stopwatch.Elapsed:mm\\:ss\\.fff})");
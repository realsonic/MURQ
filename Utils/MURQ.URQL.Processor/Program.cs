using MURQ.URQL.Lexing;
using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Locations;
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

List<char> source = [.. FileUtilities.ReadFile(urqlFilePath)];
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
List<SubstitutionTree> substitutionTrees = [.. lines.ToSubstitutionTrees()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 6. Распознавание подстановок --------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {substitutionTrees.ToJoinedNumberedLines(Serializer.Serialize)}
    ----------------------------------------------------------------------

    """);

Console.WriteLine($"Сумма времени всех этапов: \t{totalTime}");

// тест цепочки
Console.Write("Общее время единой цепочкой: \t");
stopwatch.Restart();
List<SubstitutionTree> result = FileUtilities.ReadFile(urqlFilePath)
    .ToEnumerableWithoutCarriageReturn()
    .ToPositionedEnumerable()
    .ToEnumerableWithoutComments()
    .ToEnumerableWithoutLineContinuations()
    .SplitByLineBreaks()
    .ToSubstitutionTrees()
    .ToList();
stopwatch.Stop();
Console.WriteLine($"{stopwatch.Elapsed}\n");

stopwatch.Restart();
List<List<(char Character, Position Position)>> urqlLines = [.. substitutionTrees.Select(substitutionTree => substitutionTree.ToRawUrql(new GameContextEmulation()).ToList())];
stopwatch.Stop();
WriteBlock(
    "Шаг 1. Эмуляция раскрытия подстановок",
    urqlLines.Select(line => line.ToJoinedString()),
    stopwatch);

stopwatch.Restart();
List<List<Token>> tokens = [.. urqlLines.Select(line => new UrqlLexer(line.Select(element => element.Character)).Scan().ToList())];
stopwatch.Stop();
WriteBlock(
    "Шаг 2. Эмуляция получения токенов",
    tokens.Select(line => line.ToJoinedString()),
    stopwatch);

#region Утилитарные методы
static void WriteBlock(string title, IEnumerable<string> dataLines, Stopwatch stopwatch)
{
    const int borderLength = 77;
    int upperBorderTailLength = borderLength - title.Length - 6;
    int lowerBorderTailLength = borderLength - title.Length - 6 - 12;

    Console.BackgroundColor = ConsoleColor.DarkYellow;
    Console.ForegroundColor = ConsoleColor.Black;
    Console.Write("-->> " + title + " " + new string('-', upperBorderTailLength));
    Console.ResetColor();
    Console.WriteLine();

    var numberedDataLines = dataLines.ToNumberedLines();
    foreach (var line in numberedDataLines)
    {
        Console.WriteLine(line);
    }
    
    Console.BackgroundColor = ConsoleColor.DarkYellow;
    Console.ForegroundColor = ConsoleColor.Black;
    Console.Write("--<< " + title + " " + new string('-', lowerBorderTailLength) + $" ({stopwatch.Elapsed:mm\\:ss\\.fff})");
    Console.ResetColor();
    Console.WriteLine("\n");
}
#endregion
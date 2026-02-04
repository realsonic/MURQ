using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Locations;
using MURQ.URQL.Processor.Json;
using MURQ.URQL.Substitutions;

using System.Diagnostics;

Console.WriteLine("MURQ. Утилита обработки URQL. v.0.1\n");

if (args is not [string urqlFilePath])
{
    Console.WriteLine("Нет обязательного параметра - пути к URQL-файлу.");
    return;
}

TimeSpan totalTime = default;
Stopwatch stopwatch = Stopwatch.StartNew();

List<char> source = [.. ReadFile(urqlFilePath)];
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
List<IEnumerable<(char Character, Position Position)>> lines = [.. uncontinuedSource.SplitByLineBreaks()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 5. Разбивка на строки ---------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {lines.ToNumberedLines(line => line.ToJoinedString())}
    ----------------------------------------------------------------------

    """);

stopwatch.Restart();
List<SubstitutionTree> substitutionTrees = [.. lines.ToSubstitutionTrees()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 6. Распознавание подстановок --------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {substitutionTrees.ToNumberedLines(Serializer.Serialize)}
    ----------------------------------------------------------------------

    """);

Console.WriteLine($"Сумма времени всех этапов: \t{totalTime}");

// тест цепочки
Console.Write("Общее время единой цепочкой: \t");
stopwatch.Restart();
var result = ReadFile(urqlFilePath)
    .ToEnumerableWithoutCarriageReturn()
    .ToPositionedEnumerable()
    .ToEnumerableWithoutComments()
    .ToEnumerableWithoutLineContinuations()
    .SplitByLineBreaks()
    .ToSubstitutionTrees()
    .ToList();
stopwatch.Stop();
Console.WriteLine($"{stopwatch.Elapsed}");

static IEnumerable<char> ReadFile(string filePath)
{
    if (!Path.Exists(filePath))
        throw new InvalidOperationException($"Файл {filePath} не найден.");

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
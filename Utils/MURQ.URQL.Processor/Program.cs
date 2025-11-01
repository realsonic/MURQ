using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions;

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

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
List<SubstitutionTree> substitutedLines = [.. lines.ToSubstitutedLines()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
JsonSerializerOptions jsonSerializerOptions = new()
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
Console.WriteLine($"""
    -- Этап 6. Распознавание подстановок --------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {substitutedLines.ToNumberedLines(line => JsonSerializer.Serialize(line, jsonSerializerOptions))}
    ----------------------------------------------------------------------

    """);

Console.WriteLine($"Сумма времени всех этапов: \t{totalTime}");

// тест цепочки
Console.Write("Общее время единой цепочкой...\t");
stopwatch.Restart();
var result = ReadFile(urqlFilePath)
    .ToEnumerableWithoutCarriageReturn()
    .ToPositionedEnumerable()
    .ToEnumerableWithoutComments()
    .ToEnumerableWithoutLineContinuations()
    .SplitByLineBreaks()
    .ToSubstitutedLines()
    .ToList();
stopwatch.Stop();
Console.WriteLine($"{stopwatch.Elapsed}");


static IEnumerable<char> ReadFile(string filePath)
{
    if (!Path.Exists(filePath))
        throw new InvalidOperationException($"Файл {filePath} не найден.");

    using StreamReader streamReader = File.OpenText(filePath);

    char[] buffer = new char[1];

    while (streamReader.ReadBlock(buffer.AsSpan()) > 0)
    {
        yield return buffer[0];
    }
}

static class Extensions
{
    public static IEnumerable<SubstitutionTree> ToSubstitutedLines(this IEnumerable<IEnumerable<(char Character, Position Position)>> lines)
    {
        foreach (var line in lines)
        {
            SubstitutionParser substitutionParser = new(new SubstitutionLexer());
            yield return substitutionParser.ParseLine(line);
        }
    }

    public static string ToJoinedString(this IEnumerable<char> chars) => string.Join(null, chars);
    public static string ToJoinedString(this IEnumerable<(char Character, Position Position)> chars) => string.Join(null, chars.Select(pc => pc.Character));

    public static string ToNumberedLines<T>(this IEnumerable<T> elements, Func<T, string> convertElement)
        => string.Join("\n", elements.Select((element, number) => $"[{number + 1}] {convertElement(element)}"));

    public static string ToPrintableChar(this char @char) => char.IsControl(@char) ? $"#{Convert.ToInt32(@char)}" : @char.ToString();
}
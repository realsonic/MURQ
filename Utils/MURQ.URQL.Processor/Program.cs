using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Locations;

using System.Diagnostics;

Console.WriteLine("MURQ. Утилита обработки URQL. v.0.1\n");

if (args is not [string urqlFile])
{
    Console.WriteLine("Нет обязательного параметра - пути к URQL-файлу.");
    return;
}

TimeSpan totalTime = default;
Stopwatch stopwatch = Stopwatch.StartNew();

List<char> source = [.. ReadFileAsync(urqlFile)];
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
List<IEnumerable<(char Character, Position Position)>> lines = [.. uncontinuedSource.ToEnumerableByLineBreakes()];
stopwatch.Stop();
totalTime += stopwatch.Elapsed;
Console.WriteLine($"""
    -- Этап 5. Разбивка на строки ---------------------------- ({stopwatch.Elapsed:mm\:ss\.fff})
    {string.Join("\n", lines.Select((line, number) => $"[{number + 1}] {line.ToJoinedString()}"))}
    ----------------------------------------------------------------------

    """);

Console.WriteLine($@"Общее время всех этапов: {totalTime:mm\:ss\.fff}");


IEnumerable<char> ReadFileAsync(string filePath)
{
    if (!Path.Exists(filePath))
        throw new InvalidOperationException($"Файл {filePath} не найден.");

    using StreamReader streamReader = File.OpenText(urqlFile);

    char[] chars = new char[1];

    while (streamReader.ReadBlock(chars.AsSpan()) > 0)
    {
        yield return chars[0];
    }
}

static class Extensions
{
    public static string ToJoinedString(this IEnumerable<char> chars) => string.Join(null, chars);
    public static string ToJoinedString(this IEnumerable<(char Character, Position Position)> chars) => string.Join(null, chars.Select(pc => pc.Character));

    public static string ToPrintableChar(this char @char) => char.IsControl(@char) ? $"#{Convert.ToInt32(@char)}" : @char.ToString();
}
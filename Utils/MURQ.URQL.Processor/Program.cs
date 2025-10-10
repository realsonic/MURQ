using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Locations;

using System.Text;

Console.WriteLine("MURQ. Утилита обработки URQL. v.0.1\n");

if (args is not [string urqlFile])
{
    Console.WriteLine("Нет обязательного параметра - пути к URQL-файлу.");
    return;
}

StringBuilder sourceString = new();
IEnumerable<char> sourceChars = ReadFileAsync(urqlFile).Intercept(@char => sourceString.Append(@char));
StringBuilder trimmedString = new();
IEnumerable<char> trimmedSource = sourceChars.ToEnumerableWithoutCarriageReturn().Intercept(@char => trimmedString.Append(@char));
StringBuilder positionedString = new();
IEnumerable<(char, Position)> positionedSource = trimmedSource.ToPositionedEnumerable().Intercept(@char => positionedString.Append(@char));
StringBuilder uncommentedString = new();
IEnumerable<(char, Position)> umcommentedSource = positionedSource.ToEnumerableWithoutComments().Intercept(@char => uncommentedString.Append(@char));

List<(char, Position)> result = [.. umcommentedSource];

Console.WriteLine($"""
    -- Исходный файл -----------------------------------------------------
    {sourceString}
    ----------------------------------------------------------------------
    
    -- Этап 1. Удаление ненужных символов --------------------------------
    {trimmedString}
    ----------------------------------------------------------------------
        
    -- Этап 2. Добавление координат --------------------------------------
    {positionedString}
    ----------------------------------------------------------------------    
        
    -- Этап 3. Удаление комментариев -------------------------------------
    {uncommentedString}
    ----------------------------------------------------------------------    
    """);



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
    public static IEnumerable<char> Intercept(this IEnumerable<char> chars, Action<char> receiveChar)
    {
        foreach (var @char in chars)
        {
            receiveChar(@char);
            yield return @char;
        }
    }

    public static IEnumerable<(char, Position)> Intercept(this IEnumerable<(char, Position)> chars, Action<char> receiveChar)
    {
        foreach (var @char in chars)
        {
            receiveChar(@char.Item1);
            yield return @char;
        }
    }
}
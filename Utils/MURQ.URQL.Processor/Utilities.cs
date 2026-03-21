using System.Diagnostics;

class Utilities
{
    public static IEnumerable<char> ReadFile(string filePath)
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

    public static void WriteBlock(string title, IEnumerable<string> dataLines, Stopwatch stopwatch)
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
}
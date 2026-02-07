class FileUtilities
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
}
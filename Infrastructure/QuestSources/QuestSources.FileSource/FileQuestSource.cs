using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
using MURQ.Common;
using MURQ.Common.Exceptions;
using MURQ.Domain.Quests;

using System.Text;

namespace QuestSources.FileSource;

public class FileQuestSource(UrqLoader urqLoader, string? filePath, Encoding? encoding) : IQuestSource
{
    public async Task<(Quest Quest, string SourceName)> GetQuestAsync(CancellationToken cancellationToken)
    {
        if (filePath is null)
            throw new MurqException("Путь к файлу квеста не задан");

        if (!File.Exists(filePath))
            throw new MurqException($"Заданный файл квеста ({filePath}) не найден (ищу по пути: {Path.GetFullPath(filePath)})");

        IEnumerable<char> questSource = ReadFile(filePath, encoding);
        Quest quest = urqLoader.LoadQuest(questSource);

        return (quest, $"Файл: {Path.GetFileName(filePath)}");
    }

    private static IEnumerable<char> ReadFile(string filePath, Encoding? encoding)
    {
        encoding ??= CommonEncodings.Windows;
        const int bufferSize = 1024 * 8; // 8 KB — хороший компромисс

        using StreamReader streamReader = new StreamReader(filePath, encoding, true, bufferSize);
        char[] buffer = new char[bufferSize];

        int charsRead;
        while ((charsRead = streamReader.ReadBlock(buffer, 0, bufferSize)) > 0)
        {
            for (int i = 0; i < charsRead; i++)
                yield return buffer[i];
        }
    }
}

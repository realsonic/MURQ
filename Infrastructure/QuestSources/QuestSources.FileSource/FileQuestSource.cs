using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
using MURQ.Common.Exceptions;
using MURQ.Domain.Quests;

using System.Text;

namespace QuestSources.FileSource;

public class FileQuestSource(UrqLoader urqLoader, string? filePath, Encoding encoding) : IQuestSource
{
    public async Task<(Quest Quest, string SourceName)> GetQuest(CancellationToken stoppingToken)
    {
        if (filePath is null)
            throw new MurqException("Путь к файлу квеста не задан");

        if (!File.Exists(filePath))
            throw new MurqException($"Заданный файл квеста ({filePath}) не найден (ищу по пути: {Path.GetFullPath(filePath)})");

        string questSource = await File.ReadAllTextAsync(filePath, encoding, stoppingToken);

        Quest quest = urqLoader.LoadQuest(questSource);

        return (quest, $"Файл: {Path.GetFileName(filePath)}");
    }
}

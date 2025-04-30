using MURQ.Application;
using MURQ.Application.Interfaces;
using MURQ.Common.Exceptions;
using MURQ.Domain.Quests;

using System.Text;

namespace MURQ.Infrastructure.QuestLoaders;

public class FileQuestLoader : IQuestLoader
{
    public FileQuestLoader(string? qstFilePath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        QstFilePath = qstFilePath;
    }

    public string? QstFilePath { get; }

    public string? QuestName => $"Файл: {Path.GetFileName(QstFilePath)}";

    public async Task<Quest> LoadQuest(CancellationToken stoppingToken)
    {
        if (QstFilePath == null)
            throw new MurqException("Путь к файлу квеста не задан");

        if (!File.Exists(QstFilePath))
            throw new MurqException($"Заданный файл квеста ({QstFilePath}) не найден (ищу по пути: {Path.GetFullPath(QstFilePath)})");

        string questSource = await File.ReadAllTextAsync(QstFilePath, Encoding.GetEncoding("Windows-1251"), stoppingToken);
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();

        return quest;
    }
}
using MURQ.Domain.Quests;

namespace MURQ.Application.Interfaces;

/// <summary>
/// Загрузчик квестов из какого-либо источника.
/// </summary>
public interface IQuestLoader
{
    /// <summary>
    /// Имя загруженного квеста.
    /// </summary>
    string? QuestName { get; }

    /// <summary>
    /// Загрузить квест.
    /// </summary>
    /// <param name="stoppingToken">Токен остановки.</param>
    /// <returns>Квест.</returns>

    Task<Quest> LoadQuest(CancellationToken stoppingToken);
}
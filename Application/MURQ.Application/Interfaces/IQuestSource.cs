using MURQ.Domain.Quests;

namespace MURQ.Application.Interfaces;
/// <summary>
/// Источник квеста
/// </summary>
public interface IQuestSource
{
    Task<(Quest Quest, string SourceName)> GetQuest(CancellationToken stoppingToken);
}

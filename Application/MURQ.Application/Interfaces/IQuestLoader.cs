﻿using MURQ.Domain.Quests;

namespace MURQ.Application.Interfaces;

/// <summary>
/// Загрузчик квестов из какого-либо источника.
/// </summary>
public interface IQuestLoader
{
    /// <summary>
    /// Загрузить квест.
    /// </summary>
    /// <param name="stoppingToken">Токен остановки.</param>
    /// <returns>Квест.</returns>

    Task<(Quest Quest, string QuestName)> LoadQuest(CancellationToken stoppingToken);
}
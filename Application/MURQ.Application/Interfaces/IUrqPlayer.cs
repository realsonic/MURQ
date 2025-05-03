namespace MURQ.Application.Interfaces;

/// <summary>
/// Плеер URQ-игр. Является самодостаточной службой.
/// </summary>
public interface IUrqPlayer
{
    /// <summary>
    /// Запустить плеер.
    /// </summary>
    /// <param name="stoppingToken">Токен остановки.</param>
    /// <returns>Задача запущенного плеера.</returns>
    public Task Run(CancellationToken stoppingToken = default);
}
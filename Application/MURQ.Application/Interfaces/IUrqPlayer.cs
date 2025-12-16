namespace MURQ.Application.Interfaces;

/// <summary>
/// Плеер URQ-игр. Является самодостаточной службой.
/// </summary>
public interface IUrqPlayer
{
    /// <summary>
    /// Запустить плеер.
    /// </summary>
    /// <param name="cancellationToken">Токен остановки.</param>
    /// <returns>Задача запущенного плеера.</returns>
    public Task RunAsync(CancellationToken cancellationToken = default);
}
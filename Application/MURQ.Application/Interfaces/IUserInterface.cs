using MURQ.Domain.Games;

namespace MURQ.Application.Interfaces;

/// <summary>
/// Интерфейс пользователя.
/// </summary>
public interface IUserInterface
{
    /// <summary>
    /// Вывести текст игроку.
    /// </summary>
    /// <param name="text">Текст.</param>
    void Write(string? text = null);

    /// <summary>
    /// Вывести текст игроку и добавить новую строку в конце.
    /// </summary>
    /// <param name="text">Текст.</param>
    void WriteLine(string? text = null);
    
    /// <summary>
    /// Очистить экран.
    /// </summary>
    void ClearSceen();

    void WriteHighlighted(string? text = null);
    void WriteLineHighlighted(string? text = null);
    void SetTitle(string title);
    void ReportException(Exception exception);
    UserChoice ShowButtonsAndGetChoice(IEnumerable<Game.Button> buttons);
    void WaitAnyKey();

    public abstract record UserChoice();
    public record ButtonChosen(Game.Button Button) : UserChoice;
    public record QuitChosen : UserChoice;
}
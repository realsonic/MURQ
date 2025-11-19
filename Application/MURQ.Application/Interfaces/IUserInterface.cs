using MURQ.Domain.Games;

namespace MURQ.Application.Interfaces;

/// <summary>
/// Интерфейс пользователя.
/// </summary>
public interface IUserInterface
{
    /// <summary>
    /// Цвет текста.
    /// </summary>
    InterfaceColor ForegroundColor { set; }

    /// <summary>
    /// Цвет фона текста.
    /// </summary>
    InterfaceColor BackgroundColor { set; }

    /// <summary>
    /// Напечатать текст для игрока.
    /// </summary>
    /// <param name="text">Текст.</param>
    void Print(string? text);

    /// <summary>
    /// Напечатать текст для игрока и новую строку в конце.
    /// </summary>
    /// <param name="text">Текст.</param>
    void PrintLine(string? text = null);

    /// <summary>
    /// Очистить экран.
    /// </summary>
    void ClearSceen();

    void PrintHighlighted(string? text = null);
    void PrintLineHighlighted(string? text = null);
    void SetTitle(string title);
    void ReportException(Exception exception);
    UserChoice ShowButtonsAndGetChoice(IEnumerable<Game.Button> buttons);
    void WaitAnyKey();

    public abstract record UserChoice();
    public record ButtonChosen(Game.Button Button, char ButtonCharacter) : UserChoice;
    public record ReloadChosen : UserChoice;
    public record QuitChosen : UserChoice;
}
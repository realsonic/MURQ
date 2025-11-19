using MURQ.Domain.Games;

namespace MURQ.Application.Interfaces;

/// <summary>
/// Интерфейс пользователя.
/// </summary>
public interface IUserInterface
{
    /// <summary>
    /// Напечатать текст для игрока.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <param name="foreground">Цвет текста.</param>
    /// <param name="background">Цвет фона.</param>
    void Print(string? text, InterfaceColor? foreground = null, InterfaceColor? background = null);

    /// <summary>
    /// Напечатать текст для игрока и новую строку в конце.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <param name="foreground">Цвет текста.</param>
    /// <param name="background">Цвет фона.</param>
    void PrintLine(string? text = null, InterfaceColor? foreground = null, InterfaceColor? background = null);

    /// <summary>
    /// Вывести выделенный текст.
    /// </summary>
    /// <param name="text">Текст</param>
    void PrintHighlighted(string? text = null);

    /// <summary>
    /// Вывести ошибку.
    /// </summary>
    /// <param name="exception">Ошибка</param>
    void PrintException(Exception exception);

    /// <summary>
    /// Очистить экран.
    /// </summary>
    void ClearSceen();

    void SetTitle(string title);
    UserChoice PrintButtonsAndWaitChoice(IEnumerable<Game.Button> buttons);
    void WaitAnyKey();

    public abstract record UserChoice();
    public record ButtonChosen(Game.Button Button, char ButtonCharacter) : UserChoice;
    public record ReloadChosen : UserChoice;
    public record QuitChosen : UserChoice;
}
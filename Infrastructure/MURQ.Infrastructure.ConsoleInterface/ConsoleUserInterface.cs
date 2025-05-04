using MURQ.Application.Interfaces;
using MURQ.Common.Exceptions;
using MURQ.Domain.Games;
using MURQ.URQL;

using System.Text;

using static MURQ.Application.Interfaces.IUserInterface;

namespace MURQ.Infrastructure.ConsoleInterface;

public class ConsoleUserInterface : IUserInterface
{
    public ConsoleUserInterface()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false; // без курсора красивее :)
    }

    public void SetTitle(string title) => Console.Title = title;

    public void Write(string? text = null)
    {
        Console.Write(text);
        lastWrittenText = text;
    }

    public void WriteLine(string? text = null) => Write(text + '\n');

    public void WriteHighlighted(string? text = null) => DoInColors(ConsoleColor.Black, ConsoleColor.DarkYellow, () => Write(text));

    public void WriteLineHighlighted(string? text = null)
    {
        WriteHighlighted(text);
        WriteLine(); // нужно новую строку делать не подсвеченной, иначе подсветка залезает на следующую строку
    }

    public UserChoice ShowButtonsAndGetChoice(IEnumerable<Game.Button> buttons)
    {
        var numberedButtons = MapButtonToNumbers(buttons);

        ShowButtons(numberedButtons);
        UserChoice userChoice = GetValidChoice(numberedButtons);

        WriteLine();
        return userChoice;
    }

    public void WaitAnyKey() => Console.ReadKey(true);

    public void ReportException(Exception exception)
    {
        Console.Beep();
        WriteLine();

        DoInColors(ConsoleColor.Black, ConsoleColor.Red, () => WriteLine($" [ОШИБКА] {ClassifyExceptionMessage(exception)} "));
        
        WriteLine();
    }

    private static Dictionary<int, Game.Button> MapButtonToNumbers(IEnumerable<Game.Button> buttons) => buttons
        .Select((button, index) => (Number: index + 1, Button: button))
        .ToDictionary(keySelector: map => map.Number, elementSelector: map => map.Button);

    private void ShowButtons(Dictionary<int, Game.Button> numberedButtons)
    {
        if (lastWrittenText?[^1] is not '\n')
            WriteLine(); // если текст локации не кончается новой строкой, то перед кнопками нужно добавить

        DoInColors(ConsoleColor.Cyan, null, () =>
        {
            foreach (var numberedButton in numberedButtons)
            {
                WriteLine($"[{numberedButton.Key}] {numberedButton.Value.Caption}");
            }
        });
    }

    private static UserChoice GetValidChoice(Dictionary<int, Game.Button> numberedButtons)
    {
        int? minNumber = numberedButtons.Count > 0 ? numberedButtons.Keys.Min() : null;
        int? maxNumber = numberedButtons.Count > 0 ? numberedButtons.Keys.Max() : null;

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (IsButtonNumber(keyInfo))
            {
                var pressedNumber = Convert.ToInt32(keyInfo.KeyChar.ToString());
                if (pressedNumber >= minNumber && pressedNumber <= maxNumber)
                    return new ButtonChosen(numberedButtons[pressedNumber]);
            }

            if (IsQuit(keyInfo))
            {
                return new QuitChosen();
            }
        }

        static bool IsButtonNumber(ConsoleKeyInfo keyInfo) => char.IsDigit(keyInfo.KeyChar);
        static bool IsQuit(ConsoleKeyInfo keyInfo) => keyInfo.Key is ConsoleKey.Q && keyInfo.Modifiers is ConsoleModifiers.Control;
    }

    private static string ClassifyExceptionMessage(Exception exception) => exception switch
    {
        MurqException murqException => murqException switch
        {
            UrqlException => $"Ошибка при загрузке URQL: {exception.Message}",
            _ => exception.Message
        },
        _ => $"""
            Непредвиденная ошибка: {exception.Message}.
            
            Сообщите разработчикам детали:
            {exception}
            """
    };

    private static void DoInColors(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, Action action)
    {
        ConsoleColor previousForegroundColor = Console.ForegroundColor;
        ConsoleColor previousBackgroundColor = Console.BackgroundColor;
        try
        {
            Console.ForegroundColor = foregroundColor ?? Console.ForegroundColor;
            Console.BackgroundColor = backgroundColor ?? Console.BackgroundColor;
            action();
        }
        finally
        {
            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;
        }
    }

    /// <inheritdoc/>
    public void ClearSceen()
    {
        Console.Clear();
        lastWrittenText = string.Empty;
    }

    private string? lastWrittenText = null;
}
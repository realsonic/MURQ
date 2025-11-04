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
        if (!Console.IsOutputRedirected)
        {
            Console.CursorVisible = false; // без курсора красивее :)
        }
    }

    /// <inheritdoc/>>
    public InterfaceColor ForegroundColor { set => Console.ForegroundColor = (ConsoleColor)value; }

    /// <inheritdoc/>>
    public InterfaceColor BackgroundColor { set => Console.BackgroundColor = (ConsoleColor)value; }

    public void SetTitle(string title) => Console.Title = title;

    public void Write(string? text = null)
    {
        Console.Write(text);

        if (!string.IsNullOrEmpty(text))
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
        var buttonMap = MapButtonsToCharacters(buttons);

        ShowButtons(buttonMap);
        UserChoice userChoice = GetValidChoice(buttonMap);

        WriteLine();
        return userChoice;
    }

    /// <inheritdoc/>
    public void ClearSceen()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();

        lastWrittenText = null;
    }

    public void WaitAnyKey()
    {
        if (!Console.IsInputRedirected)
            Console.ReadKey(true);
        else
            Console.Read();
    }

    public void ReportException(Exception exception)
    {
        Console.Beep();
        WriteLine();

        DoInColors(ConsoleColor.Black, ConsoleColor.Red, () => WriteLine($" [ОШИБКА] {ClassifyExceptionMessage(exception)} "));

        WriteLine();
    }

    private static Dictionary<char, Game.Button> MapButtonsToCharacters(IEnumerable<Game.Button> buttons) => buttons
        .Select((button, index) => (Character: GetButtonCharacterByNumber(index + 1), Button: button))
        .ToDictionary(map => map.Character, map => map.Button);

    private static char GetButtonCharacterByNumber(int number) => number switch
    {
        >= 1 and <= 9 => Convert.ToChar('1' + number - 1),
        >= 10 and <= 35 => Convert.ToChar('a' + number - 10),
        _ => '❌'
    };

    private void ShowButtons(Dictionary<char, Game.Button> buttonMap)
    {
        if (lastWrittenText?[^1] is not '\n')
            WriteLine(); // если текст локации не кончается новой строкой, то перед кнопками нужно добавить

        DoInColors(ConsoleColor.Cyan, null, () =>
        {
            foreach (var mappedButton in buttonMap)
            {
                WriteLine($"[{mappedButton.Key}] {mappedButton.Value.Caption}");
            }
        });
    }

    private static UserChoice GetValidChoice(Dictionary<char, Game.Button> buttonMap)
    {
        while (true)
        {
            UserInput userInput = UserInput.GetInput();

            if (userInput.IsButtonCharacter())
            {
                char pressedButtonCharacter = userInput.GetButtonCharacter();
                if (buttonMap.TryGetValue(pressedButtonCharacter, out Game.Button? button))
                    return new ButtonChosen(button, pressedButtonCharacter);
            }

            if (userInput.IsReload())
                return new ReloadChosen();

            if (userInput.IsQuit())
                return new QuitChosen();
        }
    }

    private abstract record UserInput
    {
        public static UserInput GetInput()
        {
            if (!Console.IsInputRedirected)
            {
                return new KeyInfoInput(Console.ReadKey(true));
            }
            else
            {
                int readChar = Console.Read();
                return new CharInput(readChar is not -1 ? Convert.ToChar(readChar) : null);
            }
        }

        public abstract bool IsButtonCharacter();
        public abstract char GetButtonCharacter();
        public abstract bool IsReload();
        public abstract bool IsQuit();
    }

    private record KeyInfoInput(ConsoleKeyInfo ConsoleKeyInfo) : UserInput
    {
        public override bool IsButtonCharacter() => char.IsAsciiLetterOrDigit(ConsoleKeyInfo.KeyChar);
        public override char GetButtonCharacter() => ConsoleKeyInfo.KeyChar;
        public override bool IsReload() => ConsoleKeyInfo.Key is ConsoleKey.R && ConsoleKeyInfo.Modifiers is ConsoleModifiers.Control;
        public override bool IsQuit() => ConsoleKeyInfo.Key is ConsoleKey.Q && ConsoleKeyInfo.Modifiers is ConsoleModifiers.Control;
    }

    private record CharInput(char? Char) : UserInput
    {
        public override bool IsButtonCharacter() => Char is not null && char.IsAsciiLetterOrDigit(Char.Value);
        public override char GetButtonCharacter() => Char ?? throw new MurqException("Неожиданное обращение к символу при ненажатой кнопке");
        public override bool IsReload() => Char is 'r' or 'R';
        public override bool IsQuit() => Char is null or 'q' or 'Q';
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

    public void FinishWork()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.CursorVisible = true; // возвращаем курсор на место
        }
    }

    private string? lastWrittenText = null;
}
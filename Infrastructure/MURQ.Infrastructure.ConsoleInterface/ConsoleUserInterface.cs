using MURQ.Application.Interfaces;
using MURQ.Common.Exceptions;
using MURQ.Domain.Games;
using MURQ.URQL;

using System.Text;

using static MURQ.Application.Interfaces.IUserInterface;

namespace MURQ.Infrastructure.ConsoleInterface;

public class ConsoleUserInterface : IUserInterface, IDisposable
{
    public ConsoleUserInterface()
    {
        originalOutputEncoding = Console.OutputEncoding;
        Console.OutputEncoding = Encoding.UTF8;

        Console.ResetColor();

        if (!Console.IsOutputRedirected)
        {
            if (OperatingSystem.IsWindows())
                originalCursorVisible = Console.CursorVisible;

            Console.CursorVisible = false; // без курсора красивее :)
        }
    }

    /// <inheritdoc/>
    public void SetTitle(string title) => Console.Title = title;


    /// <inheritdoc/>
    public void Print(string? text, InterfaceColor? foreground = null, InterfaceColor? background = null)
    {
        string textToWrite = text ?? string.Empty;

        try
        {
            if (foreground is not null) Console.ForegroundColor = (ConsoleColor)foreground.Value;
            if (background is not null) Console.BackgroundColor = (ConsoleColor)background.Value;

            Console.Write(textToWrite);
        }
        finally
        {
            Console.ResetColor();
        }
    }

    /// <inheritdoc/>
    public void PrintLine(string? text = null, InterfaceColor? foreground = null, InterfaceColor? background = null)
    {
        Print(text, foreground, background);
        Console.WriteLine();
    }

    /// <inheritdoc/>
    public void PrintHighlighted(string? text) => Print(text, InterfaceColor.Black, InterfaceColor.DarkYellow);

    /// <inheritdoc/>
    public void PrintException(Exception exception)
    {
        Console.Beep();
        PrintLine();
        PrintLine($" [ОШИБКА] {ClassifyExceptionMessage(exception)} ", InterfaceColor.Black, InterfaceColor.Red);
        PrintLine();
    }

    /// <inheritdoc/>
    public UserChoice PrintButtonsAndWaitChoice(IEnumerable<Game.Button> buttons, InterfaceColor foreground, InterfaceColor background)
    {
        var buttonMap = MapButtonsToCharacters(buttons);

        PrintButtons(buttonMap, foreground, background);
        UserChoice userChoice = GetValidChoice(buttonMap);

        PrintLine();
        return userChoice;
    }

    /// <inheritdoc/>
    public void ClearSceen()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();
    }

    /// <inheritdoc/>
    public void WaitAnyKey()
    {
        if (!Console.IsInputRedirected)
            Console.ReadKey(true);
        else
            Console.Read();
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

    private void PrintButtons(Dictionary<char, Game.Button> buttonMap, InterfaceColor foreground, InterfaceColor background)
    {
        PrintLine();

        foreach (var mappedButton in buttonMap)
        {
            PrintLine($"[{mappedButton.Key}] {mappedButton.Value.Caption}", foreground, background);
        }
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

    public void Dispose()
    {
        CleanUp();
        GC.SuppressFinalize(this);
    }

    protected virtual void CleanUp()
    {
        if (!isDisposed)
        {
            Console.OutputEncoding = originalOutputEncoding;
            if (!Console.IsOutputRedirected)
            {
                Console.CursorVisible = originalCursorVisible ?? true; // возвращаем курсор на место
            }
            Console.ResetColor();

            isDisposed = true;
        }
    }

    ~ConsoleUserInterface()
    {
        CleanUp();
    }

    private readonly Encoding originalOutputEncoding;
    private readonly bool? originalCursorVisible;
    private bool isDisposed;

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
}
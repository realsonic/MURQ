using MURQ.Application.Interfaces;
using MURQ.Domain.Games;

using System.Text;

namespace MURQ.EndToEndTests;

internal class UserInterfaceMock(int[] buttonInput) : IUserInterface
{
    public static string GetHeader(string fileName) => MurqLogo + GetQuestNameLine(fileName);

    public static string GetFooter() => """


             Вы нажали выход. До свидания! 

            Нажмите любую клавишу для выхода...
            """;

    public string? Title { get; private set; }

    public StringBuilder Output { get; } = new();

    public void ClearSceen() => Output.Clear();

    public void ReportException(Exception exception) => throw new Exception($"Плеер выкинул ошибку: {exception}");

    public void SetTitle(string title) => Title = title;

    public IUserInterface.UserChoice ShowButtonsAndGetChoice(IEnumerable<Game.Button> buttons)
    {
        if (buttonIndex > buttonInput.Length - 1)
            throw new ArgumentException($"Индекс кнопки {buttonIndex} превысил кол-во вводных кнопок {buttonInput}");

        int buttonNumber = buttonInput[buttonIndex++];

        if (buttonNumber <= 0)
            return new IUserInterface.QuitChosen();

        return new IUserInterface.ButtonChosen(buttons.ToList()[buttonNumber]);
    }

    public void WaitAnyKey() { }

    public void Write(string? text = null) => Output.Append(text);

    public void WriteHighlighted(string? text = null) => Write(text);

    public void WriteLine(string? text = null) => Output.AppendLine(text);

    public void WriteLineHighlighted(string? text = null) => WriteLine(text);

    private static string MurqLogo { get; } = $"""

                /\_/\
               ( o.o )
            |   >   < 
             | /     \             v. 
             _(___ __ )_ _____ _____
            |     |  |  | ___ |     | 
            | | | |  |  |    -|  |  | 
            |_|_|_|_____|__|__|__  _| 
                                 |__|

        """;

    private static string GetQuestNameLine(string fileName) => $"""

        Файл: {fileName}{'\n'}

        """;

    private int buttonIndex = 0;
}
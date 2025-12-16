using System.Text;

using MURQ.Demo._2;
using MURQ.Domain.Games;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 2: Поиске";

var game = new Game(PoiskeQuestBuilder.BuildPoiskeQuest());
await game.StartAsync();

while (true)
{
    Console.Clear();
    Console.WriteLine(game.CurrentLocation.Text);

    int choicenButtonIndex = ShowButtonsAndGetChoice(game.CurrentLocation.Buttons) - 1;

    await game.CurrentLocation.Buttons[choicenButtonIndex].PressAsync();
}

static int ShowButtonsAndGetChoice(IReadOnlyList<Game.Button> readOnlyList)
{
    ShowButtons(readOnlyList);
    return GetValidChoice(readOnlyList.Count);
}

static void ShowButtons(IReadOnlyList<Game.Button> buttons)
{
    ConsoleColor previousColor = Console.ForegroundColor;

    Console.ForegroundColor = ConsoleColor.Cyan;
    try
    {
        for (var buttonIndex = 0; buttonIndex <= buttons.Count - 1; buttonIndex++)
        {
            Game.Button button = buttons[buttonIndex];
            int buttonNumber = buttonIndex + 1;
            Console.WriteLine($"[{buttonNumber}] {button.Caption}");
        }
    }
    finally
    {
        Console.ForegroundColor = previousColor;
    }
}

static int GetValidChoice(int maxNumber)
{
    while (true)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        if (!char.IsDigit(keyInfo.KeyChar)) continue;

        var number = Convert.ToInt32(keyInfo.KeyChar.ToString());
        if (number >= 1 && number <= maxNumber) return number;
    }
}
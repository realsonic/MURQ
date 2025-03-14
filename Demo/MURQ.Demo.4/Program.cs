using MURQ.Application;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 4: poiske.qst";

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

string questSource = await File.ReadAllTextAsync(@"poiske.qst", Encoding.GetEncoding("Windows-1251"));
UrqLoader urqLoader = new(questSource);
Quest quest = urqLoader.LoadQuest();

var game = new Game(quest);
game.Start();

while (true)
{
    Console.WriteLine(game.CurrentLocation.Text);

    int choicenButtonIndex = ShowButtonsAndGetChoice(game.CurrentLocation.Buttons) - 1;

    Console.WriteLine();

    game.CurrentLocation.Buttons[choicenButtonIndex].Press();
}

static int ShowButtonsAndGetChoice(IReadOnlyList<Game.Button> buttonList)
{
    ShowButtons(buttonList);
    return GetValidChoice(buttonList.Count);
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
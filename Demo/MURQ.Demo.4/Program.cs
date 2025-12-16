using MURQ.Application.UrqLoaders;
using MURQ.Application.UrqLoaders.UrqStrings;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 4: poiske.qst";

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

string questSource = await File.ReadAllTextAsync(@"poiske.qst", Encoding.GetEncoding("Windows-1251"));
UrqLoader urqLoader = new(new UrqStringLoader(new UrqStringLexer()));
Quest quest = urqLoader.LoadQuest(questSource);

var game = new Game(quest);
await game.StartAsync();

while (true)
{
    Console.WriteLine(game.CurrentLocation.Text);

    int choicenButtonIndex = ShowButtonsAndGetChoice(game.CurrentLocation.Buttons) - 1;

    Console.WriteLine();

    await game.CurrentLocation.Buttons[choicenButtonIndex].PressAsync();
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
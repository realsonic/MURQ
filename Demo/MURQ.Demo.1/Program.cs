using System.Text;

using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Instructions;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 1";

var hereLabel = new LabelInstruction { Label = "Здесь" };
var thereLabel = new LabelInstruction { Label = "Там" };

var quest = new Quest([
    hereLabel,
    new PrintInstruction
    {
        Text =
            "Привет! Это простое демо MURQ: квест, в котором есть пара локаций да пара кнопок, чтобы ходить туда-сюда.\n"
    },
    new ButtonInstruction { Caption = "Туда", LabelInstruction = thereLabel },
    new EndInstruction(),
    thereLabel,
    new PrintInstruction { Text = "Вы попали туда!\n" },
    new ButtonInstruction { Caption = "Сюда", LabelInstruction = hereLabel }
]);

var game = new Game(quest);

game.Start();

while (true)
{
    Game.CurrentLocationView currentLocation = game.CurrentLocation;

    Console.Clear();
    Console.WriteLine(currentLocation.Text);

    for (var buttonIndex = 0; buttonIndex <= currentLocation.Buttons.Count - 1; buttonIndex++)
    {
        Game.Button button = currentLocation.Buttons[buttonIndex];
        int buttonNumber = buttonIndex + 1;
        Console.WriteLine($"[{buttonNumber}] {button.Caption}");
    }

    int choicenButtonIndex = GetValidChoice(currentLocation.Buttons.Count) - 1;
    game.CurrentLocation.Buttons[choicenButtonIndex].Press();
}

static int GetValidChoice(int maxNumber)
{
    while (true)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        if (!char.IsDigit(keyInfo.KeyChar)) continue;

        var number = Convert.ToInt32(keyInfo.KeyChar.ToString());
        if (number == 1 && number <= maxNumber) return number;
    }
}
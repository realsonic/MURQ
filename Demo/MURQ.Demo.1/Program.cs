using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 1";

var hereLabel = new LabelStatement { Label = "Здесь" };
var thereLabel = new LabelStatement { Label = "Там" };

var quest = new Quest([
    hereLabel,
    new PrintStatement
    {
        Text =
            "Привет! Это простое демо MURQ: квест, в котором есть пара локаций да пара кнопок, чтобы ходить туда-сюда.\n"
    },
    new ButtonStatement { Caption = "Туда", LabelStatement = thereLabel },
    new EndStatement(),
    thereLabel,
    new PrintStatement { Text = "Вы попали туда!\n" },
    new ButtonStatement { Caption = "Сюда", LabelStatement = hereLabel }
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
        if (number >= 1 && number <= maxNumber) return number;
    }
}
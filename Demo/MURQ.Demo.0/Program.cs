using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Instructions;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 0";

var quest = new Quest([
    new PrintInstruction
        { Text = "Привет! Это самое простое демо MURQ: квест, в котором только эта строчка текста. Пока!" }
]);

var game = new Game(quest);

game.Start();
Console.WriteLine(game.CurrentLocation.Text);

Console.Write("🔶 Нажмите любую клавишу для выхода.");
Console.ReadKey(true);
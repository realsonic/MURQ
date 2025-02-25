using MURQ.Application;
using MURQ.Domain.Games;

using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Мурка. Демо 3";

string questSource = await File.ReadAllTextAsync(@"Demo3.qst");
var urqLoader = new UrqLoader(questSource);
var quest = urqLoader.LoadQuest();
var game = new Game(quest);

game.Start();
Console.WriteLine(game.CurrentLocation.Text);

Console.Write(">> Нажмите любую клавишу для выхода. <<");
Console.ReadKey(true);
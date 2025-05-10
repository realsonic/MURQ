using MURQ.Application.Interfaces;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

using static MURQ.Application.Interfaces.IUserInterface;

namespace MURQ.Application.Services;

public class UrqPlayer(IQuestLoader questLoader, IUserInterface userInterface, IVersionProvider versionProvider) : IUrqPlayer
{
    public async Task Run(CancellationToken stoppingToken)
    {
        ShowTitleAndLogo();

        try
        {
            Game game = await LoadQuestAndStartGame(stoppingToken);

            await RunPlayCycle(game, stoppingToken);

            SayGoodbye();
            PromptAnyKey();
        }
        catch (Exception ex)
        {
            ReportException(ex);
            PromptAnyKey();
        }
    }

    private async Task<Game> LoadQuestAndStartGame(CancellationToken stoppingToken)
    {
        (Quest quest, string questName) = await questLoader.LoadQuest(stoppingToken);

        var game = new Game(quest);
        game.OnScreenCleared += userInterface.ClearSceen;

        ShowQuestName(questName);

        game.Start();

        return game;
    }

    private async Task RunPlayCycle(Game game, CancellationToken stoppingToken)
    {
        while (true)
        {
            userInterface.Write(game.CurrentLocation.Text);

            var userChoice = userInterface.ShowButtonsAndGetChoice(game.CurrentLocation.Buttons);

            userInterface.WriteLine();

            switch (userChoice)
            {
                case ButtonChosen buttonChosen:
                    buttonChosen.Button.Press();
                    break;
                case ReloadChosen:
                    game = await LoadQuestAndStartGame(stoppingToken);
                    break;
                case QuitChosen:
                    return;
            }
        }
    }

    private void ShowTitleAndLogo()
    {
        string version = versionProvider.Version;
        userInterface.SetTitle($"MURQ.Console {version}");
        var versionWithPrefix = $"v.{version}";
        userInterface.WriteLine($"""

                /\_/\
               ( o.o )
            |   >   < 
             | /     \     {versionWithPrefix,10} 
             _(___ __ )_ _____ _____
            |     |  |  | ___ |     | 
            | | | |  |  |    -|  |  | 
            |_|_|_|_____|__|__|__  _| 
                                 |__|

        """);
    }

    private void ShowQuestName(string? questName) => userInterface.WriteLine($"{questName}\n");

    private void SayGoodbye()
    {
        userInterface.WriteLine();
        userInterface.WriteLineHighlighted(" Вы нажали выход. До свидания! ");
        userInterface.WriteLine();
    }

    private void PromptAnyKey()
    {
        userInterface.Write("Нажмите любую клавишу для выхода...");
        userInterface.WaitAnyKey();
    }

    private void ReportException(Exception ex) => userInterface.ReportException(ex);
}
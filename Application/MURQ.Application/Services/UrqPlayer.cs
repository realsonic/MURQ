using MURQ.Application.Interfaces;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

using static MURQ.Application.Interfaces.IUserInterface;

namespace MURQ.Application.Services;

public class UrqPlayer(IQuestSource questSource, IUserInterface userInterface, IVersionProvider versionProvider) : IUrqPlayer
{
    public async Task Run(CancellationToken cancellationToken)
    {
        ShowTitleAndLogo();

        try
        {
            game = await LoadQuestAndStartGame(cancellationToken);

            await RunPlayCycle(cancellationToken);

            SayGoodbye();
            PromptAnyKey();
        }
        catch (Exception ex)
        {
            ReportException(ex);
            PromptAnyKey();
        }
    }

    private async Task<Game> LoadQuestAndStartGame(CancellationToken cancellationToken)
    {
        (Quest quest, string questName) = await questSource.GetQuest(cancellationToken);

        var game = new Game(quest);
        game.OnTextPrinted += Write;
        game.OnScreenCleared += userInterface.ClearSceen;
         
        ShowQuestName(questName);

        game.Start();

        return game;
    }

    private void Write(string text, InterfaceColor foreground, InterfaceColor background)
    {
        userInterface.ForegroundColor = foreground;
        userInterface.BackgroundColor = background;
        userInterface.Write(text);
    }

    private async Task RunPlayCycle(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(game);

        while (true)
        {
            var userChoice = userInterface.ShowButtonsAndGetChoice(game.CurrentLocation.Buttons);

            userInterface.WriteLine();

            switch (userChoice)
            {
                case ButtonChosen buttonChosen:
                    ReportPressedButton(buttonChosen);
                    buttonChosen.Button.Press();
                    break;
                case ReloadChosen:
                    game = await LoadQuestAndStartGame(cancellationToken);
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

    private void ReportPressedButton(ButtonChosen buttonChosen)
        => userInterface.WriteLine($"> [{buttonChosen.ButtonCharacter}] {buttonChosen.Button.Caption}\n");

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

    private Game? game;
}
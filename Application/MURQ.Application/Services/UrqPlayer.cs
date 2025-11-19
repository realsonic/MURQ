using MURQ.Application.Interfaces;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

using static MURQ.Application.Interfaces.IUserInterface;

namespace MURQ.Application.Services;

public class UrqPlayer(IQuestSource questSource, IUserInterface userInterface, IVersionProvider versionProvider) : IUrqPlayer
{
    public async Task Run(CancellationToken cancellationToken)
    {
        PrintTitleAndLogo();

        try
        {
            game = await LoadQuestAndStartGame(cancellationToken);

            await RunPlayCycle(cancellationToken);

            PrintGoodbye();
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
        game.OnTextPrinted += Game_OnTextPrinted;
        game.OnScreenCleared += userInterface.ClearSceen;

        PrintQuestName(questName);

        game.Start();

        return game;
    }

    private void Game_OnTextPrinted(object? sender, Game.OnTextPrintedEventArgs e)
    {
        userInterface.ForegroundColor = e.ForegroundColor;
        userInterface.BackgroundColor = e.BackgroundColor;

        if (e.IsNewLineAtEnd)
            userInterface.PrintLine(e.Text);
        else
            userInterface.Print(e.Text);
    }

    private async Task RunPlayCycle(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(game);

        while (true)
        {
            var userChoice = userInterface.ShowButtonsAndGetChoice(game.CurrentLocation.Buttons);

            userInterface.PrintLine();

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

    private void PrintTitleAndLogo()
    {
        string version = versionProvider.Version;
        userInterface.SetTitle($"MURQ.Console {version}");
        var versionWithPrefix = $"v.{version}";
        userInterface.PrintLine($"""

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

    private void PrintQuestName(string? questName)
    {
        userInterface.PrintLine($"{questName}");
        userInterface.PrintLine();
    }

    private void ReportPressedButton(ButtonChosen buttonChosen)
    {
        userInterface.PrintLine($"> [{buttonChosen.ButtonCharacter}] {buttonChosen.Button.Caption}");
        userInterface.PrintLine();
    }

    private void PrintGoodbye()
    {
        userInterface.PrintLine();
        userInterface.PrintLineHighlighted(" Вы нажали выход. До свидания! ");
        userInterface.PrintLine();
    }

    private void PromptAnyKey()
    {
        userInterface.Print("Нажмите любую клавишу для выхода...");
        userInterface.WaitAnyKey();
    }

    private void ReportException(Exception ex) => userInterface.ReportException(ex);

    private Game? game;
}
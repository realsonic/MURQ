using MURQ.Application.Interfaces;
using MURQ.Common.Exceptions;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.URQL;

using System.Diagnostics;

using static MURQ.Application.Interfaces.IUserInterface;

namespace MURQ.Application.Services;

public class UrqPlayer(IQuestSource questSource, IUserInterface userInterface, IVersionProvider versionProvider) : IUrqPlayer
{
    public async Task RunAsync(CancellationToken cancellationToken)
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
        Stopwatch stopwatch = Stopwatch.StartNew();
        (Quest quest, string questName) = await questSource.GetQuestAsync(cancellationToken);
        stopwatch.Stop();

        var game = new Game(quest);
        game.OnTextPrinted += Game_OnTextPrinted;
        game.OnScreenCleared += userInterface.ClearSceen;
        game.OnUrqlError += Game_OnUrqlError;

        PrintQuestInfo(questName, stopwatch.Elapsed);

        await game.StartAsync(cancellationToken);

        return game;
    }

    private void Game_OnTextPrinted(object? sender, Game.OnTextPrintedEventArgs e)
    {
        if (e.IsNewLineAtEnd)
            userInterface.PrintLine(e.Text, e.ForegroundColor, e.BackgroundColor);
        else
            userInterface.Print(e.Text, e.ForegroundColor, e.BackgroundColor);
    }

    private void Game_OnUrqlError(object? sender, Game.OnErrorEventArgs e)
    {
        ReportException(e.Exception);
    }

    private async Task RunPlayCycle(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(game);

        while (!cancellationToken.IsCancellationRequested)
        {
            var userChoice = userInterface.PrintButtonsAndWaitChoice(game.CurrentLocation.Buttons, game.ButtonForegroundColor, game.ButtonBackgroundColor);

            userInterface.PrintLine();

            switch (userChoice)
            {
                case ButtonChosen buttonChosen:
                    ReportPressedButton(buttonChosen);
                    await buttonChosen.Button.PressAsync(cancellationToken);
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
        
        // Обычная версия:
        /*userInterface.PrintLine($"""

                /\_/\
               ( o.o )
            |   >   < 
             | /     \     {versionWithPrefix,10} 
             _(___ __ )_ _____ _____
            |     |  |  | ___ |     | 
            | | | |  |  |    -|  |  | 
            |_|_|_|_____|__|__|__  _| 
                                 |__|

        """);*/

        // Новогодняя версия:
        userInterface.PrintLine($"""           

                          *
                         / \
                /\_/\   //' \
               ( o.o ) /   ' \
            |   >   <  //   `\
             | /     \/    {versionWithPrefix,8}
             _(___ __ )_'_____\_____
            |     |  |  | ___ |     |
            | | | |  |  |    -|  |  |
            |_|_|_|_____|__|__|__  _|
                                 |__|

        """);
    }

    private void PrintQuestInfo(string? questName, TimeSpan loadDuration)
    {
        userInterface.PrintLine($"{questName}, загружен за {loadDuration:mm\\:ss\\.fff}.");
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
        userInterface.PrintHighlighted(" Вы нажали выход. До свидания! ");
        userInterface.PrintLine();
        userInterface.PrintLine();
    }

    private void PromptAnyKey()
    {
        userInterface.Print("Нажмите любую клавишу для выхода...");
        userInterface.WaitAnyKey();
    }

    private void ReportException(Exception ex) => userInterface.PrintError(ClassifyExceptionMessage(ex));

    private static string ClassifyExceptionMessage(Exception exception) => exception switch
    {
        MurqException murqException => murqException switch
        {
            UrqlException => $"Ошибка при загрузке URQL: {exception.Message}",
            _ => exception.Message
        },
        _ => $"""
            Непредвиденная ошибка: {exception.Message}.
            
            Сообщите разработчикам детали:
            {exception}
            """
    };

    private Game? game;
}
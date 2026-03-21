using MURQ.Domain.Games;
using MURQ.Domain.Quests;

namespace MURQ.Domain.Tests;

public class GameTests
{
    [Fact(DisplayName = "Пустой квест запускается")]
    public async Task Empty_quest_starts()
    {
        // Arrange
        var quest = new Quest([]);
        var game = new Game(quest);

        // Act & Assert
        await game.StartAsync();
    }
}
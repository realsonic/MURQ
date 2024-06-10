using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Tests;

public class GameTests
{
    [Fact]
    public void Empty_quest_start()
    {
        // Arrange
        var quest = new Quest([]);
        var game = new Game(quest);

        // Act & Assert
        game.Start();
    }

    [Fact]
    public void Only_print_shows_text()
    {
        // Arrange
        var printInstruction = new PrintInstruction { Text = "Hello World!" };
        var quest = new Quest([printInstruction]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Text.Should().Be("Hello World!");
    }
}
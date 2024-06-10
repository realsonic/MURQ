using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Tests;

public class GameTests
{
    [Fact]
    public void Empty_quest_starts()
    {
        // Arrange
        var quest = new Quest([]);
        var game = new Game(quest);

        // Act & Assert
        game.Start();
    }

    [Fact]
    public void Print_shows_text()
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

    [Fact]
    public void ButtonInstruction_adds_button()
    {
        // Arrange
        var btnInstruction = new ButtonInstruction { Caption = "Push me" };
        var quest = new Quest([btnInstruction]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Buttons!.First().Caption.Should().Be("Push me");
    }

    [Fact]
    public void Button_goes_to_label()
    {
        // Arrange
        var labelInstruction = new LabelInstruction{Label = "Начало"};
        var quest = new Quest([
            labelInstruction,
            new PrintInstruction {Text = "Text"},
            new ButtonInstruction {Caption = "Push me", LabelInstruction = labelInstruction}
        ]);
        var game = new Game(quest);

        // Act & Assert: start game
        game.Start();
        game.CurrentLocation.Text.Should().Be("Text");
        game.CurrentLocation.Buttons!.First().Caption.Should().Be("Push me");

        // Act & Assert: push button
        game.CurrentLocation.Buttons!.First().Press();
        game.CurrentLocation.Text.Should().Be("Text");
        game.CurrentLocation.Buttons!.First().Caption.Should().Be("Push me");
    }
}
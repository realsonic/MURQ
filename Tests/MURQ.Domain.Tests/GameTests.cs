using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Tests;

public class GameTests
{
    [Fact(DisplayName = "������ ����� �����������")]
    public void Empty_quest_starts()
    {
        // Arrange
        var quest = new Quest([]);
        var game = new Game(quest);

        // Act & Assert
        game.Start();
    }

    [Fact(DisplayName = "Print ������� �����")]
    public void Print_shows_text()
    {
        // Arrange
        var printStatement = new PrintStatement { Text = "Hello World!" };
        var quest = new Quest([printStatement]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Text.Should().Be("Hello World!");
    }

    [Fact(DisplayName = "Button ��������� ������")]
    public void ButtonInstruction_adds_button()
    {
        // Arrange
        var btnStatement = new ButtonStatement { Caption = "Push me" };
        var quest = new Quest([btnStatement]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Buttons![0].Caption.Should().Be("Push me");
    }

    [Fact(DisplayName = "������ ��������� �� �����")]
    public void Button_goes_to_label()
    {
        // Arrange
        var labelStatement = new LabelStatement { Label = "������" };
        var quest = new Quest([
            labelStatement,
            new PrintStatement { Text = "Text" },
            new ButtonStatement { Caption = "Push me", LabelStatement = labelStatement }
        ]);
        var game = new Game(quest);

        // Act & Assert: start game
        game.Start();
        game.CurrentLocation.Text.Should().Be("Text");
        game.CurrentLocation.Buttons![0].Caption.Should().Be("Push me");

        // Act & Assert: push button
        game.CurrentLocation.Buttons![0].Press();
        game.CurrentLocation.Text.Should().Be("Text");
        game.CurrentLocation.Buttons![0].Caption.Should().Be("Push me");
    }

    [Fact(DisplayName = "End ������������� ����������")]
    public void End_stops()
    {
        // Arrange
        var quest = new Quest([
            new LabelStatement { Label = "������ �������" },
            new EndStatement(),
            new LabelStatement { Label = "������ �������" }
        ]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Name.Should().Be("������ �������");
    }
}
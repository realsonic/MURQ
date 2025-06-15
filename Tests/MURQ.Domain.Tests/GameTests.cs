using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Tests;

public class GameTests
{
    [Fact(DisplayName = "Пустой квест запускается")]
    public void Empty_quest_starts()
    {
        // Arrange
        var quest = new Quest([]);
        var game = new Game(quest);

        // Act & Assert
        game.Start();
    }

    [Fact(DisplayName = "Print выводит текст")]
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

    [Fact(DisplayName = "Button добавляет кнопку")]
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

    [Fact(DisplayName = "Кнопка переходит на метку")]
    public void Button_goes_to_label()
    {
        // Arrange
        var labelStatement = new LabelStatement { Label = "Начало" };
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

    [Fact(DisplayName = "End останавливает выполнение")]
    public void End_stops()
    {
        // Arrange
        var quest = new Quest([
            new LabelStatement { Label = "Первая локация" },
            new EndStatement(),
            new LabelStatement { Label = "Вторая локация" }
        ]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Name.Should().Be("Первая локация");
    }

    [Fact(DisplayName = "If проверяет простое условие сравнения переменной с числом и выполняет команду")]
    public void If_checks_relation_and_runs_statement()
    {
        // Arrange
        var quest = new Quest([
            new AssignVariableStatement { VariableName = "a", Expression = new DecimalConstantExpression { Value = 4 } },
            new IfStatement {
                Condition = new RelationExpression {
                    LeftExpression = new VariableExpression { VariableName = "A" },
                    RightExpression = new DecimalConstantExpression { Value = 4 }
                },
                ThenStatement = new PrintStatement { Text = "Всего хорошего!", IsNewLineAtEnd = true }
            }
        ]);
        var game = new Game(quest);

        // Act
        game.Start();

        // Assert
        game.CurrentLocation.Text.Should().Be("Всего хорошего!\n");
    }
}
using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.Statements;

using System.Diagnostics;

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

    [Fact(DisplayName = "Print выводит текст")]
    public async Task Print_shows_text()
    {
        // Arrange
        var printStatement = new PrintStatement { UrqString = "Hello World!" };
        var quest = new Quest([printStatement]);
        var game = new Game(quest);

        // Act
        await game.StartAsync();

        // Assert
        game.CurrentLocation.Text.Should().Be("Hello World!");
    }

    [Fact(DisplayName = "Button добавляет кнопку")]
    public async Task ButtonInstruction_adds_button()
    {
        // Arrange
        var btnStatement = new ButtonStatement { Caption = "Push me" };
        var quest = new Quest([btnStatement]);
        var game = new Game(quest);

        // Act
        await game.StartAsync();

        // Assert
        game.CurrentLocation.Buttons![0].Caption.Should().Be("Push me");
    }

    [Fact(DisplayName = "Кнопка переходит на метку")]
    public async Task Button_goes_to_label()
    {
        // Arrange
        var labelStatement = new LabelStatement { Label = "Начало" };
        var quest = new Quest([
            labelStatement,
            new PrintStatement { UrqString = "Text" },
            new ButtonStatement { Caption = "Push me", LabelStatement = labelStatement }
        ]);
        var game = new Game(quest);

        // Act & Assert: start game
        await game.StartAsync();
        game.CurrentLocation.Text.Should().Be("Text");
        game.CurrentLocation.Buttons![0].Caption.Should().Be("Push me");

        // Act & Assert: push button
        await game.CurrentLocation.Buttons![0].PressAsync();
        game.CurrentLocation.Text.Should().Be("Text");
        game.CurrentLocation.Buttons![0].Caption.Should().Be("Push me");
    }

    [Fact(DisplayName = "End останавливает выполнение")]
    public async Task End_stops()
    {
        // Arrange
        var quest = new Quest([
            new LabelStatement { Label = "Первая локация" },
            new PrintStatement { UrqString = "Первая локация" },
            new EndStatement(),
            new LabelStatement { Label = "Вторая локация" }
        ]);
        var game = new Game(quest);

        // Act
        await game.StartAsync();

        // Assert
        game.CurrentLocation.Text.Should().Be("Первая локация");
    }

    [Fact(DisplayName = "If проверяет простое условие сравнения переменной с числом и выполняет команду")]
    public async Task If_checks_relation_and_runs_statement()
    {
        // Arrange
        var quest = new Quest([
            new AssignVariableStatement { VariableName = "a", Expression = new DecimalConstantExpression { Value = 4 } },
            new IfStatement {
                Condition = new RelationExpression {
                    LeftExpression = new VariableExpression { Name = "A" },
                    RightExpression = new DecimalConstantExpression { Value = 4 }
                },
                ThenStatement = new PrintStatement { UrqString = "Всего хорошего!", IsNewLineAtEnd = true }
            }
        ]);
        var game = new Game(quest);

        // Act
        await game.StartAsync();

        // Assert
        game.CurrentLocation.Text.Should().Be("Всего хорошего!" + Environment.NewLine);
    }

    [Fact(DisplayName = "Pause приостанавливает игру на заданное кол-во миллисекунд")]
    public async Task Pause_pauses()
    {
        // Arrange
        var label = new LabelStatement { Label = "1" };
        var quest = new Quest([
            new ButtonStatement { Caption = "", LabelStatement = label},
            new EndStatement(),
            label,
            new PauseStatement { Duration = 100 }
        ]);
        var game = new Game(quest);

        // Act
        await game.StartAsync();
        Stopwatch stopwatch = Stopwatch.StartNew();
        await game.CurrentLocation.Buttons.Single().PressAsync();
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(100);
    }
}
using FluentAssertions;

using MURQ.Application;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.ApplicationTests;

public class UrqLoaderTests
{
    [Fact(DisplayName = "Квест из двух p загружается")]
    public void Quest_of_2p_loads()
    {
        // Arrange
        UrqLoader sut = new("""
            p Привет, 
            p мир!
            """);

        // Act
        Quest quest = sut.LoadQuest();

        // Assert
        quest.Statements.Should().BeEquivalentTo([
            new PrintStatement{ Text = "Привет, " },
            new PrintStatement{ Text = "мир!"}
        ]);
    }
}
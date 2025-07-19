using FluentAssertions;

using MURQ.Application.UrqLoaders;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Application.Tests;

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
            new PrintStatement{ UrqString = "Привет, " },
            new PrintStatement{ UrqString = "мир!"}
        ]);
    }
}
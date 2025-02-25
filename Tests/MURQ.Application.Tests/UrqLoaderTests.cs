using FluentAssertions;

using MURQ.Application;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.ApplicationTests;

public class UrqLoaderTests
{
    [Fact(DisplayName = "����� �� ���� p �����������")]
    public void Quest_of_2p_loads()
    {
        // Arrange
        UrqLoader sut = new("""
            p ������, 
            p ���!
            """);

        // Act
        Quest quest = sut.LoadQuest();

        // Assert
        quest.Statements.Should().BeEquivalentTo([
            new PrintStatement{ Text = "������, " },
            new PrintStatement{ Text = "���!"}
        ]);
    }
}
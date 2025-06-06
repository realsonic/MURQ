using FluentAssertions;

using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Application.Tests;

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
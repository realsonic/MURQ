using FluentAssertions;

using MURQ.Application.UrqLoaders;
using MURQ.Application.UrqLoaders.UrqStrings;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Application.Tests;

public class UrqLoaderTests
{
    [Fact(DisplayName = "����� �� ���� p �����������")]
    public void Quest_of_2p_loads()
    {
        // Arrange
        const string questSource = """
            p ������, 
            p ���!
            """;
        UrqLoader sut = new(new UrqStringLoader(new UrqStringLexer()));

        // Act
        Quest quest = sut.LoadQuest(questSource);

        // Assert
        quest.Statements.Should().BeEquivalentTo([
            new PrintStatement{ UrqString = "������, " },
            new PrintStatement{ UrqString = "���!"}
        ]);
    }
}
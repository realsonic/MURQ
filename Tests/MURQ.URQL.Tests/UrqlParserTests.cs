using FluentAssertions;

using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Parser;
using MURQ.URQL.Tokens.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "Пустой исходник не падает")]
    public void Empty_works()
    {
        // Arrange
        UrqlParser sut = new([]);

        // Act
        Quest quest = sut.ParseQuest();

        // Arrange
        quest.Statements.Should().BeEmpty();
    }

    [Fact(DisplayName = "Одна инструкция p распознаётся")]
    public void One_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([new PrintToken("Привет!", "p Привет!", ((1, 1), (1, 9)))]);

        // Act
        Quest quest = sut.ParseQuest();

        // Asssert
        quest.Statements.Should().BeEquivalentTo([new PrintStatement { Text = "Привет!" }]);
    }

    [Fact(DisplayName = "Две инструкции p распознаются")]
    public void Two_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([new PrintToken("Привет!", "p Привет!", ((1, 1), (1, 9))), new PrintToken("Пока!", "p Пока!", ((1, 1), (1, 7)))]);

        // Act
        Quest quest = sut.ParseQuest();

        // Asssert
        quest.Statements.Should().BeEquivalentTo([
            new PrintStatement { Text = "Привет!" },
            new PrintStatement { Text = "Пока!" }
        ]);
    }
}
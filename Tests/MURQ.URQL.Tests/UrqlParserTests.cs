using FluentAssertions;

using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Parser;
using MURQ.URQL.Tokens.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "Одна инструкция p распознаётся")]
    public void One_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([new PrintToken("Привет!", "p Привет!", ((1, 1), (1, 9)))]);

        // Act
        Quest quest = sut.ParseQuest();

        // Asssert
        quest.Statements.Should().HaveCount(1);
        Statement statement = quest.Statements[0];
        statement.Should().BeOfType<PrintStatement>();
        PrintStatement? printStatement = statement as PrintStatement;
        printStatement!.Text.Should().Be("Привет!");
    }
}
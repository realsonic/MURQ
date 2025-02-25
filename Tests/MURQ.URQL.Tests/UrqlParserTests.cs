using FluentAssertions;

using MURQ.URQL.Parsers;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "Пустой исходник не падает")]
    public void Empty_works()
    {
        // Arrange
        UrqlParser sut = new([]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Arrange
        questSto.Statements.Should().BeEmpty();
    }

    [Fact(DisplayName = "Одна инструкция p распознаётся")]
    public void One_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("Привет!", "p Привет!", ((1, 1), (1, 9)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("Привет!")
        ]);
    }

    [Fact(DisplayName = "Две инструкции p распознаются")]
    public void Two_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("Привет!", "p Привет!", ((1, 1), (1, 9))), 
            new PrintToken("Пока!", "p Пока!", ((1, 1), (1, 7)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("Привет!"),
            new PrintStatementSto("Пока!")
        ]);
    }
}
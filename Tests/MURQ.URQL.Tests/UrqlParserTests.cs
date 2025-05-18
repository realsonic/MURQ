using FluentAssertions;

using MURQ.URQL.Parsers;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "Когда нет токенов, разбор завершается успешно")]
    public void Empty_works()
    {
        // Arrange
        UrqlParser sut = new([]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Arrange
        questSto.Statements.Should().BeEmpty();
    }

    [Fact(DisplayName = "Один токен Print разбирается")]
    public void One_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("Привет!", false, "p Привет!", ((1, 1), (1, 9)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("Привет!", false){ Location = ((1, 1), (1, 9)) }
        ]);
    }

    [Fact(DisplayName = "Два токена Print разбираются")]
    public void Two_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("Привет!", false, "p Привет!", ((1, 1), (1, 9))),
            new NewLineToken("\n", ((1, 1), (1, 10))),
            new PrintToken("Пока!", false, "p Пока!", ((1, 1), (1, 7)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("Привет!", false) { Location = ((1, 1), (1, 9)) },
            new PrintStatementSto("Пока!", false) { Location = ((1, 1), (1, 7)) }
        ]);
    }
}
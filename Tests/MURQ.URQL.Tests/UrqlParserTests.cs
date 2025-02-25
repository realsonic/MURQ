using FluentAssertions;

using MURQ.URQL.Parsers;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "������ �������� �� ������")]
    public void Empty_works()
    {
        // Arrange
        UrqlParser sut = new([]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Arrange
        questSto.Statements.Should().BeEmpty();
    }

    [Fact(DisplayName = "���� ���������� p �����������")]
    public void One_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("������!", "p ������!", ((1, 1), (1, 9)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("������!")
        ]);
    }

    [Fact(DisplayName = "��� ���������� p ������������")]
    public void Two_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("������!", "p ������!", ((1, 1), (1, 9))), 
            new PrintToken("����!", "p ����!", ((1, 1), (1, 7)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("������!"),
            new PrintStatementSto("����!")
        ]);
    }
}
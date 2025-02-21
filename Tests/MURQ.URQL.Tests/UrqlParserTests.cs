using FluentAssertions;

using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;
using MURQ.URQL.Parser;
using MURQ.URQL.Tokens.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "������ �������� �� ������")]
    public void Empty_works()
    {
        // Arrange
        UrqlParser sut = new([]);

        // Act
        Quest quest = sut.ParseQuest();

        // Arrange
        quest.Statements.Should().BeEmpty();
    }

    [Fact(DisplayName = "���� ���������� p �����������")]
    public void One_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([new PrintToken("������!", "p ������!", ((1, 1), (1, 9)))]);

        // Act
        Quest quest = sut.ParseQuest();

        // Asssert
        quest.Statements.Should().BeEquivalentTo([new PrintStatement { Text = "������!" }]);
    }

    [Fact(DisplayName = "��� ���������� p ������������")]
    public void Two_p_parsed()
    {
        // Arrange
        UrqlParser sut = new([new PrintToken("������!", "p ������!", ((1, 1), (1, 9))), new PrintToken("����!", "p ����!", ((1, 1), (1, 7)))]);

        // Act
        Quest quest = sut.ParseQuest();

        // Asssert
        quest.Statements.Should().BeEquivalentTo([
            new PrintStatement { Text = "������!" },
            new PrintStatement { Text = "����!" }
        ]);
    }
}
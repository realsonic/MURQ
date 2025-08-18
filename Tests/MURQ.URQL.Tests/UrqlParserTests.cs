using FluentAssertions;

using MURQ.URQL.Locations;
using MURQ.URQL.Parsing;
using MURQ.URQL.SyntaxTree;
using MURQ.URQL.SyntaxTree.Expressions;
using MURQ.URQL.SyntaxTree.Statements;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements.If;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests;

public class UrqlParserTests
{
    [Fact(DisplayName = "Пустой список токенов даёт пустой список инструкций")]
    public void No_tokens_produce_no_instructions()
    {
        // Arrange
        UrqlParser sut = new([]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Arrange
        questSto.Statements.Should().BeEmpty();
    }

    [Fact(DisplayName = "Токен Print даёт инструкцию Print")]
    public void Print_produce_print()
    {
        // Arrange
        UrqlParser sut = new([
            new PrintToken("Привет!", false, "p Привет!", ((1, 1), (1, 9)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new PrintStatementSto("Привет!", false, ((1, 1), (1, 9)))
        ]);
    }

    [Fact(DisplayName = "Токены Print, NewLine, Print дают две инструкции Print")]
    public void Print_NewLine_Print_produce_two_Prints()
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
            new PrintStatementSto("Привет!", false, ((1, 1), (1, 9))),
            new PrintStatementSto("Пока!", false, ((1, 1), (1, 7)))
        ]);
    }

    [Fact(DisplayName = "Токены If, Variable, Equality, Number, Then, Print дают инструкцию If")]
    public void If_Variable_Equality_Number_Then_Print_produce_If()
    {
        // Arrange
        UrqlParser sut = new([
            /* 
             * if a=4 then pln Привет!
             *          11111111112222
             * 12345678901234567890123
             */
            new IfToken("if", ((1, 1), (1, 2))),
            new VariableToken("a", "a", ((1 , 4), (1, 4))),
            new EqualityToken('=', new Position(1, 5)),
            new NumberToken(4, "4", ((1, 6), (1, 6))),
            new ThenToken("then", ((1, 8), (1, 11))),
            new PrintToken("Привет!", true, "pln Привет!", ((1, 13), (1, 23)))
        ]);

        // Act
        QuestSto questSto = sut.ParseQuest();

        // Asssert
        questSto.Statements.Should().BeEquivalentTo([
            new IfStatementSto(
                new RelationExpressionSto(
                    new VariableExpressionSto("a", ((1 , 4), (1, 4))),
                    new DecimalConstantExpressionSto(4, ((1, 6), (1, 6)))
                ),
                new PrintStatementSto("Привет!", true, ((1, 13), (1, 23))),
                new Position(1, 1)
            )
        ]);
    }
}
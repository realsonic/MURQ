﻿using FluentAssertions;

using MURQ.URQL.Lexing;
using MURQ.URQL.Tokens;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests.Lexer;
public class PrintTests
{
    [Fact(DisplayName = "p с пробелом и новой строкой даёт токен Print с пустым текстом")]
    public void P_space_newline_gives_PrintToken_with_empty_text()
    {
        // Arrange
        UrqlLexer sut = new("p \n");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEquivalentTo<Token>([
            new PrintToken(string.Empty, false, "p ", ((1, 1), (1, 2))),
            new NewLineToken("\n", ((1, 3), (1, 3)))
        ]);
    }

    [Fact(DisplayName = "p с текстом даёт токен Print с текстом")]
    public void P_with_text_gives_PrintToken_with_text()
    {
        // Arrange
        UrqlLexer sut = new("p Привет!");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEquivalentTo([
            new PrintToken("Привет!", false, "p Привет!", ((1, 1), (1, 9)))
        ]);
    }

    [Fact(DisplayName = "Два p с текстом дают два токена Print с текстом")]
    public void Two_p_with_text_gives_two_PrintToken_with_text()
    {
        // Arrange
        UrqlLexer sut = new("""
            p Привет, 
            p мир!
            """);

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.OfType<PrintToken>().Should().BeEquivalentTo([
            new PrintToken("Привет, ", false, "p Привет, ", ((1, 1), (1, 10))),
            new PrintToken("мир!", false, "p мир!", ((2, 1), (2, 6)))
        ]);
    }
}

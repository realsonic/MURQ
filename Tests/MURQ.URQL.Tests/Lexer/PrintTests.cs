using FluentAssertions;

using MURQ.URQL.Lexers;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests.Lexer;
public class PrintTests
{
    [Fact(DisplayName = "\"p \\n\" даёт токен Print с пустым текстом")]
    public void P_space_newline_gives_PrintToken_with_empty_text()
    {
        // Arrange
        UrqlLexer sut = new("p \n");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEquivalentTo(new PrintToken[]
        {
            new(string.Empty, "p ", ((1, 1), (1, 2)))
        });
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
            new PrintToken("Привет!", "p Привет!", ((1, 1), (1, 9)))
        ]);
    }
}

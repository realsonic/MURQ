using FluentAssertions;

using MURQ.URQL.Lexers;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Tests.Lexer;
public class UrqlLexerTests
{
    [Fact(DisplayName = "Строка пробельных символов не возвращает токенов")]
    public void Whitespace_string_returns_no_tokens()
    {
        // Arrange
        UrqlLexer sut = new(" \t  \r ");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEmpty();
    }

    [Fact(DisplayName = "\\n возвращают токены новой строки")]
    public void New_line_characters_returns_NewLineToken()
    {
        // Arrange
        UrqlLexer sut = new(" \n  \n\n ");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEquivalentTo([
            new NewLineToken("\n", ((1,2), (1,2))),
            new NewLineToken("\n", ((2,3), (2,3))),
            new NewLineToken("\n", ((3,1), (3,1)))
        ]);
    }

    [Fact(DisplayName = "При двух переносах строки номер строки - 3")]
    public void Two_new_lines_gives_Line_3()
    {
        // Arrange
        UrqlLexer sut = new(" \n \n ");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Count().Should().Be(2);
        sut.CurrentPosition.Line.Should().Be(3);
    }
}

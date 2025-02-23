using FluentAssertions;

using MURQ.URQL.Parsing.Lexers;

namespace MURQ.URQL.Tests.Lexer;
public class UrqlLexerTests
{
    [Fact(DisplayName = "Строка пробельных символов не возвращает токенов")]
    public void Whitespace_string_returns_no_tokens()
    {
        // Arrange
        UrqlLexer sut = new(" \t  \n ");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEmpty();
    }

    [Fact(DisplayName = "При двух переносах строки номер строки - 3")]
    public void Two_new_lines_gives_Line_3()
    {
        // Arrange
        UrqlLexer sut = new(" \n \n ");

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEmpty();
        sut.CurrentPosition.Line.Should().Be(3);
    }
}

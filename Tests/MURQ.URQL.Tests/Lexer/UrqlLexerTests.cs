using FluentAssertions;

using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;

namespace MURQ.URQL.Tests.Lexer;

public class UrqlLexerTests
{
    [Fact(DisplayName = "Строка пробельных символов не возвращает токенов")]
    public void Whitespace_string_returns_no_tokens()
    {
        // Arrange
        UrqlLexer sut = new(" \t  \r ".ToEnumerableWithoutCarriageReturn().ToPositionedEnumerable().AsOriginatedCharacters());

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEmpty();
    }
}

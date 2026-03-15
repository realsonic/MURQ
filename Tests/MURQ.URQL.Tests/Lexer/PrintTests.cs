using FluentAssertions;

using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
using MURQ.Domain.URQL.Tokens;
using MURQ.Domain.URQL.Tokens.Statements;

namespace MURQ.URQL.Tests.Lexer;
public class PrintTests
{
    [Fact(DisplayName = "p с текстом даёт токен Print с текстом")]
    public void P_with_text_gives_PrintToken_with_text()
    {
        // Arrange
        UrqlLexer sut = new("p Привет!".ToPositionedEnumerable().AsOriginatedCharacters());

        // Act
        var tokens = sut.Scan();

        // Assert
        tokens.Should().BeEquivalentTo([
            new PrintToken("Привет!", false, "p Привет!", ((1, 1), (1, 9)))
        ]);
    }
}

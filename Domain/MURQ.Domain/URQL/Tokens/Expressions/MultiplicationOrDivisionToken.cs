using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

using static MURQ.Domain.URQL.Tokens.Expressions.MultiplicationOrDivisionToken;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"умножение ""*"" или деление ""/""")]
public record MultiplicationOrDivisionToken(OperationEnum Operation, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public enum OperationEnum
    {
        Multiplication,
        Division
    }
}
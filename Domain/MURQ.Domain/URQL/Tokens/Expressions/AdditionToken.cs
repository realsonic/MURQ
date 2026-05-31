using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

using static MURQ.Domain.URQL.Tokens.Expressions.AdditionToken;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"плюс ""+"" или минус ""-""")]
public record AdditionToken(OperationEnum Operation, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public enum OperationEnum
    {
        Addition,
        Substraction
    }
}
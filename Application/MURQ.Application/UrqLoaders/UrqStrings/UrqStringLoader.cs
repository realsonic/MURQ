using MURQ.Application.UrqLoaders.UrqStrings.Tokens;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.UrqStrings;

namespace MURQ.Application.UrqLoaders.UrqStrings;
public class UrqStringLoader(UrqStringLexer urqStringLexer)
{
    public UrqString LoadFromString(string sourceString)
    {
        if (!sourceString.Contains('#')) // оптимизация без подстановок
            return new UrqString([new UrqStringTextPart(sourceString)]);

        Stack<Operand> operandStack = new();
        Stack<Operator> operatorStack = new();

        foreach (Token token in urqStringLexer.Scan(sourceString))
        {
            switch (token)
            {
                case TextToken textToken:
                    operandStack.Push(new TextOperand(textToken.Text));
                    break;

                case SubstitutionStartToken:
                    operatorStack.Push(new BeginIntepolationOperator());
                    break;

                case SubstitutionStopToken:
                    operatorStack.Push(new EndIntepolationOperator());
                    ShrinkStacks();
                    break;

                default: throw new UrqStringException($"Неожиданное токен в URQ-строке: {token}");
            }
        }

        if (operatorStack.Count > 0)
            throw new UrqStringException($"После анализа строки в стеке операторов остались операторы: {operatorStack}");

        IEnumerable<Operand> operands = operandStack.Reverse();
        var urqStringParts = ConvertOperandsToUrqStringParts(operands);
        return new UrqString(urqStringParts);

        void ShrinkStacks()
        {
            if (operatorStack.TryPeek(out Operator? lastOperator) && lastOperator is EndIntepolationOperator)
            {
                operatorStack.Pop();

                if (operandStack.TryPop(out Operand? interOperand))
                {
                    if (interOperand is TextOperand textOperand)
                    {
                        operandStack.Push(new VariableOperand(textOperand.Text));
                    }
                    else throw new UrqStringException($"Перед $ ожидалось имя переменной, а встретился {interOperand}");
                }
                else throw new UrqStringException("Перед $ ожидалось имя переменной, а там пусто");

                if (operatorStack.TryPop(out Operator? previousOperator))
                {
                    if (previousOperator is not BeginIntepolationOperator)
                        throw new UrqStringException($"Перед именем переменной ожидалось #, а там {previousOperator}");
                }
                else throw new UrqStringException("Перед именем переменной ожидалось #, а там пусто");
            }
        }
    }

    private static IEnumerable<UrqStringPart> ConvertOperandsToUrqStringParts(IEnumerable<Operand> operands)
    {
        foreach (Operand operand in operands)
        {
            switch (operand)
            {
                case TextOperand textOperand:
                    yield return new UrqStringTextPart(textOperand.Text);
                    break;

                case VariableOperand variableOperand:
                    yield return new UrqStringExpressionPart(new VariableExpression { VariableName = variableOperand.Name });
                    break;

                default: throw new UrqStringException($"Неизвестный тип операнда: {operand}");
            }
        }
    }

    private abstract record Operand();
    private record TextOperand(string Text) : Operand();
    private record VariableOperand(string Name) : Operand();

    private abstract record Operator();
    private record BeginIntepolationOperator() : Operator();
    private record EndIntepolationOperator() : Operator();
}
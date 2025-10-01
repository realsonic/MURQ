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
                case StringToken stringToken:
                    PushString(stringToken.Text);
                    break;

                case SubstitutionStartToken substitutionStartToken:
                    operatorStack.Push(SubstitutionStartOperator.FromToken(substitutionStartToken));
                    break;

                case SubstitutionStopToken:
                    operatorStack.Push(new SubstitutionStopOperator());
                    ShrinkStacks();
                    break;

                default: throw new NotImplementedException($"Токен URQ-строки {token} пока не обрабатывается.");
            }
        }

        if (operatorStack.Count > 0)
            throw new UrqStringException($"После анализа строки в стеке операторов остались операторы: {operatorStack}");

        IEnumerable<Operand> operands = operandStack.Reverse();
        var urqStringParts = ConvertOperandsToUrqStringParts(operands);
        return new UrqString(urqStringParts);

        void PushString(string @string) => operandStack.Push(new StringOperand(@string));

        void ShrinkStacks()
        {
            if (operatorStack.TryPeek(out Operator? stopOperator) && stopOperator is SubstitutionStopOperator)
            {
                operatorStack.Pop();

                if (!operandStack.TryPop(out Operand? interOperand))
                    throw new UrqStringException("Перед $ ожидалось имя переменной, а там пусто");
                if (interOperand is not StringOperand stringOperand)
                    throw new UrqStringException($"Перед $ ожидалось имя переменной, а встретился {interOperand}");

                if (!operatorStack.TryPop(out Operator? startOperator))
                    throw new UrqStringException("Перед именем переменной ожидалось #, а там пусто");
                if (startOperator is not SubstitutionStartOperator substitutionStartOperator)
                    throw new UrqStringException($"Перед именем переменной ожидалось #, а там {startOperator}");

                bool asString = substitutionStartOperator.Modifier == SubstitutionStartOperator.ModifierEnum.AsString;
                operandStack.Push(new VariableOperand(stringOperand.Text, asString));
            }
        }
    }

    private static IEnumerable<UrqStringPart> ConvertOperandsToUrqStringParts(IEnumerable<Operand> operands)
    {
        foreach (Operand operand in operands)
        {
            switch (operand)
            {
                case StringOperand textOperand:
                    yield return new UrqStringTextPart(textOperand.Text);
                    break;

                case VariableOperand variableOperand:
                    yield return new UrqStringExpressionPart(new VariableExpression { Name = variableOperand.Name }, variableOperand.AsString);
                    break;

                default: throw new NotImplementedException($"Операнд {operand} пока не обратывается.");
            }
        }
    }

    private abstract record Operand();
    private record StringOperand(string Text) : Operand();
    private record VariableOperand(string Name, bool AsString) : Operand();

    private abstract record Operator();
    private record SubstitutionStartOperator(SubstitutionStartOperator.ModifierEnum Modifier) : Operator()
    {
        internal static SubstitutionStartOperator FromToken(SubstitutionStartToken substitutionStartToken)
        {
            ModifierEnum substitutionModifier = substitutionStartToken.SubstitutionModifier switch
            {
                SubstitutionStartToken.ModifierEnum.None => ModifierEnum.None,
                SubstitutionStartToken.ModifierEnum.AsString => ModifierEnum.AsString,
                _ => throw new NotImplementedException($"Модификатор подстановки {substitutionStartToken.SubstitutionModifier} пока не обрабатывается."),
            };
            return new SubstitutionStartOperator(substitutionModifier);
        }

        public enum ModifierEnum
        {
            None,
            AsString
        }
    }
    private record SubstitutionStopOperator() : Operator();
}
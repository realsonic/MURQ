using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions.Tokens;

using static MURQ.URQL.Substitutions.SubstitutedLine;

namespace MURQ.URQL.Substitutions;
public class SubstitutionParser(SubstitutionLexer substitutionLexer)
{
    public SubstitutedLine ParseLine(IEnumerable<(char Character, Position Position)> line)
    {
        foreach (Token token in substitutionLexer.Scan(line))
        {
            switch (token)
            {
                case StringToken stringToken:
                    PushString(stringToken.Value);
                    break;

                case SubstitutionStartToken substitutionStartToken:
                    PushSubstitutionStart(substitutionStartToken);
                    break;

                case SubstitutionStopToken:
                    PushSubstitutionStop();
                    break;

                default: throw new NotImplementedException($"Токен URQ-строки {token} пока не обрабатывается.");
            }
        }

        return PullSubstitutedLine();
    }

    private void PushString(string @string) => substitutionElementStack.Push(new StringOperand(@string));

    private void PushSubstitutionStart(SubstitutionStartToken substitutionStartToken)
        => substitutionElementStack.Push(ConvertToOperator(substitutionStartToken));

    private void PushSubstitutionStop()
    {
        substitutionElementStack.Push(new SubstitutionStopOperator());
        CollapseSubstituton();
    }

    private SubstitutedLine PullSubstitutedLine()
    {
        // перекладываем стек в список частей подстановки
        List<SubstitutedLinePart> substitutionLineParts = [];
        while (substitutionElementStack.TryPop(out SubstitutionElement? substitutionElement))
        {
            SubstitutedLinePart substitutionLinePart = ConvertToSubstitutionPart(substitutionElement);
            substitutionLineParts.Add(substitutionLinePart);
        }

        // переворачиваем список
        substitutionLineParts.Reverse();

        return new SubstitutedLine([.. substitutionLineParts]);
    }

    private static SubstitutionStartOperator ConvertToOperator(SubstitutionStartToken substitutionStartToken)
        => new(substitutionStartToken.SubstitutionModifier switch
        {
            SubstitutionStartToken.ModifierEnum.None => SubstitutionModifierEnum.None,
            SubstitutionStartToken.ModifierEnum.AsString => SubstitutionModifierEnum.AsString,
            _ => throw new NotImplementedException($"Модификатор подстановки {substitutionStartToken.SubstitutionModifier} пока не обрабатывается.")
        });

    private SubstitutedLinePart ConvertToSubstitutionPart(SubstitutionElement substitutionElement)
    {
        switch (substitutionElement)
        {
            case StringOperand stringOperand:
                return new StringPart(stringOperand.Text);

            case SubstitutionOperand substitutionOperand:
                SubstitutionPart.SubstitutionModifierEnum modifier = substitutionOperand.Modifier switch
                {
                    SubstitutionModifierEnum.None => SubstitutionPart.SubstitutionModifierEnum.None,
                    SubstitutionModifierEnum.AsString => SubstitutionPart.SubstitutionModifierEnum.AsString,
                    _ => throw new NotImplementedException($"Модификатор подстановки {substitutionOperand.Modifier} пока не обрабатывается."),
                };
                SubstitutedLinePart[] substitutedLineParts = [.. substitutionOperand.Elements.Select(ConvertToSubstitutionPart)];
                return new SubstitutionPart(modifier, substitutedLineParts);

            case SubstitutionStartOperator:
                return new StringPart("#");

            default: throw new NotImplementedException($"Элемент подстановки {substitutionElement} пока не обратывается.");
        }
    }

    private void CollapseSubstituton()
    {
        // вынимаем закрывающий $
        if (!substitutionElementStack.TryPop(out SubstitutionElement? rightElement))
            throw new InvalidOperationException("Неожиданно в стеке пусто при схлопывании подстановки, а ожидался закрывающий $.");
        if (rightElement is not SubstitutionStopOperator)
            throw new InvalidOperationException($"Неожиданно последний операнд в стеке - {rightElement} при схлопывании подстановки, а ожидался закрывающий $.");

        // вынимаем элементы, пока не встретим # или не вынем всё
        List<SubstitutionElement> substitutionElements = [];
        SubstitutionStartOperator? metSubstitutionStartOperator = null;
        while (substitutionElementStack.TryPop(out SubstitutionElement? substitutionElement))
        {
            if (substitutionElement is SubstitutionStartOperator substitutionStartOperator)
            {
                metSubstitutionStartOperator = substitutionStartOperator;
                break;
            }
            else
            {
                substitutionElements.Add(substitutionElement);
            }
        }

        // переворачиваем элементы, т.к. они в обратном порядке
        substitutionElements.Reverse();

        // если был открывающий # - делаем подстановку и кладём в стек
        if (metSubstitutionStartOperator is not null)
        {
            SubstitutionOperand substitutionOperand = new(metSubstitutionStartOperator.Modifier, [.. substitutionElements]);
            substitutionElementStack.Push(substitutionOperand);
        }
        // иначе считаем закрывающий $ просто текстом
        else
        {
            substitutionElements.Add(new StringOperand("$"));

            foreach (var substitutionElement in substitutionElements)
            {
                substitutionElementStack.Push(substitutionElement);
            }
        }
    }

    readonly Stack<SubstitutionElement> substitutionElementStack = new();

    private abstract record SubstitutionElement();
    private record StringOperand(string Text) : SubstitutionElement();
    private record SubstitutionStartOperator(SubstitutionModifierEnum Modifier) : SubstitutionElement();
    private record SubstitutionStopOperator() : SubstitutionElement();
    private record SubstitutionOperand(SubstitutionModifierEnum Modifier, SubstitutionElement[] Elements) : SubstitutionElement();


    private enum SubstitutionModifierEnum
    {
        None,
        AsString
    }
}
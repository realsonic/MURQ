using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.Quests.QuestLines.SubstitutionTrees;
using MURQ.Domain.URQL.Locations;
using MURQ.URQL.Substitutions.Tokens;

namespace MURQ.URQL.Substitutions;

public class SubstitutionParser(SubstitutionLexer substitutionLexer)
{
    public CodeLine ParseLine(IEnumerable<(char Character, Position Position)> line)
    {
        foreach (Token token in substitutionLexer.Scan(line))
        {
            switch (token)
            {
                case StringToken stringToken:
                    PushString(stringToken);
                    break;

                case SubstitutionStartToken substitutionStartToken:
                    PushSubstitutionStart(substitutionStartToken);
                    break;

                case SubstitutionStopToken substitutionStopToken:
                    PushSubstitutionStop(substitutionStopToken);
                    break;

                default: throw new NotImplementedException($"Токен URQ-строки {token} пока не обрабатывается.");
            }
        }

        return PopSubstitutionTree();
    }

    private void PushString(StringToken token) 
        => substitutionElementStack.Push(new StringOperand(token.Value, token.Location));

    private void PushSubstitutionStart(SubstitutionStartToken token)
    {
        SubstitutionModifierEnum modifier = token.SubstitutionModifier switch
        {
            SubstitutionStartToken.ModifierEnum.None => SubstitutionModifierEnum.None,
            SubstitutionStartToken.ModifierEnum.AsString => SubstitutionModifierEnum.AsString,
            _ => throw new NotImplementedException($"Модификатор подстановки {token.SubstitutionModifier} пока не обрабатывается.")
        };

        substitutionElementStack.Push(new SubstitutionStartOperator(modifier, token.Location));
    }

    private void PushSubstitutionStop(SubstitutionStopToken token)
    {
        substitutionElementStack.Push(new SubstitutionStopOperator(token.Location));
        CollapseSubstituton();
    }

    private CodeLine PopSubstitutionTree()
    {
        // перекладываем стек в список частей подстановки
        List<TreeNode> substitutionLineParts = [];
        while (substitutionElementStack.TryPop(out SubstitutionElement? substitutionElement))
        {
            TreeNode substitutionLinePart = ConvertToNode(substitutionElement);
            substitutionLineParts.Add(substitutionLinePart);
        }

        // переворачиваем список
        substitutionLineParts.Reverse();

        return new CodeLine([.. substitutionLineParts], null); // todo проставлять location
    }

    private void CollapseSubstituton()
    {
        // вынимаем закрывающий $
        if (!substitutionElementStack.TryPop(out SubstitutionElement? rightElement))
            throw new InvalidOperationException("Неожиданно в стеке пусто при схлопывании подстановки, а ожидался закрывающий $.");
        if (rightElement is not SubstitutionStopOperator substitutionStopOperator)
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
            Position start = metSubstitutionStartOperator.Location.Start;
            Position stop = substitutionStopOperator.Location.End;
            SubstitutionOperand substitutionOperand = new(metSubstitutionStartOperator.Modifier, [.. substitutionElements], new Location(start, stop));
            substitutionElementStack.Push(substitutionOperand);
        }
        // иначе считаем закрывающий $ просто текстом
        else
        {
            substitutionElements.Add(new StringOperand("$", substitutionStopOperator.Location));

            foreach (var substitutionElement in substitutionElements)
            {
                substitutionElementStack.Push(substitutionElement);
            }
        }
    }

    private TreeNode ConvertToNode(SubstitutionElement substitutionElement)
    {
        switch (substitutionElement)
        {
            case StringOperand stringOperand:
                return new CodeNode(stringOperand.Text, stringOperand.Location);

            case SubstitutionOperand substitutionOperand:
                SubstitutionModifierEnum modifier = substitutionOperand.Modifier switch
                {
                    SubstitutionModifierEnum.None => SubstitutionModifierEnum.None,
                    SubstitutionModifierEnum.AsString => SubstitutionModifierEnum.AsString,
                    _ => throw new NotImplementedException($"Модификатор подстановки {substitutionOperand.Modifier} пока не обрабатывается."),
                };
                TreeNode[] substitutedLineParts = [.. substitutionOperand.Elements.Select(ConvertToNode)];
                return new SubstitutionNode(modifier, substitutedLineParts, substitutionOperand.Location);

            case SubstitutionStartOperator substitutionStartOperator:
                string text = substitutionStartOperator.Modifier switch
                {
                    SubstitutionModifierEnum.None => "#",
                    SubstitutionModifierEnum.AsString => "#%",
                    _ => throw new NotImplementedException($"Модификатор подстановки {substitutionStartOperator.Modifier} пока не обрабатывается."),
                };
                return new CodeNode(text, substitutionStartOperator.Location);

            default: throw new NotImplementedException($"Элемент подстановки {substitutionElement} пока не обратывается.");
        }
    }

    readonly Stack<SubstitutionElement> substitutionElementStack = new();

    private abstract record SubstitutionElement(Location Location);
    private record StringOperand(string Text, Location Location) : SubstitutionElement(Location);
    private record SubstitutionStartOperator(SubstitutionModifierEnum Modifier, Location Location) : SubstitutionElement(Location);
    private record SubstitutionStopOperator(Location Location) : SubstitutionElement(Location);
    private record SubstitutionOperand(SubstitutionModifierEnum Modifier, SubstitutionElement[] Elements, Location Location) : SubstitutionElement(Location);
}
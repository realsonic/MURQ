using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions.Tokens;

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

    private void PushString(string @string)
    {
        // todo: как-то когда-то преклеивать справа к подстановке

        operandStack.Push(new StringOperand(@string));
    }

    private void PushSubstitutionStart(SubstitutionStartToken substitutionStartToken)
        => operatorStack.Push(ConvertToOperator(substitutionStartToken));

    private static SubstitutionStartOperator ConvertToOperator(SubstitutionStartToken substitutionStartToken)
        => new(substitutionStartToken.SubstitutionModifier switch
        {
            SubstitutionStartToken.ModifierEnum.None => SubstitutionModifierEnum.None,
            SubstitutionStartToken.ModifierEnum.AsString => SubstitutionModifierEnum.AsString,
            _ => throw new NotImplementedException($"Модификатор подстановки {substitutionStartToken.SubstitutionModifier} пока не обрабатывается.")
        });

    private void PushSubstitutionStop()
    {
        operatorStack.Push(new SubstitutionStopOperator());
        CollapseSubstituton();
        GlueSubstituionToLeft();
    }

    private SubstitutedLine PullSubstitutedLine()
    {
        if (operatorStack.Count > 0)
            throw new InvalidOperationException($"Неожиданно в стеке операторов есть операторы при завершении, а ожидалось пусто:\n\t{operatorStack.JoinToString()}.");

        // todo: обработать случай, когда есть $ без # - тогда получается больше одного текста 

        if (operandStack.Count > 1)
            throw new InvalidOperationException($"Неожиданнов в стеке операндов - {operandStack.Count} операндов при завершении, а ожидался один:\n\t{operandStack.JoinToString()}");
        if (!operandStack.TryPop(out Operand? operand))
            throw new InvalidOperationException("Неожиданнов в стеке операндов пусто при завершении, а ожидался один.");

        return operand switch
        {
            StringOperand stringOperand => new StringLine(stringOperand.Text),
            StickySubstitutionOperand stickySubstitutionOperand => ConvertToSubstitutedLine(stickySubstitutionOperand),
            _ => throw new NotImplementedException($"Операнд {operand} пока не обратывается."),
        };
    }

    private SubstitutedLine ConvertToSubstitutedLine(StickySubstitutionOperand stickySubstitutionOperand)
    {
        throw new NotImplementedException();
    }

    private void CollapseSubstituton()
    {
        // вынимаем закрывающий $
        if (!operatorStack.TryPop(out Operator? rightOperator))
            throw new InvalidOperationException("Неожиданно в стеке операторов пусто при схлопывании подстановки, а ожидался закрывающий $.");
        if (rightOperator is not SubstitutionStopOperator)
            throw new InvalidOperationException($"Неожиданно последний операнд в стеке операторов - {rightOperator} при схлопывании подстановки, а ожидался закрывающий $.");

        // todo: обработать случай, когда нет открывающего # - считать $ просто текстом

        // вынимаем открывающий #
        if (!operatorStack.TryPop(out Operator? leftOperator))
            throw new SubstitutionParserException("Нет открывающего #.");
        if (leftOperator is not SubstitutionStartOperator substitutionStartOperator)
            throw new InvalidOperationException($"Неожиданно предпоследний операнд в стеке операторов - {leftOperator} при схлопывании подстановки, а ожидался открывающий #.");

        // вынимаем строку между # и $
        if (!operandStack.TryPop(out Operand? operand))
            throw new InvalidOperationException("Неожиданно в стеке операндов пусто при схлопывании подстановки, а ожидалась строка между # и $.");
        if (operand is not StringOperand stringOperand)
            throw new InvalidOperationException($"Неожиданно последний операнд в стеке операндов - {operand} при схлопывании подстановки, а ожидалась строка между # и $.");

        // превращаем текст в липкую подстановку
        StickySubstitutionOperand stickySubstitutionOperand = new(substitutionStartOperator.Modifier, null, stringOperand, null);
        operandStack.Push(stickySubstitutionOperand);
    }

    private void GlueSubstituionToLeft()
    {
        if (!operandStack.TryPop(out Operand? rightOperand))
            throw new InvalidOperationException("Неожиданно в стеке операндов пусто при приклеивании подстановки влево, а ожидалась липкая подстановка.");
        if (rightOperand is not StickySubstitutionOperand stickySubstitutionOperand)
            throw new InvalidOperationException($"Неожиданно последний операнд в стеке операндов - {rightOperand} при приклеивании подстановки влево, а ожидалась липкая подстановка.");
        if (stickySubstitutionOperand.Left is not null)
            throw new InvalidOperationException($"Неожиданно у липкой подстановки уже занято слева - {stickySubstitutionOperand.Left} при приклеивании подстановки влево, а ожидалось свободно.");

        // если есть предыдущий оператор, вынимаем и приклеиваем
        if (operandStack.TryPop(out Operand? leftOperand))
        {
            stickySubstitutionOperand = stickySubstitutionOperand with { Left = leftOperand };
        }

        // кладём липую кодстановку обратно
        operandStack.Push(stickySubstitutionOperand);
    }

    readonly Stack<Operand> operandStack = new();
    readonly Stack<Operator> operatorStack = new();

    private abstract record Operand();
    private record StringOperand(string Text) : Operand();
    private record StickySubstitutionOperand(SubstitutionModifierEnum Modifier, Operand? Left, Operand Middle, Operand? Right) : Operand();

    private abstract record Operator();
    private record SubstitutionStartOperator(SubstitutionModifierEnum Modifier) : Operator();
    private record SubstitutionStopOperator() : Operator();

    private enum SubstitutionModifierEnum
    {
        None,
        AsString
    }
}

static class Extensions
{
    public static string JoinToString<T>(this Stack<T> stack) => string.Join("\n\t", stack.ToArray());
}
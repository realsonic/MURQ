using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.URQL.Locations;
using MURQ.URQL.Substitutions;

static class ChainExtensions
{
    public static IEnumerable<CodeLine> ToCodeLines(this IEnumerable<IEnumerable<(char Character, Position Position)>> lines)
    {
        foreach (var line in lines)
        {
            SubstitutionParser substitutionParser = new(new SubstitutionLexer());
            yield return substitutionParser.ParseLine(line);
        }
    }

    public static string ToJoinedString<T>(this IEnumerable<T> chars) => string.Join(null, chars);
    public static string ToJoinedString(this IEnumerable<(char Character, Position Position)> chars) => string.Join(null, chars.Select(pc => pc.Character));

    public static string ToJoinedNumberedLines<T>(this IEnumerable<T> elements, Func<T, string> convertElement)
        => string.Join("\n", elements.Select((element, number) => $"[{number + 1}] {convertElement(element)}"));
    public static IEnumerable<string> ToNumberedLines<T>(this IEnumerable<T> elements, Func<T, string>? convertElement = null) where T : notnull
        => elements.Select((element, number) => $"[{number + 1}] {convertElement?.Invoke(element) ?? element.ToString()}");

    public static string ToPrintableChar(this char @char) => char.IsControl(@char) ? $"#{Convert.ToInt32(@char)}" : @char.ToString();
}
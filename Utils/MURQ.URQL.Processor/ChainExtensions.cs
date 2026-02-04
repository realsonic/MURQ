using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions;

static class ChainExtensions
{
    public static IEnumerable<SubstitutionTree> ToSubstitutionTrees(this IEnumerable<IEnumerable<(char Character, Position Position)>> lines)
    {
        foreach (var line in lines)
        {
            SubstitutionParser substitutionParser = new(new SubstitutionLexer());
            yield return substitutionParser.ParseLine(line);
        }
    }

    public static string ToJoinedString(this IEnumerable<char> chars) => string.Join(null, chars);
    public static string ToJoinedString(this IEnumerable<(char Character, Position Position)> chars) => string.Join(null, chars.Select(pc => pc.Character));

    public static string ToNumberedLines<T>(this IEnumerable<T> elements, Func<T, string> convertElement)
        => string.Join("\n", elements.Select((element, number) => $"[{number + 1}] {convertElement(element)}"));

    public static string ToPrintableChar(this char @char) => char.IsControl(@char) ? $"#{Convert.ToInt32(@char)}" : @char.ToString();
}
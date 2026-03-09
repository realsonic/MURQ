using MURQ.Domain.Quests;
using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.URQL;
using MURQ.Domain.URQL.Lexing.EnumerableExtensions;
using MURQ.Domain.URQL.Locations;
using MURQ.URQL.Substitutions;

namespace MURQ.Application.UrqLoaders;

public class UrqLoader(SubstitutionParser substitutionParser)
{
    public Quest LoadQuest(IEnumerable<char> source)
    {
        IEnumerable<QuestLine> questLines = LoadQuestLines(source);
        Quest quest = new(questLines);
        return quest;
    }

    private IEnumerable<QuestLine> LoadQuestLines(IEnumerable<char> source)
    {
        IEnumerable<List<(char Character, Position Position)>> sourceLines = source
            .ToEnumerableWithoutCarriageReturn()
            .ToPositionedEnumerable()
            .ToEnumerableWithoutComments()
            .ToEnumerableWithoutLineContinuations()
            .SplitByLineBreaks();

        foreach (List<(char Character, Position Position)> sourceLine in sourceLines)
        {
            if (sourceLine.IsLabelLine(out string? label))
            {
                LabelLine labelLine = MakeLabelLine(sourceLine, label);
                yield return labelLine;
                continue;
            }

            CodeLine codeLine = substitutionParser.ParseLine(sourceLine); //todo сделать Lazy-парсинг
            yield return codeLine;
        }
    }

    private static LabelLine MakeLabelLine(List<(char Character, Position Position)> sourceLine, string? label)
    {
        Position startPosition = sourceLine.FirstOrDefault().Position;

        if (string.IsNullOrWhiteSpace(label))
            throw new UrqlException($"Метка пустая ({label}) на строке {startPosition.Line}");

        Position endPosition = sourceLine.LastOrDefault().Position;

        LabelLine labelLine = new(label, Location.StartAt(startPosition).EndAt(endPosition));
        return labelLine;
    }
}

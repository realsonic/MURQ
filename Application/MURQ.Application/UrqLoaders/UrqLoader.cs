using MURQ.Domain.Quests;
using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.URQL;
using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Substitutions;

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
        IEnumerable<List<PositionedCharacter>> sourceLines = source
            .ToEnumerableWithoutCarriageReturn()
            .ToPositionedEnumerable()
            .ToEnumerableWithoutComments()
            .ToEnumerableWithoutLineContinuations()
            .SplitByLineBreaks();

        foreach (List<PositionedCharacter> sourceLine in sourceLines)
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

    private static LabelLine MakeLabelLine(List<PositionedCharacter> sourceLine, string? label)
    {
        Position startPosition = sourceLine.FirstOrDefault().Position;

        if (string.IsNullOrWhiteSpace(label))
            throw new UrqlException($"Метка пустая ({label}) на строке {startPosition.Line}");

        Position endPosition = sourceLine.LastOrDefault().Position;

        LabelLine labelLine = new(label, Location.StartAt(startPosition).EndAt(endPosition));
        return labelLine;
    }
}

using MURQ.Domain.Quests.QuestLines;

namespace MURQ.Domain.Quests;

public class Quest
{
    public Quest(IEnumerable<QuestLine> questLines)
    {
        QuestLines = [.. questLines];
        _currentQuestLineIndex = QuestLines.Count > 0 ? 0 : null;
    }

    public IReadOnlyList<QuestLine> QuestLines { get; }

    public QuestLine? CurrentQuestLine => _currentQuestLineIndex is not null ? QuestLines[_currentQuestLineIndex.Value] : null;

    public void NextQuestLine()
    {
        if (_currentQuestLineIndex is null)
            return;

        if (_currentQuestLineIndex == QuestLines.Count - 1)
        {
            _currentQuestLineIndex = null;
            return;
        }

        _currentQuestLineIndex++;
    }

    public void GoToLabel(string label)
    {
        // todo Прикрутить кэш лейбл:индекс

        IEnumerable<(QuestLine QuestLine, int RowNumber)> numberedLines = QuestLines.Select((questLine, rowNumber) => (QuestLine: questLine, RowNumber: rowNumber));

        (LabelLine? labelLine, int? rowNumber) = numberedLines
            .Where(line => line.QuestLine is LabelLine)
            .Select(line => (LabelLine: line.QuestLine as LabelLine, line.RowNumber))
            .FirstOrDefault(line => line.LabelLine!.Label.Equals(label, StringComparison.InvariantCultureIgnoreCase));

        if (rowNumber is not null)
            _currentQuestLineIndex = rowNumber;
    }

    private int? _currentQuestLineIndex;
}
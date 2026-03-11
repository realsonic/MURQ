using MURQ.Domain.Quests.QuestLines;

using System.Diagnostics.CodeAnalysis;

namespace MURQ.Domain.Quests;

public class Quest
{
    public Quest(IEnumerable<QuestLine> questLines)
    {
        QuestLines = [.. questLines];
        _currentQuestLineIndex = QuestLines.Count > 0 ? 0 : null;
        CacheLabels();
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

    public bool TryGoToLabel(string targetLabel, [NotNullWhen(true)] out string? resultLabel)
    {
        if (_labelDictionary.TryGetValue(targetLabel, out int index))
        {
            if (QuestLines[index] is LabelLine labelLine)
            {
                _currentQuestLineIndex = index;

                resultLabel = labelLine.Label;
                return true;
            }
        }

        resultLabel = null;
        return false;
    }

    private void CacheLabels()
    {
        for (int index = 0; index < QuestLines.Count; index++)
        {
            if (QuestLines[index] is LabelLine labelLine)
            {
                _labelDictionary.TryAdd(labelLine.Label, index);
            }
        }
    }

    private readonly Dictionary<string, int> _labelDictionary = new(StringComparer.InvariantCultureIgnoreCase);
    private int? _currentQuestLineIndex;
}
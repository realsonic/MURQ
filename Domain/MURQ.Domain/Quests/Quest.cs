using MURQ.Domain.Quests.QuestLines;

using System.Diagnostics.CodeAnalysis;

namespace MURQ.Domain.Quests;

public class Quest
{
    public Quest(IEnumerable<QuestLine> lines)
    {
        Lines = [.. lines];
        _currentLineIndex = Lines.Count > 0 ? 0 : null;
        CacheLabels();
    }

    public IReadOnlyList<QuestLine> Lines { get; }

    public QuestLine? CurrentLine => _currentLineIndex is not null ? Lines[_currentLineIndex.Value] : null;
    public void ClearCurrentLine() => _currentLineIndex = null;

    public void NextLine()
    {
        if (_currentLineIndex is null)
            return;

        if (_currentLineIndex == Lines.Count - 1)
        {
            _currentLineIndex = null;
            return;
        }

        _currentLineIndex++;
    }

    public bool TryGoToLabel(string targetLabel, [NotNullWhen(true)] out string? resultLabel)
    {
        if (_labelDictionary.TryGetValue(targetLabel, out int index))
        {
            if (Lines[index] is LabelLine labelLine)
            {
                _currentLineIndex = index;

                resultLabel = labelLine.Label;
                return true;
            }
        }

        resultLabel = null;
        return false;
    }

    private void CacheLabels()
    {
        for (int index = 0; index < Lines.Count; index++)
        {
            if (Lines[index] is LabelLine labelLine)
            {
                _labelDictionary.TryAdd(labelLine.Label, index);
            }
        }
    }

    private readonly Dictionary<string, int> _labelDictionary = new(StringComparer.InvariantCultureIgnoreCase);
    private int? _currentLineIndex;
}
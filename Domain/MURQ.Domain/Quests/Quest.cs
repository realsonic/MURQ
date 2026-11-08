using MURQ.Domain.Quests.QuestLines;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace MURQ.Domain.Quests;

public class Quest
{
    public Quest(IEnumerable<QuestLine> lines)
    {
        Lines = [.. lines];
        _currentLineIndex = Lines.Length > 0 ? 0 : null;
        CacheLabels();
    }

    public ImmutableArray<QuestLine> Lines { get; }

    public QuestLine? CurrentLine => _currentLineIndex is not null ? Lines[_currentLineIndex.Value] : null;
    public void ClearCurrentLine() => _currentLineIndex = null;

    public void GoToNextLine()
    {
        if (_currentLineIndex is null)
            return;

        if (_currentLineIndex == Lines.Length - 1)
        {
            _currentLineIndex = null;
            return;
        }

        _currentLineIndex++;
    }

    public bool IsLabelPhantom(string label) => _labelDictionary.ContainsKey(label) is false;

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

    public void GoToQuestLine(QuestLine questLine)
    {
        if (_linesDictionary.TryGetValue(questLine, out int index))
        {
            _currentLineIndex = index;
            return;
        }

        index = Lines.IndexOf(questLine);
        if (index == -1)
            throw new InvalidOperationException($"Не удалось найти {nameof(questLine)} в {nameof(Lines)}.");

        _currentLineIndex = index;
        _linesDictionary[questLine] = index;
    }

    private void CacheLabels()
    {
        for (int index = 0; index < Lines.Length; index++)
        {
            if (Lines[index] is LabelLine labelLine)
            {
                _labelDictionary.TryAdd(labelLine.Label, index);
            }
        }
    }

    private int? _currentLineIndex;
    private readonly Dictionary<string, int> _labelDictionary = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<QuestLine, int> _linesDictionary = [];
}
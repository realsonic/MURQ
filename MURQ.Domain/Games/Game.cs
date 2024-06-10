using System.Text;

using MURQ.Domain.Exceptions;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Games;

public class Game
{
    public Game(Quest quest)
    {
        Quest = quest;
        
        _globalRunningContext = new RunningContext();
        SubscribeToRunningContextEvents();
    }

    private void SubscribeToRunningContextEvents()
    {
        _globalRunningContext.OnTextPrinted += text => _currentScreenText.Append(text);
    }

    public Quest Quest { get; }

    public CurrentLocationView CurrentLocation => new()
    {
        Text = _currentScreenText.ToString()
    };

    public void Start()
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        ResetCurrentView();
        SetNextInstructionToFirstInQuest();
        RunInstructions();
    }

    private void ResetCurrentView()
    {
        _currentScreenText.Clear();
    }

    private void RunInstructions()
    {
        SetModeRunningInstructions();
        while (IsRunningInstructions)
        {
            RunCurrentInstruction();
            PromoteNextInstruction();
        }
    }

    private void RunCurrentInstruction()
    {
        if (CurrentInstruction is null)
        {
            SetModeWaitingUserInput();
            return;
        }

        CurrentInstruction.Run(_globalRunningContext);
    }

    private void PromoteNextInstruction()
    {
        if (_currentInstructionIndex is not null)
        {
            _previousInstructionIndex = _currentInstructionIndex;

            int nextInstructionIndex = _currentInstructionIndex.Value + 1;
            if (nextInstructionIndex <= Quest.Instructions.Count - 1)
            {
                _currentInstructionIndex = nextInstructionIndex;
            }
            else
            {
                _currentInstructionIndex = null;
            }
        }
    }

    private void SetNextInstructionToFirstInQuest() => _currentInstructionIndex = Quest.Instructions.Count > 0 ? 0 : null;
    private void SetModeRunningInstructions() => _gameMode = GameMode.RunningInstructions;
    private void SetModeWaitingUserInput() => _gameMode = GameMode.WaitingUserInput;

    private bool IsStarted => _gameMode is not GameMode.InitialState;
    private bool IsRunningInstructions => _gameMode == GameMode.RunningInstructions;

    private Instruction? CurrentInstruction =>
        _currentInstructionIndex is not null ? Quest.Instructions[_currentInstructionIndex.Value] : null;

    private Instruction? PreviousInstruction =>
        _previousInstructionIndex is not null ? Quest.Instructions[_previousInstructionIndex.Value] : null;

    public class CurrentLocationView
    {
        public string? Text { get; init; }
    }

    private GameMode _gameMode = GameMode.InitialState;
    private int? _previousInstructionIndex;
    private int? _currentInstructionIndex;
    private readonly RunningContext _globalRunningContext;
    private readonly StringBuilder _currentScreenText = new();
}
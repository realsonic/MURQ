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

    public Quest Quest { get; }

    public CurrentLocationView CurrentLocation => new()
    {
        Text = _currentScreenText.ToString(),
        Buttons = _currentScreenButtons
    };

    public void Start()
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        ClearCurrentView();
        SetNextInstructionToFirstInQuest();
        RunInstructions();
    }

    private void SubscribeToRunningContextEvents()
    {
        _globalRunningContext.OnTextPrinted += text => _currentScreenText.Append(text);
        _globalRunningContext.OnButtonAdded += (caption, labelInstruction) => _currentScreenButtons.Add(new Button
        {
            Caption = caption,
            OnButtonPressed = () =>
            {
                ClearCurrentView();
                GoToLabel(labelInstruction);
                RunInstructions();
            }
        });
    }

    private void ClearCurrentView()
    {
        _currentScreenText.Clear();
        _currentScreenButtons.Clear();
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

    private void GoToLabel(LabelInstruction? labelInstruction)
    {
        if (labelInstruction is not null)
        {
            SetCurrentInstruction(labelInstruction);
        }
    }

    private void SetCurrentInstruction(Instruction instruction)
    {
        int? instructionIndex = Quest.GetInstructionIndex(instruction);
        if (instructionIndex is not null)
        {
            _currentInstructionIndex = instructionIndex;
        }
    }

    private void PromoteNextInstruction()
    {
        if (_currentInstructionIndex is null) return;

        _previousInstructionIndex = _currentInstructionIndex;

        // todo перенести логику в Quest
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

    private void SetNextInstructionToFirstInQuest() =>
        _currentInstructionIndex = Quest.Instructions.Count > 0 ? 0 : null;

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
        public IReadOnlyCollection<Button>? Buttons { get; init; }
    }

    public class Button
    {
        public required string Caption { get; init; }
        public required Action OnButtonPressed { get; init; }

        public void Press() => OnButtonPressed();
    }

    private GameMode _gameMode = GameMode.InitialState;
    private int? _previousInstructionIndex;
    private int? _currentInstructionIndex;
    private readonly RunningContext _globalRunningContext;
    private readonly StringBuilder _currentScreenText = new();
    private readonly List<Button> _currentScreenButtons = new();
}
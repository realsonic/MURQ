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

        _globalGameContext = new GameContext();
        SubscribeToRunningContextEvents();
    }

    public Quest Quest { get; }

    public CurrentLocationView CurrentLocation => new()
    {
        Name = _currentLocationName,
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
        _globalGameContext.OnLocationChanged += locationName => _currentLocationName = locationName;
        _globalGameContext.OnTextPrinted += text => _currentScreenText.Append(text);
        _globalGameContext.OnButtonAdded += (caption, labelInstruction) => _currentScreenButtons.Add(new Button
        {
            Caption = caption,
            OnButtonPressed = () => GoToNewLocation(labelInstruction)
        });
        _globalGameContext.OnEnd += StopAndWaitUser;
    }

    private void GoToNewLocation(LabelInstruction? labelInstruction)
    {
        ClearCurrentView();
        GoToLabel(labelInstruction);
        RunInstructions();
    }

    private void StopAndWaitUser()
    {
        SetModeWaitingUserInput();
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
        if (_currentInstruction is null)
        {
            SetModeWaitingUserInput();
            return;
        }

        _currentInstruction.Run(_globalGameContext);
    }

    private void GoToLabel(LabelInstruction? labelInstruction)
    {
        if (labelInstruction is not null)
        {
            SetCurrentInstruction(labelInstruction);
        }
    }

    private void PromoteNextInstruction() => _currentInstruction = Quest.GetNextInstruction(_currentInstruction);

    private void SetNextInstructionToFirstInQuest() => _currentInstruction = Quest.FirstInstruction;
    private void SetModeRunningInstructions() => _gameMode = GameMode.RunningInstructions;
    private void SetModeWaitingUserInput() => _gameMode = GameMode.WaitingUserInput;
    private void SetCurrentInstruction(Instruction instruction) => _currentInstruction = instruction;

    private bool IsStarted => _gameMode is not GameMode.InitialState;
    private bool IsRunningInstructions => _gameMode == GameMode.RunningInstructions;

    public class CurrentLocationView
    {
        public string? Name { get; init; }
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
    private Instruction? _currentInstruction;
    private readonly GameContext _globalGameContext;
    private readonly StringBuilder _currentScreenText = new();
    private readonly List<Button> _currentScreenButtons = new();
    private string? _currentLocationName;
}
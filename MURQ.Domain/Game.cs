using MURQ.Domain.Exceptions;
using MURQ.Domain.Instructions;

namespace MURQ.Domain;

public class Game(Quest quest)
{
    public Quest Quest { get; } = quest;

    public CurrentLocationView? CurrentLocation
    {
        get { return new CurrentLocationView(); }
    }

    public void Start()
    {
        if (IsStarted) throw new MurqException("Игра уже запущена, второй раз запустить нельзя.");

        SetNextInstructionToFirstInQuest();
        RunInstructions();
    }

    private void RunInstructions()
    {
        SetModeRunningInstructions();
        while (IsRunningInstructions) RunNextInstruction();
    }

    private void RunNextInstruction()
    {
        if (_nextInstruction is null)
        {
            SetModeWaitingUserInput();
            return;
        }

        //todo RUN action for next instruction 
    }

    private void SetNextInstructionToFirstInQuest() => _nextInstruction = Quest.Instructions.FirstOrDefault();
    private void SetModeRunningInstructions() => _gameMode = GameMode.RunningInstructions;
    private void SetModeWaitingUserInput() => _gameMode = GameMode.WaitingUserInput;

    private bool IsStarted => _gameMode != GameMode.InitialState;
    private bool IsRunningInstructions => _gameMode == GameMode.RunningInstructions;

    public class CurrentLocationView
    {
        public string? Text { get; init; }
    }

    private GameMode _gameMode = GameMode.InitialState;
    private Instruction? _nextInstruction;
}
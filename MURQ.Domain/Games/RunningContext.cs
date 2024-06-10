namespace MURQ.Domain.Games;

public class RunningContext
{
    public event Action<string?>? OnTextPrinted;

    public void CallPrintText(string? text) => OnTextPrinted?.Invoke(text);
}
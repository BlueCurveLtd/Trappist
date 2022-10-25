namespace Trappist.Wpf.Bedrock.Abstractions;

public interface IUiThread
{
    void Execute(Action action);
    Task ExecuteAsync(Action action);
}

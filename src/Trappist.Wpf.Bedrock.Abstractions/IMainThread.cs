namespace Trappist.Wpf.Bedrock.Abstractions;

public interface IMainThread
{
    void Execute(Action action);
    Task ExecuteAsync(Action action);
}

public interface ICommand
{
    void StartMethod();
    void OnTick();
    void OnEnd();
    bool IsCompleted { get; }
}

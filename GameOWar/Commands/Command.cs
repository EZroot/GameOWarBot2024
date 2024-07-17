public class Command : ICommand
{
    protected int ticksRemaining;

    public Command(int duration)
    {
        ticksRemaining = duration;
    }

    public virtual void StartMethod()
    {
        // Initialization logic
    }

    public virtual void OnTick()
    {
        if (ticksRemaining > 0)
        {
            ticksRemaining--;
            // Logic for each tick
        }
    }

    public virtual void OnEnd()
    {
        // Cleanup logic
    }

    public bool IsCompleted => ticksRemaining <= 0;
}

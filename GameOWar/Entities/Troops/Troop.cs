[Serializable]
public class Troop
{
    public string Name { get; }
    public long Amount { get; private set; }
    public long Level { get; private set; }

    public Troop(string name, long amount, int level)
    {
        Name = name;
        Amount = amount;
        Level = level;
    }

    public Troop()
    {
    }

    public void AddTroopCount(long amount)
    {
        Amount += amount;
    }
    public void RemoveTroopCount(long amount)
    {
        Amount -= amount;
    }

    public void AddLevel(long amount)
    {
        Level += amount;
    }
}

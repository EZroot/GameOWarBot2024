using GameOWar.Entities;

[Serializable]
public class Building
{
    public string Name { get; }
    public long Level { get; set; }
    public long Experience { get; set; }

    public Building()
    { }

    public Building(string name)
    {
        Name = name;
        Level = 1;
        Experience = 0;
    }

    // Method to calculate experience required for a given level
    private int ExperienceForLevel(long level)
    {
        if (level <= 1) return 0;

        int totalExperience = 0;
        for (int i = 1; i < level; i++)
        {
            totalExperience += (int)Math.Floor(i + 300 * Math.Pow(2, i / 7.0));
        }
        return totalExperience / 4;
    }

    // Method to gain experience and handle leveling up
    public void GainExperience(long amount)
    {
        Experience += amount;
        while (Experience >= ExperienceForLevel(Level + 1))
        {
            Level++;
            //Console.WriteLine($"{Name} leveled up to level {Level}!");
        }
    }

    public virtual void PerformTask(Player owner, Base playerBase)
    {
        
    }
}

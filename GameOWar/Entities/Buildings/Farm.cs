using GameOWar.Entities;

[Serializable]
public class Farm : Building
{
    public Farm() : base("Farm") { }

    public override void PerformTask(Player owner, Base playerBase)
    {
        // Logic to extract ore
        long result = Level;//new Random().Next(10 * (int)Level) + 10 * Level;
        //Console.WriteLine($"Collecting {val} food");
        playerBase.CollectResource(new Food(result));
        GainExperience(result);
    }
}

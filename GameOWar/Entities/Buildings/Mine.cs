
using GameOWar.Entities;

public class Mine : Building
{
    public Mine() : base("Mine") { }

    public override void PerformTask(Player owner, Base playerBase)
    {
        // Logic to extract ore
        //Console.WriteLine("Mining 10 ore");
        var result = new Random().Next(10 * (int)Level) + 10 * Level;
        playerBase.CollectResource(new Ore(result));
        GainExperience(result);
    }
}
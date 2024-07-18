
using GameOWar.Entities;

[Serializable]
public class Logging : Building
{
    public Logging() : base("Logging") { }

    public override void PerformTask(Player owner, Base playerBase)
    {
        // Logic to extract ore
        //Console.WriteLine("Mining 10 ore");
        var result = new Random().Next(10 * (int)Level) + 10 * Level;
        playerBase.CollectResource(new Tree(result));
        GainExperience(result);
    }
}
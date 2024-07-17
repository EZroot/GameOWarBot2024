
using GameOWar.Entities;

public class MarketPlace : Building
{
    public MarketPlace() : base("MarketPlace") { }

    public override void PerformTask(Player owner, Base playerBase)
    {
        // Logic to extract ore
        //Console.WriteLine("Mining 10 ore");
        var result = new Random().Next(10 * (int)Level) + 10 * Level;
        owner.Currency.Amount += result;
        GainExperience(result);
    }
}
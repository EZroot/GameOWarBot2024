
using GameOWar.Entities;

[Serializable]
public class MarketPlace : Building
{
    public MarketPlace() : base("MarketPlace") { }

    public override void PerformTask(Player owner, Base playerBase)
    {
        //Sell 10% of product
        var oreToSell = playerBase.Resources.Find(x => x.Name == "Ore")?.Quantity;
        var treeToSell = playerBase.Resources.Find(x => x.Name == "Tree")?.Quantity;

        if (oreToSell == null) oreToSell = 0;
        if (treeToSell == null) treeToSell = 0;

        //sell 10 percent of product
        if (oreToSell > 0) playerBase.RemoveResource("Ore", (int)oreToSell/10);
        if (treeToSell > 0) playerBase.RemoveResource("Tree", (int)treeToSell/10);

        var sales = (long)(oreToSell + treeToSell);

        owner.Currency.Amount += Level + sales;

        var result = (new Random().Next((int)Level * 10) + Level + 10);
        GainExperience(result);
    }
}
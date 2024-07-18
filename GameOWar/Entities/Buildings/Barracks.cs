using GameOWar.Entities;

//Provides troop storage increase
[Serializable]
public class Barracks : Building
{
    public long Population => Level * 10;

    public Barracks() : base("Barracks") { }

    public Barracks(string name) : base(name)
    {
    }

    public override void PerformTask(Player owner, Base playerBase)
    {
        GainExperience(new Random().Next((int)Level * 10) + Level + 10);
    }
}

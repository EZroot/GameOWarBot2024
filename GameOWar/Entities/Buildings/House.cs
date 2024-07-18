using GameOWar.Entities;

//Provides civilian storage increase

[Serializable]
public class House : Building
{
    public long Population => Level * 10;
    public House() : base("House") { }

    public override void PerformTask(Player owner, Base playerBase)
    {
        GainExperience(new Random().Next((int)Level * 10) + Level * 10);
    }
}

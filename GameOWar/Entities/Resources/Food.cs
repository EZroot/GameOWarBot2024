
[Serializable]
public class Food : Resource
{
    public Food() : base ("",0) { }
    public Food(long quantity) : base("Food", quantity) { }
}

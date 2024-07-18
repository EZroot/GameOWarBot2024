[Serializable]
public class Resource
{
    public string Name { get; }
    public long Quantity { get; set; }

    public Resource(string name, long quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    public void Collect(long amount)
    {
        Quantity += amount;
    }

    public void Remove(long amount)
    {
        Quantity -= amount;
    }
}

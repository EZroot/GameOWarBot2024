using DiscordBot;
using GameOWar.Entities;
using GameOWar.Events;
using GameOWar.World;
using System.Diagnostics;

public class Base
{
    public Player Owner { get; private set; }

    public string BaseName { get; private set; }
    public List<Building> Buildings { get; private set; } = new();
    public List<Resource> Resources { get; private set; } = new();
    public List<Troop> Troops { get; private set; } = new();
    public WorldTile WorldTile { get; private set; }
    public long Population { get; private set; }
    public long PredictedPopulation => Buildings.Where(x => x.Name == "House").Sum(x => ((House)x).Population);
    public bool IsPerformingAction { get; set; }
    public bool IsTroopsRecovering { get; set; }
    public Base(string name, WorldTile location)
    {
        BaseName = name;
        WorldTile = location;

        EventHub.Subscribe<EventOnTick>(OnTick_Simulate);
    }

    public void SetOwner(Player owner, bool sendToDiscord = true)
    {
        if (Owner != null) Owner.PlayerBases.Remove(this);
        Owner = owner;
        if(sendToDiscord) BotManager.Instance.QueueMessage($"{BaseName} Setting owner to {Owner.UserName}");
    }

    private void OnTick_Simulate(EventOnTick data)
    {
        if (IsPerformingAction) return;

        SimulatePopulation();
        SimulateTroops();
        PerformTasks();
    }

    public void Destroy()
    {
        EventHub.Unsubscribe<EventOnTick>(OnTick_Simulate);
    }

    public void AddBuilding(Building building)
    {
        Buildings.Add(building);
    }

    public void AddTroop(Troop troop)
    {
        // Check if the troop type already exists
        var existingTroop = Troops.Find(t => t.Name == troop.Name);

        if (existingTroop != null)
        {
            // Add to the existing troop count
            existingTroop.AddTroopCount(troop.Amount);
        }
        else
        {
            // Add as a new troop
            Troops.Add(troop);
        }
    }

    public void RemoveTroop(string troopName, int amount)
    {
        // Check if the troop type already exists
        var existingTroop = Troops.Find(t => t.Name == troopName);

        if (existingTroop != null)
        {
            // Add to the existing troop count
            existingTroop.RemoveTroopCount(amount);
        }
    }

    public long TotalTroopCount()
    {
        return Troops.Sum(t => t.Amount);
    }

    public void CollectResource(Resource resource)
    {
        // Check if the resource type already exists
        var existingResource = Resources.Find(r => r.Name == resource.Name);

        if (existingResource != null)
        {
            // Add to the existing quantity
            existingResource.Collect(resource.Quantity);
        }
        else
        {
            // Add as a new resource
            Resources.Add(resource);
        }
    }

    public void RemoveResource(string resourceName, int amount)
    {
        var existingResource = Resources.Find(r => r.Name == resourceName);
        if(existingResource != null)
        {
            existingResource.Remove(amount);
        }
    }

    private void PerformTasks()
    {
        foreach (var building in Buildings)
        {
            building.PerformTask(Owner, this);
        }
    }

    private void SimulatePopulation()
    {
        var houses = Buildings.Where(x => x.Name == "House").ToList();
        long houseCapacity = houses.Sum(house => ((House)house).Population); // Assuming each house has a capacity property

        if (Population < houseCapacity)
        {
            Population += new Random().Next(1, houses.Count + 1); // Grow population by a random number between 1 and the number of houses
            Population = Math.Min(Population, houseCapacity); // Ensure population doesn't exceed capacity
        }
    }


    private void SimulateTroops()
    {
        var barracks = Buildings.Where(x => x.Name == "Barracks").ToList();
        long barracksCapacity = barracks.Sum(barrack => ((Barracks)barrack).Population); // Assuming each barracks has a capacity property

        if (Population > 0 && barracksCapacity > 0)
        {
            long newTroops = new Random().Next(1, (int)Math.Min(Population, barracksCapacity) / 10 + 1); // Recruit up to 10% of the population or barracks capacity as troops
            AddTroop(new Troop("infantry", newTroops, 1));
            Population -= newTroops; // Decrease population by the number of new troops recruited
        }
    }


}

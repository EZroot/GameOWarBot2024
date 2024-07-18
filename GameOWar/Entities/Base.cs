using DiscordBot;
using GameOWar.Entities;
using GameOWar.Events;
using GameOWar.World;
using System.Diagnostics;
using System;

[Serializable]
public class Base
{
    public string Owner { get; set; }

    public string BaseName { get; set; }
    public List<Building> Buildings { get; set; } = new();
    public List<Resource> Resources { get; set; } = new();
    public List<Troop> Troops { get; set; } = new();
    public WorldTile WorldTile { get; set; }
    public long Population { get; set; }
    public long PredictedPopulation => Buildings.Where(x => x.Name == "House").Sum(x => ((House)x).Population);
    public bool IsPerformingAction { get; set; }
    public bool IsTroopsRecovering { get; set; }
    public Base(string name, WorldTile location)
    {
        BaseName = name;
        WorldTile = location;
        EventHub.Subscribe<EventOnTick>(OnTick_Simulate);
    }

    public Base()
    {
        EventHub.Subscribe<EventOnTick>(OnTick_Simulate);

    }

    public void SetOwner(string owner, bool sendToDiscord = true)
    {
        if (!string.IsNullOrEmpty(Owner)) BotManager.Instance.Game.WorldMap.FindPlayer(Owner).PlayerBases.Remove(this);
        Owner = owner;
        if(sendToDiscord) BotManager.Instance.QueueMessage($"{BaseName} Setting owner to {Owner}");
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
            var player = BotManager.Instance.Game.WorldMap.FindPlayer(Owner);
            building.PerformTask(player, this);
        }
    }

    private void SimulatePopulation()
    {
        var houses = Buildings.Where(x => x.Name == "House").ToList();
        long houseCapacity = houses.Sum(house => ((House)house).Population); // Assuming each house has a capacity property
        var foodResource = Resources.Find(x => x.Name == "Food");
        long currentFood = foodResource?.Quantity ?? 0; // Get the current food quantity, default to 0 if not found

        if (currentFood > 0)
        {
            // Calculate potential population growth based on the food available
            long potentialPopulationIncrease = new Random().Next(1, houses.Count + 1);
            long requiredFoodForGrowth = potentialPopulationIncrease * 2; // Assume each new person requires 2 units of food per day

            // Ensure there is enough food for the population increase
            if (currentFood >= requiredFoodForGrowth)
            {
                Population += potentialPopulationIncrease;
                Population = Math.Min(Population, houseCapacity); // Ensure population doesn't exceed house capacity
                currentFood -= requiredFoodForGrowth;
            }

            // Ensure the current population consumes food
            long requiredFoodForPopulation = Population * 2; // Assume each person requires 2 units of food per day
            if (currentFood >= requiredFoodForPopulation)
            {
                currentFood -= requiredFoodForPopulation;
            }
            else
            {
                // If there's not enough food, reduce the population accordingly
                long populationDecrease = (requiredFoodForPopulation - currentFood) / 2; // Each person requires 2 units of food
                Population -= populationDecrease;
                currentFood = 0; // All food is consumed
            }

            // Update the food resource quantity
            if (foodResource != null)
            {
                foodResource.Quantity = currentFood;
            }
        }
        else
        {
            // If there's no food, decrease the population
            long populationDecrease = Population / 10; // Decrease population by 10% if no food is available
            Population -= populationDecrease;
        }

        // Ensure the population doesn't exceed house capacity after all adjustments
        Population = Math.Min(Population, houseCapacity);
    }



    private void SimulateTroops()
    {
        var barracks = Buildings.Where(x => x.Name == "Barracks").ToList();
        long barracksCapacity = barracks.Sum(barrack => ((Barracks)barrack).Population); // Assuming each barracks has a capacity property
        var foodResource = Resources.Find(x => x.Name == "Food");
        long currentFood = foodResource?.Quantity ?? 0; // Get the current food quantity, default to 0 if not found

        if (Population > 0 && barracksCapacity > 0)
        {
            // Calculate potential new troops based on the population and barracks capacity
            long potentialNewTroops = new Random().Next(1, (int)Math.Min(Population, barracksCapacity) / 10 + 1);
            long requiredFoodForNewTroops = potentialNewTroops * 3; // Assume each new troop requires 3 units of food per day

            // Ensure there is enough food for recruiting new troops
            if (currentFood >= requiredFoodForNewTroops)
            {
                AddTroop(new Troop("infantry", potentialNewTroops, 1));
                Population -= potentialNewTroops; // Decrease population by the number of new troops recruited
                currentFood -= requiredFoodForNewTroops;
            }

            // Ensure the current troops consume food
            long totalTroopCount = Troops.Sum(t => t.Amount);
            long requiredFoodForTroops = totalTroopCount * 3; // Assume each troop requires 3 units of food per day
            if (currentFood >= requiredFoodForTroops)
            {
                currentFood -= requiredFoodForTroops;
            }
            else
            {
                // If there's not enough food, reduce the troop count accordingly
                long troopDecrease = (requiredFoodForTroops - currentFood) / 3; // Each troop requires 3 units of food
                RemoveTroops(troopDecrease);
                currentFood = 0; // All food is consumed
            }

            // Update the food resource quantity
            if (foodResource != null)
            {
                foodResource.Quantity = currentFood;
            }
        }
        else if (Population > 0)
        {
            // If there's no barracks but there's population, decrease the population to reflect lack of training facilities
            long populationDecrease = Population / 20; // Decrease population by 5% if no barracks are available
            Population -= populationDecrease;
        }
        else if (barracksCapacity > 0)
        {
            // If there's no population but there's barracks, reduce the barracks capacity (simulate destruction)
            long capacityDecrease = barracksCapacity / 10; // Decrease capacity by 10%
                                                           // Simulate reduction in capacity (this could be implemented in a more realistic way)
        }
    }

    private void RemoveTroops(long troopDecrease)
    {
        while (troopDecrease > 0 && Troops.Count > 0)
        {
            var troop = Troops[0];
            if (troop.Amount <= troopDecrease)
            {
                troopDecrease -= troop.Amount;
                Troops.RemoveAt(0);
            }
            else
            {
                troop.RemoveTroopCount((int)troopDecrease);
                troopDecrease = 0;
            }
        }
    }


}

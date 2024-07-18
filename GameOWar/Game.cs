using System;
using System.Numerics;
using System.Timers;
using DiscordBot;
using GameOWar;
using GameOWar.Commands;
using GameOWar.Entities;
using GameOWar.Events;
using GameOWar.World;

public class Game
{
    private readonly System.Timers.Timer _tickTimer;
    private readonly int _tickInterval; // Time in milliseconds for each tick

    private WorldMap _worldMap;

    public WorldMap WorldMap => _worldMap;

    public Game(int tickInterval = GameSettings.TICK_INTERVAL) // Default to 1 second
    {
        _tickInterval = tickInterval;
        _tickTimer = new System.Timers.Timer(_tickInterval)
        {
            AutoReset = true // Ensure the timer resets automatically
        };
        _tickTimer.Elapsed += OnTick;
    }

    public void Start(string[] discordUsernames)
    {
        Console.WriteLine("Welcome to the Idle Game!");
        _worldMap = new WorldMap(128, 128);
        foreach (var p in discordUsernames)
        {
            Player? player = DataManager.LoadPlayer(p + ".json");
            if (player == null || GameSettings.FORCE_CREATE_NEW_USERS) 
            {
                Console.WriteLine("Player failed to load " + p + ".json");
                var playerbase = new Base(BaseNameGenerator.GenerateBaseName(), new WorldTile(new Random().Next(_worldMap.SizeX), new Random().Next(_worldMap.SizeY)));
                playerbase.AddBuilding(new House());
                playerbase.AddBuilding(new MarketPlace());
                player = new Player(0, p, 1, new List<Base> { playerbase }, new Currency("Money", 200));
                DataManager.SavePlayer(player, player.UserName + ".json");
            }
            //if (player != null)
            //{
            //    Console.WriteLine($"Loaded [Player]: {player.ID}");
            //    Console.WriteLine($"Loaded [Player]: {player.UserName}");
            //    Console.WriteLine($"Loaded [Player]: {player.Currency.Name}");
            //    Console.WriteLine($"Loaded [Player]: {player.Currency.Amount}");
            //    foreach (var b in player.PlayerBases)
            //    {
            //        Console.WriteLine($"Loaded [Base]: {b.BaseName}");
            //        Console.WriteLine($"Loaded [Base]: {b.WorldTile.X},{b.WorldTile.Y}");
            //        Console.WriteLine($"Loaded [Base]: {b.Owner}");
            //        Console.WriteLine($"Loaded [Base]: {b.Population}");
            //        foreach (var n in b.Buildings)
            //        {
            //            Console.WriteLine($"Loaded [Building]: {n.Name}");
            //            Console.WriteLine($"Loaded [Building]: {n.Level}");

            //        }
            //        foreach (var n in b.Troops)
            //        {
            //            Console.WriteLine($"Loaded [Troop]: {n.Name}");
            //            Console.WriteLine($"Loaded [Troop]: {n.Level}");

            //        }

            //    }

            //    foreach (var b in player.Knowledge.BaseKnowledge)
            //    {
            //        Console.WriteLine($"Loaded Knowledge [Base]: {b.BaseName}");
            //        Console.WriteLine($"Loaded Knowledge [Base]: {b.WorldTile.X},{b.WorldTile.Y}");
            //        Console.WriteLine($"Loaded Knowledge [Base]: {b.Owner}");
            //        Console.WriteLine($"Loaded Knowledge [Base]: {b.Population}");
            //        foreach (var n in b.Buildings)
            //        {
            //            Console.WriteLine($"Loaded Knowledge [Building]: {n.Name}");
            //            Console.WriteLine($"Loaded Knowledge [Building]: {n.Level}");

            //        }
            //        foreach (var n in b.Troops)
            //        {
            //            Console.WriteLine($"Loaded Knowledge [Troop]: {n.Name}");
            //            Console.WriteLine($"Loaded Knowledge [Troop]: {n.Level}");

            //        }

            //    }
            //}
            foreach (var playerBase in player.PlayerBases)
                _worldMap.AddBase(playerBase);
            _worldMap.AddPlayer(player);
        }

        _worldMap.DrawMapToFile("WorldMap.png");
        _tickTimer.Start(); // Start the timer
    }

    private void OnTick(object sender, ElapsedEventArgs e)
    {
        SimulateGameEvents();
        CommandHub.Tick();
        _ = Task.Run(async () => await BotManager.Instance.Client.SetGameAsync($"Game O War: Day {CommandHub.TicksPassed}"));
        SaveGame();
    }

    private void SimulateGameEvents()
    {
        EventHub.Publish(new EventOnTick());
    }

    public void GenerateCommands()
    {
        CommandParser.ClearCommand();
        var random = new Random();

        foreach (var p in _worldMap.WorldPlayers)
        {
            foreach (var worldBase in _worldMap.WorldBases)
            {
                if (worldBase.Owner.ToLower() != p.UserName.ToLower()) continue;
                foreach (var buildingType in BuildingFactory.BuildingFactories.Keys)
                {
                    CommandParser.RegisterCommand(p.UserName, $"{worldBase.BaseName} build {buildingType}", new CommandBuild(DiceRoller.RollD10()+5, worldBase, buildingType));
                }

                foreach (var worldBaseX in _worldMap.WorldBases)
                {
                    CommandParser.RegisterCommand(p.UserName, $"{worldBase.BaseName} distance {worldBaseX.BaseName}".ToLower(), new CommandDistance(0, worldBase, worldBaseX, _worldMap));
                    CommandParser.RegisterCommand(p.UserName, $"{worldBase.BaseName} attack {worldBaseX.BaseName}".ToLower(), new CommandAttack((int)_worldMap.CalculateDistance(worldBase.WorldTile, worldBaseX.WorldTile), worldBase, worldBaseX, "test"));
                    CommandParser.RegisterCommand(p.UserName, $"{worldBase.BaseName} scout {worldBaseX.BaseName}".ToLower(), new CommandScout((int)_worldMap.CalculateDistance(worldBase.WorldTile, worldBaseX.WorldTile), worldBase, _worldMap, worldBaseX.BaseName));
                }
            }

            CommandParser.RegisterCommand(p.UserName, $"commands", new CommandDisplayCommands(0, p.UserName));
            CommandParser.RegisterCommand(p.UserName, $"help", new CommandDisplayCommands(0, p.UserName));
        }
    }

    private void SaveGame()
    {
        foreach(var player in _worldMap.WorldPlayers)
        {
            DataManager.SavePlayer(player, player.UserName + ".json");
        }
    }

    private void LoadGame(string[] discordUsernames)
    {
        foreach(var name in discordUsernames)
        {
            var player = DataManager.LoadPlayer(name+".json");
            foreach(var playerBase in player.PlayerBases)
            {
                _worldMap.AddBase(playerBase);
            }
            _worldMap.AddPlayer(player);
        }
    }
}

public class PlayerRequest
{
    public Player Player;
    public string Message;

    public PlayerRequest(Player player, string message)
    {
        Player = player;
        Message = message;
    }
}

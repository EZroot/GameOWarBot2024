using System;
using System.Numerics;
using System.Timers;
using DiscordBot;
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

    public Game(int tickInterval = ConfigManager.TICK_INTERVAL) // Default to 1 second
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
            Console.WriteLine($"Creating player: {p}");
            Base playerbase = new Base(BaseNameGenerator.GenerateBaseName(), new GameOWar.World.WorldTile(new Random().Next(_worldMap.SizeX), new Random().Next(_worldMap.SizeY)));
            playerbase.AddBuilding(new House());
            Player player = new Player(0, p, 1, new List<Base> { playerbase }, new Currency("Money", 10)); ;
            _worldMap.AddBase(playerbase);
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
                if (worldBase.Owner.UserName != p.UserName) continue;
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

            CommandParser.RegisterCommand(p.UserName, $"knowledge", new CommandDisplayWorldInfo(0, p, _worldMap));
            //CommandParser.RegisterCommand(p.UserName, $"display player", new CommandDisplayPlayerInfo(0, _worldMap));
            //CommandParser.RegisterCommand(p.UserName, $"display base", new CommandDisplayBaseInfo(0, p));
            CommandParser.RegisterCommand(p.UserName, $"commands", new CommandDisplayCommands(0, p.UserName));
            CommandParser.RegisterCommand(p.UserName, $"help", new CommandDisplayCommands(0, p.UserName));
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

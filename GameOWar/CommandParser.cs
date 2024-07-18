using DiscordBot;
using System;
using System.Collections.Generic;

public static class CommandParser
{
    private static readonly Dictionary<string, Dictionary<string, ICommand>> _commandMaps = new Dictionary<string, Dictionary<string, ICommand>>();

    public static void RegisterCommand(string userName, string commandName, ICommand command, bool executeImmediately = false)
    {
        Log($"Registering: {userName} {commandName}");
        var commandMap = GetCommandMap(userName);
        if (commandMap == null)
        {
            commandMap = new Dictionary<string, ICommand>();
            _commandMaps[userName] = commandMap;
        }

        commandMap[commandName.ToLower()] = command;

        if (executeImmediately)
        {
            CommandHub.RegisterCommand(command);
        }
    }

    public static void ParseAndQueue(string userName, string input)
    {
        Log($"Parsing: {userName}:> {input}");
        var commandMap = GetCommandMap(userName);
        if (commandMap != null && commandMap.TryGetValue(input.ToLower(), out var command))
        {
            CommandHub.RegisterCommand(command);
        }
    }

    public static void ShowCommands()
    {
        _ = Task.Run(async () => await BotManager.Instance.SendMessage(GetAllCommands()));
    }

    public static string GetAllCommands()
    {
        var allCommands = "**[Commands]**\n";
        allCommands += "`!<yourbase> distance <targetbase>` - calculate distance to base\n";
        allCommands += "`!<yourbase> scout <targetbase>` - scouts a base\n";
        allCommands += "`!<yourbase> attack <targetbase>` - attacks a base\n";
        allCommands += "`!<yourbase> build <building>` - builds new buildings\n";
        allCommands += "`!map` - displays the world map\n";
        allCommands += "`/knowledge` - displays your world knowledge\n";
        allCommands += "`/status` - displays your base statuses\n";
        allCommands += "`/help` - displays this msg\n";
        allCommands += "**[Buildings]**\n";
        allCommands += "`Farm (Food)    Marketplace (Money)    House (Population)    Mine (Ore)   Logging (Trees)   Barracks (Troops)`\n";
        allCommands += "**[Game Info]**\nScout other bases, attack them, take them over.\n";
        allCommands += "**Markets** make money, the sell Ore and Trees\n";
        allCommands += "**Houses** make people, they need food\n";
        allCommands += "**Farms** make food, **Mine** makes ore, **Logging** makes trees\n";
        allCommands += "**Barracks** make troops\n";
        return allCommands;
    }
    public static void ClearCommand()
    {
        _commandMaps.Clear();
    }

    private static Dictionary<string, ICommand> GetCommandMap(string userName)
    {
        if (_commandMaps.TryGetValue(userName, out var commandMap))
        {
            return commandMap;
        }
        return null;
    }

    private static void Log(string message, LogLevel level = LogLevel.INFO)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        if (level == LogLevel.INFO)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        else
        {
            Console.ForegroundColor = originalColor;
        }

        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}");
        Console.ForegroundColor = originalColor;
    }

    private enum LogLevel
    {
        INFO,
        WARN,
        ERROR,
        DEBUG
    }
}

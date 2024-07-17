using System;
using System.Collections.Generic;

public static class CommandHub
{
    private static List<ICommand> commands = new List<ICommand>();
    public static long TicksPassed { get; private set; }

    public static void RegisterCommand(ICommand command)
    {
        commands.Add(command);
        command.StartMethod();
    }

    public static void Tick()
    {
        for (int i = commands.Count - 1; i >= 0; i--)
        {
            var command = commands[i];
            command.OnTick();

            if (command.IsCompleted)
            {
                command.OnEnd();
                commands.RemoveAt(i);
            }
        }
        TicksPassed++;
        LogConsole($"Day {TicksPassed}: Simulating...");
    }

    private static void LogConsole(string msg)
    {
        // Save the current console color
        ConsoleColor originalColor = Console.ForegroundColor;

        // Set the console color to blue
        Console.ForegroundColor = ConsoleColor.Blue;

        // Write the message to the console
        Console.WriteLine(msg);

        // Restore the original console color
        Console.ForegroundColor = originalColor;
    }
}

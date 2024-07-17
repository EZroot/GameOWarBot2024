using DiscordBot;
using System;

public static class Program
{
    public static async Task Main()
    {
        await BotManager.Instance.Initialize();
    }
}
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameOWar;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows.Input;

namespace DiscordBot
{
    internal class BotManager
    {
        // The static instance of the singleton
        private static BotManager _instance;

        // Lock object for thread safety
        private static readonly object _lock = new();
        private readonly ConcurrentQueue<string> _messageQueue;
        private readonly System.Timers.Timer _messageTimer;

        // Private constructor to prevent instantiation
        private BotManager()
        {
            _messageQueue = new ConcurrentQueue<string>();
            _messageTimer = new System.Timers.Timer(GameSettings.TICK_INTERVAL); 
            _messageTimer.Elapsed += SendQueuedMessage;
            _messageTimer.AutoReset = true;
            _messageTimer.Start();
        }

        // Public method to access the singleton instance
        public static BotManager Instance
        {
            get
            {
                // Double-check locking for thread safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BotManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private DiscordSocketClient _client;
        private Game _game;
        private ulong _lastMessageId = 0; // Track the ID of the last sent message

        public DiscordSocketClient Client => _client;
        public Game Game => _game;
        public ulong GuildID => 308708637679812608;
        public ulong ChannelID => 1263290434193719328;

        public async Task Initialize()
        {

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // Enable the necessary gateway intents 
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.MessageContent |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.GuildMembers
            });

            _client.Log += Log;

            var token = ConfigManager.LoadConfig();
            Console.WriteLine("API KEY " + token.ApiKey);
            await _client.LoginAsync(TokenType.Bot, "MTI2MjQ3NjU0NjQxNzg4OTM4MQ.GFTbuw.-8-ncmdsH00uQibeKyibLhgnKXmcHBKOmALHC8");//token.ApiKey.Trim());
            await _client.StartAsync();

            _client.Ready += Client_Ready;
            _client.MessageReceived += MessageReceivedAsync;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Client_Ready()
        {
            // Ensure you have the correct guild ID
            ulong guildId = GuildID;  // Replace with your actual guild ID.
            var guild = _client.GetGuild(guildId);

            var helpCommand = new SlashCommandBuilder()
            .WithName("help")
            .WithDescription("Displays commands");
            
            var optIn = new SlashCommandBuilder()
            .WithName("optin")
            .WithDescription("Gives access to GameOWar channel");

            var optOut = new SlashCommandBuilder()
            .WithName("optout")
            .WithDescription("Removes access from GameOWar channel");

            var status = new SlashCommandBuilder()
            .WithName("status")
            .WithDescription("Checks on the status of your bases");

            var knowledge = new SlashCommandBuilder()
            .WithName("knowledge")
            .WithDescription("Checks in on your players knowledge");

            var slashCommands = new SlashCommandBuilder[] { helpCommand, optIn, optOut, status, knowledge };  
            foreach(var slashCmd in slashCommands)
            {
                await guild.CreateApplicationCommandAsync(slashCmd.Build());
            }
            _client.SlashCommandExecuted += SlashCommandHandler;

            var guildUsers = await guild.GetUsersAsync().FlattenAsync();

            // Filter out bot users
            var humanUsers = guildUsers.Where(user => !user.IsBot);

            // Extract usernames from the filtered users
            var usernames = humanUsers.Select(user => user.Username).ToArray();

            _ = Task.Run(() =>
            {
                _game = new Game();
                _game.Start(usernames);
            });

            //await SendMessage("Game O' War has started!");
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // Ignore messages from bots
            if (message.Author.IsBot)
                return;

            // Cast the message to a SocketUserMessage, which allows easier access to properties
            var userMessage = message as SocketUserMessage;
            if (userMessage == null)
                return;

            // Here, you can access various properties of the message
            string content = userMessage.Content; // This gets the content of the message
            ulong userId = userMessage.Author.Id; // This gets the ID of the user who sent the message

            // Example of logging the message content
            Console.WriteLine($"Received message from {userMessage.Author.Username}: {content}");

            // Example of parsing commands using Discord.Commands
            int argPos = 0;
            if (userMessage.HasStringPrefix("!", ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, userMessage);
                
                _game.GenerateCommands();
                var command = content.Substring(1, content.Length-1);
                if (command.ToLower().Contains("map")) 
                    await SendFile("WorldMap.png");
                else
                    CommandParser.ParseAndQueue(userMessage.Author.Username.ToString(), command);
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "help":
                    //_ = Task.Run(async () => await command.RespondAsync(text: "Showing commands", ephemeral: true));
                    _game.GenerateCommands();
                    await command.RespondAsync(CommandParser.GetAllCommands(), ephemeral:true);
                    break;
                case "optin":
                    _ = Task.Run(async () =>
                    {
                        await command.RespondAsync(text: "Opting you in to GameOWar Channel", ephemeral: true);
                        var role = _client.GetGuild(GuildID).Roles.FirstOrDefault(x => x.Name.Equals("GameOWarPlayer", StringComparison.OrdinalIgnoreCase));
                        var user = command.User as IGuildUser;
                        // Assign the role to the user
                        await user.AddRoleAsync(role);
                    });
                    break;
                case "optout":
                    _ = Task.Run(async () =>
                    {
                        await command.RespondAsync(text: "Opting you out of GameOWar Channel", ephemeral: true);
                        var role = _client.GetGuild(GuildID).Roles.FirstOrDefault(x => x.Name.Equals("GameOWarPlayer", StringComparison.OrdinalIgnoreCase));
                        var user = command.User as IGuildUser;
                        // Assign the role to the user
                        await user.RemoveRoleAsync(role);
                    });
                    break;
                case "status":
                    _ = Task.Run(async () =>
                    {
                        await command.RespondAsync(text: "Checking on your bases...", ephemeral: true);
                        var user = command.User as IGuildUser;
                        var worldMap = _game.WorldMap;
                        var player = worldMap.WorldPlayers.Find(x => x.UserName == user.Username.ToLower());
                        var stats = $"{player.UserName} ${player.Currency.Amount}\n";
                        foreach(var playerBase in player.PlayerBases)
                        {
                            stats += $"[{playerBase.BaseName}]\n";
                            stats += $"Pop:{playerBase.Population}/{playerBase.PredictedPopulation} Troops:{playerBase.TotalTroopCount()}\n";
                            var houseCount = playerBase.Buildings.Count(x => x.Name == "House");
                            var barrackCount = playerBase.Buildings.Count(x => x.Name == "Barracks");
                            var marketCount = playerBase.Buildings.Count(x => x.Name == "MarketPlace");
                            var mineCount = playerBase.Buildings.Count(x => x.Name == "Mine");
                            var farmCount = playerBase.Buildings.Count(x => x.Name == "Farm");
                            stats += $"House:{houseCount} Barracks:{barrackCount} Markets:{marketCount} Farm:{farmCount} Mine:{mineCount}\n";
                            if (playerBase.IsPerformingAction) stats += $"* is scouting or attacking right now.\n";
                            if (playerBase.IsTroopsRecovering) stats += $"* is recovering troops.\n";
                        }
                        await command.ModifyOriginalResponseAsync((m) => m.Content = stats);
                    });
                    break;
                case "knowledge":
                    _ = Task.Run(async () =>
                    {
                        await command.RespondAsync(text: "Checking on your knowledge...", ephemeral: true);
                        var user = command.User as IGuildUser;
                        var worldMap = _game.WorldMap;
                        var player = worldMap.WorldPlayers.Find(x => x.UserName == user.Username.ToLower());
                        var stats = $"{player.UserName} ${player.Currency.Amount}\n";
                        foreach (var playerBase in player.Knowledge.BaseKnowledge)
                        {
                            stats += $"[{playerBase.BaseName}]\n";
                            stats += $"Pop:{playerBase.Population}/{playerBase.PredictedPopulation} Troops:{playerBase.TotalTroopCount()}\n";
                            var houseCount = playerBase.Buildings.Count(x => x.Name == "House");
                            var barrackCount = playerBase.Buildings.Count(x => x.Name == "Barracks");
                            var marketCount = playerBase.Buildings.Count(x => x.Name == "MarketPlace");
                            var mineCount = playerBase.Buildings.Count(x => x.Name == "Mine");
                            var farmCount = playerBase.Buildings.Count(x => x.Name == "Farm");
                            stats += $"House:{houseCount} Barracks:{barrackCount} Markets:{marketCount} Farm:{farmCount} Mine:{mineCount}\n";
                            if (playerBase.IsPerformingAction) stats += $"* is scouting or attacking right now.\n";
                            if (playerBase.IsTroopsRecovering) stats += $"* is recovering troops.\n";
                        }
                        await command.ModifyOriginalResponseAsync((m) => m.Content = stats);
                    });
                    break;
            }
        }

        public async Task SendMessage(string message)
        {
            var channel = _client.GetChannel(ChannelID) as IMessageChannel;
            if (channel != null)
            {
                await channel.SendMessageAsync(message);
            }
        }

        public void QueueMessage(string message)
        {
            _messageQueue.Enqueue(message);
        }

        private async void SendQueuedMessage(object sender, ElapsedEventArgs e)
        {
            var messagesToSend = new StringBuilder();
            foreach (var msg in _messageQueue)
            {
                if (_messageQueue.TryDequeue(out var message))
                {
                    messagesToSend.AppendLine(message);
                }
            }

            var channel = _client.GetChannel(ChannelID) as IMessageChannel;
            if (channel != null && messagesToSend.Length > 0)
            {
                // Check if the message length exceeds Discord's limit (2000 characters)
                if (messagesToSend.Length <= 1950)
                {
                    var msg = $"[**Day {CommandHub.TicksPassed}**]\n" + messagesToSend.ToString();
                    await LogConsole(msg);
                    await channel.SendMessageAsync(msg);
                }
                else
                {
                    // Split the message into chunks of 2000 characters
                    var chunks = SplitMessage(messagesToSend.ToString());

                    // Send each chunk separately
                    foreach (var chunk in chunks)
                    {
                        var msg = $"[**Day {CommandHub.TicksPassed}**]\n" + chunk;
                        await LogConsole(msg);
                        await channel.SendMessageAsync(msg);
                    }
                }
            }
        }

        private IEnumerable<string> SplitMessage(string message)
        {
            const int maxChunkLength = 1950;
            for (int i = 0; i < message.Length; i += maxChunkLength)
            {
                yield return message.Substring(i, Math.Min(maxChunkLength, message.Length - i));
            }
        }

        public async Task SendFile(string filePath)
        {
            var channel = _client.GetChannel(ChannelID) as IMessageChannel;
            if (channel != null)
            {
                await channel.SendFileAsync(filePath);
            }
        }
        private async Task ClearNonBotMessagesAsync(ulong channelId)
        {
            var channel = _client.GetChannel(channelId) as IMessageChannel;
            if (channel == null)
            {
                Console.WriteLine($"Channel with ID {channelId} not found or is not a valid message channel.");
                return;
            }

            var messages = await channel.GetMessagesAsync().FlattenAsync();

            foreach (var message in messages)
            {
                // Check if the message was not sent by a bot
                if (!message.Author.IsBot && !string.IsNullOrEmpty(message.Content))
                {
                    await message.DeleteAsync();
                    await Task.Delay(1000); // 1000 milliseconds delay between deletions (adjust as needed)
                }
            }
        }
        
        private static Task Log(LogMessage msg)
        {
            string timeStamp = DateTime.Now.ToString("HH:mm:ss"); // Format to include only hour, minute, and second

            Console.ForegroundColor = ConsoleColor.Cyan; // Purple color
            Console.Write($"{timeStamp} [BotManager] ");
            Console.ForegroundColor = ConsoleColor.White; // Green color
            Console.WriteLine(msg.ToString());
            Console.ResetColor(); // Reset to default color

            return Task.CompletedTask;
        }
        private static Task LogConsole(string msg)
        {
            string timeStamp = DateTime.Now.ToString("HH:mm:ss"); // Format to include only hour, minute, and second

            Console.ForegroundColor = ConsoleColor.DarkCyan; // Purple color
            Console.Write($"{timeStamp} [BotManager] ");
            Console.ForegroundColor = ConsoleColor.Yellow; // Green color
            Console.WriteLine(msg.ToString());
            Console.ResetColor(); // Reset to default color

            return Task.CompletedTask;
        }
    }
}

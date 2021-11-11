using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saucisse_bot.Bots.Commands;
using Saucisse_bot.Bots.Handlers.Message;
using Saucisse_bot.Bots.JsonParsers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots
{
    class Bot
    {
        private MessageHandler _messageHandler;
        private ConfigJson _configJson;

        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
       
        public Bot(IServiceProvider services)
        {
            _messageHandler = new MessageHandler();
            var json = string.Empty;

            #region config.json
            // Get the data from the config.json file
            // If there's a error because of this file, it is either because the file doesn't exist, or because it's is not in the right folder
            using (var fs = File.OpenRead("Sources/JsonDocs/config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            _configJson = JsonConvert.DeserializeObject<ConfigJson>(json); 
            #endregion

            #region Client management
            // Sets up the config for the bot
            // The minimum log level is lower in DEBUG mode
            var config = new DiscordConfiguration
            {                
                TokenType = TokenType.Bot,
                AutoReconnect = true,
#if DEBUG
                Token = _configJson.TokenDev,    // Use the dev bot Token when in debug
                MinimumLogLevel = LogLevel.Debug // Lower Log level due to DEBUG mode
#else
                Token = _configJson.TokenProd,   // Use the prod bot Token when running the bot
                MinimumLogLevel = LogLevel.Information
#endif
            };

            Client = new DiscordClient(config);

            // This is where the client is subscribed to Events
            Client.Ready += OnClientReady;
            Client.MessageCreated += OnMessageRecieved;

            // Interactivity is used when waiting for an user based action
            // The timespan can be changed
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            }); 
            #endregion

            #region Command management
            // Sets up the config for the commands
            // DmHelp : set to true if you want the help to be sent in DMs
            // EnableDms : set to true if you want to be able to use commands in DMs
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { _configJson.Prefix },
                EnableMentionPrefix = true,
                DmHelp = false,
                EnableDms = true,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            // Add every commands script here 
            // If the class is not set to public, it won't work
            // If you didn't add the class here, you won't be able to call the commands inside of it
            Commands.RegisterCommands<DebugCommands>();
            Commands.RegisterCommands<RandomCommands>();
            Commands.RegisterCommands<ItemCommands>();
            Commands.RegisterCommands<ProfileCommands>();
            Commands.RegisterCommands<DatabaseCommands>();
            #endregion

            Client.ConnectAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// This event is called when the bot is up and running. For now, it has no use.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// This event is called whenever a message is recieved. The latter is then processed if it isn't a message from a bot or a command
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnMessageRecieved(DiscordClient client, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot && e.Message.Content.Substring(0, 1) != _configJson.Prefix)
            {
                await _messageHandler.HandleMessage(e.Message.Content, e.Channel, e.Author.Id).ConfigureAwait(false);
            }
        }
    }
}

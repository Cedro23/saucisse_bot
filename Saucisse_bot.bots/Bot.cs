using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saucisse_bot.Bots.Commands;
using Saucisse_bot.Bots.Handlers.Message;
using Saucisse_bot.Bots.JsonParser;
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

            using (var fs = File.OpenRead("Sources/JsonDocs/config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            _configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            #region Client management
            var config = new DiscordConfiguration
            {
                Token = _configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            Client = new DiscordClient(config);


            Client.Ready += OnClientReady;
            Client.MessageCreated += OnMessageRecieved;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            }); 
            #endregion

            #region Command management
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { _configJson.Prefix },
                EnableMentionPrefix = true,
                DmHelp = true,
                EnableDms = true,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<DebugCommands>();
            Commands.RegisterCommands<RandomCommands>();
            Commands.RegisterCommands<ItemCommands>();
            Commands.RegisterCommands<ProfileCommands>();
            Commands.RegisterCommands<DatabaseCommands>(); 
            #endregion

            Client.ConnectAsync();
        }

        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task OnMessageRecieved(DiscordClient client, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot && e.Message.Content.Substring(0, 1) != _configJson.Prefix)
            {
                await _messageHandler.HandleMessage(e.Message.Content, e.Channel, e.Author.Id).ConfigureAwait(false);
            }
        }
    }
}

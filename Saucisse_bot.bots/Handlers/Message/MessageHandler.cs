using DSharpPlus.Entities;
using Newtonsoft.Json;
using Saucisse_bot.Bots.Handlers.Experience;
using Saucisse_bot.Bots.JsonParsers;
using Saucisse_bot.Core.Services.Profiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Handlers.Message
{
    /// <summary>
    /// This class is used to handle messages which are not commands
    /// </summary>
    public class MessageHandler
    {
        private const string PATTERN_QUOI = @"quoi *[\?][?,;. !/()'xDmdr]*";
        private const string PATTERN_QUI = @"qui *[\?][?,;. !/()'xDmdr]*";

        private ExperienceHandler _expHandler;
        private IGoldService _goldService;

        private UsersJson _usersJson;
        private Regex _rgQuoi;
        private Regex _rgQui;

        // Main constructor
        public MessageHandler(IExperienceService experienceService, IGoldService goldService)
        {
            _expHandler = new ExperienceHandler(experienceService);
            _goldService = goldService;

            var json = string.Empty;
            using (var fs = File.OpenRead("Sources/JsonDocs/users.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            _usersJson = JsonConvert.DeserializeObject<UsersJson>(json);

            _rgQuoi = new Regex(PATTERN_QUOI);
            _rgQui = new Regex(PATTERN_QUI);
        }

        public async Task HandleMessage(DiscordGuild guild, DiscordChannel channel, ulong userID, string message)
        {
            Random rnd = new Random();
            double nxtDouble;
            string filePath = String.Empty;

            if (message.EndsWith("quoi", StringComparison.OrdinalIgnoreCase) || _rgQuoi.IsMatch(message.ToLower()))
                await SendAnswerMsg(channel, "feur").ConfigureAwait(false);
            if (message.EndsWith("qui", StringComparison.OrdinalIgnoreCase) || _rgQui.IsMatch(message.ToLower()))
                await SendAnswerMsg(channel, "kette").ConfigureAwait(false);
            if (message.Equals("marco", StringComparison.OrdinalIgnoreCase))
                await SendAnswerMsg(channel, "Polo").ConfigureAwait(false);
            if (message.EndsWith("tg", StringComparison.OrdinalIgnoreCase))
                await SendAnswerMsg(channel, "v c:").ConfigureAwait(false);
            if (message.Contains("quand même", StringComparison.OrdinalIgnoreCase))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Sources\\Images\\kan_meme.jpg");
                await SendFile(channel, "", filePath);
            }

            if (userID == _usersJson.Roro)
            {
                nxtDouble = rnd.NextDouble();
                if (nxtDouble <= 0.05f)
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Sources\\Images\\oeuf.png");
                    await SendFile(channel, $"Nombre obtenu : {nxtDouble}", filePath);
                }
            }

            await GiveGoldAndExp(guild, channel, userID);
        }

        /// <summary>
        /// Sends a message as an answer to a certain type of message (i.e : the if statements where this function is called)
        /// </summary>
        /// <param name="channel">The channel where the message will be sent</param>
        /// <param name="answer">The content of the answer</param>
        /// <returns></returns>
        private async Task SendAnswerMsg(DiscordChannel channel, string answer)
        {
            await channel.SendMessageAsync(answer).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a file as an answer
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="content"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task SendFile(DiscordChannel channel, string content, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var msg = await new DiscordMessageBuilder()
                    .WithContent(content)
                    .WithFiles(new Dictionary<string, Stream>() { { filePath, fs } })
                    .SendAsync(channel);
            }
        }

        private async Task GiveGoldAndExp(DiscordGuild guild, DiscordChannel channel, ulong memberId)
        {
            await _expHandler.GrantExp(guild, channel, memberId, 1, 2).ConfigureAwait(false);
            await _goldService.GrantGolds(guild.Id, memberId, 1, 2).ConfigureAwait(false);
        }
    }
}

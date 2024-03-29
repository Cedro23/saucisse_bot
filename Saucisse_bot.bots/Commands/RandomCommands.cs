﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using Saucisse_bot.Bots.Handlers.Experience;
using Saucisse_bot.Bots.JsonParsers;
using Saucisse_bot.Core.Services.Profiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("rnd")]
    public class RandomCommands : BaseCommandModule
    {
        private UsersJson _usersJson;
        private ExperienceHandler _expHandler;
        private IGoldService _goldService;

        public RandomCommands(IExperienceService experienceService, IGoldService goldService)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("Sources/JsonDocs/users.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            _usersJson = JsonConvert.DeserializeObject<UsersJson>(json);

            _expHandler = new ExperienceHandler(experienceService);
            _goldService = goldService;
        }

        [Command("poticha")]
        [Description("Returns a random name for Basile's cat")]
        [Cooldown(1, 86400, CooldownBucketType.Guild)]
        public async Task GenerateRandomCatNameFromList(CommandContext ctx)
        {           
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Sources/Dictionnaries/CatList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)];

            await _expHandler.GrantExp(ctx.Guild, ctx.Channel, ctx.Member.Id, 100, 200).ConfigureAwait(false);
            await _goldService.GrantGolds(ctx.Guild.Id, ctx.Member.Id, 25, 50).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"Le nom du potichat aujourd'hui est : {name}").ConfigureAwait(false);
        }

        [Command("pau")]
        [Description("Returns a random name for Pauline and renames her")]
        [Cooldown(1, 86400, CooldownBucketType.Guild)]
        public async Task GenerateRandomPauNameFromList(CommandContext ctx)
        {
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Sources/Dictionnaries/PauList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)];

            try
            {
                var uPau = await ctx.Guild.GetMemberAsync(_usersJson.Pau).ConfigureAwait(false);
                if (uPau != null)
                {
                    await uPau.ModifyAsync(x =>
                    {
                        x.Nickname = name;
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            await _expHandler.GrantExp(ctx.Guild, ctx.Channel, ctx.Member.Id, 100, 200).ConfigureAwait(false);
            await _goldService.GrantGolds(ctx.Guild.Id, ctx.Member.Id, 25, 50).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"Pau, ton nouveau nom est : {name}").ConfigureAwait(false);            
        }
    }
}

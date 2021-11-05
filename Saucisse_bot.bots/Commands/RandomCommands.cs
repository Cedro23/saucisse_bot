using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using Saucisse_bot.Bots.JsonParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("rnd")]
    [RequireOwner]
    public class RandomCommands : BaseCommandModule
    {
        private UsersJson _usersJson;

        public RandomCommands()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("Sources/JsonDocs/users.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            _usersJson = JsonConvert.DeserializeObject<UsersJson>(json);
        }

        [Command("poticha")]
        [Description("Returns a random name for Basile's cat")]
        public async Task GenerateRandomCatNameFromList(CommandContext ctx)
        {           
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Sources/Dictionnaries/CatList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)];

            await ctx.Channel.SendMessageAsync($"Le nom du potichat aujourd'hui est : {name}");
        }

        [Command("pau")]
        [Description("Returns a random name for Pauline and renames her")]
        public async Task GenerateRandomPauNameFromList(CommandContext ctx)
        {
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Sources/Dictionnaries/PauList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)];

            var uPau = await ctx.Guild.GetMemberAsync(_usersJson.Pau);

            await ctx.Channel.SendMessageAsync($"Pau, ton nouveau nom est : {name}");
            
            if (uPau != null)
            {
                await uPau.ModifyAsync(x =>
                {
                    x.Nickname = name;
                }); ;
            }
        }
    }
}

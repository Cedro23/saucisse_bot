using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("rnd")]
    [RequireOwner]
    public class RandomCommands : BaseCommandModule
    {
        [Command("poticha")]
        [Description("Returns a random name for Basile's cat")]
        public async Task GenerateRandomCatNameFromList(CommandContext ctx)
        {           
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Dictionnaries/CatList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)].ToString();

            await ctx.Channel.SendMessageAsync($"Le nom du potichat aujourd'hui est : {name}");
        }

        [Command("pau")]
        [Description("Returns a random name for Pauline and renames her")]
        public async Task GenerateRandomPauNameFromList(CommandContext ctx)
        {
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Dictionnaries/PauList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)].ToString();

            var uBot = ctx.Guild.CurrentMember;
            ctx.Guild.Members.TryGetValue(691309813849653280, out var uPau);

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

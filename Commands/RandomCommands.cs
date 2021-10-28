using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saucisse_bot.Commands
{
    [Group("rnd")]
    class RandomCommands : BaseCommandModule
    {
        [Command("poticha")]
        [Description("Returns a random name for Basile's cat")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        public async Task GenerateRandomCatNameFromList(CommandContext _ctx)
        {           
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Dictionnaries/CatList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)].ToString();

            await _ctx.Channel.SendMessageAsync($"Le nom du potichat aujourd'hui est : {name}");
        }

        [Command("pau")]
        [Description("Returns a random name for Pauline and renames her")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        public async Task GenerateRandomPauNameFromList(CommandContext _ctx)
        {
            var namesFile = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Dictionnaries/PauList.txt"));
            List<string> namesList = new List<string>(namesFile);
            Random rnd = new Random();

            string name = namesList[rnd.Next(0, namesList.Count - 1)].ToString();

            var uBot = _ctx.Guild.CurrentMember;
            _ctx.Guild.Members.TryGetValue(691309813849653280, out var uPau);

            await _ctx.Channel.SendMessageAsync($"Pau, ton nouveau nom est : {name}");
            
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

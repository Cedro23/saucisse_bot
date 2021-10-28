using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Items;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    class RPGCommands : BaseCommandModule
    {
        private readonly RPGContext _context;

        public RPGCommands(RPGContext context)
        {
            _context = context;
        }

        [Command("additem")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        public async Task AddItem(CommandContext ctx, string name)
        {
            await _context.Items.AddAsync(new Item { Name = name, Description = "Test description"}).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        [Command("item")]
        public async Task GetItem(CommandContext ctx, string name)
        {
            //var item = await _context.Items.FirstOrDefaultAsync(
            //    x => x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);
            var items = await _context.Items.ToListAsync().ConfigureAwait(false);

            //await ctx.Channel.SendMessageAsync(item.Description).ConfigureAwait(false);
        }
    }
}

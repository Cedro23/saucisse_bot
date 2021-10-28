using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Saucisse_bot.Core.Services.Database;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [RequireOwner]
    [RequireDirectMessage]
    public class DatabaseCommands : BaseCommandModule
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseCommands(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [Command("purgeitems")]
        public async Task PurgeItems(CommandContext ctx)
        {
            var result = await _databaseService.PurgeTables("Items").ConfigureAwait(false);

            if (!result) await ctx.Channel.SendMessageAsync("There was a problem purging tables Items and ProfileItems!").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync("Tables Items and ProfileItems purged succesfully!").ConfigureAwait(false);
        }
    }
}

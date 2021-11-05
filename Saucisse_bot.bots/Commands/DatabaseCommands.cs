using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using Saucisse_bot.Bots.Handlers.Dialogue;
using Saucisse_bot.Bots.Handlers.Dialogue.Steps;
using Saucisse_bot.Core.Services.Database;
using Saucisse_bot.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    /// <summary>
    /// This class contains commands that have a direct impact on the database
    /// </summary>
    [RequireOwner]
    public class DatabaseCommands : BaseCommandModule
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseCommands(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Purges the entries of a table. For now it only works for the item table.
        /// This class is meant to be modified to be able to purge a table given as a parameter with a dialogue.
        /// Also, it will show an embed to the user with all the existing tables
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("purgeitems")]
        public async Task PurgeItems(CommandContext ctx)
        {
            var result = await _databaseService.PurgeTables("Items").ConfigureAwait(false);

            if (!result) await ctx.Channel.SendMessageAsync("There was a problem purging tables Items and ProfileItems!").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync("Tables Items and ProfileItems purged succesfully!").ConfigureAwait(false);
        }

        /// <summary>
        /// Returns general informations about a table.
        /// The table name is asked to the user after he's shown an embed with all the existing tables.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("tableinfo")]
        public async Task TableInfo(CommandContext ctx)
        {
            Dictionary<string, int> tableList = await _databaseService.GetTablesCount().ConfigureAwait(false);


            var embed = new DiscordEmbedBuilder()
            {
                Title = $"Here are the number of entities for each table:"
            };

            foreach (var table in tableList)
            {
                embed.AddField(table.Key, table.Value.ToString());
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}
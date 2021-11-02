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
    [RequireDirectMessage]
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
            int value = 0;
            //List<IEntityType> tables = await _databaseService.GetTablesName().ConfigureAwait(false);
            Dictionary<string, string> embedFields = new Dictionary<string, string>();
            string content = $"Please enter the number associated with the table!\n";

            int n = 0;
            //foreach (var t in tables)
            //{
            //    content += $"{n}-{t.ClrType}\n";
            //    n++;
            //}
            
            var inputStep = new IntStep(content, null); // maxValue: tables.Count - 1

            inputStep.OnValidResult += async (result) =>
            {
                embedFields = await _databaseService.GetTableInfo();
            };

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                inputStep);

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"Here are the infos about the table "
            };

            foreach (var field in embedFields)
            {
                embed.AddField(field.Key, field.Value);
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}
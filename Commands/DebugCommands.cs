using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Saucisse_bot.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Commands
{
    public class DebugCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Ping(CommandContext _ctx)
        {
            await _ctx.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
        }

        [Command("getlatency")]
        [Description("Returns client latency")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        public async Task GetLatency(CommandContext _ctx)
        {
            await _ctx.Channel
                .SendMessageAsync($"Current ping is {_ctx.Client.Ping}ms")
                .ConfigureAwait(false);
        }

        [Command("clear")]
        [Description("Deletes X messages")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageMessages)]
        public async Task Clear(CommandContext _ctx,
            [Description("Number of messages to delete")] int _n)
        {
            var messages = await _ctx.Channel.GetMessagesAsync(_n + 1);
            await _ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("poll")]
        [Description("Creates a poll")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        public async Task Poll(CommandContext _ctx, TimeSpan _duration, params DiscordEmoji[] _emojisOptions)
        {
            var interactivity = _ctx.Client.GetInteractivity();
            var options = _emojisOptions.Select(x => x.ToString());

            // Creates the embed
            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            // Sends the embed in the channel
            var pollMessage = await _ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            // Adds the emoji reactions to the embed
            foreach (var option in _emojisOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, _duration).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var results = distinctResult.Select(x => $"{x.Emoji} : {x.Total}");

            await _ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }


        //[Command("respondmsg")]
        //public async Task Respondmsg(CommandContext ctx)
        //{
        //    var interactivity = ctx.Client.GetInteractivity();

        //    var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

        //    await ctx.Channel.SendMessageAsync(message.Result.Content).ConfigureAwait(false);
        //}
    }
}

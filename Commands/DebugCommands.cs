using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System.Threading.Tasks;

namespace Saucisse_bot.Commands
{
    public class DebugCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
        }

        [Command("getlatency")]
        [Description("Returns client latency")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        public async Task GetLatency(CommandContext ctx)
        {
            await ctx.Channel
                .SendMessageAsync($"Current ping is {ctx.Client.Ping.ToString()}ms")
                .ConfigureAwait(false);
        }

        [Command("clear")]
        [Description("Deletes X messages")]
        [RequireRoles(RoleCheckMode.SpecifiedOnly, "Owner")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageMessages)]
        public async Task Clear(CommandContext ctx, 
            [Description("Number of messages to delete")] int n)
        {
            var messages = await ctx.Channel.GetMessagesAsync(n + 1);
            await ctx.Channel.DeleteMessagesAsync(messages);
        }


        [Command("respondmsg")]
        public async Task Respondmsg(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        [Command("respondreaction")]
        public async Task Respondreaction(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Emoji);
        }
    }
}

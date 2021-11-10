using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Saucisse_bot.Bots.Attributes;
using Saucisse_bot.Bots.Handlers.Dialogue;
using Saucisse_bot.Bots.Handlers.Dialogue.Steps;
using Saucisse_bot.Bots.Handlers.ExperienceHandler;
using Saucisse_bot.Core.Services.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    /// <summary>
    /// Debug commands are simple commands used to see if the bot works as intended.
    /// This class is also used to try out stuff in commands
    /// </summary>
    public class DebugCommands : BaseCommandModule
    {
        private ExperienceHandler _expHandler;

        public DebugCommands(IExperienceService experienceService)
        {
            _expHandler = new ExperienceHandler(experienceService);  
        }

        [Command("ping")]
        [Description("Returns pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
        }

        [Command("getlatency")]
        [Description("Returns the client latency")]
        [RequireOwner]
        public async Task GetLatency(CommandContext ctx)
        {
            await ctx.Channel
                .SendMessageAsync($"Current ping is {ctx.Client.Ping}ms")
                .ConfigureAwait(false);
        }

        // Move in a new command script made for owners/admin only
        [Command("clear")]
        [Description("Deletes X messages")]
        [RequireOwner]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageMessages)]
        public async Task Clear(CommandContext ctx,
            [Description("Number of messages to delete")] int n)
        {
            var messages = await ctx.Channel.GetMessagesAsync(n + 1);
            await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("poll")]
        [Description("Creates a poll")]
        [RequireOwner]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojisOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojisOptions.Select(x => x.ToString());

            // Creates the embed
            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            // Sends the embed in the channel
            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            // Adds the emoji reactions to the embed
            foreach (var option in emojisOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var results = distinctResult.Select(x => $"{x.Emoji} : {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }

        // Shows how the interactivity module works
        //[Command("respondmsg")]
        //public async Task Respondmsg(CommandContext ctx)
        //{
        //    var interactivity = ctx.Client.GetInteractivity();

        //    var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

        //    await ctx.Channel.SendMessageAsync(message.Result.Content).ConfigureAwait(false);
        //}


        // This is an exemple for a dialogue with the user
        [Command("dialogue")]
        [RequireOwner]
        public async Task Dialogue(CommandContext ctx)
        {
            var inputStep = new StringStep("Enter something interesting!", null);
            var funnyStep = new IntStep("Haha, funny!", null, maxValue: 100);

            string input = string.Empty;
            int value = 0;

            inputStep.OnValidResult += (result) =>
            {
                input = result;

                if (result == "something funny")
                {
                    inputStep.SetNextStep(funnyStep);
                }

            };

            funnyStep.OnValidResult += (result) => value = result;


            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                inputStep);

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(value.ToString()).ConfigureAwait(false);
        }

        [Command("emojidialogue")]
        [RequireOwner]
        public async Task EmojiDialogue(CommandContext ctx)
        {
            var yesStep = new StringStep("You chose yes", null);
            var noStep = new StringStep("You chose no", null);
            var emojiStep = new ReactionStep("Yes or No ?", new Dictionary<DiscordEmoji, ReactionStepData>
            {
                {DiscordEmoji.FromName(ctx.Client, ":thumbsup:"), new ReactionStepData{ Content = "This means yes", NextStep = yesStep} },
                {DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"), new ReactionStepData{ Content = "This means no", NextStep = noStep} }
            });

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                emojiStep);

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }
        }

        [Command("testexp")]
        public async Task TestExp(CommandContext ctx)
        {
            await _expHandler.GrantExp(ctx, 10, 100);
        }
    }
}

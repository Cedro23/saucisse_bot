using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL.Models.Profiles;
using System;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("gamble")]
    [Aliases("gmb")]
    [Description("These commands revolve around making money in a fun way")]
    public class GambleCommands : BaseCommandModule
    {
        private IProfileService _profileService;

        public GambleCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("lowroll")]
        [Aliases("lroll")]
        [Description("")]
        public async Task LowRoll(CommandContext ctx, 
            [Description("The bet you want to wager, limited to 999999")]int betAmount,
            [Description("The difficulty, limited between 1 and 10")] float difficulty)
        {
            Profile profile = await _profileService.GetProfileAsync(ctx.Guild.Id, ctx.Member.Id).ConfigureAwait(false);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            difficulty = Math.Clamp(difficulty, 1, 10);

            if (profile != null)
            {
                if (betAmount <= profile.Gold && betAmount > 0)
                {
                    Random random = new Random();
                    double rndDouble = random.NextDouble();
                    bool hasWon = rndDouble <= 1 / difficulty;

                    embed.Title = "Lowroll !";
                    embed.AddField("Bet", betAmount.ToString());
                    embed.AddField("Difficulty", $"1 in {difficulty} chances");

                    if (hasWon)
                    {
                        int gains = (int)(betAmount * difficulty);
                        embed.Description = $"Congratulations {ctx.Member.Mention}, you won {gains}g.";
                        embed.Color = DiscordColor.Green;
                        await _profileService.ManageResourcesAsync(ctx.Guild.Id, ctx.Member.Id, gains - betAmount, true, ResourceType.Gold);
                    }
                    else
                    {
                        embed.Description = $"F {ctx.Member.Mention}... You lost {betAmount}g.";
                        embed.Color = DiscordColor.Red;
                        await _profileService.ManageResourcesAsync(ctx.Guild.Id, ctx.Member.Id, betAmount, false, ResourceType.Gold);
                    } 
                }
                else
                {
                    if (betAmount > 0)
                    {
                        embed.Title = "Not enough golds";
                        embed.Description = $"Sorry {ctx.Member.Mention}, you don't have enough golds.";
                        embed.Color = DiscordColor.Orange; 
                    }
                    else
                    {
                        embed.Title = "Wrong value";
                        embed.Description = $"Sorry {ctx.Member.Mention}, you cannot bet {betAmount} golds.";
                        embed.Color = DiscordColor.Orange;
                    }
                }
            }
            else
            {
                embed.Title = "404 PROFILE NOT FOUND";
                embed.Description = "If you want to create your profile, please use the command \"!profile create\"";
                embed.Color = DiscordColor.Red;
            }

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false); 
        }

    }
}

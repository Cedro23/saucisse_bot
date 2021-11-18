using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL.Models.Profiles;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    [Group("sudo")]
    [Description("These commands revolve around everythin")]
    [RequireOwner]
    public class SudoCommands : BaseCommandModule
    {
        private IProfileService _profileService;

        public SudoCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #region Profiles
        [Command("givegolds")]
        public async Task GiveGolds(CommandContext ctx, DiscordMember member, int amount)
        {
            Result res = await _profileService.AddGoldsAsync(ctx.Guild.Id, member.Id, amount);
            DiscordEmbedBuilder profileEmbed;

            if (!res.IsOk)
            {
                profileEmbed = new DiscordEmbedBuilder
                {
                    Title = "404 PROFILE NOT FOUND",
                    Description = res.ErrMsg,
                    Color = DiscordColor.Red
                };
            }
            else
            {
                profileEmbed = new DiscordEmbedBuilder
                {
                    Title = "Succes!",
                    Description = $"{amount} golds were given to {member.Mention}!",
                    Color = DiscordColor.Green
                };
            }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }


        #endregion
    }
}

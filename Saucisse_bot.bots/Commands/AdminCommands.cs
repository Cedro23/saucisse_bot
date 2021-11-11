using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    /// <summary>
    /// This class contains commands that are usable by the owner only
    /// </summary>
    [RequireOwner]
    public class AdminCommands : BaseCommandModule
    {
        private IProfileService _profileService;

        public AdminCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #region Manage profil
        [Command("addgolds")]
        [Description("Adds golds to a profile")]
        public async Task AddGolds(CommandContext ctx, DiscordMember member, int amount)
        {
            var result = await _profileService.AddGoldsAsync(member.Id, amount, ctx.Guild.Id).ConfigureAwait(false);

            if (!result) await ctx.Channel.SendMessageAsync($"There was a problem adding golds to {member.DisplayName}!").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"Succesfully added {amount} golds to {member.DisplayName}!").ConfigureAwait(false);
        }

        [Command("removegolds")]
        [Description("Removes golds from a profile")]
        public async Task RemoveGolds(CommandContext ctx, DiscordMember member, int amount)
        {

        }

        
        #endregion
    }
}

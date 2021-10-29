using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL.Models.Profiles;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Commands
{
    public class ProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;

        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #region Display profile
        [Command("profile")]
        //[Description("Returns the server based profile of the user who used the command")]
        public async Task Profile(CommandContext ctx)
        {
            await GetProfileToDisplayAsync(ctx, ctx.Member.Id);
        }

        [Command("profile")]
        //[Description("Returns the server based profile of the mentionned user")]
        public async Task Profile(CommandContext ctx, DiscordMember member)
        {
            await GetProfileToDisplayAsync(ctx, member.Id);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id).ConfigureAwait(false);

            DiscordMember member = ctx.Guild.Members[profile.DiscordId];

            DiscordEmbedBuilder.EmbedThumbnail thumbnail = new DiscordEmbedBuilder.EmbedThumbnail();
            thumbnail.Url = member.AvatarUrl;

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile",
                Thumbnail = thumbnail
            };

            profileEmbed.AddField("Level", profile.Level.ToString());
            profileEmbed.AddField("Xp", profile.Xp.ToString());
            profileEmbed.AddField("Gold", profile.Gold.ToString());
            if (profile.Items.Count > 0)
            {
                profileEmbed.AddField("Items", string.Join(", ", profile.Items.Select(x => x.Item.Name)));
            }

            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }
        #endregion


        #region Manage profil (owner only)
        [Command("addgolds")]
        [RequireOwner]
        public async Task AddGolds(CommandContext ctx, DiscordMember member, int amount)
        {
            var result = await _profileService.AddGolds(member.Id, amount, ctx.Guild.Id).ConfigureAwait(false);
            
            if (!result) await ctx.Channel.SendMessageAsync($"There was a problem adding golds to {member.DisplayName}!").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync($"Succesfully added {amount} golds to {member.DisplayName}!").ConfigureAwait(false);
        }
        #endregion
    }
}

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using System;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Handlers.ExperienceHandler
{
    /// <summary>
    /// This class is used to handle giving experience points to profiles and displaying whether they levelled up or not
    /// </summary>
    public class ExperienceHandler
    {
        private IExperienceService _experienceService;

        public ExperienceHandler(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        public async Task GrantExp(CommandContext ctx, int minExp, int maxExp)
        {
            Random random = new Random();
            int rndExp = random.Next(minExp, maxExp);

            var grantXpViewModel = await _experienceService.GrantXpAsync(ctx.Member.Id, ctx.Guild.Id, rndExp).ConfigureAwait(false);

            // Retrieving the member's thumbnail
            DiscordMember member = await ctx.Guild.GetMemberAsync(ctx.Member.Id);
            DiscordEmbedBuilder.EmbedThumbnail thumbnail = new DiscordEmbedBuilder.EmbedThumbnail();
            thumbnail.Url = member.AvatarUrl;

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Level up !",
                Description = $"Congratulations {ctx.Member.Mention}, you levelled up to level {grantXpViewModel.Profile.Level}",
                Thumbnail = thumbnail,
                Timestamp = DateTime.UtcNow,    
                Color = DiscordColor.Blurple
            };
            if (grantXpViewModel.LevelledUp)
                await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);

#if DEBUG
            await ctx.Channel.SendMessageAsync($"You got {rndExp} points of experience").ConfigureAwait(false); 
#endif

            
        }
    }
}

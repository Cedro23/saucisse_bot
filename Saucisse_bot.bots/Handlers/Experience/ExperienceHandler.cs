using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Saucisse_bot.Core.Services.Profiles;
using System;
using System.Threading.Tasks;

namespace Saucisse_bot.Bots.Handlers.Experience
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

        public async Task GrantExp(DiscordGuild guild, DiscordChannel channel, ulong memberId, int minExp, int maxExp)
        {
            Random rand = new Random();
            int rndExp = rand.Next(minExp, maxExp + 1);

            var grantXpViewModel = await _experienceService.GrantXpAsync(guild.Id, memberId, rndExp).ConfigureAwait(false);

            // Retrieving the member's thumbnail
            DiscordMember member = await guild.GetMemberAsync(memberId).ConfigureAwait(false);
            DiscordEmbedBuilder.EmbedThumbnail thumbnail = new DiscordEmbedBuilder.EmbedThumbnail();
            thumbnail.Url = member.AvatarUrl;

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Level up !",
                Description = $"Congratulations {member.Mention}, you levelled up to level {grantXpViewModel.Profile.Level}",
                Thumbnail = thumbnail,
                Timestamp = DateTime.UtcNow,    
                Color = DiscordColor.Blurple
            };
            if (grantXpViewModel.LevelledUp)
                await channel.SendMessageAsync(embed: embed).ConfigureAwait(false);

#if DEBUG
            await channel.SendMessageAsync($"You got {rndExp} points of experience").ConfigureAwait(false); 
#endif 
        }
    }
}

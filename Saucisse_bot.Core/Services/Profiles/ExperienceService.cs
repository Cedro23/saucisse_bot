using Microsoft.EntityFrameworkCore;
using Saucisse_bot.Core.ViewModels;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Profiles;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Profiles
{
    public interface IExperienceService
    {
        Task<GrantXpViewModel> GrantXpAsync(ulong guildId, ulong memberId, int xpAmount);
    }

    public class ExperienceService : IExperienceService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public ExperienceService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task<GrantXpViewModel> GrantXpAsync(ulong guildId, ulong memberId, int xpAmount)
        {
            using var context = new RPGContext(_options);

            Profile profile = await _profileService.GetProfileAsync(guildId, memberId).ConfigureAwait(false);

            int levelBefore = profile.Level;

            profile.Xp += xpAmount;

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            int levelAfter = profile.Level;

            return new GrantXpViewModel
            {
                Profile = profile,
                LevelledUp = levelAfter > levelBefore
            };
        }
    }
}
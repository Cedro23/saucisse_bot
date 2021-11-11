using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Profiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Profiles
{
    public interface IProfileService
    {
        Task<Profile> GetProfileAsync(ulong memberId, ulong guildId);
        Task<bool> CreateProfileAsync(ulong memberId, ulong guildId);
        Task<bool> ResetProfileAsync(ulong memberId, ulong guildId);
        Task<bool> DeleteProfileAsync(ulong memberId, ulong guildId);
        Task<bool> AddGoldsAsync(ulong memberId, int amount, ulong guildId);
        Task<List<Profile>> GetProfileListAsync(ulong guildId);
    }

    public class ProfileService : IProfileService
    {
        private readonly DbContextOptions<RPGContext> _options;

        public ProfileService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }

        public async Task<Profile> GetProfileAsync(ulong memberId, ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .Include(x => x.Items)
                .Include(x => x.Items).ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.DiscordId == memberId).ConfigureAwait(false);

            return profile;
        }

        public async Task<bool> CreateProfileAsync(ulong memberId, ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profile = new Profile
            {
                DiscordId = memberId,
                GuildId = guildId,
                Gold = 100
            };

            context.Add(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
            return profile != null;
        }

        public async Task<List<Profile>> GetProfileListAsync(ulong guildId)
        {
            using var context = new RPGContext(_options);
            return await context.Profiles.Where(x => x.GuildId == guildId).ToListAsync<Profile>().ConfigureAwait(false);
        }

        #region Manage profile

        public async Task<bool> AddGoldsAsync(ulong memberId, int amount, ulong guildId)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == memberId).ConfigureAwait(false);

            if (profile != null) 
            {
                profile.Gold += amount;
            }

            context.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public Task<bool> ResetProfileAsync(ulong memberId, ulong guildId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteProfileAsync(ulong memberId, ulong guildId)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

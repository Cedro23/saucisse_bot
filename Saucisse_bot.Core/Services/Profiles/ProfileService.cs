using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Profiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Profiles
{
    public struct Result
    {
        public bool IsOk;
        public string ErrMsg;
    }

    public interface IProfileService
    {
        Task<Profile> GetProfileAsync(ulong guildId, ulong memberId);
        Task<bool> CreateProfileAsync(ulong guildId, ulong memberId);
        Task<Result> ResetProfileAsync(ulong guildId, ulong memberId);
        Task<Result> ResetAllProfilesAsync(ulong guildId);
        Task<Result> DeleteProfileAsync(ulong guildId, ulong memberId);
        Task<Result> DeleteAllProfilesAsync(ulong guildId);
        Task<Result> AddGoldsAsync(ulong guildId, ulong memberId, int amount);
        //Task<Result> RemoveGoldsAsync(ulong guildId, ulong memberId, int amount);
        Task<List<Profile>> GetProfileListAsync(ulong guildId);
    }

    public class ProfileService : IProfileService
    {
        private readonly DbContextOptions<RPGContext> _options;

        public ProfileService(DbContextOptions<RPGContext> options)
        {
            _options = options;
        }

        public async Task<Profile> GetProfileAsync(ulong guildId, ulong memberId)
        {
            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .Include(x => x.Items)
                .Include(x => x.Items).ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.DiscordId == memberId).ConfigureAwait(false);

            return profile;
        }

        public async Task<bool> CreateProfileAsync(ulong guildId, ulong memberId)
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
        public async Task<Result> ResetProfileAsync(ulong guildId, ulong memberId)
        {
            Result response = new Result();
            response.IsOk = false;
            response.ErrMsg = string.Empty;

            using var context = new RPGContext(_options);
            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == memberId).ConfigureAwait(false);

            if (profile != null)
            {
                profile.Gold = 100;
                profile.Xp = 0;
            }
            else
            {
                response.ErrMsg = "Could not find the account";
                return response;
            }

            try
            {
                context.Update(profile);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                response.ErrMsg = e.Message;
                return response;
            }
            response.IsOk = true;
            return response;
        }

        public async Task<Result> ResetAllProfilesAsync(ulong guildId)
        {
            Result response = new Result();
            response.IsOk = false;
            response.ErrMsg = string.Empty;

            using var context = new RPGContext(_options);
            var profiles = await context.Profiles
                                        .Where(x => x.GuildId == guildId)
                                        .ToListAsync<Profile>().ConfigureAwait(false);

            if (profiles.Count > 0)
            {
                for (int i = 0; i < profiles.Count; i++)
                {
                    profiles[i].Gold = 100;
                    profiles[i].Xp = 0;
                } 
            }
            else
            {
                response.ErrMsg = "There are no users on this server";
                return response;
            }
            
            try
            {
                context.UpdateRange(profiles);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                response.ErrMsg = e.Message;
                return response;
            }
            response.IsOk = true;
            return response;
        }

        public async Task<Result> DeleteProfileAsync(ulong guildId, ulong memberId)
        {
            Result response = new Result();
            response.IsOk = false;
            response.ErrMsg = string.Empty;

            using var context = new RPGContext(_options);
            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == memberId).ConfigureAwait(false);

            if (profile != null)
            {
                try
                {
                    context.Remove(profile);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (System.Exception e)
                {
                    response.ErrMsg = e.Message;
                    return response;
                }
            }
            else
            {
                response.ErrMsg = "Could not find the account";
                return response;
            }
            response.IsOk = true;
            return response;
        }

        public async Task<Result> DeleteAllProfilesAsync(ulong guildId)
        {
            Result response = new Result();
            response.IsOk = false;
            response.ErrMsg = string.Empty;

            using var context = new RPGContext(_options);
            var profiles = await context.Profiles
                                        .Where(x => x.GuildId == guildId)
                                        .ToListAsync<Profile>().ConfigureAwait(false);

            if (profiles.Count > 0)
            {
                try
                {
                    context.RemoveRange(profiles);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (System.Exception e)
                {
                    response.ErrMsg = e.Message;
                    return response;
                }
            }
            else
            {
                response.ErrMsg = "There are no users on this server";
                return response;
            }
            
            response.IsOk = true;
            return response;
        }

        /// <summary>
        /// Adds certain amount of golds to a profile.
        /// Returns a boolean based on the success of the operation.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="amount"></param>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public async Task<Result> AddGoldsAsync(ulong guildId, ulong memberId, int amount)
        {
            Result response = new Result();
            response.IsOk = false;
            response.ErrMsg = string.Empty;

            using var context = new RPGContext(_options);

            var profile = await context.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == memberId).ConfigureAwait(false);

            if (profile != null)
                profile.Gold += amount;
            else
            {
                response.ErrMsg = "Could not find the account";
                return response;
            }

            try
            {
                context.Update(profile);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                response.ErrMsg = e.Message;
                return response;
            }
            response.IsOk = true;
            return response;
        }
        #endregion
    }
}

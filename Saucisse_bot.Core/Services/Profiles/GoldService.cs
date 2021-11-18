using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Profiles;
using System;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Profiles
{
    public interface IGoldService
    {
        Task GrantGolds(ulong guildId, ulong memberId, int minAmount, int maxAmount);
    }    

    public class GoldService : IGoldService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public GoldService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task GrantGolds(ulong guildId, ulong memberId, int minAmount, int maxAmount)
        {
            Profile profile = await _profileService.GetProfileAsync(guildId, memberId).ConfigureAwait(false);
            Random rand = new Random();
            int amount = rand.Next(minAmount, maxAmount + 1);

            await _profileService.ManageGoldsAsync(guildId, memberId, amount, true);

#if DEBUG
            //await channel.SendMessageAsync($"You got {rndExp} points of experience").ConfigureAwait(false);
#endif
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Items;
using Saucisse_bot.DAL.Models.Profiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Items
{
    public struct ResultItem
    {
        public Item Item;
        public bool IsOk;
        public string ErrMsg;
    }

    public interface IItemService
    {
        Task CreateNewItemAsync(Item item);
        Task<Item> GetItemByNameAsync(ulong guildId, string itemName);
        Task<List<Item>> GetItemList(ulong guildId);
        Task<ResultItem> PurchaseItemAsync(ulong discordId, ulong guildId, string itemName);
    }

    public class ItemService : IItemService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;

        public ItemService(DbContextOptions<RPGContext> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task CreateNewItemAsync(Item item)
        {
            using var context = new RPGContext(_options);

            context.Add(item);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Item> GetItemByNameAsync(ulong guildId, string itemName)
        {
            using var context = new RPGContext(_options);

            return await context.Items.Where(x => x.GuildId == guildId).FirstOrDefaultAsync(x => x.Name.ToLower() == itemName.ToLower()).ConfigureAwait(false);
        }

        public async Task<List<Item>> GetItemList(ulong guildId)
        {
            using var context = new RPGContext(_options);
            return await context.Items.Where(x => x.GuildId == guildId).ToListAsync<Item>().ConfigureAwait(false);
        }

        public async Task<ResultItem> PurchaseItemAsync(ulong memeberId, ulong guildId, string itemName)
        {
            using var context = new RPGContext(_options);
            ResultItem res = new ResultItem();
            res.Item = await GetItemByNameAsync(guildId, itemName).ConfigureAwait(false);

            if (res.Item == null)
            {
                res.ErrMsg = "This item does not exist.";
                res.IsOk = false;
                return res;
            }

            Profile profile = await _profileService.GetProfileAsync(guildId, memeberId).ConfigureAwait(false);

            if (profile.Gold < res.Item.Price)
            {
                res.ErrMsg = $"Not enough golds. You need {res.Item.Price - profile.Gold} more golds.";
                res.IsOk = false;
                return res;
            }

            profile.Gold -= res.Item.Price;
            profile.Items.Add(new ProfileItem
            {
                ItemId = res.Item.Id,
                ProfileId = profile.Id
            });

            context.Profiles.Update(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            res.IsOk = true;
            return res;
        }
    }
}

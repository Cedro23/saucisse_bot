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
        Task<bool> DeleteItemAsync(Item item);
        Task<Item> GetItemByNameAsync(ulong guildId, string itemName);
        Task<List<Item>> GetItemList(ulong guildId);
        Task<ResultItem> PurchaseItemAsync(ulong guildId, ulong memberId, string itemName);
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

        public async Task<bool> DeleteItemAsync(Item item)
        {
            using var context = new RPGContext(_options);

            try
            {
                context.Remove(item);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
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

        public async Task<ResultItem> PurchaseItemAsync(ulong guildId, ulong memberId, string itemName)
        {
            using var context = new RPGContext(_options);
            ResultItem res = new ResultItem();
            res.Item = await GetItemByNameAsync(guildId, itemName).ConfigureAwait(false);
            Profile profile = await _profileService.GetProfileAsync(guildId, memberId).ConfigureAwait(false);
            ProfileItem profItem = await GetProfileItemAsync(profile.Id, res.Item.Id).ConfigureAwait(false);

            if (res.Item == null)
            {
                res.ErrMsg = "This item does not exist.";
                res.IsOk = false;
                return res;
            }

            if (profile.Gold < res.Item.Price)
            {
                res.ErrMsg = $"Not enough golds. You need {res.Item.Price - profile.Gold} more golds.";
                res.IsOk = false;
                return res;
            }

            if (profItem == null)
            {
                profile.Inventory.Add(new ProfileItem
                {
                    ItemId = res.Item.Id,
                    ProfileId = profile.Id,
                    Quantity = 1
                });
            }
            else
            {
                bool limitReached = res.Item.MaxBuyableQuantiy == -1 ? false : (res.Item.MaxBuyableQuantiy - profItem.Quantity == 0);
                if (!limitReached)
                {
                    profile.Inventory.Where(x => x.Id == profItem.Id).FirstOrDefault().Quantity += 1;
                }
                else
                {
                    res.IsOk = false;
                    res.ErrMsg = "Max buyable quantity for this item reached";
                    return res;
                }
            }

            profile.Gold -= res.Item.Price;
            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);

            res.IsOk = true;
            return res;
        }

        private async Task<ProfileItem> GetProfileItemAsync(int profileId, int itemId)
        {
            using var context = new RPGContext(_options);
            ProfileItem profileItem = await context.ProfileItems.Where(x => x.ProfileId == profileId && x.ItemId == itemId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (profileItem == null) { return null; }

            return profileItem;
        }
    }
}

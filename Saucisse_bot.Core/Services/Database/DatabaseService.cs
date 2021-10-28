using Microsoft.EntityFrameworkCore;
using Saucisse_bot.Core.Services.Items;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Database
{
    public interface IDatabaseService
    {
        Task<bool> PurgeTables(string tableName = "");
    }
    public class DatabaseService : IDatabaseService
    {
        private readonly DbContextOptions<RPGContext> _options;
        private readonly IProfileService _profileService;
        private readonly IItemService _itemService;

        public DatabaseService(DbContextOptions<RPGContext> options, IProfileService profileService, IItemService itemService)
        {
            _options = options;
            _profileService = profileService;
            _itemService = itemService;
        }

        public async Task<bool> PurgeTables(string tableName = "")
        {
            using var context = new RPGContext(_options);

            try
            {
                switch (tableName)
                {
                    case "Items":
                        foreach (var entry in context.Items)
                            context.Items.Remove(entry);
                        foreach (var entry in context.ProfileItems)
                            context.ProfileItems.Remove(entry);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                        break;
                    case "Profiles":
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }
    }
}

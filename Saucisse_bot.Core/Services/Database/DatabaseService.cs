using Microsoft.EntityFrameworkCore;
using Saucisse_bot.Core.Services.Items;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Database
{
    public interface IDatabaseService
    {
        Task<bool> PurgeTables(string tableName = "");
        Dictionary<string, int> GetTablesCount();
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

        public Dictionary<string, int> GetTablesCount()
        {
            using var context = new RPGContext(_options);

            Dictionary<string, int> tableInfos = new Dictionary<string, int>();
            var tables = context.DbSetCounts;


            foreach (var table in tables)
            {

                tableInfos.Add(table.Key, table.Value);
            }

            return tableInfos;
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
                    case "ProfileItems":
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Saucisse_bot.Core.Services.Items;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL;
using Saucisse_bot.DAL.Models.Items;
using Saucisse_bot.DAL.Models.Profiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saucisse_bot.Core.Services.Database
{
    public interface IDatabaseService
    {
        Task<bool> PurgeTables(string tableName = "");
        Task<List<IEntityType>> GetTablesName();
        Task<Dictionary<string, string>> GetTableInfo();
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

        public async Task<Dictionary<string, string>> GetTableInfo()
        {
            using var context = new RPGContext(_options);
            
            Dictionary<string, string> embedFieldsDic = new Dictionary<string, string>();


            var table = context.Model.FindEntityType("Items");

            //embedFieldsDic.Add("Name", table.DisplayName());
            //embedFieldsDic.Add("Count", table.GetNavigations().ToString());

            return embedFieldsDic;
        }

        public async Task<List<IEntityType>> GetTablesName()
        {
            using var context = new RPGContext(_options);
            
            List<IEntityType> tableNames = new List<IEntityType>();
            var tables = context.Model.GetEntityTypes();

            foreach (var table in tables)
            {
                tableNames.Add(table);
            }

            return tableNames;
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

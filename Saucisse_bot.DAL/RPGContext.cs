using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL.Models.Items;
using Saucisse_bot.DAL.Models.Profiles;
using System.Collections.Generic;
using System.Linq;

namespace Saucisse_bot.DAL
{
    public class RPGContext : DbContext
    {
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ProfileItem> ProfileItems { get; set; }
        public Dictionary<string, int> DbSetCounts { get => GetDbSetsCounts(); }

        private Dictionary<string, int> GetDbSetsCounts()
        {
            Dictionary<string, int> dbSets = new Dictionary<string, int>();

            dbSets.Add("Items", this.Items.Count());
            dbSets.Add("Profiles", this.Profiles.Count());
            dbSets.Add("ProfileItems", this.ProfileItems.Count());

            return dbSets;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL.Models.Items;
using Saucisse_bot.DAL.Models.Profiles;
using System.Collections.Generic;

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

            dbSets.Add("Items", this.Items.Local.Count);
            dbSets.Add("Profiles", this.Profiles.Local.Count);
            dbSets.Add("ProfileItems", this.ProfileItems.Local.Count);

            return dbSets;
        }
    }
}

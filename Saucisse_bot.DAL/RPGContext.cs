using Saucisse_bot.DAL.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Saucisse_bot.DAL
{
    public class RPGContext : DbContext
    {
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
    }
}

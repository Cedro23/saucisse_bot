using Microsoft.EntityFrameworkCore;
using Saucisse_bot.DAL.Models.Items;

namespace Saucisse_bot.DAL
{
    public class RPGContext : DbContext
    {
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
    }
}

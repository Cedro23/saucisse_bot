namespace Saucisse_bot.DAL.Models.Items
{
    public class Item : Entity
    {
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
    }
}

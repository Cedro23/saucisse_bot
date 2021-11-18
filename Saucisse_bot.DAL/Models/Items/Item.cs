namespace Saucisse_bot.DAL.Models.Items
{
    public class Item : Entity
    {
        public enum ItemRarity
        {
            Common = 0, //Grey
            Uncommon = 1, //Green
            Rare = 2, //Blue
            Epic = 3, //Violet
            Legendary = 4 //Orange
        }

        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
        public ItemRarity Rarity { get; set; } 
        public int MaxBuyableQuantiy { get; set; }
    }
}

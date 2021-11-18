using Saucisse_bot.DAL.Models.Profiles;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saucisse_bot.DAL.Models.Items
{
    public class ProfileItem : Entity
    {
        public int ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
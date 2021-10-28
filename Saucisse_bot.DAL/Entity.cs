using System.ComponentModel.DataAnnotations;

namespace Saucisse_bot.DAL
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}

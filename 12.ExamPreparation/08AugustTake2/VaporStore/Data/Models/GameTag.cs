using System.ComponentModel.DataAnnotations.Schema;

namespace VaporStore.Data.Models
{
    public class GameTag
    {
        [ForeignKey("GameId")]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [ForeignKey("TagId")]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace Cringe.Types.Database
{
    public class Bot
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Host { get; set; }
        public string Hook { get; set; }
        public string Token { get; set; }
    }
}

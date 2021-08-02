
using System.ComponentModel.DataAnnotations;

namespace Cringe.Types.Database
{
    public class Friends
    {
        [Key]
        public int Id { get; set; }

        public int? FromId { get; set; }
        public Player From { get; set; }

        public int? ToId { get; set; }
        public Player To { get; set; }
    }
}

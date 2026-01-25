using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common;

namespace Node.ModelLibrary.Models
{
    public class Conversation : BaseEntity
    {
        public Guid MatchId { get; set; }
        public virtual Match Match { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}

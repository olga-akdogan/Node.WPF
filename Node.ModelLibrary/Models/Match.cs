using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common;
using Node.ModelLibrary.Identity;

namespace Node.ModelLibrary.Models
{
    public enum MatchStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class Match : BaseEntity
    {
        public string UserAId { get; set; } = string.Empty;
        public string UserBId { get; set; } = string.Empty;

        public MatchStatus Status { get; set; } = MatchStatus.Pending;
        public double CompatibilityScore { get; set; }

        public virtual AppUser UserA { get; set; } = null!;
        public virtual AppUser UserB { get; set; } = null!;

      
        public virtual Conversation? Conversation { get; set; }
    }
}

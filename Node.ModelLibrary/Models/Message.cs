using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common; 
using Node.ModelLibrary.Identity;

namespace Node.ModelLibrary.Models
{
    public class Message : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;
        public virtual AppUser Sender { get; set; } = null!;

        
        public Guid ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
    }
}

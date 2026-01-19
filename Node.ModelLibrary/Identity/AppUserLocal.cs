using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common; 

namespace Node.ModelLibrary.Identity
{
    public class AppUserLocal : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public virtual Models.Profile? Profile { get; set; }    
    }
}

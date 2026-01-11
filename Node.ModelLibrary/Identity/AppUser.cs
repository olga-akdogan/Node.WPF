using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Node.ModelLibrary.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public bool IsBlocked { get; set; } = false;

        public virtual Models.Profile? Profile { get; set; }
    }
}

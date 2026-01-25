using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Node.ModelLibrary.Models;

namespace Node.ModelLibrary.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = "";

        public bool IsBlocked { get; set; }

        public Profile? Profile { get; set; }
    }
}
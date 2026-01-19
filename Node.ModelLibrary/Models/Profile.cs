using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common;
using Node.ModelLibrary.Identity;

namespace Node.ModelLibrary.Models
{
    public class Profile : BaseEntity
    {
        public string Bio { get; set; } = string.Empty;
        public string Hobbies { get; set; } = string.Empty;

        public DateTime BirthDateTimeUtc { get; set; }
        public string BirthPlace { get; set; } = string.Empty;
        public double BirthLatitude { get; set; }
        public double BirthLongitude { get; set; }

        public Guid AppUserLocalId { get; set; }
        public virtual AppUserLocal AppUserLocal { get; set; } = null!;   
        public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

    }
}

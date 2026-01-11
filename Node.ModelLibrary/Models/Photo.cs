using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common;

namespace Node.ModelLibrary.Models
{
    public class Photo : BaseEntity 
    {
        public string FilePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; } = null!;
    }
}

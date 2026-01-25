using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common;

namespace Node.ModelLibrary.Models
{
    public class NatalChart : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; } = null!;

      
        public string ChartJson { get; set; } = string.Empty;

        
        public double? SunLongitude { get; set; }
        public double? MoonLongitude { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

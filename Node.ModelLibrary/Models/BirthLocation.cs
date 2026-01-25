using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Node.ModelLibrary.Common;

namespace Node.ModelLibrary.Models
{
    public class BirthLocation : BaseEntity
    {
        public string PlaceName { get; set; } = string.Empty;
        public string? PlaceId { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; }

       
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; } = null!;
    }
}

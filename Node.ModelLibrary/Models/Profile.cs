using Node.ModelLibrary.Common;
using Node.ModelLibrary.Identity;

namespace Node.ModelLibrary.Models
{
    public class Profile : BaseEntity
    {
      
        public string AppUserId { get; set; } = default!;
        public AppUser AppUser { get; set; } = default!;

      
        public string Bio { get; set; } = "";
        public string Hobbies { get; set; } = "";

        public DateTime BirthDateTimeUtc { get; set; }
        public string BirthPlace { get; set; } = "";
        public double BirthLatitude { get; set; }
        public double BirthLongitude { get; set; }

       
        public BirthLocation? BirthLocation { get; set; }
        public NatalChart? NatalChart { get; set; }
    }
}
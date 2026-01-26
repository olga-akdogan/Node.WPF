using System.Text.Json;

namespace Node.ModelLibrary.Models
{ 
    public class EphemerisResult
    {
        public JsonDocument Positions { get; set; } = default!;
        public JsonDocument Aspects { get; set; } = default!;
    }
}
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public class ChartService
    {
        private readonly EphemerisClient _client;

        public ChartService(EphemerisClient client)
        {
            _client = client;
        }

        public async Task<(JsonDocument positions, JsonDocument aspects)> CalculateNatalChartAsync(
            DateTime birthDateTimeUtc,
            double lat,
            double lon)
        {
            
            var bodies = "sun,moon,mercury,venus,mars,jupiter,saturn,uranus,neptune,pluto,chiron";

            var positions = await _client.GetEphemerisDataAsync(birthDateTimeUtc, lat, lon, bodies);
            var aspects = await _client.CalculateAspectsAsync(birthDateTimeUtc, lat, lon, bodies);

            return (positions, aspects);
        }
    }
}

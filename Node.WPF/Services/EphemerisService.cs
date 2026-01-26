using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Node.ModelLibrary.Models;

namespace Node.WPF.Services
{
    public class EphemerisService
    {
        private readonly HttpClient _http;

        public EphemerisService(HttpClient http)
        {
            _http = http;
        }

        public async Task<EphemerisResult> GetNatalChartAsync(
            DateTime birthDateTimeUtc,
            double latitude,
            double longitude)
        {
            var inv = CultureInfo.InvariantCulture;

          
            var dtUtc = birthDateTimeUtc.Kind == DateTimeKind.Utc
                ? birthDateTimeUtc
                : DateTime.SpecifyKind(birthDateTimeUtc, DateTimeKind.Utc);

            var datetime = dtUtc.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", inv);

            
            var bodies = "sun,moon,mercury,venus,mars,jupiter,saturn,uranus,neptune,pluto,chiron";

           
            var lat = latitude.ToString(inv);
            var lon = longitude.ToString(inv);

            var positionsUrl =
                $"get_ephemeris_data" +
                $"?bodies={Uri.EscapeDataString(bodies)}" +
                $"&latitude={Uri.EscapeDataString(lat)}" +
                $"&longitude={Uri.EscapeDataString(lon)}" +
                $"&datetime={Uri.EscapeDataString(datetime)}";

            var aspectsUrl =
                $"calculate_aspects" +
                $"?bodies={Uri.EscapeDataString(bodies)}" +
                $"&latitude={Uri.EscapeDataString(lat)}" +
                $"&longitude={Uri.EscapeDataString(lon)}" +
                $"&datetime={Uri.EscapeDataString(datetime)}";

            System.Diagnostics.Debug.WriteLine($"[Ephemeris] GET {new Uri(_http.BaseAddress!, positionsUrl)}");
            System.Diagnostics.Debug.WriteLine($"[Ephemeris] GET {new Uri(_http.BaseAddress!, aspectsUrl)}");

            var positionsResp = await _http.GetAsync(positionsUrl);
            positionsResp.EnsureSuccessStatusCode();

            var aspectsResp = await _http.GetAsync(aspectsUrl);
            aspectsResp.EnsureSuccessStatusCode();

            var positionsDoc = await positionsResp.Content.ReadFromJsonAsync<JsonDocument>()
                               ?? throw new InvalidOperationException("Positions response was empty.");

            var aspectsDoc = await aspectsResp.Content.ReadFromJsonAsync<JsonDocument>()
                             ?? throw new InvalidOperationException("Aspects response was empty.");

            return new EphemerisResult
            {
                Positions = positionsDoc,
                Aspects = aspectsDoc
            };
        }
    }
}
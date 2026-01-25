using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public class EphemerisClient
    {
        private readonly HttpClient _http;

        public EphemerisClient(HttpClient http)
        {
            _http = http;
        }

        public Task<JsonDocument> GetEphemerisDataAsync(
            DateTime datetimeUtc,
            double latitude,
            double longitude,
            string bodiesCsv,
            CancellationToken ct = default)
        {
            var dt = datetimeUtc.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

            var url =
                $"ephemeris/get_ephemeris_data" +
                $"?bodies={Uri.EscapeDataString(bodiesCsv)}" +
                $"&latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
                $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
                $"&datetime={Uri.EscapeDataString(dt)}";

            return GetJsonAsync(url, ct);
        }

        public Task<JsonDocument> CalculateAspectsAsync(
            DateTime datetimeUtc,
            double latitude,
            double longitude,
            string bodiesCsv,
            CancellationToken ct = default)
        {
            var dt = datetimeUtc.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

            var url =
                $"ephemeris/calculate_aspects" +
                $"?bodies={Uri.EscapeDataString(bodiesCsv)}" +
                $"&latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
                $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
                $"&datetime={Uri.EscapeDataString(dt)}";

            return GetJsonAsync(url, ct);
        }

        private async Task<JsonDocument> GetJsonAsync(string relativeUrl, CancellationToken ct)
        {
            using var resp = await _http.GetAsync(relativeUrl, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Ephemeris API error {(int)resp.StatusCode}: {body}");

            return JsonDocument.Parse(body);
        }
    }
}

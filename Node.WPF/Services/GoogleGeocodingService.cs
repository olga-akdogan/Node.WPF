using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public class GoogleGeocodingService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GoogleGeocodingService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["GoogleMaps:ApiKey"]
                ?? throw new InvalidOperationException(
                    "GoogleMaps:ApiKey is missing. Add it to appsettings.json.");
        }

        public async Task<(double lat, double lng)> GeocodeAsync(string place)
        {
            if (string.IsNullOrWhiteSpace(place))
                throw new ArgumentException("Place cannot be empty.", nameof(place));

            var url =
                $"https://maps.googleapis.com/maps/api/geocode/json" +
                $"?address={Uri.EscapeDataString(place)}" +
                $"&key={Uri.EscapeDataString(_apiKey)}";

            using var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var root = doc.RootElement;

            var status = root.GetProperty("status").GetString();
            if (status != "OK")
                throw new Exception($"Geocoding failed: {status}");

            var location = root
                .GetProperty("results")[0]
                .GetProperty("geometry")
                .GetProperty("location");

            return (
                location.GetProperty("lat").GetDouble(),
                location.GetProperty("lng").GetDouble()
            );
        }
    }
}

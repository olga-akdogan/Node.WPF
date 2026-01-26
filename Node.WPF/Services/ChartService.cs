using System;
using System.Text.Json;
using System.Threading.Tasks;
using Node.ModelLibrary.Models;
using Node.WPF.Models;

namespace Node.WPF.Services
{
    public class ChartService
    {
        private readonly EphemerisService _ephemeris;

        public ChartService(EphemerisService ephemeris)
        {
            _ephemeris = ephemeris;
        }

        public async Task<NatalChartResult> BuildNatalChartAsync(
            DateTime birthDateTimeUtc,
            double latitude,
            double longitude)
        {
            var result = await _ephemeris.GetNatalChartAsync(birthDateTimeUtc, latitude, longitude);

            
            var positions = result.Positions.RootElement;
            var aspects = result.Aspects.RootElement;

            double? sun = TryExtractLongitude(positions, "Sun");
            double? moon = TryExtractLongitude(positions, "Moon");

            var chartJson = JsonSerializer.Serialize(new
            {
                generatedAtUtc = DateTime.UtcNow,
                birthDateTimeUtc,
                latitude,
                longitude,
                positions,
                aspects
            });

            return new NatalChartResult
            {
                ChartJson = chartJson,
                SunLongitude = sun,
                MoonLongitude = moon
            };
        }

        private static double? TryExtractLongitude(JsonElement positions, string bodyName)
        {
        
            if (positions.ValueKind == JsonValueKind.Object)
            {
               
                if (positions.TryGetProperty(bodyName, out var body))
                    return ReadLon(body);

                
                foreach (var prop in positions.EnumerateObject())
                {
                    if (string.Equals(prop.Name, bodyName, StringComparison.OrdinalIgnoreCase))
                        return ReadLon(prop.Value);
                }

                return null;
            }

            
            if (positions.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in positions.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                        continue;

                    if (item.TryGetProperty("name", out var nameProp) &&
                        nameProp.ValueKind == JsonValueKind.String &&
                        string.Equals(nameProp.GetString(), bodyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return ReadLon(item);
                    }
                }
            }

            return null;
        }

        private static double? ReadLon(JsonElement bodyObj)
        {
            if (bodyObj.ValueKind != JsonValueKind.Object)
                return null;

           
            if (TryReadDouble(bodyObj, "lon", out var lon)) return lon;
            if (TryReadDouble(bodyObj, "longitude", out lon)) return lon;
            if (TryReadDouble(bodyObj, "Longitude", out lon)) return lon;

           
            if (bodyObj.TryGetProperty("position", out var positionObj) &&
                positionObj.ValueKind == JsonValueKind.Object)
            {
                if (TryReadDouble(positionObj, "lon", out lon)) return lon;
                if (TryReadDouble(positionObj, "longitude", out lon)) return lon;
            }

            return null;
        }

        private static bool TryReadDouble(JsonElement obj, string propName, out double value)
        {
            value = default;

            if (!obj.TryGetProperty(propName, out var prop))
                return false;

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.TryGetDouble(out value);

            if (prop.ValueKind == JsonValueKind.String &&
                double.TryParse(prop.GetString(), out value))
                return true;

            return false;
        }
    }
}
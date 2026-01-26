using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public sealed class OpenAIService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public OpenAIService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _http.BaseAddress = new Uri("https://api.openai.com/v1/");
        }

        private void EnsureAuthHeader(string apiKey)
        {
            if (_http.DefaultRequestHeaders.Authorization == null)
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        public async Task<string> GenerateProfileSummaryAsync(string natalChartJson)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var model = _config["OpenAI:Model"] ?? "gpt-4.1-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Missing OpenAI:ApiKey. Set it via user-secrets.");

            EnsureAuthHeader(apiKey);

            var body = new
            {
                model,
                messages = new object[]
                {
                    new { role = "system", content = "You are an astrology assistant. Use only the provided JSON." },
                    new { role = "user", content = "Summarize this natal chart:\n\n" + natalChartJson }
                },
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(body);
            using var resp = await _http.PostAsync(
                "chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var respText = await resp.Content.ReadAsStringAsync();
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(respText);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }

        public async Task<string> CreateLoveProfileAsync(
            string prompt,
            CancellationToken ct = default)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var model = _config["OpenAI:Model"] ?? "gpt-4.1-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Missing OpenAI:ApiKey.");

            EnsureAuthHeader(apiKey);

            var body = new
            {
                model,
                messages = new object[]
                {
                    new { role = "system", content = "You are an astrology dating assistant." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(body);
            using var resp = await _http.PostAsync(
                "chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json"),
                ct);

            var respText = await resp.Content.ReadAsStringAsync(ct);
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(respText);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }
    }
}
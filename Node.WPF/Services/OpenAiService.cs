using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public sealed class OpenAiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenAiService(HttpClient http, string apiKey, string model = "gpt-4.1-mini")
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _apiKey = string.IsNullOrWhiteSpace(apiKey) ? throw new ArgumentException("OpenAI API key missing", nameof(apiKey)) : apiKey;
            _model = string.IsNullOrWhiteSpace(model) ? "gpt-4.1-mini" : model;
        }

        public async Task<string> CreateLoveProfileAsync(string prompt, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var body = new
            {
                model = _model,
                input = prompt,
               
                text = new { format = new { type = "text" } }
            };

            req.Content = JsonContent.Create(body);

            using var res = await _http.SendAsync(req, ct).ConfigureAwait(false);
            var json = await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAI error: {(int)res.StatusCode} {res.ReasonPhrase}\n{json}");

            using var doc = JsonDocument.Parse(json);
            
            var root = doc.RootElement;

          
            if (root.TryGetProperty("output", out var output) && output.GetArrayLength() > 0)
            {
                var content = output[0].GetProperty("content");
                if (content.GetArrayLength() > 0 && content[0].TryGetProperty("text", out var text))
                    return text.GetString() ?? "";
            }

           
            return json;
        }
    }
}
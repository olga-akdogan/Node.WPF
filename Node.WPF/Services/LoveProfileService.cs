using Node.ModelLibrary.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public sealed class LoveProfileService
    {
        private readonly OpenAiService _openai;

        public LoveProfileService(OpenAiService openai)
        {
            _openai = openai;
        }

        public Task<string> GenerateAsync(Profile profile, CancellationToken ct = default)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            if (profile.BirthDateTimeUtc == default ||
                string.IsNullOrWhiteSpace(profile.BirthPlace))
            {
                throw new InvalidOperationException("Please fill in your birth data first.");
            }

            var prompt = BuildPrompt(profile);
            return _openai.CreateLoveProfileAsync(prompt, ct);
        }

        private static string BuildPrompt(Profile p)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are an astrologer writing a dating-app style love profile.");
            sb.AppendLine("Be warm, practical, specific. Keep it under 250 words.");
            sb.AppendLine("No medical/diagnostic claims. Avoid absolute predictions.");
            sb.AppendLine();
            sb.AppendLine($"Birth UTC: {p.BirthDateTimeUtc:O}");
            sb.AppendLine($"Birth place: {p.BirthPlace}");
            sb.AppendLine($"Lat/Lon: {p.BirthLatitude}, {p.BirthLongitude}");
            sb.AppendLine();
            sb.AppendLine("Return EXACTLY these sections:");
            sb.AppendLine("1) Love vibe (3 bullets)");
            sb.AppendLine("2) What they need in a partner (3 bullets)");
            sb.AppendLine("3) Green flags they show (3 bullets)");
            sb.AppendLine("4) Red flags to watch (3 bullets, gentle)");
            sb.AppendLine("5) Best first-date ideas (5 bullets)");
            return sb.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.WPF.Services
{
    public sealed class OpenAIOptions
    {
        public string ApiKey { get; set; } = "";
        public string Model { get; set; } = "gpt-4.1-mini";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.WPF.Models
{
    public sealed class NatalChartResult
    {
        public string ChartJson { get; init; } = "{}";
        public double? SunLongitude { get; init; }
        public double? MoonLongitude { get; init; }
    }
}
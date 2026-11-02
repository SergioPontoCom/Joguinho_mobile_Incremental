using System;
using System.Globalization;

namespace PixelIdle.Core
{
    /// <summary>Formata números grandes: 1.5K, 2.30M, 4.10B...</summary>
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp" };

        public static string Format(double value, string smallFormat = "0")
        {
            if (value < 1000d)
                return value.ToString(smallFormat, CultureInfo.InvariantCulture);

            int tier = (int)(Math.Log10(value) / 3);
            tier = Math.Min(tier, Suffixes.Length - 1);
            double scaled = value / Math.Pow(1000d, tier);
            return scaled.ToString("0.00", CultureInfo.InvariantCulture) + Suffixes[tier];
        }

        /// <summary>Para o contador: arredonda para baixo (14,9 aparece como 14).</summary>
        public static string FormatPixels(double value) => Format(Math.Floor(value));

        /// <summary>Para taxas por segundo: até 2 casas decimais.</summary>
        public static string FormatRate(double value) => Format(value, "0.##");
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Pact.Core.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Formats the color as a hash-prefixed hex value (for CSS)
        /// </summary>
        /// <param name="value">The color</param>
        /// <param name="withAlpha">Whether to include the alpha channel</param>
        /// <returns></returns>
        public static string ToCssHex(this Color value, bool withAlpha = false) => $"#{(withAlpha?value.A.ToString("X2"):string.Empty)}{value.R:X2}{value.G:X2}{value.B:X2}";

        /// <summary>
        /// Gets a specific number of distinct random known colors 
        /// </summary>
        /// <param name="number">How many you want</param>
        /// <returns>The list of colors</returns>
        public static IList<Color> GetRandomKnownColors(int number)
        {
            var colors = new List<Color>();
            var random = new Random();

            for (var i = 0; i < number; i++)
            {
                do
                {
                    var color = random.KnownColor();
                    if (colors.Any(x => x == color)) continue;

                    colors.Add(color);
                    break;
                } while (true);
            }

            return colors;
        }

        private static Color KnownColor(this Random random)
        {
            while (true)
            {
                var colorStruct = typeof(Color).GetTypeInfo();
                var knownColorProps = colorStruct.DeclaredProperties.Where(x => x.PropertyType == typeof(Color)).ToList();

                var index = random.Next(knownColorProps.Count - 1);
                var colorProp = knownColorProps.ElementAt(index);

                var color = (Color)colorProp.GetValue(null);

                if ((decimal)color.GetSaturation() < (decimal)0.4) continue;
                if ((decimal)color.GetBrightness() > (decimal)0.7) continue;
                return color;
            }
        }
    }
}

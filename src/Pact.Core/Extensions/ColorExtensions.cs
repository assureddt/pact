using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Pact.Core.Extensions
{
    public static class ColorExtensions
    {
        public static string ToCssHex(this Color value) => $"#{value.R:X2}{value.G:X2}{value.B:X2}";

        public static string[] GetRandomKnownColors(int number)
        {
            var colors = new List<string>();
            var random = new Random();

            for (var i = 0; i < number; i++)
            {
                do
                {
                    var color = random.KnownColor().ToCssHex();
                    if (colors.Any(x => x == color)) continue;

                    colors.Add(color);
                    break;
                } while (true);
            }

            return colors.ToArray();
        }

        public static Color KnownColor(this Random random)
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

using System.Collections.Generic;
using System.Linq;
using Alteridem.Todo.Domain.Common;
using ColoredConsole;

namespace Alteridem.Todo.Extensions
{
    public static class ColoredStringExtensions
    {
        public static ColorToken ToColorToken(this ColoredString coloredString) =>
            new ColorToken(coloredString.Text, coloredString.Color, coloredString.BackgroundColor);

        public static ColorToken[] ToColorTokens(this IEnumerable<ColoredString> coloredStrings) =>
            coloredStrings.Select(cs => cs.ToColorToken()).ToArray();

        public static string ToPlainString(this IEnumerable<ColoredString> coloredStrings) =>
            string.Join(null, coloredStrings.Select(c => c.Text));
    }
}

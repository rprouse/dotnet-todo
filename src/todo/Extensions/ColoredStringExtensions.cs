using System.Collections.Generic;
using System.Linq;
using Alteridem.Todo.Domain.Entities;
using ColoredConsole;

namespace Alteridem.Todo.Extensions
{
    public static class ColoredStringExtensions
    {
        public static ColorToken ToColorToken(this ColoredString coloredString) =>
            new ColorToken(coloredString.Text, coloredString.Color, coloredString.BackgroundColor);

        public static ColorToken[] ToColorTokens(this IEnumerable<ColoredString> coloredStrings) =>
            coloredStrings.Select(cs => cs.ToColorToken()).ToArray();
    }
}

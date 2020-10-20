using System;

namespace Alteridem.Todo.Domain.Common
{
    /// <summary>
    /// The foreground and background colors of items in output
    /// </summary>
    public class ConfigurationColor
    {
        public ConsoleColor? Color { get; set; }
        public ConsoleColor? BackgroundColor { get; set; }
    }
}

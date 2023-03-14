using System;

namespace Alteridem.Todo.Domain.Common;

public struct ColoredString
{
    public string Text { get; }
    public ConsoleColor? Color { get; }
    public ConsoleColor? BackgroundColor { get; }

    public ColoredString(string text, ConsoleColor? color = null, ConsoleColor? backgroundColor = null)
    {
        Text = text;
        Color = color;
        BackgroundColor = backgroundColor;
    }
}

using System.Collections.Generic;
using Alteridem.Todo.Domain.Common;

namespace Alteridem.Todo.Domain.Interfaces;

public interface ITaskConfiguration
{
    string TodoDirectory { get; }
    string TodoFile { get; }
    string DoneFile { get; }
    string ReportFile { get; }
    Dictionary<char, ConfigurationColor> Priorities { get; }
    ConfigurationColor DoneColor { get; }
    ConfigurationColor ProjectColor { get; }
    ConfigurationColor ContextColor { get; }
    ConfigurationColor DateColor { get; }
    ConfigurationColor NumberColor { get; }

    /// <summary>Highlighting for metadata key:value for example DUE:2010-10-21</summary>
    ConfigurationColor MetaColor { get; }

    string GetFullFilename(string filename);
}

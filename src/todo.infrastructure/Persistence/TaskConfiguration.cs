using System;
using System.Collections.Generic;
using System.IO;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Infrastructure.Persistence
{
    public class TaskConfiguration : ITaskConfiguration
    {
        public string TodoDirectory { get; set; }

        public string TodoFile { get; set; }
        public string DoneFile { get; set; }
        public string ReportFile { get; set; }
        public Dictionary<char, ConfigurationColor> Priorities { get; set; }
        public ConfigurationColor DoneColor { get; set; }
        public ConfigurationColor ProjectColor { get; set; }
        public ConfigurationColor ContextColor { get; set; }
        public ConfigurationColor DateColor { get; set; }
        public ConfigurationColor NumberColor { get; set; }

        /// <summary>Highlighting for metadata key:value for example DUE:2010-10-21</summary>
        public ConfigurationColor MetaColor { get; set; }

        public TaskConfiguration()
        {
            TodoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Todo");
            TodoFile = "Todo.txt";
            DoneFile = "Done.txt";
            ReportFile = "Report.txt";

            Priorities = new Dictionary<char, ConfigurationColor>
            {
                { 'A', new ConfigurationColor { Color = ConsoleColor.Yellow }},
                { 'B', new ConfigurationColor { Color = ConsoleColor.Green }},
                { 'C', new ConfigurationColor { Color = ConsoleColor.Blue }},
                { 'D', new ConfigurationColor { Color = ConsoleColor.Cyan }},
            };
            DoneColor = new ConfigurationColor { Color = ConsoleColor.DarkGray };
            ProjectColor = new ConfigurationColor { Color = ConsoleColor.Red };
            ContextColor = new ConfigurationColor { Color = ConsoleColor.Red };
            DateColor = new ConfigurationColor { Color = ConsoleColor.Blue };
            NumberColor = new ConfigurationColor { Color = ConsoleColor.Gray };
            MetaColor = new ConfigurationColor { Color = ConsoleColor.DarkCyan };
        }

        public string GetFullFilename(string filename) =>
            Path.Combine(TodoDirectory, filename);
    }
}

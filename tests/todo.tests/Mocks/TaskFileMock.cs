using System.Collections.Generic;
using System.Linq;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using Alteridem.Todo.Infrastructure.Persistence;

namespace Alteridem.Todo.Tests.Mocks
{
    public class TaskFileMock : ITaskFile
    {
        private readonly ITaskConfiguration _config = new TaskConfiguration();

        public List<string> TaskLines { get; set; } = new List<string>();

        public List<string> DoneLines { get; set; } = new List<string>();

        public List<string> OtherLines { get; set; } = new List<string>();

        public string LineAppended { get; private set; }

        public string AppendToFilename { get; set; }

        public void AppendTo(string filename, string line)
        {
            AppendToFilename = filename;
            LineAppended = line;
            if (filename == _config.TodoFile)
                TaskLines.Add(line);
            else if (filename == _config.DoneFile)
                DoneLines.Add(line);
            else
                OtherLines.Add(line);
        }

        public void Clear(string filename)
        {
            if (filename == _config.TodoFile)
                TaskLines.Clear();
            else if (filename == _config.DoneFile)
                DoneLines.Clear();
            else
                OtherLines.Clear();
        }

        public IList<TaskItem> LoadTasks(string filename)
        {
            IList<string> lines;
            if (filename == _config.TodoFile)
                lines = TaskLines;
            else if (filename == _config.DoneFile)
                lines = DoneLines;
            else
                lines = OtherLines;

            return lines.Select((line, index) => new TaskItem(line, index + 1))
                 .Where(t => !t.Empty)
                 .ToList();
        }
    }
}

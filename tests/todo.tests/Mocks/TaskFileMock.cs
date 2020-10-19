using System.Collections.Generic;
using System.Linq;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Tests.Mocks
{
    public class TaskFileMock : ITaskFile
    {
        public List<string> TaskLines { get; set; } = new List<string>();

        public List<string> DoneLines { get; set; } = new List<string>();

        public List<string> OtherLines { get; set; } = new List<string>();

        public string LineAppended { get; private set; }

        public string AppendToFilename { get; set; }

        public void AppendTo(string filename, string line)
        {
            AppendToFilename = filename;
            LineAppended = line;
            if (filename == StandardFilenames.Todo)
                TaskLines.Add(line);
            else if (filename == StandardFilenames.Done)
                DoneLines.Add(line);
            else
                OtherLines.Add(line);
        }

        public void Clear(string filename)
        {
            if (filename == StandardFilenames.Todo)
                TaskLines.Clear();
            else if (filename == StandardFilenames.Done)
                DoneLines.Clear();
            else
                OtherLines.Clear();
        }

        public IList<TaskItem> LoadTasks(string filename)
        {
            IList<string> lines;
            if (filename == StandardFilenames.Todo)
                lines = TaskLines;
            else if (filename == StandardFilenames.Done)
                lines = DoneLines;
            else
                lines = OtherLines;

            return lines.Select((line, index) => new TaskItem(line, index + 1))
                 .Where(t => !t.Empty)
                 .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Tests.Mocks
{
    public class TaskFileMock : ITaskFile
    {
        public List<string> Lines { get; set; } = new List<string>();

        public string LineAppended { get; private set; }
        public void AppendLine(string line)
        {
            LineAppended = line;
            Lines.Add(line);
        }

        public IList<TaskItem> LoadTasks() =>
            Lines.Select((line, index) => new TaskItem(line, index))
                 .Where(t => !t.Empty)
                 .ToList();
    }
}

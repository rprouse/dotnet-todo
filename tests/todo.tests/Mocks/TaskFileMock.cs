using System;
using System.Collections.Generic;
using System.Linq;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Tests.Mocks
{
    public class TaskFileMock : ITaskFile
    {
        public List<string> TaskLines { get; set; } = new List<string>();

        public List<string> DoneLines { get; set; } = new List<string>();

        public string LineAppended { get; private set; }

        public void AppendTodo(string line)
        {
            LineAppended = line;
            TaskLines.Add(line);
        }

        public void AppendDone(string line) =>
            DoneLines.Add(line);

        public void ClearTodo() => TaskLines.Clear();

        public IList<TaskItem> LoadTasks() =>
            TaskLines.Select((line, index) => new TaskItem(line, index + 1))
                 .Where(t => !t.Empty)
                 .ToList();
    }
}

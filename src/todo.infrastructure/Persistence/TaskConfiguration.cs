using System;
using System.IO;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Infrastructure.Persistence
{
    public class TaskConfiguration : ITaskConfiguration
    {
        public string TaskDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Todo");

        public string TodoFilename =>
            Path.Combine(TaskDirectory, "todo.txt");

        public string DoneFilename =>
            Path.Combine(TaskDirectory, "done.txt");
    }
}

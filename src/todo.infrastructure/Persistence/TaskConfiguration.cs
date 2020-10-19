using System;
using System.IO;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Infrastructure.Persistence
{
    public class TaskConfiguration : ITaskConfiguration
    {
        public string TaskDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Todo");

        public string GetFullFilename(string filename) =>
            Path.Combine(TaskDirectory, filename);
    }
}

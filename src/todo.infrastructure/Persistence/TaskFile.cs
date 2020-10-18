using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;

namespace Alteridem.Todo.Infrastructure.Persistence
{
    public class TaskFile : ITaskFile
    {
        private ITaskConfiguration _configuration;

        public TaskFile(ITaskConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AppendLine(string line)
        {
            File.AppendAllText(_configuration.TodoFilename, line + "\n");
        }

        public IList<TaskItem> LoadTasks()
        {
            if (!File.Exists(_configuration.TodoFilename))
                throw new FileNotFoundException("Task file not found", _configuration.TodoFilename);

            return File.ReadAllLines(_configuration.TodoFilename)
                       .Select((line, index) => new TaskItem(line, index + 1))
                       .Where(t => !t.Empty)
                       .ToList();
        }
    }
}

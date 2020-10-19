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

        public void AppendTo(string filename, string line)
        {
            File.AppendAllText(_configuration.GetFullFilename(filename), line);
        }

        public void Clear(string filename)
        {
            File.Delete(_configuration.GetFullFilename(filename));
        }

        public IList<TaskItem> LoadTasks(string filename)
        {
            string fullpath = _configuration.GetFullFilename(filename);
            if (!File.Exists(fullpath))
                return new List<TaskItem>();

            return File.ReadAllLines(fullpath)
                       .Select((line, index) => new TaskItem(line, index + 1))
                       .Where(t => !t.Empty)
                       .ToList();
        }
    }
}

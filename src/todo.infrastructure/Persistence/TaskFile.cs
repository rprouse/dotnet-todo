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
            string fullpath = _configuration.GetFullFilename(filename);
            string text = $"{(LastLineIsNewline(fullpath) ? "" : "\n")}{line}\n";
            File.AppendAllText(fullpath, text);
        }

        private bool LastLineIsNewline(string fullpath)
        {
            if (!File.Exists(fullpath))
                return true;

            using (var reader = new StreamReader(fullpath))
            {
                if (reader.BaseStream.Length < 2)
                    return false;

                reader.BaseStream.Seek(-2, SeekOrigin.End);
                int c1 = reader.Read();
                int c2 = reader.Read();
                return (c1 == '\n' || c2 == '\n');
            }
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

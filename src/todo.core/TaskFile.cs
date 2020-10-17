using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alteridem.Todo.Core
{
    public class TaskFile
    {
        private string _filename;

        IList<Task> Tasks { get; set; }

        public TaskFile(string filename)
        {
            _filename = filename;
        }

        public void Add(string line, bool addDate)
        {
            var task = new Task(line);
            if (addDate) task.CreationDate = DateTime.Now.Date;
            var taskStr = task.ToString();

            File.AppendAllText(_filename, taskStr + "\n");

            var tasks = LoadTasks();
            Console.WriteLine($"{tasks.Count} {taskStr}");
            Console.WriteLine($"TODO: {tasks.Count} added.");
        }

        private IList<Task> LoadTasks()
        {
            if (!File.Exists(_filename))
                throw new FileNotFoundException("Task file not found", _filename);

            return File.ReadAllLines(_filename)
                       .Where(l => !string.IsNullOrEmpty(l))
                       .Select(line => new Task(line))
                       .ToList();
        }
    }
}

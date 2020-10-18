using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColoredConsole;

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

            var tasks = LoadTasks().ToList();
            Console.WriteLine($"{tasks.Count} {taskStr}");
            Console.WriteLine($"TODO: {tasks.Count} added.");
        }

        public void List(string[] terms)
        {
            var tasks = LoadTasks();
            int count = tasks.Count();
            IEnumerable<Task> search = tasks;
            foreach(var term in terms)
            {
                if(term.StartsWith("-") && term.Length > 1)
                    search = search.Where(t => !t.Description.Contains(term.Substring(1)));
                else
                    search = search.Where(t => t.Description.Contains(term));
            }
            search = search.OrderBy(t => t.Priority ?? '[');
            foreach(var task in search)
            {
                ColorConsole.WriteLine(task.ToColorString(true));
            }
            Console.WriteLine("--");
            Console.WriteLine($"TODO: {search.Count()} of {tasks.Count} tasks shown");
        }

        public void Complete(uint[] items)
        {

        }

        private IList<Task> LoadTasks()
        {
            if (!File.Exists(_filename))
                throw new FileNotFoundException("Task file not found", _filename);

            return File.ReadAllLines(_filename)
                       .Select((line, index) => new Task(line, index + 1))
                       .Where(t => !t.Empty)
                       .ToList();
        }
    }
}

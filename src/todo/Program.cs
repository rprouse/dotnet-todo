using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Alteridem.Todo.Core;

namespace Alteridem.Todo
{
    class Program
    {
        static void Main(string[] args)
        {
            var todoDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var todo = new TaskFile(Path.Combine(todoDirectory, "todo.txt"));

            var add = new Command("add", "Adds THING I NEED TO DO to your todo.txt file on its own line.")
            {
                new Argument("task")
            };
            add.AddAlias("a");
            add.Handler = CommandHandler.Create((string task, bool t) => todo.Add(task, t));

            var root = new RootCommand
            {
                add,
            };

            root.AddOption(new Option<bool>("-t", "Prepend the current date to a task automatically when it's added."));
            root.Invoke(args);
        }
    }
}

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Alteridem.Todo.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo
{
    public class TodoApplication
    {
        public IServiceProvider Services { get; }

        public TodoApplication(IServiceCollection serviceCollection)
        {
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        public void Run()
        {
            var root = CreateCommands();
            root.Invoke(Environment.GetCommandLineArgs());
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            // TODO: Needed?
        }

        private void Add(string task, bool prependCreateDate)
        {
            var todoDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var todo = new TaskFile(Path.Combine(todoDirectory, "todo.txt"));
            todo.Add(task, prependCreateDate);
        }

        private void List(string[] terms)
        {
            var todoDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var todo = new TaskFile(Path.Combine(todoDirectory, "todo.txt"));
            todo.List(terms);
        }

        private void Complete(uint[] items)
        {
            var todoDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var todo = new TaskFile(Path.Combine(todoDirectory, "todo.txt"));
            todo.Complete(items);
        }

        private RootCommand CreateCommands()
        {
            var add = new Command("add", "Adds THING I NEED TO DO to your todo.txt file on its own line.");
            add.AddArgument(new Argument("task"));
            add.AddAlias("a");
            add.Handler = CommandHandler.Create((string task, bool t) => Add(task, t));

            var list = new Command("list", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).");
            list.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            list.AddAlias("ls");
            list.Handler = CommandHandler.Create((string[] terms) => List(terms));

            var @do = new Command("do", "Marks task(s) on line ITEM# as done in todo.txt.");
            @do.AddArgument(new Argument<uint[]>("items", () => new uint[] { }));
            @do.Handler = CommandHandler.Create((uint[] items) => Complete(items));

            var root = new RootCommand
            {
                add,
                list,
                @do
            };

            root.AddOption(new Option<bool>("-t", "Prepend the current date to a task automatically when it's added."));

            return root;
        }
    }
}

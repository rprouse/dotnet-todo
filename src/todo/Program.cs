using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Alteridem.Todo.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Alteridem.Todo
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var app = new TodoApplication(serviceCollection);
            app.Run(args);
        }

        static private void ConfigureServices(IServiceCollection serviceCollection)
        {
        }
    }

    public class TodoApplication
    {
        public IServiceProvider Services { get; }

        public TodoApplication(IServiceCollection serviceCollection)
        {
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        public void Run(string[] args)
        {
            var todoDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var todo = new TaskFile(Path.Combine(todoDirectory, "todo.txt"));

            var add = new Command("add", "Adds THING I NEED TO DO to your todo.txt file on its own line.")
            {
                new Argument("task")
            };
            add.AddAlias("a");
            add.Handler = CommandHandler.Create((string task, bool t) => todo.Add(task, t));

            var list = new Command("list", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).")
            {
                new Argument<string[]>("terms", () => new string[]{ })
            };
            list.AddAlias("ls");
            list.Handler = CommandHandler.Create((string[] terms) => todo.List(terms));

            var @do = new Command("do", "Marks task(s) on line ITEM# as done in todo.txt.")
            {
                new Argument<uint[]>("items", () => new uint[]{ })
            };
            @do.Handler = CommandHandler.Create((uint[] items) => todo.Complete(items));

            var root = new RootCommand
            {
                add,
                list,
                @do
            };

            root.AddOption(new Option<bool>("-t", "Prepend the current date to a task automatically when it's added."));
            root.Invoke(args);
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {

        }
    }
}

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Add;
using Alteridem.Todo.Application.Commands.Archive;
using Alteridem.Todo.Application.Commands.Do;
using Alteridem.Todo.Application.Queries.List;
using Alteridem.Todo.Extensions;
using ColoredConsole;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo
{
    public class TodoApplication
    {
        private IServiceProvider Services { get; }
        private IMediator Mediator { get; }

        public TodoApplication(IServiceCollection serviceCollection)
        {
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            Mediator = Services.GetService<IMediator>();
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

        private async Task Add(string str, bool addCreationDate)
        {
            var addTaskCommand = new AddTaskCommand { Task = str, AddCreationDate = addCreationDate };
            var task = await Mediator.Send(addTaskCommand);

            Console.WriteLine(task.ToString());
            Console.WriteLine($"TODO: {task.LineNumber} added.");
        }

        private async Task Archive()
        {
            var archiveCommand = new ArchiveTasksCommand();
            var result = await Mediator.Send(archiveCommand);

            foreach(var task in result)
            {
                Console.WriteLine(task.ToString(true));
            }
            Console.WriteLine($"{result.Count} tasks archived.");
        }

        private async Task List(string[] terms)
        {
            var listTasksQuery = new ListTasksQuery { Terms = terms };
            var result = await Mediator.Send(listTasksQuery);
            foreach (var task in result.Tasks)
            {
                ColorConsole.WriteLine(task.ToColorString(true).ToColorToken());
            }
            Console.WriteLine("--");
            Console.WriteLine($"TODO: {result.Tasks.Count} of {result.TotalTasks} tasks shown");
        }

        private async Task Complete(int[] items, bool dontArchive)
        {
            var doTasksCommand = new DoTasksCommand { ItemNumbers = items, DontArchive = dontArchive };
            var result = await Mediator.Send(doTasksCommand);
            foreach(var completed in result)
            {
                Console.WriteLine(completed.ToString(true));
                Console.WriteLine($"TODO: {completed.LineNumber} marked as done");
            }
        }

        private RootCommand CreateCommands()
        {
            var add = new Command("add", "Adds THING I NEED TO DO to your todo.txt file on its own line.");
            add.AddArgument(new Argument("task"));
            add.AddAlias("a");
            add.Handler = CommandHandler.Create(async (string task, bool t) => await Add(task, t));

            var archive = new Command("archive", "Moves all done tasks from todo.txt to done.txt and removes blank lines.");
            archive.Handler = CommandHandler.Create(async () => await Archive());

            var list = new Command("list", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).");
            list.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            list.AddAlias("ls");
            list.Handler = CommandHandler.Create(async (string[] terms) => await List(terms));

            var @do = new Command("do", "Marks task(s) on line ITEM# as done in todo.txt.");
            @do.AddArgument(new Argument<int[]>("items", () => new int[] { }));
            @do.Handler = CommandHandler.Create(async (int[] items, bool a) => await Complete(items, a));

            var root = new RootCommand
            {
                add,
                archive,
                list,
                @do
            };

            root.AddOption(new Option<bool>("-a", "Don't auto-archive tasks automatically on completion"));
            root.AddOption(new Option<bool>("-t", "Prepend the current date to a task automatically when it's added."));

            return root;
        }
    }
}

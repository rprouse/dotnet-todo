using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Add;
using Alteridem.Todo.Application.Commands.Archive;
using Alteridem.Todo.Application.Commands.Delete;
using Alteridem.Todo.Application.Commands.Do;
using Alteridem.Todo.Application.Commands.Priority;
using Alteridem.Todo.Application.Queries.IndividualTask;
using Alteridem.Todo.Application.Queries.List;
using Alteridem.Todo.Application.Queries.ListContexts;
using Alteridem.Todo.Application.Queries.ListProjects;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using Alteridem.Todo.Extensions;
using ColoredConsole;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo
{
    public class TodoApplication
    {
        readonly char[] ValidPriorities;

        private IServiceProvider Services { get; }
        private IMediator Mediator { get; }
        private ITaskConfiguration Configuration { get; }

        public TodoApplication(IServiceCollection serviceCollection)
        {
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            Mediator = Services.GetService<IMediator>();
            Configuration = Services.GetService<ITaskConfiguration>();

            ValidPriorities = Enumerable.Range(0, 26)
                .Select(i => (char)(i + 'A'))
                .ToArray();
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

        private async Task Add(string text, bool addCreationDate)
        {
            var command = new AddTaskCommand { Filename = Configuration.TodoFile, Task = text, AddCreationDate = addCreationDate };
            var task = await Mediator.Send(command);

            Console.WriteLine(task.ToString());
            Console.WriteLine($"TODO: {task.LineNumber} added.");
        }

        private async Task AddMultiple(string[] lines, bool addCreationDate)
        {
            foreach (string text in lines)
            {
                await Add(text, addCreationDate);
            }
        }

        private async Task AddTo(string filename, string text, bool addCreationDate)
        {
            var command = new AddTaskCommand { Filename = filename, Task = text, AddCreationDate = addCreationDate };
            var task = await Mediator.Send(command);

            Console.WriteLine(task.ToString());
            Console.WriteLine($"TODO: {task.LineNumber} added to {filename}.");
        }

        private async Task Archive()
        {
            var command = new ArchiveTasksCommand();
            var result = await Mediator.Send(command);

            foreach (var task in result)
            {
                Console.WriteLine(task.ToString(true));
            }
            Console.WriteLine($"TODO: {result.Count} tasks archived.");
        }

        private async Task Delete(int item, string term)
        {
            var query = new TaskQuery { ItemNumber = item };
            var queryResult = await Mediator.Send(query);
            if (queryResult == null)
            {
                Console.WriteLine($"TODO: {item} not found; no removal done.");
                return;
            }

            if (string.IsNullOrWhiteSpace(term))
                await DeleteTask(queryResult);
            else
                await DeleteTerm(queryResult, term);
        }

        private async Task DeleteTerm(TaskItem task, string term)
        {
            Console.WriteLine(task.ToString(true));
            var command = new DeleteTermCommand { ItemNumber = task.LineNumber, Term = term };
            var result = await Mediator.Send(command);
            if(result.Success)
            {
                Console.WriteLine($"TODO: Removed '{term}' from task.");
                Console.WriteLine(result.Task.ToString(true));
            }
            else
            {
                Console.WriteLine($"TODO: {term} not found; no removal done.");
            }
        }

        private async Task DeleteTask(TaskItem task)
        {
            Console.WriteLine($"Delete '{task.Text}'? (y/n)");
            var key = Console.ReadKey();
            if (key.KeyChar == 'y' || key.KeyChar == 'Y')
            {
                var command = new DeleteTaskCommand { ItemNumber = task.LineNumber };
                var result = await Mediator.Send(command);
                Console.WriteLine($"TODO: {result} deleted.");
            }
        }

        private async Task List(string[] terms)
        {
            var query = new ListTasksQuery { Filename = Configuration.TodoFile, Terms = terms };
            var result = await Mediator.Send(query);
            foreach (var task in result.Tasks)
            {
                ColorConsole.WriteLine(task.ToColorString(true).ToColorToken());
            }
            Console.WriteLine("--");
            Console.WriteLine($"TODO: {result.Tasks.Count} of {result.TotalTasks} tasks shown");
        }

        private async Task ListAll(string[] terms)
        {
            var query = new ListAllQuery { Terms = terms };
            var result = await Mediator.Send(query);
            foreach (var task in result.Tasks)
            {
                ColorConsole.WriteLine(task.ToColorString(true).ToColorToken());
            }
            Console.WriteLine("--");
            Console.WriteLine($"TODO: {result.ShownTasks} of {result.TotalTasks} tasks shown");
            Console.WriteLine($"DONE: {result.ShownDone} of {result.TotalDone} tasks shown");
            Console.WriteLine($"total {result.ShownTasks + result.ShownDone} of {result.TotalTasks + result.TotalDone} tasks shown");
        }

        private async Task ListContexts(string[] terms)
        {
            var query = new ListContextsQuery { Terms = terms };
            var result = await Mediator.Send(query);
            foreach (string context in result)
                Console.WriteLine(context);
        }

        private async Task ListFile(string filename, string[] terms)
        {
            var query = new ListTasksQuery { Filename = filename, Terms = terms };
            var result = await Mediator.Send(query);
            foreach (var task in result.Tasks)
            {
                ColorConsole.WriteLine(task.ToColorString(true).ToColorToken());
            }
            Console.WriteLine("--");
            Console.WriteLine($"{filename.ToUpper()}: {result.Tasks.Count} of {result.TotalTasks} tasks shown");
        }

        private async Task ListProjects(string[] terms)
        {
            var query = new ListProjectsQuery { Terms = terms };
            var result = await Mediator.Send(query);
            foreach (string project in result)
                Console.WriteLine(project);
        }

        private async Task Complete(int[] items, bool dontArchive)
        {
            var command = new DoTasksCommand { ItemNumbers = items, DontArchive = dontArchive };
            var result = await Mediator.Send(command);
            foreach (var completed in result)
            {
                Console.WriteLine(completed.ToString(true));
                Console.WriteLine($"TODO: {completed.LineNumber} marked as done");
            }
        }

        private async Task AddPriority(int item, char priority)
        {
            if(!ValidPriorities.Contains(priority))
            {
                Console.WriteLine($"note: PRIORITY must be anywhere from A to Z.");
                return;
            }
            var command = new AddPriorityCommand { ItemNumber = item, Priority = priority };
            var result = await Mediator.Send(command);
            if (result is null)
            {
                Console.WriteLine($"TODO: No task {item}.");
            }
            else
            {
                Console.WriteLine(result.ToString(true));
                Console.WriteLine($"TODO: {result.LineNumber} prioritized {result.Priority}");
            }
        }

        private RootCommand CreateCommands()
        {
            var add = new Command("add", "Adds THING I NEED TO DO to your todo.txt file on its own line.");
            add.AddArgument(new Argument<string>("task"));
            add.AddAlias("a");
            add.Handler = CommandHandler.Create(async (string task, bool t) => await Add(task, t));

            var addm = new Command("addm", "Adds \"FIRST THING I NEED TO DO\" to your todo.txt on its own line and adds \"SECOND THING I NEED TO DO\" to you todo.txt on its own line.");
            addm.AddArgument(new Argument<string[]>("tasks"));
            addm.Handler = CommandHandler.Create(async (string[] tasks, bool t) => await AddMultiple(tasks, t));

            var addTo = new Command("addto", "Adds a line of text to any file located in the todo.txt directory.");
            addTo.AddArgument(new Argument<string>("filename"));
            addTo.AddArgument(new Argument<string>("task"));
            addTo.Handler = CommandHandler.Create(async (string filename, string task, bool t) => await AddTo(filename, task, t));

            var archive = new Command("archive", "Moves all done tasks from todo.txt to done.txt and removes blank lines.");
            archive.Handler = CommandHandler.Create(async () => await Archive());

            var delete = new Command("delete", "Deletes the task on line ITEM# in todo.txt. If TERM specified, deletes only TERM from the task.");
            delete.AddArgument(new Argument<int[]>("item"));
            delete.AddArgument(new Argument<string>("term", () => null));
            delete.AddAlias("rm");
            delete.Handler = CommandHandler.Create(async (int item, string term) => await Delete(item, term));

            var @do = new Command("do", "Marks task(s) on line ITEM# as done in todo.txt.");
            @do.AddArgument(new Argument<int[]>("items", () => new int[] { }));
            @do.Handler = CommandHandler.Create(async (int[] items, bool a) => await Complete(items, a));

            var list = new Command("list", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).");
            list.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            list.AddAlias("ls");
            list.Handler = CommandHandler.Create(async (string[] terms) => await List(terms));

            var listall = new Command("listall", "Displays all the lines in todo.txt AND done.txt that contain TERM(s) sorted by priority with line numbers. Hides all tasks that contain TERM(s) preceded by a minus sign(i.e. -TERM). If no TERM specified, lists entire todo.txt AND done.txt concatenated and sorted.");
            listall.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listall.Handler = CommandHandler.Create(async (string[] terms) => await ListAll(terms));

            var listcon = new Command("listcon", "Lists all the task contexts that start with the @ sign in todo.txt. If TERM specified, considers only tasks that contain TERM(s).");
            listcon.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listcon.Handler = CommandHandler.Create(async (string[] terms) => await ListContexts(terms));

            var listfile = new Command("listfile", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).");
            listfile.AddArgument(new Argument<string>("filename"));
            listfile.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listfile.Handler = CommandHandler.Create(async (string filename, string[] terms) => await ListFile(filename, terms));

            var listproj = new Command("listproj", "Lists all the projects (terms that start with a + sign) in todo.txt. If TERM specified, considers only tasks that contain TERM(s).");
            listproj.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listproj.Handler = CommandHandler.Create(async (string[] terms) => await ListProjects(terms));

            var pri = new Command("pri", "Adds PRIORITY to task on line ITEM#. If the task is already prioritized, replaces current priority with new PRIORITY. PRIORITY must be a letter between A and Z.");
            pri.AddArgument(new Argument<int>("item"));
            pri.AddArgument(new Argument<char>("priority"));
            pri.Handler = CommandHandler.Create(async (int item, char priority) => await AddPriority(item, priority));

            var root = new RootCommand
            {
                add,
                addm,
                addTo,
                archive,
                delete,
                @do,
                list,
                listall,
                listcon,
                listfile,
                listproj,
                pri
            };

            root.AddOption(new Option<bool>("-a", "Don't auto-archive tasks automatically on completion"));
            root.AddOption(new Option<bool>("-t", "Prepend the current date to a task automatically when it's added."));

            return root;
        }
    }
}

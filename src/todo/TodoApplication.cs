using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alteridem.Todo.Application;
using Alteridem.Todo.Application.Commands.Add;
using Alteridem.Todo.Application.Commands.Append;
using Alteridem.Todo.Application.Commands.Archive;
using Alteridem.Todo.Application.Commands.Delete;
using Alteridem.Todo.Application.Commands.Deprioritize;
using Alteridem.Todo.Application.Commands.Do;
using Alteridem.Todo.Application.Commands.Prepend;
using Alteridem.Todo.Application.Commands.Priority;
using Alteridem.Todo.Application.Commands.Replace;
using Alteridem.Todo.Application.Queries.IndividualTask;
using Alteridem.Todo.Application.Queries.List;
using Alteridem.Todo.Application.Queries.ListContexts;
using Alteridem.Todo.Application.Queries.ListPriority;
using Alteridem.Todo.Application.Queries.ListProjects;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using Alteridem.Todo.Extensions;
using Alteridem.Todo.Infrastructure;
using ColoredConsole;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo
{
    public class TodoApplication
    {
        readonly char[] ValidPriorities;

        private IServiceProvider Services { get; set; }
        private IMediator Mediator { get; set; }
        private ITaskConfiguration Configuration { get; set; }

        public TodoApplication()
        {
            ValidPriorities = Enumerable.Range(0, 26)
                .Select(i => (char)(i + 'A'))
                .ToArray();
        }

        private void Configure(string configFile)
        {
            if (configFile != null && !File.Exists(configFile))
            {
                ColorConsole.WriteLine($"{configFile} does not exist.".Red());
                ColorConsole.WriteLine("Using default configuration.".Red());
                configFile = null;
            }

            if (configFile == null)
                configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo.json");

            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection
                .AddApplication()
                .AddInfrastructure(configFile);

            Services = serviceCollection.BuildServiceProvider();
            Mediator = Services.GetService<IMediator>();
            Configuration = Services.GetService<ITaskConfiguration>();
        }

        public void Run()
        {
            var root = CreateCommands();
            root.Invoke(Environment.GetCommandLineArgs());
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

        private async Task Append(int item, string text)
        {
            var command = new AppendCommand { ItemNumber = item, Text = text };
            var result = await Mediator.Send(command);

            if(result is null)
                Console.WriteLine($"TODO: No task {item}.");
            else
                Console.WriteLine(result.ToString(true));
        }

        private async Task Archive(string configFile)
        {
            var command = new ArchiveTasksCommand();
            var result = await Mediator.Send(command);

            foreach (var task in result)
            {
                Console.WriteLine(task.ToString(true));
            }
            Console.WriteLine($"TODO: {result.Count} tasks archived.");
        }

        private async Task Delete(int item, string term, bool force)
        {
            var query = new TaskQuery { ItemNumber = item };
            var queryResult = await Mediator.Send(query);
            if (queryResult == null)
            {
                Console.WriteLine($"TODO: {item} not found; no removal done.");
                return;
            }

            if (string.IsNullOrWhiteSpace(term))
                await DeleteTask(queryResult, force);
            else
                await DeleteTerm(queryResult, term);
        }

        private async Task DeleteTerm(TaskItem task, string term)
        {
            Console.WriteLine(task.ToString(true));
            var command = new DeleteTermCommand { ItemNumber = task.LineNumber, Term = term };
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                Console.WriteLine($"TODO: Removed '{term}' from task.");
                Console.WriteLine(result.Task.ToString(true));
            }
            else
            {
                Console.WriteLine($"TODO: {term} not found; no removal done.");
            }
        }

        private async Task DeleteTask(TaskItem task, bool force)
        {
            if (!force)
            {
                Console.WriteLine($"Delete '{task.Text}'? (y/n)");
                var key = Console.ReadKey();
                if (key.KeyChar != 'y' && key.KeyChar != 'Y')
                {
                    return;
                }
            }
            var command = new DeleteTaskCommand { ItemNumber = task.LineNumber };
            var result = await Mediator.Send(command);
            Console.WriteLine($"TODO: {result} deleted.");
        }

        private async Task Deprioritize(int[] items)
        {
            var command = new DeprioritizeCommand { ItemNumbers = items };
            var result = await Mediator.Send(command);
            foreach(var task in result)
            {
                Console.WriteLine(task.ToString(true));
                Console.WriteLine($"TODO: {task.LineNumber} deprioritized.");
            }
        }

        private async Task List(string[] terms)
        {
            var query = new ListTasksQuery { Filename = Configuration.TodoFile, Terms = terms };
            var result = await Mediator.Send(query);
            foreach (var task in result.Tasks)
            {
                ColorConsole.WriteLine(task.ToColorString(true, Configuration).ToColorTokens());
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
                ColorConsole.WriteLine(task.ToColorString(true, Configuration).ToColorTokens());
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
                ColorConsole.WriteLine(task.ToColorString(true, Configuration).ToColorTokens());
            }
            Console.WriteLine("--");
            Console.WriteLine($"{filename.ToUpper()}: {result.Tasks.Count} of {result.TotalTasks} tasks shown");
        }

        private async Task ListPriorities(string priorities, string[] terms)
        {
            var query = new ListPriorityQuery { Priorities = priorities, Terms = terms };
            var result = await Mediator.Send(query);
            foreach (var task in result.Tasks)
            {
                ColorConsole.WriteLine(task.ToColorString(true, Configuration).ToColorTokens());
            }
            Console.WriteLine("--");
            Console.WriteLine($"TODO: {result.Tasks.Count} of {result.TotalTasks} tasks shown");
        }

        private async Task ListProjects(string[] terms)
        {
            var query = new ListProjectsQuery { Terms = terms };
            var result = await Mediator.Send(query);
            foreach (string project in result)
                Console.WriteLine(project);
        }

        private async Task Prepend(int item, string text)
        {
            var command = new PrependCommand { ItemNumber = item, Text = text };
            var result = await Mediator.Send(command);

            if (result is null)
                Console.WriteLine($"TODO: No task {item}.");
            else
                Console.WriteLine(result.ToString(true));
        }

        private async Task Replace(int item, string text)
        {
            var command = new ReplaceCommand { ItemNumber = item, Text = text };
            var result = await Mediator.Send(command);

            if (result is null)
            {
                Console.WriteLine($"TODO: No task {item}.");
            }
            else
            {
                Console.WriteLine("TODO: Replaced task with:");
                Console.WriteLine(result.ToString(true));
            }
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
            if (!ValidPriorities.Contains(priority))
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
            add.Handler = CommandHandler.Create(async (string task, bool t, string d) =>
            {
                Configure(d);
                await Add(task, t);
            });

            var addm = new Command("addm", "Adds \"FIRST THING I NEED TO DO\" to your todo.txt on its own line and adds \"SECOND THING I NEED TO DO\" to you todo.txt on its own line.");
            addm.AddArgument(new Argument<string[]>("tasks"));
            addm.Handler = CommandHandler.Create(async (string[] tasks, bool t, string d) =>
            {
                Configure(d);
                await AddMultiple(tasks, t);
            });

            var addTo = new Command("addto", "Adds a line of text to any file located in the todo.txt directory.");
            addTo.AddArgument(new Argument<string>("filename"));
            addTo.AddArgument(new Argument<string>("task"));
            addTo.Handler = CommandHandler.Create(async (string filename, string task, bool t, string d) =>
            {
                Configure(d);
                await AddTo(filename, task, t);
            });

            var append = new Command("append", "Adds TEXT TO APPEND to the end of the task on line ITEM#.");
            append.AddArgument(new Argument<int>("item"));
            append.AddArgument(new Argument<string>("text"));
            append.AddAlias("app");
            append.Handler = CommandHandler.Create(async (int item, string text, string d) =>
            {
                Configure(d);
                await Append(item, text);
            });

            var archive = new Command("archive", "Moves all done tasks from todo.txt to done.txt and removes blank lines.");
            archive.Handler = CommandHandler.Create(async (string d) =>
            {
                Configure(d);
                await Archive(d);
            });

            var delete = new Command("delete", "Deletes the task on line ITEM# in todo.txt. If TERM specifiedeletes only TERM from the task.");
            delete.AddArgument(new Argument<int>("item"));
            delete.AddArgument(new Argument<string>("term", () => null));
            delete.AddAlias("rm");
            delete.Handler = CommandHandler.Create(async (int item, string term, bool f, string d) =>
            {
                Configure(d);
                await Delete(item, term, f);
            });

            var depri = new Command("depri", "Deprioritizes (removes the priority) from the task(s) on line ITEM# in todo.txt.");
            depri.AddAlias("dp");
            depri.AddArgument(new Argument<int[]>("items", () => new int[] { }));
            depri.Handler = CommandHandler.Create(async (int[] items, string d) => 
            {
                Configure(d);
                await Deprioritize(items);
            });

            var @do = new Command("do", "Marks task(s) on line ITEM# as done in todo.txt.");
            @do.AddArgument(new Argument<int[]>("items", () => new int[] { }));
            @do.Handler = CommandHandler.Create(async (int[] items, bool a, string d) =>
            {
                Configure(d);
                await Complete(items, a);
            });

            var list = new Command("list", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).");
            list.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            list.AddAlias("ls");
            list.Handler = CommandHandler.Create(async (string[] terms, string d) =>
            {
                Configure(d);
                await List(terms);
            });

            var listall = new Command("listall", "Displays all the lines in todo.txt AND done.txt that contain TERM(s) sorted by priority with line numbers. Hides all tasks that contain TERM(s) preceded by a minus sign(i.e. -TERM). If no TERM specified, lists entire todo.txt AND done.txt concatenated and sorted.");
            listall.AddAlias("lsa");
            listall.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listall.Handler = CommandHandler.Create(async (string[] terms, string d) =>
            {
                Configure(d);
                await ListAll(terms);
            });

            var listcon = new Command("listcon", "Lists all the task contexts that start with the @ sign in todo.txt. If TERM specified, considers only tasks that contain TERM(s).");
            listcon.AddAlias("lsc");
            listcon.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listcon.Handler = CommandHandler.Create(async (string[] terms, string d) =>
            {
                Configure(d);
                await ListContexts(terms);
            });

            var listfile = new Command("listfile", "Displays all tasks that contain TERM(s) sorted by priority with line numbers. Each task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. -TERM).");
            listfile.AddAlias("lf");
            listfile.AddArgument(new Argument<string>("filename"));
            listfile.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listfile.Handler = CommandHandler.Create(async (string filename, string[] terms, string d) =>
            {
                Configure(d);
                await ListFile(filename, terms);
            });

            var listpri = new Command("listpri", "Displays all tasks prioritized PRIORITIES. PRIORITIES can be a single one (A) or a range (A-C). If no PRIORITIES specified, lists all prioritized tasks. If TERM specified, lists only prioritized tasks that contain TERM(s). Hides all tasks that contain TERM(s) preceded by a minus sign(i.e. -TERM).");
            listpri.AddAlias("lsp");
            listpri.AddArgument(new Argument<string>("priorities", () => "A-Z"));
            listpri.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listpri.Handler = CommandHandler.Create(async (string priorities, string[] terms, string d) =>
            {
                Configure(d);
                await ListPriorities(priorities, terms);
            });

            var listproj = new Command("listproj", "Lists all the projects (terms that start with a + sign) in todo.txt. If TERM specified, considers only tasks that contain TERM(s).");
            listproj.AddAlias("lspj");
            listproj.AddArgument(new Argument<string[]>("terms", () => new string[] { }));
            listproj.Handler = CommandHandler.Create(async (string[] terms, string d) =>
            {
                Configure(d);
                await ListProjects(terms);
            });

            var prepend = new Command("prepend", "Adds TEXT TO PREPEND to the beginning of the task on line ITEM#.");
            prepend.AddArgument(new Argument<int>("item"));
            prepend.AddArgument(new Argument<string>("text"));
            prepend.AddAlias("prep");
            prepend.Handler = CommandHandler.Create(async (int item, string text, string d) =>
            {
                Configure(d);
                await Prepend(item, text);
            });

            var pri = new Command("pri", "Adds PRIORITY to task on line ITEM#. If the task is already prioritized, replaces current priority with new PRIORITY. PRIORITY must be a letter between A and Z.");
            pri.AddAlias("p");
            pri.AddArgument(new Argument<int>("item"));
            pri.AddArgument(new Argument<char>("priority"));
            pri.Handler = CommandHandler.Create(async (int item, char priority, string d) =>
            {
                Configure(d);
                await AddPriority(item, priority);
            });

            var replace = new Command("replace", "Replaces task on line ITEM# with UPDATED TODO.");
            replace.AddArgument(new Argument<int>("item"));
            replace.AddArgument(new Argument<string>("text"));
            replace.Handler = CommandHandler.Create(async (int item, string text, string d) =>
            {
                Configure(d);
                await Replace(item, text);
            });

            var root = new RootCommand("todo")
            {
                add,
                addm,
                addTo,
                append,
                archive,
                delete,
                depri,
                @do,
                list,
                listall,
                listcon,
                listfile,
                listpri,
                listproj,
                prepend,
                pri,
                replace
            };

            root.AddOption(new Option<bool>("-a", "Don't auto-archive tasks automatically on completion"));
            root.AddOption(new Option<string>("-d", "Use a configuration file other than the default ~/.todo/config"));
            root.AddOption(new Option<bool>("-f", "Forces actions without confirmation or interactive input."));
            root.AddOption(new Option<bool>("-t", "Prepend the current date to a task automatically when it's added."));

            return root;
        }
    }
}

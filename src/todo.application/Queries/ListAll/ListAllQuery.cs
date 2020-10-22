using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Queries.List
{
    public sealed class ListAllQuery : IRequest<ListAllResponse>
    {
        private string[] _terms;

        public string[] Terms { get => _terms ?? new string[0]; set => _terms = value; }
    }

    public sealed class ListAllQueryHandler : IRequestHandler<ListAllQuery, ListAllResponse>
    {
        private readonly ITaskFile _taskFile;
        private readonly ITaskConfiguration _config;

        public ListAllQueryHandler(ITaskFile taskFile, ITaskConfiguration config)
        {
            _taskFile = taskFile;
            _config = config;
        }

        public Task<ListAllResponse> Handle(ListAllQuery request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(_config.TodoFile);
            var done = _taskFile.LoadTasks(_config.DoneFile).Select(t => new TaskItem(t.Text, 0)).ToList();    // Set line numbers to 0 for done
            IEnumerable<TaskItem> taskSearch = tasks;
            IEnumerable<TaskItem> doneSearch = done;
            foreach (var term in request.Terms)
            {
                if (term.StartsWith("-") && term.Length > 1)
                {
                    taskSearch = taskSearch.Where(t => !t.Description.Contains(term.Substring(1)));
                    doneSearch = doneSearch.Where(t => !t.Description.Contains(term.Substring(1)));
                }
                else
                {
                    taskSearch = taskSearch.Where(t => t.Description.Contains(term));
                    doneSearch = doneSearch.Where(t => t.Description.Contains(term));
                }
            }
            taskSearch = taskSearch.OrderBy(t => t.Priority ?? '[');

            var all = taskSearch.ToList();
            all.AddRange(doneSearch.ToList());

            return Task.FromResult(new ListAllResponse { Tasks = all, ShownTasks = taskSearch.Count(), TotalTasks = tasks.Count, ShownDone = doneSearch.Count(), TotalDone = done.Count });
        }
    }
}

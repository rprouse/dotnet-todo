using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Queries.List
{
    public sealed class ListTasksQuery : IRequest<ListTaskResponse>
    {
        private string[] _terms;

        public string[] Terms { get => _terms ?? new string[0]; set => _terms = value; }
    }

    public sealed class ListTasksQueryHandler : IRequestHandler<ListTasksQuery, ListTaskResponse>
    {
        private readonly ITaskFile _taskFile;

        public ListTasksQueryHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<ListTaskResponse> Handle(ListTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(StandardFilenames.Todo);
            int count = tasks.Count();
            IEnumerable<TaskItem> search = tasks;
            foreach (var term in request.Terms)
            {
                if (term.StartsWith("-") && term.Length > 1)
                    search = search.Where(t => !t.Description.Contains(term.Substring(1)));
                else
                    search = search.Where(t => t.Description.Contains(term));
            }
            search = search.OrderBy(t => t.Priority ?? '[');

            return Task.FromResult(new ListTaskResponse { Tasks = search.ToList(), TotalTasks = tasks.Count });
        }
    }
}

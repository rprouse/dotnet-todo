using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Queries.List
{
    public sealed class ListTasksQuery : IRequest<ListTaskResponse>
    {
        public string[] Terms { get; set; }
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
            var tasks = _taskFile.LoadTasks();
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

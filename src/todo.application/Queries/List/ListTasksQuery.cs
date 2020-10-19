using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Queries.List
{
    public sealed class ListTasksQuery : IRequest<ListTasksResponse>
    {
        public string Filename;

        private string[] _terms;

        public string[] Terms { get => _terms ?? new string[0]; set => _terms = value; }
    }

    public sealed class ListTasksQueryHandler : IRequestHandler<ListTasksQuery, ListTasksResponse>
    {
        private readonly ITaskFile _taskFile;

        public ListTasksQueryHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<ListTasksResponse> Handle(ListTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(request.Filename);
            IEnumerable<TaskItem> search = tasks;
            foreach (var term in request.Terms)
            {
                if (term.StartsWith("-") && term.Length > 1)
                    search = search.Where(t => !t.Description.Contains(term.Substring(1)));
                else
                    search = search.Where(t => t.Description.Contains(term));
            }
            search = search.OrderBy(t => t.Priority ?? '[');

            return Task.FromResult(new ListTasksResponse { Tasks = search.ToList(), TotalTasks = tasks.Count });
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Queries;

public sealed class ListPriorityQuery : IRequest<ListPriorityResponse>
{
    public string Priorities { get; set; }

    private string[] _terms;

    public string[] Terms { get => _terms ?? new string[0]; set => _terms = value; }
}

public sealed class ListPriorityQueryHandler : IRequestHandler<ListPriorityQuery, ListPriorityResponse>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public ListPriorityQueryHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<ListPriorityResponse> Handle(ListPriorityQuery request, CancellationToken cancellationToken)
    {
        char highestPriority = 'A';
        char lowestPriority = 'Z';
        if (!string.IsNullOrWhiteSpace(request.Priorities))
        {
            if (request.Priorities.Length == 1 && char.IsLetter(request.Priorities[0]))
            {
                highestPriority = lowestPriority = char.ToUpper(request.Priorities[0]);
            }
            else if (request.Priorities.Length == 3 &&
                    char.IsLetter(request.Priorities[0]) &&
                    char.IsLetter(request.Priorities[2]))
            {
                highestPriority = char.ToUpper(request.Priorities[0]);
                lowestPriority = char.ToUpper(request.Priorities[2]);
            }
            else
            {
                // Make sure we don't return anything
                highestPriority = ']';
                highestPriority = '[';
            }
        }

        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        IEnumerable<TaskItem> search = tasks
            .Where(t => t.Priority.HasValue &&
                   t.Priority >= highestPriority &&
                   t.Priority <= lowestPriority);
        foreach (var term in request.Terms)
        {
            if (term.StartsWith("-") && term.Length > 1)
                search = search.Where(t => !t.Description.Contains(term.Substring(1)));
            else
                search = search.Where(t => t.Description.Contains(term));
        }
        search = search.OrderBy(t => t.Priority ?? '[');

        return Task.FromResult(new ListPriorityResponse { Tasks = search.ToList(), TotalTasks = tasks.Count });
    }
}

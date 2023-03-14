using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Queries;

public class ListProjectsQuery : IRequest<string[]>
{

    private string[] _terms;

    public string[] Terms { get => _terms ?? new string[0]; set => _terms = value; }
}

public sealed class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, string[]>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public ListProjectsQueryHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<string[]> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
    {
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        string[] positiveTerms = request.Terms
            .Where(t => t.StartsWith("+"))
            .ToArray();
        string[] negativeTerms = request.Terms
            .Where(t => t.StartsWith("-+"))
            .Select(t => t.Substring(1))
            .ToArray();
        IEnumerable<string> search = tasks
            .SelectMany(t => t.ProjectTags)
            .OrderBy(p => p)
            .Distinct();
        if (positiveTerms.Length > 0)
            search = search.Where(t => positiveTerms.Contains(t));
        if (negativeTerms.Length > 0)
            search = search.Where(t => !negativeTerms.Contains(t));

        return Task.FromResult(search.ToArray());
    }
}

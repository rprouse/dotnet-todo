using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands;

public sealed class DeprioritizeCommand : IRequest<IList<TaskItem>>
{
    public int[] ItemNumbers { get; set; }
}

internal sealed class DeprioritizeCommandHandler : IRequestHandler<DeprioritizeCommand, IList<TaskItem>>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public DeprioritizeCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<IList<TaskItem>> Handle(DeprioritizeCommand request, CancellationToken cancellationToken)
    {
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        IList<TaskItem> updated = new List<TaskItem>();
        foreach (int item in request.ItemNumbers)
        {
            var task = tasks.FirstOrDefault(t => t.LineNumber == item);
            if (task != null)
            {
                task.Priority = null;
                updated.Add(task);
            }
        }
        if (updated.Any())
        {
            _taskFile.Clear(_config.TodoFile);
            foreach (var t in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTo(_config.TodoFile, t.ToString());
            }
        }
        return Task.FromResult(updated);
    }
}

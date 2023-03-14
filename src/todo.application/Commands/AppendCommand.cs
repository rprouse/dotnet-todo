using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands;

public sealed class AppendCommand : IRequest<TaskItem>
{
    public int ItemNumber { get; set; }
    public string Text { get; set; }
}

internal sealed class AppendCommandHandler : IRequestHandler<AppendCommand, TaskItem>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public AppendCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<TaskItem> Handle(AppendCommand request, CancellationToken cancellationToken)
    {
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        var task = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
        if (task != null)
        {
            task.Description = task.Description + $" {request.Text.Trim()}";
            _taskFile.Clear(_config.TodoFile);
            foreach (var t in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTo(_config.TodoFile, t.ToString());
            }
        }
        return Task.FromResult(task);
    }
}

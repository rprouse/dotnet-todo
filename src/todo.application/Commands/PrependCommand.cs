using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands;

public sealed class PrependCommand : IRequest<TaskItem>
{
    public int ItemNumber { get; set; }
    public string Text { get; set; }
}

public sealed class PrependCommandHandler : IRequestHandler<PrependCommand, TaskItem>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public PrependCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<TaskItem> Handle(PrependCommand request, CancellationToken cancellationToken)
    {
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        var task = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
        if (task != null)
        {
            task.Description = $"{request.Text.Trim()} " + task.Description;
            _taskFile.Clear(_config.TodoFile);
            foreach (var t in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTo(_config.TodoFile, t.ToString());
            }
        }
        return Task.FromResult(task);
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands;

public sealed class ArchiveTasksCommand : IRequest<IList<TaskItem>>
{
}

public sealed class ArchiveTasksCommandHandler : IRequestHandler<ArchiveTasksCommand, IList<TaskItem>>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public ArchiveTasksCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<IList<TaskItem>> Handle(ArchiveTasksCommand request, CancellationToken cancellationToken)
    {
        IList<TaskItem> completed = new List<TaskItem>();
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        _taskFile.Clear(_config.TodoFile);
        foreach (var task in tasks)
        {
            if (task.Completed)
            {
                _taskFile.AppendTo(_config.DoneFile, task.ToString());
                completed.Add(task);
            }
            else
            {
                _taskFile.AppendTo(_config.TodoFile, task.ToString());
            }
        }
        return Task.FromResult(completed);
    }
}

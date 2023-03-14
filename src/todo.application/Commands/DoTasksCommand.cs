using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands;

public sealed class DoTasksCommand : IRequest<IList<TaskItem>>
{
    private int[] _itemNumbers;

    public int[] ItemNumbers { get => _itemNumbers ?? new int[0]; set => _itemNumbers = value; }
    public bool DontArchive { get; set; }
}

internal sealed class DoTasksCommandHandler : IRequestHandler<DoTasksCommand, IList<TaskItem>>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public DoTasksCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<IList<TaskItem>> Handle(DoTasksCommand request, CancellationToken cancellationToken)
    {
        IList<TaskItem> completed = new List<TaskItem>();
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        foreach (uint taskNum in request.ItemNumbers)
        {
            var task = tasks.FirstOrDefault(t => t.LineNumber == taskNum);
            if (task != null)
            {
                task.Completed = true;
                completed.Add(task);
            }
        }
        _taskFile.Clear(_config.TodoFile);
        foreach (var task in tasks.OrderBy(t => t.LineNumber))
        {
            if (request.DontArchive)
            {
                _taskFile.AppendTo(_config.TodoFile, task.ToString());
            }
            else if (!request.ItemNumbers.Contains(task.LineNumber))
            {
                _taskFile.AppendTo(_config.TodoFile, task.ToString());
            }
            else
            {
                _taskFile.AppendTo(_config.DoneFile, task.ToString());
            }
        }

        return Task.FromResult(completed);
    }
}

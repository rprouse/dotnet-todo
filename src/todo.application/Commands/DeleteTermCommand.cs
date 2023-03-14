using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands;

public sealed class DeleteTermCommand : IRequest<DeleteTermResult>
{
    public int ItemNumber { get; set; }

    public string Term { get; set; }
}

internal sealed class DeleteTermCommandHandler : IRequestHandler<DeleteTermCommand, DeleteTermResult>
{
    private readonly ITaskFile _taskFile;
    private readonly ITaskConfiguration _config;

    public DeleteTermCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
    {
        _taskFile = taskFile;
        _config = config;
    }

    public Task<DeleteTermResult> Handle(DeleteTermCommand request, CancellationToken cancellationToken)
    {
        var tasks = _taskFile.LoadTasks(_config.TodoFile);
        var task = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
        if (task is null)
        {
            return Task.FromResult(new DeleteTermResult { Success = false });
        }

        string text = task.Text.Replace($" {request.Term}", "");
        if (text.Length == task.Text.Length)
        {
            return Task.FromResult(new DeleteTermResult { Success = false });
        }

        tasks.Remove(task);
        task = new TaskItem(text, task.LineNumber);
        tasks.Add(task);

        _taskFile.Clear(_config.TodoFile);
        foreach (var t in tasks.OrderBy(t => t.LineNumber))
        {
            _taskFile.AppendTo(_config.TodoFile, t.ToString());
        }
        return Task.FromResult(new DeleteTermResult { Task = task, Success = true });
    }
}

public class DeleteTermResult
{
    public bool Success { get; set; }

    public TaskItem Task { get; set; }
}

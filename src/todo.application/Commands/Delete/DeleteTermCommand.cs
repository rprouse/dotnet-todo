using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands.Delete
{
    public class DeleteTermCommand : IRequest<DeleteTermResult>
    {
        public int ItemNumber { get; set; }

        public string Term { get; set; }
    }

    public class DeleteTermCommandHandler : IRequestHandler<DeleteTermCommand, DeleteTermResult>
    {
        private readonly ITaskFile _taskFile;

        public DeleteTermCommandHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<DeleteTermResult> Handle(DeleteTermCommand request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(StandardFilenames.Todo);
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

            _taskFile.Clear(StandardFilenames.Todo);
            foreach (var t in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTo(StandardFilenames.Todo, t.ToString());
            }
            return Task.FromResult(new DeleteTermResult { Task = task, Success = true });
        }
    }

    public class DeleteTermResult
    {
        public bool Success { get; set; }

        public TaskItem Task { get; set; }
    }
}

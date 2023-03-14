using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands
{
    public sealed class ReplaceCommand : IRequest<TaskItem>
    {
        public int ItemNumber { get; set; }
        public string Text { get; set; }
    }

    public sealed class ReplaceCommandHandler : IRequestHandler<ReplaceCommand, TaskItem>
    {
        private readonly ITaskFile _taskFile;
        private readonly ITaskConfiguration _config;

        public ReplaceCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
        {
            _taskFile = taskFile;
            _config = config;
        }

        public Task<TaskItem> Handle(ReplaceCommand request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(_config.TodoFile);
            var task = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            if (task != null)
            {
                var newTask = new TaskItem(request.Text);
                if (newTask.Priority.HasValue)
                    task.Priority = newTask.Priority;
                if (newTask.Completed)
                    task.Completed = true;
                if (newTask.CompletionDate.HasValue)
                    task.CompletionDate = newTask.CompletionDate;
                if (newTask.CreationDate.HasValue)
                    task.CreationDate = newTask.CreationDate;

                task.Description = newTask.Description;

                _taskFile.Clear(_config.TodoFile);
                foreach (var t in tasks.OrderBy(t => t.LineNumber))
                {
                    _taskFile.AppendTo(_config.TodoFile, t.ToString());
                }
            }
            return Task.FromResult(task);
        }
    }
}

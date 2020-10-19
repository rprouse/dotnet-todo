using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands.Archive
{
    public sealed class ArchiveTasksCommand : IRequest<IList<TaskItem>>
    {
    }

    public sealed class ArchiveTasksCommandHandler : IRequestHandler<ArchiveTasksCommand, IList<TaskItem>>
    {
        private readonly ITaskFile _taskFile;

        public ArchiveTasksCommandHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<IList<TaskItem>> Handle(ArchiveTasksCommand request, CancellationToken cancellationToken)
        {
            IList<TaskItem> completed = new List<TaskItem>();
            var tasks = _taskFile.LoadTasks(StandardFilenames.Todo);
            _taskFile.Clear(StandardFilenames.Todo);
            foreach (var task in tasks)
            {
                if (task.Completed)
                {
                    _taskFile.AppendTo(StandardFilenames.Done, task.ToString());
                    completed.Add(task);
                }
                else
                {
                    _taskFile.AppendTo(StandardFilenames.Todo, task.ToString());
                }
            }
            return Task.FromResult(completed);
        }
    }
}

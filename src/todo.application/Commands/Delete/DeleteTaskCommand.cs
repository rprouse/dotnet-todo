using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands.Delete
{
    public class DeleteTaskCommand : IRequest<int>
    {
        public int ItemNumber { get; set; }
    }

    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, int>
    {
        private readonly ITaskFile _taskFile;

        public DeleteTaskCommandHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<int> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(StandardFilenames.Todo);
            var delTask = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            if (delTask is null)
            {
                return Task.FromResult(0);
            }
            tasks.Remove(delTask);
            _taskFile.Clear(StandardFilenames.Todo);
            foreach (var task in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTo(StandardFilenames.Todo, task.ToString());
            }
            return Task.FromResult(delTask.LineNumber);
        }
    }
}

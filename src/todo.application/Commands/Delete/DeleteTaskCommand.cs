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
        private readonly ITaskConfiguration _config;

        public DeleteTaskCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
        {
            _taskFile = taskFile;
            _config = config;
        }

        public Task<int> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(_config.TodoFile);
            var delTask = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            if (delTask is null)
            {
                return Task.FromResult(0);
            }
            tasks.Remove(delTask);
            _taskFile.Clear(_config.TodoFile);
            foreach (var task in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTo(_config.TodoFile, task.ToString());
            }
            return Task.FromResult(delTask.LineNumber);
        }
    }
}

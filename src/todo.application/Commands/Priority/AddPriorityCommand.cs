using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands.Priority
{
    public class AddPriorityCommand : IRequest<TaskItem>
    {
        public int ItemNumber { get; set; }

        public char Priority { get; set; }
    }

    public class AddPriorityCommandHandler : IRequestHandler<AddPriorityCommand, TaskItem>
    {
        private readonly ITaskFile _taskFile;
        private readonly ITaskConfiguration _config;

        public AddPriorityCommandHandler(ITaskFile taskFile, ITaskConfiguration config)
        {
            _taskFile = taskFile;
            _config = config;
        }

        public Task<TaskItem> Handle(AddPriorityCommand request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(_config.TodoFile);
            var task = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            if (task != null)
            {
                task.Priority = request.Priority;
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

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

        public AddPriorityCommandHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<TaskItem> Handle(AddPriorityCommand request, CancellationToken cancellationToken)
        {
            var tasks = _taskFile.LoadTasks(StandardFilenames.Todo);
            var task = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            if (task != null)
            {
                task.Priority = request.Priority;
                _taskFile.Clear(StandardFilenames.Todo);
                foreach (var t in tasks.OrderBy(t => t.LineNumber))
                {
                    _taskFile.AppendTo(StandardFilenames.Todo, task.ToString());
                }
            }
            return Task.FromResult(task);
        }
    }
}

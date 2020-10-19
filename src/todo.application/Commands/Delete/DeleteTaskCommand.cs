using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            IList<TaskItem> completed = new List<TaskItem>();
            var tasks = _taskFile.LoadTasks();
            var delTask = tasks.FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            if (delTask is null)
            {
                return Task.FromResult(0);
            }
            tasks.Remove(delTask);
            _taskFile.ClearTodo();
            foreach (var task in tasks.OrderBy(t => t.LineNumber))
            {
                _taskFile.AppendTodo(task.ToString());
            }
            return Task.FromResult(delTask.LineNumber);
        }
    }
}

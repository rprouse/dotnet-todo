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

namespace Alteridem.Todo.Application.Queries.IndividualTask
{
    public sealed class TaskQuery : IRequest<TaskItem>
    {
        public int ItemNumber { get; set; }
    }

    public sealed class TaskQueryHandler : IRequestHandler<TaskQuery, TaskItem>
    {
        private readonly ITaskFile _taskFile;
        private readonly ITaskConfiguration _config;

        public TaskQueryHandler(ITaskFile taskFile, ITaskConfiguration config)
        {
            _taskFile = taskFile;
            _config = config;
        }

        public Task<TaskItem> Handle(TaskQuery request, CancellationToken cancellationToken)
        {
            var task = _taskFile.LoadTasks(_config.TodoFile).FirstOrDefault(t => t.LineNumber == request.ItemNumber);
            return Task.FromResult(task);
        }
    }
}

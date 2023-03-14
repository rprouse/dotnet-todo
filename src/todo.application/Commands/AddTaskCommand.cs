using System;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Domain.Interfaces;
using MediatR;

namespace Alteridem.Todo.Application.Commands
{
    public sealed class AddTaskCommand : IRequest<TaskItem>
    {
        public string Filename { get; set; }

        public string Task { get; set; }

        public bool AddCreationDate { get; set; }
    }

    public sealed class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, TaskItem>
    {
        private readonly ITaskFile _taskFile;

        public AddTaskCommandHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<TaskItem> Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            var task = new TaskItem(request.Task);
            if (request.AddCreationDate) task.CreationDate = DateTime.Now.Date;
            var taskStr = task.ToString();

            _taskFile.AppendTo(request.Filename, taskStr);

            task.LineNumber = _taskFile.LoadTasks(request.Filename).Count;

            return Task.FromResult(task);
        }
    }
}

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

namespace Alteridem.Todo.Application.Commands.Do
{
    public class DoTasksCommand : IRequest<IList<TaskItem>>
    {
        private int[] _itemNumbers;

        public int[] ItemNumbers { get => _itemNumbers ?? new int[0]; set => _itemNumbers = value; }
        public bool DontArchive { get; set; }
    }

    public class DoTasksCommandHandler : IRequestHandler<DoTasksCommand, IList<TaskItem>>
    {
        private readonly ITaskFile _taskFile;

        public DoTasksCommandHandler(ITaskFile taskFile)
        {
            _taskFile = taskFile;
        }

        public Task<IList<TaskItem>> Handle(DoTasksCommand request, CancellationToken cancellationToken)
        {
            IList<TaskItem> completed = new List<TaskItem>();
            var tasks = _taskFile.LoadTasks(StandardFilenames.Todo);
            foreach(uint taskNum in request.ItemNumbers)
            {
                var task = tasks.FirstOrDefault(t => t.LineNumber == taskNum);
                if(task != null)
                {
                    task.Completed = true;
                    completed.Add(task);
                }
            }
            _taskFile.Clear(StandardFilenames.Todo);
            foreach(var task in tasks.OrderBy(t => t.LineNumber))
            {
                if(request.DontArchive)
                {
                    _taskFile.AppendTo(StandardFilenames.Todo, task.ToString());
                }
                else if(!request.ItemNumbers.Contains(task.LineNumber))
                {
                    _taskFile.AppendTo(StandardFilenames.Todo, task.ToString());
                }
                else
                {
                    _taskFile.AppendTo(StandardFilenames.Done, task.ToString());
                }
            }

            return Task.FromResult(completed);
        }
    }
}

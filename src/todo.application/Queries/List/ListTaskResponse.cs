using System.Collections.Generic;
using Alteridem.Todo.Domain.Entities;

namespace Alteridem.Todo.Application.Queries.List
{
    public sealed class ListTaskResponse
    {
        public IList<TaskItem> Tasks { get; set; }

        public int TotalTasks { get; set; }
    }
}

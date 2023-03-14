using System.Collections.Generic;
using Alteridem.Todo.Domain.Entities;

namespace Alteridem.Todo.Application.Queries;

public sealed class ListTasksResponse
{
    public IList<TaskItem> Tasks { get; set; }

    public int TotalTasks { get; set; }
}

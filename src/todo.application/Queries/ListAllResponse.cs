using System.Collections.Generic;
using Alteridem.Todo.Domain.Entities;

namespace Alteridem.Todo.Application.Queries;

public sealed class ListAllResponse
{
    public IList<TaskItem> Tasks { get; set; }

    public int ShownTasks { get; set; }

    public int TotalTasks { get; set; }

    public int ShownDone { get; set; }

    public int TotalDone { get; set; }
}

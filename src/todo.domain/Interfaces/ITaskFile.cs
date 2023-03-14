using System.Collections.Generic;
using Alteridem.Todo.Domain.Entities;

namespace Alteridem.Todo.Domain.Interfaces;

public interface ITaskFile
{
    /// <summary>
    /// Appends a line to the file in the todo.txt directory
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="line"></param>
    void AppendTo(string filename, string line);

    /// <summary>
    /// Clears the given file
    /// </summary>
    void Clear(string filename);

    /// <summary>
    /// Loads all tasks in the given file
    /// </summary>
    /// <returns></returns>
    IList<TaskItem> LoadTasks(string filename);
}

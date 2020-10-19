using System;
using System.Collections.Generic;
using System.Text;
using Alteridem.Todo.Domain.Entities;

namespace Alteridem.Todo.Domain.Interfaces
{
    public interface ITaskFile
    {
        /// <summary>
        /// Appends a line to the todo file
        /// </summary>
        /// <param name="line"></param>
        void AppendTodo(string line);

        /// <summary>
        /// Appends a line to the done file
        /// </summary>
        /// <param name="line"></param>
        void AppendDone(string line);

        /// <summary>
        /// Clears the given file
        /// </summary>
        void ClearTodo();

        /// <summary>
        /// Loads all tasks in the file
        /// </summary>
        /// <returns></returns>
        IList<TaskItem> LoadTasks();
    }
}

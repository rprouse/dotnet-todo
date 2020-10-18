using System;
using System.Collections.Generic;
using System.Text;
using Alteridem.Todo.Domain.Entities;

namespace Alteridem.Todo.Domain.Interfaces
{
    public interface ITaskFile
    {
        /// <summary>
        /// Appends a line to the file
        /// </summary>
        /// <param name="line"></param>
        void AppendLine(string line);

        /// <summary>
        /// Loads all tasks in the file
        /// </summary>
        /// <returns></returns>
        IList<TaskItem> LoadTasks();
    }
}

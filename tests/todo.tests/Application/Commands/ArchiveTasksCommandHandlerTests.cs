using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Archive;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class ArchiveTasksCommandHandlerTests
    {
        TaskFileMock _taskFile;
        ArchiveTasksCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _taskFile.TaskLines = new List<string>
            {
                "2020-10-16 This is something to do!",
                "2020-10-16 This is something to do with @context and a +project!",
                "x 2020-10-18 2020-10-14 This is high priority",
                "",
                "(D) This is lower",
                "x 2020-10-18 This is in between with @context",
                "(Z) Very low priority with +project",
                "(E) Midrange",
            };
            _taskFile.DoneLines = new List<string>
            {
                "x 2020-09-27 Remember mom's birthday @home",
                "x 2020-08-12 2020-07-23 Pay bills @home"
            };
            _handler = new ArchiveTasksCommandHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task ArchiveTasksCommandHandler_ReturnsArchivedTasks()
        {
            var command = new ArchiveTasksCommand();
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().HaveCount(2);
        }

        [Test]
        public async Task ArchiveTasksCommandHandler_RemovesTasksAndBlankLinesFromTodo()
        {
            var command = new ArchiveTasksCommand();
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(5);
        }

        [Test]
        public async Task ArchiveTasksCommandHandler_MovesCompletedTasksToDone()
        {
            var command = new ArchiveTasksCommand();
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.DoneLines.Should().HaveCount(4);
        }
    }
}

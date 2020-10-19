using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Delete;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class DeleteTasksCommandHandlerTests
    {
        TaskFileMock _taskFile;
        DeleteTaskCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _taskFile.TaskLines = new List<string>
            {
                "2020-10-16 This is something to do!",
                "2020-10-16 This is something to do with @context and a +project!",
                "(A) This is high priority",
                "",
                "(D) This is lower",
                "(B) This is in between with @context",
                "(Z) Very low priority with +project",
                "(E) Midrange",
};
            _handler = new DeleteTaskCommandHandler(_taskFile);
        }

        [Test]
        public async Task DeleteTaskCommandHandler_ReturnsLineNumberOfDeletedTask()
        {
            var command = new DeleteTaskCommand { ItemNumber = 3 };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().Be(3);
        }

        [Test]
        public async Task DeleteTaskCommandHandler_RemovesTaskAndBlankLinesFromFile()
        {
            var command = new DeleteTaskCommand { ItemNumber = 3 };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(6);
            _taskFile.TaskLines[2].Should().Be("(D) This is lower");
        }

        [Test]
        public async Task DeleteTaskCommandHandler_ReturnsZeroForNonExistantTask()
        {
            var command = new DeleteTaskCommand { ItemNumber = 10 };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().Be(0);
        }

        [Test]
        public async Task DeleteTaskCommandHandler_ReturnsZeroForBlankLine()
        {
            var command = new DeleteTaskCommand { ItemNumber = 4 };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().Be(0);
        }
    }
}

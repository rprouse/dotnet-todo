using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class AddPriorityCommandHandlerTests
    {
        TaskFileMock _taskFile;
        AddPriorityCommandHandler _handler;

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
            _handler = new AddPriorityCommandHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task AddPriorityCommandHandler_RemovesBlankLinesFromTodoFile()
        {
            var command = new AddPriorityCommand { ItemNumber = 2, Priority = 'A' };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(7);
        }

        [Test]
        public async Task AddPriorityCommandHandler_SetsPriority()
        {
            var command = new AddPriorityCommand { ItemNumber = 2, Priority = 'A' };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.Priority.Should().Be('A');
        }

        [Test]
        public async Task AddPriorityCommandHandler_PriorityIsWrittenToFile()
        {
            var command = new AddPriorityCommand { ItemNumber = 2, Priority = 'A' };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[1].Should().StartWith("(A) ");
        }

        [Test]
        public async Task AddPriorityCommandHandler_MissingTask_ReturnsNull()
        {
            var command = new AddPriorityCommand { ItemNumber = 12, Priority = 'A' };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().BeNull();
        }
    }
}

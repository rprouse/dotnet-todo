using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class DeprioritizeCommandHandlerTests
    {
        TaskFileMock _taskFile;
        DeprioritizeCommandHandler _handler;

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
            _handler = new DeprioritizeCommandHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task DeprioritizeCommandHandler_RemovesBlankLinesFromTodoFile()
        {
            var command = new DeprioritizeCommand { ItemNumbers = new[] { 2 } };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(7);
        }

        [Test]
        public async Task DeprioritizeCommandHandler_RemovesPriority()
        {
            var command = new DeprioritizeCommand { ItemNumbers = new[] { 3, 5, 6, 7, 8 } };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            result.First().Priority.Should().BeNull();
            result.Last().Priority.Should().BeNull();
        }

        [Test]
        public async Task DeprioritizeCommandHandler_RemovesMultiplePriorities()
        {
            var command = new DeprioritizeCommand { ItemNumbers = new[] { 3 } };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Priority.Should().BeNull();
        }

        [Test]
        public async Task DeprioritizeCommandHandler_DepriorityIsWrittenToFile()
        {
            var command = new DeprioritizeCommand { ItemNumbers = new[] { 3 } };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[2].Should().Be("This is high priority");
        }

        [Test]
        public async Task DeprioritizeCommandHandler_MissingTask_ReturnsEmptyList()
        {
            var command = new DeprioritizeCommand { ItemNumbers = new[] { 12 } };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }
    }
}

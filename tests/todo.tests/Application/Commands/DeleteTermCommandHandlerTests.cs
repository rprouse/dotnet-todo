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
    public class DeleteTermCommandHandlerTests
    {
        TaskFileMock _taskFile;
        DeleteTermCommandHandler _handler;

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
                "(B) This @context is in between with @context",
                "(Z) Very low priority with +project",
                "(E) Midrange",
            };
            _handler = new DeleteTermCommandHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task DeleteTermCommandHandler_TermFound_ReturnsUpdatedTask()
        {
            var command = new DeleteTermCommand { ItemNumber = 2, Term = "+project" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Task.Should().NotBeNull();
            result.Task.LineNumber.Should().Be(2);
            result.Task.Description.Should().Be("This is something to do with @context and a!");
        }

        [Test]
        public async Task DeleteTermCommandHandler_MultipleTermFound_ReturnsUpdatedTask()
        {
            var command = new DeleteTermCommand { ItemNumber = 6, Term = "@context" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Success.Should().BeTrue();
            result.Task.Should().NotBeNull();
            result.Task.LineNumber.Should().Be(6);
            result.Task.Description.Should().Be("This is in between with");
        }

        [Test]
        public async Task DeleteTermCommandHandler_TermNotFound_ReturnsNoTask()
        {
            var command = new DeleteTermCommand { ItemNumber = 2, Term = "+carebears" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Task.Should().BeNull();
        }

        [Test]
        public async Task DeleteTermCommandHandler_LineNotFound_ReturnsNoTask()
        {
            var command = new DeleteTermCommand { ItemNumber = 10, Term = "+project" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Success.Should().BeFalse();
            result.Task.Should().BeNull();
        }

        [Test]
        public async Task DeleteTermCommandHandler_TermFound_UpdatesTaskInFile()
        {
            var command = new DeleteTermCommand { ItemNumber = 2, Term = "+project" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[1].Should().Be("2020-10-16 This is something to do with @context and a!");
        }

        [Test]
        public async Task DeleteTermCommandHandler_MultipleTermFound_UpdatesTaskInFile()
        {
            var command = new DeleteTermCommand { ItemNumber = 6, Term = "@context" };
            _ = await _handler.Handle(command, new CancellationToken());
            // This is 4 because the blank line gets removed too
            _taskFile.TaskLines[4].Should().Be("(B) This is in between with");
        }

        [Test]
        public async Task DeleteTermCommandHandler_TermNotFound_DoesNotUpdateTaskInFile()
        {
            var command = new DeleteTermCommand { ItemNumber = 2, Term = "+carebears" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[1].Should().Be("2020-10-16 This is something to do with @context and a +project!");
        }
    }
}

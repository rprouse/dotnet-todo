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
    public class AppendCommandHandlerTest
    {
        TaskFileMock _taskFile;
        AppendCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _taskFile.TaskLines = new List<string>
            {
                "2020-10-16 This is something to do!",
                "(B) 2020-10-16 This is something to do with @context and a +project!",
                "",
                "(A) This is high priority"
            };
            _handler = new AppendCommandHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task AppendCommandHandler_RemovesBlankLinesFromTodoFile()
        {
            var command = new AppendCommand { ItemNumber = 2, Text = "Add" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(3);
        }

        [Test]
        public async Task AppendCommandHandler_AppendsText()
        {
            var command = new AppendCommand { ItemNumber = 4, Text = "Add" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.Description.Should().EndWith("priority Add");
        }

        [Test]
        public async Task AppendCommandHandler_StripsWhitespace()
        {
            var command = new AppendCommand { ItemNumber = 4, Text = " Add " };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.Description.Should().EndWith("priority Add");
        }

        [Test]
        public async Task AppendCommandHandler_AppendedIsWrittenToFile()
        {
            var command = new AppendCommand { ItemNumber = 4, Text = "Add" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[2].Should().EndWith("priority Add");
        }

        [Test]
        public async Task AppendCommandHandler_MissingTask_ReturnsNull()
        {
            var command = new AppendCommand { ItemNumber = 12, Text = "Add" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().BeNull();
        }
    }
}

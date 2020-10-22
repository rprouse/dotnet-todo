using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Prepend;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class PrependCommandHandlerTests
    {
        public class PrependCommandHandlerTest
        {
            TaskFileMock _taskFile;
            PrependCommandHandler _handler;

            [SetUp]
            public void SetUp()
            {
                _taskFile = new TaskFileMock();
                _taskFile.TaskLines = new List<string>
            {
                "2020-10-16 This is something to do!",
                "(B) 2020-10-16 This is something to do with @context and a +project!",
                "",
                "This is a todo",
                "(A) This is high priority"
            };
                _handler = new PrependCommandHandler(_taskFile, new TaskConfiguration());
            }

            [Test]
            public async Task PrependCommandHandler_RemovesBlankLinesFromTodoFile()
            {
                var command = new PrependCommand { ItemNumber = 2, Text = "Add" };
                _ = await _handler.Handle(command, new CancellationToken());
                _taskFile.TaskLines.Should().HaveCount(4);
            }

            [TestCase(1, "2020-10-16 Add This")]
            [TestCase(2, "(B) 2020-10-16 Add This")]
            [TestCase(4, "Add This is a todo")]
            [TestCase(5, "(A) Add This is high priority")]
            public async Task PrependCommandHandler_PrependsText(int line, string expected)
            {
                var command = new PrependCommand { ItemNumber = line, Text = "Add" };
                var result = await _handler.Handle(command, new CancellationToken());
                result.Should().NotBeNull();
                result.ToString().Should().StartWith(expected);
            }

            [Test]
            public async Task PrependCommandHandler_StripsWhitespace()
            {
                var command = new PrependCommand { ItemNumber = 4, Text = " Add " };
                var result = await _handler.Handle(command, new CancellationToken());
                result.Should().NotBeNull();
                result.Description.Should().StartWith("Add This");
            }

            [Test]
            public async Task PrependCommandHandler_PrependedIsWrittenToFile()
            {
                var command = new PrependCommand { ItemNumber = 4, Text = "Add" };
                _ = await _handler.Handle(command, new CancellationToken());
                _taskFile.TaskLines[2].Should().StartWith("Add This");
            }

            [Test]
            public async Task PrependCommandHandler_MissingTask_ReturnsNull()
            {
                var command = new PrependCommand { ItemNumber = 12, Text = "Add" };
                var result = await _handler.Handle(command, new CancellationToken());
                result.Should().BeNull();
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Replace;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class ReplaceCommandHandlerTests
    {
        TaskFileMock _taskFile;
        ReplaceCommandHandler _handler;

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
            _handler = new ReplaceCommandHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task ReplaceCommandHandler_RemovesBlankLinesFromTodoFile()
        {
            var command = new ReplaceCommand { ItemNumber = 2, Text = "Replace" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(4);
        }

        [TestCase(1, "2020-10-16 Replace")]
        [TestCase(2, "(B) 2020-10-16 Replace")]
        [TestCase(4, "Replace")]
        [TestCase(5, "(A) Replace")]
        public async Task ReplaceCommandHandler_ReplacesText(int line, string expected)
        {
            var command = new ReplaceCommand { ItemNumber = line, Text = "Replace" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.ToString().Should().Be(expected);
        }

        [Test]
        public async Task ReplaceCommandHandler_ReplacedIsWrittenToFile()
        {
            var command = new ReplaceCommand { ItemNumber = 4, Text = "Replace" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[2].Should().Be("Replace");
        }

        [TestCase("Replace", "(B) 2020-10-16 Replace")]
        [TestCase("(A) Replace", "(A) 2020-10-16 Replace")]
        [TestCase("2020-10-22 Replace", "(B) 2020-10-22 Replace")]
        [TestCase("(A) 2020-10-22 Replace", "(A) 2020-10-22 Replace")]
        public async Task ReplaceCommandHandler_ReplacesPartsThatAreSpecified(string replace, string expected)
        {
            var command = new ReplaceCommand { ItemNumber = 2, Text = replace };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().NotBeNull();
            result.ToString().Should().Be(expected);
        }

        [Test]
        public async Task ReplaceCommandHandler_MissingTask_ReturnsNull()
        {
            var command = new ReplaceCommand { ItemNumber = 12, Text = "Replace" };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().BeNull();
        }
    }
}

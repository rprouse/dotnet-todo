using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Do;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using MediatR;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class DoTasksCommandHandlerTests
    {
        TaskFileMock _taskFile;
        DoTasksCommandHandler _handler;

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
            _handler = new DoTasksCommandHandler(_taskFile);
        }

        [Test]
        public void DoTasksCommandHandler_HandlesNullItemNumbers()
        {
            var command = new DoTasksCommand { ItemNumbers = null };
            Assert.DoesNotThrowAsync(async () => await _handler.Handle(command, new CancellationToken()));
        }

        [Test]
        public async Task DoTasksCommandHandler_RemovesGivenTasksAndBlankLinesFromTodoFile()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 } };
            var result = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(5);
        }

        [Test]
        public async Task DoTasksCommandHandler_AddsGivenTasksToDoneFile()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 } };
            var result = await _handler.Handle(command, new CancellationToken());
            _taskFile.DoneLines.Should().HaveCount(2);
        }

        [Test]
        public async Task DoTasksCommandHandler_CompletesGivenTasks()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 } };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Where(t => t.Completed).Should().HaveCount(2);
        }

        [Test]
        public async Task DoTasksCommandHandler_WithoutArchive_RemovesBlankLinesFromTodoFile()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 }, DontArchive = true };
            var result = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines.Should().HaveCount(7);
        }

        [Test]
        public async Task DoTasksCommandHandler_WithoutArchive_MarksBlankCompletedInTodoFile()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 }, DontArchive = true };
            var result = await _handler.Handle(command, new CancellationToken());
            _taskFile.TaskLines[0].Should().StartWith("x ");
            _taskFile.TaskLines[3].Should().StartWith("x ");
        }

        [Test]
        public async Task DoTasksCommandHandler_DoesntAddsGivenTasksToDoneFile()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 }, DontArchive = true };
            var result = await _handler.Handle(command, new CancellationToken());
            _taskFile.DoneLines.Should().HaveCount(0);
        }

        [Test]
        public async Task DoTasksCommandHandler_WithoutArchive_CompletesGivenTasks()
        {
            var command = new DoTasksCommand { ItemNumbers = new int[] { 1, 5 }, DontArchive = true };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Where(t => t.Completed).Should().HaveCount(2);
            _taskFile.DoneLines.Should().HaveCount(0);
            _taskFile.TaskLines.Should().HaveCount(7);
            _taskFile.TaskLines.Where(l => l.StartsWith("x ")).Should().HaveCount(2);
        }
    }
}

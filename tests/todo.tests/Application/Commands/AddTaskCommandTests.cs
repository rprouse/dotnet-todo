using System;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands.Add;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Commands
{
    public class AddTaskCommandTests
    {
        TaskFileMock _taskFile;
        AddTaskCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _handler = new AddTaskCommandHandler(_taskFile);
        }

        [Test]
        public async Task AddTaskCommandHandler_AppendsTextToFile()
        {
            var command = new AddTaskCommand { Task = "Test string" };
            _ = await _handler.Handle(command, new CancellationToken());
            _taskFile.LineAppended.Should().Be("Test string");
        }

        [Test]
        public async Task AddTaskCommandHandler_PrependsCreationDateToFile()
        {
            var command = new AddTaskCommand { Task = "Test string", AddCreationDate = true };
            _ = await _handler.Handle(command, new CancellationToken());
            string now = DateTime.Now.Date.ToString("yyyy-MM-dd");
            _taskFile.LineAppended.Should().Be($"{now} Test string");
        }

        public async Task AddTaskCommandHandler_ReturnsLineNumber()
        {
            var command = new AddTaskCommand { Task = "Test string", AddCreationDate = true };
            TaskItem result = await _handler.Handle(command, new CancellationToken());
            result.LineNumber.Should().Be(1);
        }
    }
}

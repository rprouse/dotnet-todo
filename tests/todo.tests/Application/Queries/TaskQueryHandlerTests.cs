using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Queries.IndividualTask;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Queries
{
    public class TaskQueryHandlerTests
    {
        TaskFileMock _taskFile;
        TaskQueryHandler _handler;

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
            _handler = new TaskQueryHandler(_taskFile, new TaskConfiguration());
        }

        [Test]
        public async Task TaskQueryHandler_ExistingTask_ReturnsTask()
        {
            var command = new TaskQuery { ItemNumber = 3 };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Text.Should().Be("(A) This is high priority");
        }

        [Test]
        public async Task TaskQueryHandler_BlankLine_ReturnsNull()
        {
            var command = new TaskQuery { ItemNumber = 4 };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().BeNull();
        }

        [Test]
        public async Task TaskQueryHandler_NonExisting_ReturnsNull()
        {
            var command = new TaskQuery { ItemNumber = 10 };
            var result = await _handler.Handle(command, new CancellationToken());
            result.Should().BeNull();
        }
    }
}

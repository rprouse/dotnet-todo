using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Queries.List;
using Alteridem.Todo.Domain.Common;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Queries
{
    public class ListAllQueryHandlerTests
    {
        TaskFileMock _taskFile;
        ListAllQueryHandler _handler;

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
            _taskFile.DoneLines = new List<string>
           {
               "x One @context",
               "x Two +project"
           };
            _handler = new ListAllQueryHandler(_taskFile);
        }

        [Test]
        public void ListAllQueryHandler_HandlesNullTerms()
        {
            var command = new ListAllQuery { Terms = null };
            Assert.DoesNotThrowAsync(async () => await _handler.Handle(command, new CancellationToken()));
        }

        [Test]
        public async Task ListAllQueryHandler_ReturnsTasksInPriorityOrder()
        {
            var command = new ListAllQuery { Terms = null };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
            result.Tasks.Last().Text.Should().Be("x Two +project");
            result.ShownTasks.Should().Be(7);
            result.ShownDone.Should().Be(2);
            result.TotalTasks.Should().Be(7);
            result.TotalDone.Should().Be(2);
        }

        [Test]
        public async Task ListAllQueryHandler_ReturnsAllTasksWithoutBlanksWithoutSearchTerms()
        {
            var command = new ListAllQuery { Terms = new string[0] };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(9);
        }

        [Test]
        public async Task ListAllQueryHandler_ReturnsTasksWithSearchTerms()
        {
            var command = new ListAllQuery { Terms = new string[] { "+project" } };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(3);
            result.ShownTasks.Should().Be(2);
            result.ShownDone.Should().Be(1);
            result.TotalTasks.Should().Be(7);
            result.TotalDone.Should().Be(2);
        }

        [Test]
        public async Task ListAllQueryHandler_AndsTheQueryForMultipleSearchTerms()
        {
            var command = new ListAllQuery { Terms = new string[] { "+project", "@context" } };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(1);
            result.ShownTasks.Should().Be(1);
            result.ShownDone.Should().Be(0);
            result.TotalTasks.Should().Be(7);
            result.TotalDone.Should().Be(2);
        }

        [Test]
        public async Task ListAllQueryHandler_HandlesNegativeSearchTerms()
        {
            var command = new ListAllQuery { Terms = new string[] { "-+project" } };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(6);
            result.ShownTasks.Should().Be(5);
            result.ShownDone.Should().Be(1);
            result.TotalTasks.Should().Be(7);
            result.TotalDone.Should().Be(2);
        }

        [Test]
        public async Task ListAllQueryHandler_HandlesMultipleNegativeSearchTerms()
        {
            var command = new ListAllQuery { Terms = new string[] { "-+project", "-@context" } };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(4);
            result.ShownTasks.Should().Be(4);
            result.ShownDone.Should().Be(0);
            result.TotalTasks.Should().Be(7);
            result.TotalDone.Should().Be(2);
        }

        [Test]
        public async Task ListAllQueryHandler_HandlesNegativeAndPositiveSearchTerms()
        {
            var command = new ListAllQuery { Terms = new string[] { "-+project", "@context" } };
            ListAllResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(2);
            result.ShownTasks.Should().Be(1);
            result.ShownDone.Should().Be(1);
            result.TotalTasks.Should().Be(7);
            result.TotalDone.Should().Be(2);
        }
    }
}

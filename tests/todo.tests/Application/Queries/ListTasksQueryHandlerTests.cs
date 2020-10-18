using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Queries.List;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using Microsoft.VisualBasic.CompilerServices;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Queries
{
    public class ListTasksQueryHandlerTests
    {
        TaskFileMock _taskFile;
        ListTasksQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _taskFile.Lines = new List<string>
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
            _handler = new ListTasksQueryHandler(_taskFile);
        }

        [Test]
        public void ListTasksQueryHandler_HandlesNullTerms()
        {
            var command = new ListTasksQuery { Terms = null };
            Assert.DoesNotThrowAsync(async () => await _handler.Handle(command, new CancellationToken()));
        }

        [Test]
        public async Task ListTasksQueryHandler_ReturnsTasksInPriorityOrder()
        {
            var command = new ListTasksQuery { Terms = null };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
            result.Tasks.Last().Text.Should().Be("2020-10-16 This is something to do with @context and a +project!");
        }

        [Test]
        public async Task ListTasksQueryHandler_ReturnsAllTasksWithoutBlanksWithoutSearchTerms()
        {
            var command = new ListTasksQuery { Terms = new string[0] };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(7);
        }

        [Test]
        public async Task ListTasksQueryHandler_ReturnsTasksWithSearchTerms()
        {
            var command = new ListTasksQuery { Terms = new string[] { "+project"} };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(2);
        }

        [Test]
        public async Task ListTasksQueryHandler_AndsTheQueryForMultipleSearchTerms()
        {
            var command = new ListTasksQuery { Terms = new string[] { "+project", "@context" } };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(1);
        }

        [Test]
        public async Task ListTasksQueryHandler_HandlesNegativeSearchTerms()
        {
            var command = new ListTasksQuery { Terms = new string[] { "-+project" } };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(5);
        }

        [Test]
        public async Task ListTasksQueryHandler_HandlesMultipleNegativeSearchTerms()
        {
            var command = new ListTasksQuery { Terms = new string[] { "-+project", "-@context" } };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(4);
        }

        [Test]
        public async Task ListTasksQueryHandler_HandlesNegativeAndPositiveSearchTerms()
        {
            var command = new ListTasksQuery { Terms = new string[] { "-+project", "@context" } };
            ListTaskResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(1);
        }
    }
}

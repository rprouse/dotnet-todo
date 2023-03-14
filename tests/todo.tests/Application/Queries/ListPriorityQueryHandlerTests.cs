using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Queries;
using Alteridem.Todo.Domain.Interfaces;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Queries
{
    public class ListPriorityQueryHandlerTests
    {
        private readonly ITaskConfiguration _config = new TaskConfiguration();
        TaskFileMock _taskFile;
        ListPriorityQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _taskFile.TaskLines = new List<string>
            {
                "2020-10-16 This is something to do!",
                "2020-10-16 This is something to do with @context and a +project!",
                "(A) This is high priority",
                "(D) This is lower @context",
                "",
                "(B) This is in between with @context +project",
                "(A) This is also a high priority +project @context",
                "(Z) Very low priority with +project",
                "(B) This is secondary",
                "(E) Midrange +project",
            };
            _taskFile.OtherLines = new List<string>
            {
                "One",
                "Two"
            };
            _handler = new ListPriorityQueryHandler(_taskFile, _config);
        }

        [Test]
        public void ListPriorityQueryHandler_HandlesNullTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A", Terms = null };
            Assert.DoesNotThrowAsync(async () => await _handler.Handle(command, new CancellationToken()));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task ListPriorityQueryHandler_WithoutPriority_ReturnsAllPrioritizedTasks(string priorities)
        {
            var command = new ListPriorityQuery { Priorities = priorities, Terms = null };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(7);
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
            result.Tasks.Last().Text.Should().Be("(Z) Very low priority with +project");
        }

        [Test]
        public async Task ListPriorityQueryHandler_ReturnsMultiplePrioritiesInPriorityOrder()
        {
            var command = new ListPriorityQuery { Priorities = "A-B", Terms = null };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(4);
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
            result.Tasks.Last().Text.Should().Be("(B) This is secondary");
        }

        [Test]
        public async Task ListPriorityQueryHandler_HandlesMultipleLowercasePriorities()
        {
            var command = new ListPriorityQuery { Priorities = "a-b", Terms = null };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(4);
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
            result.Tasks.Last().Text.Should().Be("(B) This is secondary");
        }

        [Test]
        public async Task ListPriorityQueryHandler_ReturnsSinglePriority()
        {
            var command = new ListPriorityQuery { Priorities = "B", Terms = new string[0] };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(2);
            result.Tasks.First().Text.Should().StartWith("(B) ");
        }

        [Test]
        public async Task ListPriorityQueryHandler_ReturnsSingleLowercasePriority()
        {
            var command = new ListPriorityQuery { Priorities = "b", Terms = new string[0] };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(2);
            result.Tasks.First().Text.Should().StartWith("(B) ");
        }

        [Test]
        public async Task ListPriorityQueryHandler_ReturnsAllTasksWithoutBlanksWithoutSearchTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A-Z", Terms = new string[0] };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(7);
        }

        [Test]
        public async Task ListPriorityQueryHandler_ReturnsTasksWithSearchTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A", Terms = new string[] { "+project" } };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(1);
            result.Tasks.First().Text.Should().Be("(A) This is also a high priority +project @context");
        }

        [Test]
        public async Task ListPriorityQueryHandler_AndsTheQueryForMultipleSearchTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A-B", Terms = new string[] { "+project", "@context" } };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(2);
            result.Tasks.First().Text.Should().Be("(A) This is also a high priority +project @context");
        }

        [Test]
        public async Task ListPriorityQueryHandler_HandlesNegativeSearchTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A", Terms = new string[] { "-+project" } };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(1);
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
        }

        [Test]
        public async Task ListPriorityQueryHandler_HandlesMultipleNegativeSearchTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A-B", Terms = new string[] { "-+project", "-@context" } };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(2);
            result.Tasks.First().Text.Should().Be("(A) This is high priority");
        }

        [Test]
        public async Task ListPriorityQueryHandler_HandlesNegativeAndPositiveSearchTerms()
        {
            var command = new ListPriorityQuery { Priorities = "A-E", Terms = new string[] { "-+project", "@context" } };
            ListPriorityResponse result = await _handler.Handle(command, new CancellationToken());
            result.Tasks.Should().HaveCount(1);
            result.Tasks.First().Text.Should().Be("(D) This is lower @context");
        }
    }
}

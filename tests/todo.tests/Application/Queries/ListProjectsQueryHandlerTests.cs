using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Queries.ListProjects;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Queries
{
    public class ListProjectsQueryHandlerTests
    {
        TaskFileMock _taskFile;
        ListProjectsQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _taskFile = new TaskFileMock();
            _taskFile.TaskLines = new List<string>
          {
              "2020-10-16 This is something to do!",
              "2020-10-16 This is something to do with @context and a +project1 task!",
              "(A) This is high priority",
              "",
              "(D) This is lower",
              "(B) This is in between with @context",
              "(Z) Very low priority with +project2",
              "(E) Midrange",
          };
            _handler = new ListProjectsQueryHandler(_taskFile);
        }

        [Test]
        public void ListProjectsQueryHandler_HandlesNullTerms()
        {
            var command = new ListProjectsQuery { Terms = null };
            Assert.DoesNotThrowAsync(async () => await _handler.Handle(command, new CancellationToken()));
        }

        [Test]
        public async Task ListProjectsQueryHandler_ReturnsContextsSorted()
        {
            var command = new ListProjectsQuery { Terms = null };
            string[] result = await _handler.Handle(command, new CancellationToken());
            result.Length.Should().Be(2);
            result[0].Should().Be("+project1");
            result[1].Should().Be("+project2");
        }

        [Test]
        public async Task ListProjectsQueryHandler_ReturnsContextsWithSearchTerms()
        {
            var command = new ListProjectsQuery { Terms = new string[] { "+project1" } };
            string[] result = await _handler.Handle(command, new CancellationToken());
            result.Length.Should().Be(1);
            result[0].Should().Be("+project1");
        }

        [Test]
        public async Task ListProjectsQueryHandler_OrsTheQueryForMultipleSearchTerms()
        {
            var command = new ListProjectsQuery { Terms = new string[] { "+project1", "+project2" } };
            string[] result = await _handler.Handle(command, new CancellationToken());
            result.Length.Should().Be(2);
            result[0].Should().Be("+project1");
            result[1].Should().Be("+project2");
        }

        [Test]
        public async Task ListProjectsQueryHandler_HandlesNegativeSearchTerms()
        {
            var command = new ListProjectsQuery { Terms = new string[] { "-+project1" } };
            string[] result = await _handler.Handle(command, new CancellationToken());
            result.Length.Should().Be(1);
            result[0].Should().Be("+project2");
        }

        [Test]
        public async Task ListProjectsQueryHandler_HandlesMultipleNegativeSearchTerms()
        {
            var command = new ListProjectsQuery { Terms = new string[] { "-+project1", "-+project2" } };
            string[] result = await _handler.Handle(command, new CancellationToken());
            result.Length.Should().Be(0);
        }

        [Test]
        public async Task ListProjectsQueryHandler_HandlesNegativeAndPositiveSearchTerms()
        {
            var command = new ListProjectsQuery { Terms = new string[] { "-+project2", "+project1" } };
            string[] result = await _handler.Handle(command, new CancellationToken());
            result.Length.Should().Be(1);
            result[0].Should().Be("+project1");
        }
    }
}

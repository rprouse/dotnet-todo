using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Queries;
using Alteridem.Todo.Infrastructure.Persistence;
using Alteridem.Todo.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Application.Queries;

public class ListContextsQueryHandlerTests
{
    TaskFileMock _taskFile;
    ListContextsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _taskFile = new TaskFileMock();
        _taskFile.TaskLines = new List<string>
       {
           "2020-10-16 This is something to do!",
           "2020-10-16 This is something to do with @context2 and a +project!",
           "(A) This is high priority",
           "",
           "(D) This is lower",
           "(B) This is in between with @context1",
           "(Z) Very low priority with +project",
           "(E) Midrange",
       };
        _handler = new ListContextsQueryHandler(_taskFile, new TaskConfiguration());
    }

    [Test]
    public void ListContextsQueryHandler_HandlesNullTerms()
    {
        var command = new ListContextsQuery { Terms = null };
        Assert.DoesNotThrowAsync(async () => await _handler.Handle(command, new CancellationToken()));
    }

    [Test]
    public async Task ListContextsQueryHandler_ReturnsContextsSorted()
    {
        var command = new ListContextsQuery { Terms = null };
        string[] result = await _handler.Handle(command, new CancellationToken());
        result.Length.Should().Be(2);
        result[0].Should().Be("@context1");
        result[1].Should().Be("@context2");
    }

    [Test]
    public async Task ListContextsQueryHandler_ReturnsContextsWithSearchTerms()
    {
        var command = new ListContextsQuery { Terms = new string[] { "@context1" } };
        string[] result = await _handler.Handle(command, new CancellationToken());
        result.Length.Should().Be(1);
        result[0].Should().Be("@context1");
    }

    [Test]
    public async Task ListContextsQueryHandler_OrsTheQueryForMultipleSearchTerms()
    {
        var command = new ListContextsQuery { Terms = new string[] { "@context1", "@context2" } };
        string[] result = await _handler.Handle(command, new CancellationToken());
        result.Length.Should().Be(2);
        result[0].Should().Be("@context1");
        result[1].Should().Be("@context2");
    }

    [Test]
    public async Task ListContextsQueryHandler_HandlesNegativeSearchTerms()
    {
        var command = new ListContextsQuery { Terms = new string[] { "-@context1" } };
        string[] result = await _handler.Handle(command, new CancellationToken());
        result.Length.Should().Be(1);
        result[0].Should().Be("@context2");
    }

    [Test]
    public async Task ListContextsQueryHandler_HandlesMultipleNegativeSearchTerms()
    {
        var command = new ListContextsQuery { Terms = new string[] { "-@context1", "-@context2" } };
        string[] result = await _handler.Handle(command, new CancellationToken());
        result.Length.Should().Be(0);
    }

    [Test]
    public async Task ListContextsQueryHandler_HandlesNegativeAndPositiveSearchTerms()
    {
        var command = new ListContextsQuery { Terms = new string[] { "-@context2", "@context1" } };
        string[] result = await _handler.Handle(command, new CancellationToken());
        result.Length.Should().Be(1);
        result[0].Should().Be("@context1");
    }
}

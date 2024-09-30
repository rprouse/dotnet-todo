using System;
using Alteridem.Todo.Domain.Entities;
using Alteridem.Todo.Infrastructure.Persistence;
using FluentAssertions;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Domain.Entities
{
    public class TaskItemTests
    {
        [TestCase("x 2011-03-03 Call Mom")]
        [TestCase(" xylophone lesson")]
        [TestCase("X 2012-01-01 Make resolutions")]
        [TestCase("(A) x Find ticket prices ")]
        public void WhenTaskCompleteUnchanged_ToString_ShouldReturnProvidedStringTrimmed(string line)
        {
            var task = new TaskItem(line);
            Assert.That(task.ToString(), Is.EqualTo(line.Trim()));
        }
         
        [TestCase("Test raw string")]
        [TestCase("(A) Test raw string with priority")]
        [TestCase("2020-10-04 Test raw string with creation date")]
        public void WhenUncompletedTaskIsCompleted_CompletionDate_ShouldBeSet(string line)
        {
            var task = new TaskItem(line);
            task.Completed = true;
            Assert.That(task.CompletionDate, Is.Not.Null);
        }

        [TestCase("Test raw string")]
        [TestCase("(A) Test raw string with priority")]
        [TestCase("(B) 2020-10-04 Test raw string with creation date")]
        public void WhenUncompletedTaskIsCompleted_Priority_ShouldBeUnset(string line)
        {
            var task = new TaskItem(line);
            task.Completed = true;
            Assert.That(task.Priority, Is.Null);
        }

        [TestCase("Test raw string", "Test raw string")]
        [TestCase("(A) Test raw string with priority", "Test raw string with priority")]
        [TestCase("(B) 2020-10-04 Test raw string with creation date", "2020-10-04 Test raw string with creation date")]
        public void WhenUncompletedTaskIsCompleted_ToString_ShouldIncludeCompletedAndDate(string line, string expected)
        {
            var n = DateTime.Now;
            expected = $"x {n:yyyy-MM-dd} {expected}";
            var task = new TaskItem(line);
            task.Completed = true;
            Assert.That(task.ToString(), Is.EqualTo(expected));
        }

        [TestCase("x Test raw string", "Test raw string")]
        [TestCase("x 2020-10-05 2020-10-04 Test raw string with creation date", "2020-10-04 Test raw string with creation date")]
        public void WhenCompletedTaskIsUncompleted_ToString_ShouldNotIncludeCompletedAndDate(string line, string expected)
        {
            var task = new TaskItem(line);
            task.Completed = false;
            Assert.That(task.ToString(), Is.EqualTo(expected));
        }

        [TestCase("Item without a date", "2020-10-10", "2020-10-10 Item without a date")]
        [TestCase("2019-01-01 Item with a date", "2020-10-10", "2020-10-10 Item with a date")]
        [TestCase("(A) Item without a date", "2020-10-10", "(A) 2020-10-10 Item without a date")]
        [TestCase("(B) 2019-01-01 Item with a date", "2020-10-10", "(B) 2020-10-10 Item with a date")]
        public void AddingCreationDateToTask_IncludesDateInToString(string line, DateTime date, string expected)
        {
            var task = new TaskItem(line);
            task.CreationDate = date;
            Assert.That(task.ToString(), Is.EqualTo(expected));
        }

        [TestCase("")]
        [TestCase("    ")]
        public void HandlesBlankLinesAndWhiteSpace(string line)
        {
            _ = new TaskItem(line);
        }

        [TestCase("(A) Yellow", ConsoleColor.Yellow)]
        [TestCase("(B) Green", ConsoleColor.Green)]
        [TestCase("(C) Blue", ConsoleColor.Cyan)]
        [TestCase("(D) Empty", null)]
        public void SetsColorBasedOnPriorityAndComplete(string line, ConsoleColor? expected)
        {
            var config = new TaskConfiguration();
            var task = new TaskItem(line);
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(2);
            result[0].Color.Should().Be(expected);
            result[0].BackgroundColor.Should().BeNull();
        }

        [TestCase("x Complete")]
        [TestCase("x 2020-10-22 Complete")]
        public void SetsColorBasedOnComplete(string line)
        {
            var config = new TaskConfiguration();
            var task = new TaskItem(line);
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(3);
            result[0].Color.Should().Be(ConsoleColor.DarkGray);
            result[0].BackgroundColor.Should().BeNull();
        }

        [Test]
        public void CanOverrideBackgroundColor()
        {
            var config = new TaskConfiguration();
            config.Priorities['A'].BackgroundColor = ConsoleColor.DarkRed;
            var task = new TaskItem("(A) Yellow");
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(2);
            result[0].Color.Should().Be(ConsoleColor.Yellow);
            result[0].BackgroundColor.Should().Be(ConsoleColor.DarkRed);
        }

        [Test]
        public void SetsTheCreationDateColor()
        {
            var config = new TaskConfiguration();
            var task = new TaskItem("(A) 2020-10-15 Todo");
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(3);
            result[1].Color.Should().Be(ConsoleColor.Magenta);
            result[1].BackgroundColor.Should().BeNull();
        }

        [Test]
        public void IfDateColorIsNullUsesPriorityColor()
        {
            var config = new TaskConfiguration();
            config.DateColor = null;
            var task = new TaskItem("(A) 2020-10-15 Todo");
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(3);
            result[1].Color.Should().Be(ConsoleColor.Yellow);
            result[1].BackgroundColor.Should().BeNull();
        }

        [Test]
        public void SetsTheCompletionDateColor()
        {
            var config = new TaskConfiguration();
            var task = new TaskItem("x 2020-10-22 2020-10-15 Complete");
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(4);
            result[1].Color.Should().Be(ConsoleColor.Magenta);
            result[1].BackgroundColor.Should().BeNull();
        }

        [Test]
        public void IfDateColorIsNullUsesDoneColor()
        {
            var config = new TaskConfiguration();
            config.DateColor = null;
            var task = new TaskItem("x 2020-10-22 2020-10-15 Complete");
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(4);
            result[1].Color.Should().Be(ConsoleColor.DarkGray);
            result[1].BackgroundColor.Should().BeNull();
        }

        [TestCase("+project one two", 0)]
        [TestCase("one +project two", 1)]
        [TestCase("one two +project", 2)]
        public void TokenizesProjects(string line, int pos)
        {
            var config = new TaskConfiguration();
            var task = new TaskItem(line);
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(3);
            result[pos].Color.Should().Be(ConsoleColor.Red);
            result[pos].BackgroundColor.Should().BeNull();
        }

        [TestCase("@context one two", 0)]
        [TestCase("one @context two", 1)]
        [TestCase("one two @context", 2)]
        public void TokenizesContexts(string line, int pos)
        {
            var config = new TaskConfiguration();
            var task = new TaskItem(line);
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(3);
            result[pos].Color.Should().Be(ConsoleColor.Red);
            result[pos].BackgroundColor.Should().BeNull();
        }

        [TestCase("due:2020-10-22 one two", 0)]
        [TestCase("one due:2020-10-22 two", 1)]
        [TestCase("one two due:2020-10-22", 2)]
        public void TokenizesMeta(string line, int pos)
        {
            var config = new TaskConfiguration();
            var task = new TaskItem(line);
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(3);
            result[pos].Color.Should().Be(ConsoleColor.DarkCyan);
            result[pos].BackgroundColor.Should().BeNull();
        }

        [TestCase("id:1234 one two")]
        [TestCase("one id:1234 two")]
        [TestCase("one two id:1234")]
        [TestCase("updated:1234 one two")]
        [TestCase("one updated:1234 two")]
        [TestCase("one two updated:1234")]
        [TestCase("one two id:1234 updated:1234")]
        [TestCase("one two Id:1234 Updated:1234")]
        [TestCase("one two ID:1234 UPDATED:1234")]
        public void DoesNotIncludeIdOrUpdatedMeta(string line)
        {
            var config = new TaskConfiguration();
            var task = new TaskItem(line);
            var result = task.ToColorString(false, config);
            result.Should().HaveCount(2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Alteridem.Todo.Core;
using NUnit.Framework;

namespace Alteridem.Todo.Tests
{
    public class TaskTests
    {
        [TestCase("x 2011-03-03 Call Mom")]
        [TestCase(" xylophone lesson")]
        [TestCase("X 2012-01-01 Make resolutions")]
        [TestCase("(A) x Find ticket prices ")]
        public void WhenTaskCompleteUnchanged_ToString_ShouldReturnProvidedStringTrimmed(string line)
        {
            var task = new Task(line);
            Assert.That(task.ToString(), Is.EqualTo(line.Trim()));
        }
         
        [TestCase("Test raw string")]
        [TestCase("(A) Test raw string with priority")]
        [TestCase("2020-10-04 Test raw string with creation date")]
        public void WhenUncompletedTaskIsCompleted_CompletionDate_ShouldBeSet(string line)
        {
            var task = new Task(line);
            task.Completed = true;
            Assert.That(task.CompletionDate, Is.Not.Null);
        }

        [TestCase("Test raw string")]
        [TestCase("(A) Test raw string with priority")]
        [TestCase("(B) 2020-10-04 Test raw string with creation date")]
        public void WhenUncompletedTaskIsCompleted_Priority_ShouldBeUnset(string line)
        {
            var task = new Task(line);
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
            var task = new Task(line);
            task.Completed = true;
            Assert.That(task.ToString(), Is.EqualTo(expected));
        }

        [TestCase("x Test raw string", "Test raw string")]
        [TestCase("x 2020-10-05 2020-10-04 Test raw string with creation date", "2020-10-04 Test raw string with creation date")]
        public void WhenCompletedTaskIsUncompleted_ToString_ShouldNotIncludeCompletedAndDate(string line, string expected)
        {
            var task = new Task(line);
            task.Completed = false;
            Assert.That(task.ToString(), Is.EqualTo(expected));
        }
    }
}

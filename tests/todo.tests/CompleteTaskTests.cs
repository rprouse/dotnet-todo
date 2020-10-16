using System;
using System.Collections.Generic;
using Alteridem.Todo.Core;
using NUnit.Framework;

namespace Alteridem.Todo.Tests
{
    public class CompleteTaskTests
    {
        // https://github.com/todotxt/todo.txt#rule-1-a-completed-task-starts-with-an-lowercase-x-character-x
        [TestCase("x 2011-03-03 Call Mom", true)]
        [TestCase("xylophone lesson", false)]
        [TestCase("X 2012-01-01 Make resolutions", false)]
        [TestCase("(A) x Find ticket prices", false)]
        public void Rule1_ACompletedTaskStartsWithALowercaseXCharacter(string line, bool completed)
        {
            var task = new Task(line);
            Assert.That(task.Completed, Is.EqualTo(completed));
        }

        // https://github.com/todotxt/todo.txt#rule-2-the-date-of-completion-appears-directly-after-the-x-separated-by-a-space
        [Test]
        public void Rule2_TheDateOfCompletionAppearsDirectlyAfterTheXSeparatedByASpace()
        {
            var task = new Task("x 2011-03-02 2011-03-01 Review Tim's pull request +TodoTxtTouch @github");
            Assert.That(task.CompletionDate, Is.EqualTo(DateTime.Parse("2011-03-02")));
            Assert.That(task.CreationDate, Is.EqualTo(DateTime.Parse("2011-03-01")));
        }

        [TestCase("x 2011-03-03 Call Mom", "Call Mom")]
        [TestCase("x 2011-03-02 2011-03-01 Review Tim's pull request +TodoTxtTouch @github", "Review Tim's pull request +TodoTxtTouch @github")]
        public void ParsesOutDescriptionForCompleteTasks(string line, string description)
        {
            var task = new Task(line);
            Assert.That(task.Description, Is.EqualTo(description));
        }
    }
}

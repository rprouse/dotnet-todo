using System;
using System.Collections.Generic;
using Alteridem.Todo.Domain.Entities;
using NUnit.Framework;

namespace Alteridem.Todo.Tests.Domain.Entities
{
    public class IncompleteTaskItemTests
    {
        // https://github.com/todotxt/todo.txt#rule-2-a-tasks-creation-date-may-optionally-appear-directly-after-priority-and-a-space
        [TestCase("(A) Call Mom", 'A')]
        [TestCase("Really gotta call Mom (A) @phone @someday", null)]
        [TestCase("(b) Get back to the boss", null)]
        [TestCase("(B)->Submit TPS report", null)]
        public void Rule1_IfPriorityExistsItAlwaysAppearsFirst(string line, char? priority)
        {
            var task = new TaskItem(line);
            Assert.That(task.Priority, Is.EqualTo(priority));
        }

        // https://github.com/todotxt/todo.txt#rule-2-a-tasks-creation-date-may-optionally-appear-directly-after-priority-and-a-space
        [TestCase("2011-03-02 Document +TodoTxt task format", "2011-03-02")]
        [TestCase("(A) 2011-03-02 Call Mom", "2011-03-02")]
        [TestCase("(A) Call Mom 2011-03-02", null)]
        public void Rule2_ATasksCreationDateMayOptionallyAppearDirectlyAfterPriorityAndASpace(string line, DateTime? creationDate)
        {
            var task = new TaskItem(line);
            Assert.That(task.CreationDate, Is.EqualTo(creationDate));
        }

        // https://github.com/todotxt/todo.txt#rule-3-contexts-and-projects-may-appear-anywhere-in-the-line-after-priorityprepended-date
        [TestCaseSource(nameof(Rule3Data))]
        public void Rule3_ContextsAndProjectsMayAppearAnywhereInTheLineAfterPriorityAndPrependedDate(string line, string[] projectTags, string[] contextTags)
        {
            var task = new TaskItem(line);
            Assert.That(task.ProjectTags, Is.EqualTo(projectTags));
            Assert.That(task.ContextTags, Is.EqualTo(contextTags));
        }

        [TestCase("(A) Call Mom", "Call Mom")]
        [TestCase("Really gotta call Mom (A) @phone @someday", "Really gotta call Mom (A) @phone @someday")]
        [TestCase("(b) Get back to the boss", "(b) Get back to the boss")]
        [TestCase("(B)->Submit TPS report", "(B)->Submit TPS report")]
        [TestCase("2011-03-02 Document +TodoTxt task format", "Document +TodoTxt task format")]
        [TestCase("(A) 2011-03-02 Call Mom", "Call Mom")]
        [TestCase("(A) Call Mom 2011-03-02", "Call Mom 2011-03-02")]
        public void ParsesOutDescriptionForIncompleteTasks(string line, string description)
        {
            var task = new TaskItem(line);
            Assert.That(task.Description, Is.EqualTo(description));
        }

        // https://github.com/todotxt/todo.txt#additional-file-format-definitions
        [TestCaseSource(nameof(AdditionalFileFormatDefinitionData))]
        public void ParsesOutAdditionalFileFormatDefinitions(string line, IDictionary<string, string> specialTags)
        {
            var task = new TaskItem(line);
            Assert.That(task.SpecialTags, Is.EqualTo(specialTags));
        }


        public static IEnumerable<TestCaseData> Rule3Data =>
            new[]
            {
                new TestCaseData(
                    "(A) Call Mom +Family +PeaceLoveAndHappiness @iphone @phone", 
                    new string[]{ "+Family", "+PeaceLoveAndHappiness" }, 
                    new string[]{ "@iphone", "@phone" }
                ),
               new TestCaseData(
                   "Email SoAndSo at soandso@example.com",
                   new string[]{ },
                   new string[]{ }
               ),
               new TestCaseData(
                   "Learn how to add 2+2",
                   new string[]{ },
                   new string[]{ }
               ),
            };

        public static IEnumerable<TestCaseData> AdditionalFileFormatDefinitionData =>
            new[]
            {
                new TestCaseData(
                    "(A) Call Mom +Family +PeaceLoveAndHappiness @iphone @phone key:value due:2010-01-02",
                    new Dictionary<string, string>
                    {
                        { "key", "value" },
                        { "due", "2010-01-02" }
                    }
                ),
               new TestCaseData(
                   "Email SoAndSo at email:soandso@example.com or phone:(555)365-1234",
                    new Dictionary<string, string>
                    {
                        { "email", "soandso@example.com" },
                        { "phone", "(555)365-1234" }
                    }
               ),
               new TestCaseData(
                   "Don't take leading :colons",
                   new Dictionary<string, string>()
               ),
               new TestCaseData(
                   "Don't take trailing: colons either",
                   new Dictionary<string, string>()
               ),
            };
    }
}

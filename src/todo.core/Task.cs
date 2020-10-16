using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Alteridem.Todo.Core
{
    public class Task
    {
        private static readonly Regex CompletedRegex = new Regex(@"^x\s((\d{4})-(\d{2})-(\d{2}))?", RegexOptions.Compiled);
        private static readonly Regex PriorityRegex = new Regex(@"^\((?<priority>[A-Z])\)\s", RegexOptions.Compiled);
        private static readonly Regex DateRegex = new Regex(@"^(?<date>(\d{4})-(\d{2})-(\d{2}))", RegexOptions.Compiled);
        private static readonly Regex ProjectRegex = new Regex(@"(?<proj>(?<=^|\s)\+[^\s]+)", RegexOptions.Compiled);
        private static readonly Regex ContextRegex = new Regex(@"(?<context>(?<=^|\s)\@[^\s]+)", RegexOptions.Compiled);
        private static readonly Regex SpecialTagRegex = new Regex(@"(?<tag>(?<=^|\s)[^\s:]+\:[^\s:]+)", RegexOptions.Compiled);

        private bool _completed;

        /// <summary>
        /// Is the task complete?
        /// </summary>
        public bool Completed
        {
            get => _completed;
            set
            {
                _completed = value;
                if(_completed)
                {
                    Priority = null;
                    CompletionDate = DateTime.Now.Date;
                }
                else
                {
                    CompletionDate = null;
                }
            }
        }

        /// <summary>
        /// The priority of the task if set uppercase A-Z
        /// </summary>
        public char? Priority { get; set; }

        /// <summary>
        /// The optional completion date
        /// </summary>
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// The optional creation date. Required if Completion date is set.
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// The description of the todo including any tags
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of any + project tags including the +
        /// </summary>
        public IList<string> ProjectTags { get; } = new List<string>();

        /// <summary>
        /// A list of any @ context tags including the @
        /// </summary>
        public IList<string> ContextTags { get; } = new List<string>();

        /// <summary>
        /// A dictionary of special tag key/value pairs
        /// </summary>
        public IDictionary<string, string> SpecialTags { get; } = new Dictionary<string, string>();

        /// <summary>
        /// The original full line of text for the todo
        /// </summary>
        public string Line { get; }

        public Task(string line)
        {
            Line = line.Trim();
            Parse(line);
        }

        private void Parse(string line)
        {
            // Parse and strip completed and completed date
            var str = CompletedRegex.Match(line).Value.Trim();

            if(string.IsNullOrWhiteSpace(str))
            {
                Completed = false;
                CompletionDate = null;
            }
            else
            {
                Completed = true;
                if (str.Length > 2)
                {
                    string date = str.Substring(2);
                    if (!string.IsNullOrWhiteSpace(date))
                        CompletionDate = DateTime.Parse(date);
                }
            }
            line = CompletedRegex.Replace(line, "").Trim();

            // Parse and strip the priority
            string priority = PriorityRegex.Match(line).Groups["priority"].Value.Trim();
            if (priority.Length == 1)
                Priority = priority[0];
            line = PriorityRegex.Replace(line, "").Trim();

            // Parse and strip the created date
            string created = DateRegex.Match(line).Groups["date"].Value.Trim();
            if (!string.IsNullOrEmpty(created))
                CreationDate = DateTime.Parse(created);
            line = DateRegex.Replace(line, "").Trim();

            // Parse projects but don't strip them
            var projects = ProjectRegex.Matches(line);
            foreach (var project in projects)
                ProjectTags.Add(project.ToString());

            // Parse contexts but don't strip them
            var contexts = ContextRegex.Matches(line);
            foreach (var context in contexts)
                ContextTags.Add(context.ToString());

            // Parse out special tags but don't strip them
            var tags = SpecialTagRegex.Matches(line);
            foreach(var pair in tags)
            {
                var split = pair.ToString().Split(':');
                SpecialTags.Add(split[0], split[1]);
            }

            Description = line;
        }

        public override string ToString()
        {
            // If completed hasn't changed, just return the line
            bool prevComplete = Line.StartsWith("x ");
            if(prevComplete == Completed)
                return Line;

            if(Completed)
            {
                return string.Format("x {0}{1}{2}",
                    CompletionDate is null ? "" : CompletionDate.Value.ToString("yyyy-MM-dd") + " ",
                    CreationDate is null ? "" : CreationDate.Value.ToString("yyyy-MM-dd") + " ",
                    Description
                );
            }
            else
            {
                return string.Format("{0}{1}{2}",
                    Priority is null ? "" : $"({Priority}) ",
                    CreationDate is null ? "" : CreationDate.Value.ToString("yyyy-MM-dd") + " ",
                    Description);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using YouTrackSharp.Projects;

namespace YouTrackBot
{
    public class CommandParser
    {

        private static List<Project> Projects { get; set; } = new List<Project>();
        private static YouTrackPoster youTrackPoster;

        public CommandParser(CustomSettings settings) {
            youTrackPoster = new YouTrackPoster(settings);
            Projects = Task.Run(async () => await youTrackPoster.GetProjects()).Result;
        }

        public string GetResult(InlineQueryEventArgs inlineQueryEventArgs)
        {
            string result = "";
            var query = inlineQueryEventArgs.InlineQuery.Query;
            var command = query.Substring(0, query.IndexOf("///")).Split(' ');
            var projectName = command[0].ToLower();
            var taskType = command[1].ToLower();
            taskType = taskType.Contains("task") || taskType.Contains("feature") || taskType.Contains("bug") ? taskType : "";
            var taskName = command[2];
            var taskDesc = BuildDescription(command);

            Project currentProject = null;
            foreach (var item in Projects)
            {
                if (item.Name.ToLower() == projectName.ToLower() || item.ShortName.ToLower() == projectName.ToLower())
                {
                    currentProject = item;
                    break;
                }
            }

            if (currentProject == null)
                result = $"Проект с именем {projectName} не найден!";
            else
            {
                result = Task.Run(async () => await youTrackPoster.PostIssue(currentProject, taskName, taskType, taskDesc)).Result;
                //result = $"Проект: {projectName}\r\nТип задачи: {taskType}\r\nНаменование: {taskName}\r\nОписание: {taskDesc}\r\nРезультат: Ok";
            }
            return result;
        }

        private static string BuildDescription(string[] commandParts)
        {
            string retVal = "";

            for (int idx = 3; idx < commandParts.Length; idx++)
            {
                retVal += commandParts[idx] + " ";
            }

            return retVal.Trim();
        }
    }
}

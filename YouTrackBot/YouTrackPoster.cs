using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTrackSharp;
using YouTrackSharp.Projects;

namespace YouTrackBot
{
    public class YouTrackPoster
    {
        public List<Project> Projects { get; set; }

        private static CustomSettings Settings { get; set; }
        private BearerTokenConnection Connection { get; set; }

        public YouTrackPoster(CustomSettings settings)
        {
            Settings = settings;
            Connection = new BearerTokenConnection("http://toshiba-note:8080/", Settings.YouTrackToken);
        }

        public async Task<List<Project>> GetProjects()
        {
            var Projects = new List<Project>();
            var projectsService = Connection.CreateProjectsService();
            var projectsForCurrentUser = await projectsService.GetAccessibleProjects();
            Projects.AddRange(projectsForCurrentUser);

            return Projects;
        }

        public async Task<string> PostIssue(Project project, string taskName, string taskType, string taskDescription)
        {
            string retVal = "";

            string [] tags = taskType.Split('/');
            List<SubValue<string>> tagList = new List<SubValue<string>>();
            if (tags.Length > 0)
            {
                foreach(var item in tags)
                {
                    tagList.Add(new SubValue<string> { Value = item });
                }
            }
            else
                tagList.Add(new SubValue<string> { Value = taskType });

            var issueService = Connection.CreateIssuesService();
            try
            {
                await issueService.CreateIssue(project.ShortName, new YouTrackSharp.Issues.Issue { Summary = taskName, Description = taskDescription, Tags = tagList });
            }
            catch (Exception ex)
            {
                retVal = "Произошла ошибка при добавлении задачи: ";
                if (!String.IsNullOrEmpty(ex.Message))
                    retVal += ex.Message;
                else if (!String.IsNullOrEmpty(ex.InnerException.Message))
                    retVal += ex.InnerException.Message;

                return retVal;
            }

            retVal = $"Проект: {project.Name}\r\nТип задачи: {taskType}\r\nНаменование: {taskName}\r\nОписание: {taskDescription}\r\nРезультат: Добавлено!";
            return retVal;
        }
    }
}

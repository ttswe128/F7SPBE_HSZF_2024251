using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251.Application
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectDataProvider dp;

        public ProjectService(IProjectDataProvider provider)
        {
            dp = provider;
        }

        public List<Project> GetProjects()
        {
            return dp.GetProjects();
        }

        public List<Project> GetProjectsWithoutProgrammers()
        {
            return dp.GetProjectsWithoutProgrammers();
        }

        public Project GetProject(int id)
        {
            return dp.GetProject(id);
        }

        public void CreateProject(Project project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project), "The project cannot be null.");

            if (string.IsNullOrWhiteSpace(project.Name)) throw new ArgumentException("Project name cannot be empty.");

            if (project.StartDate == default) throw new ArgumentException("Project start date must be provided.");

            dp.CreateProject(project);
        }

        public void CreateProjects(List<Project> projects)
        {
            dp.CreateProjects(projects);
        }

        public void AssignProgrammersToProject(int projectId, List<int> programmerIds)
        {
            dp.AssignProgrammersToProject(projectId, programmerIds);
        }

        public void CloseProject(Project project)
        {

            project.Status = EStatus.CLOSED;
            project.EndDate = DateTime.Now;


            dp.UpdateProject(project.Id, project);
        }

        public void UpdateProject(int id, Project project)
        {
            dp.UpdateProject(id, project);
        }

        public List<Project> GetClosableProjects() => dp.GetClosableProjects();


        public Project AssignProgrammersToProject(int id, Project projectSelected)
        {
            dp.UpdateProject(projectSelected.Id, projectSelected);

            return projectSelected;

        }

        public List<Project> GetProjectsOfProgrammer(Programmer programmer)
        {

            List<Project> projects = dp.GetProjectsOfProgrammer(programmer);
            if (!projects.Any()) throw new Exception($"{programmer.Name} is not assigned to any projects.");

            return projects;

        }

        public Project AddTask(Project project, Programmer programmer, Task task)
        {

            Project projectToReturn = dp.AddTask(project, programmer, task);
            return projectToReturn;
        }

        public List<Task> GetTasksForModifying(Project project, Programmer programmer)
        {
            return dp.GetTasksForModifying(project, programmer);
        }

        public Project ModifyTask(Project project, Task task, Programmer programmer)
        {
            dp.ModifyTask(project, task);
            return project;

        }

        public void ExportDelayedProjects()
        {
            var delayedProjects = dp.GetDelayedProjects();

            if (!delayedProjects.Any())
            {
                throw new Exception("No delayed projects found.");
            }

            var xmlDocument = new XElement("Projects",
                delayedProjects.Select(p =>
                    new XElement("Project",
                        new XElement("Name", p.Name),
                        new XElement("Description", p.Description),
                        new XElement("Tasks",
                            p.Tasks.Select(t =>
                                new XElement("Task",
                                new XElement("Name", t.Name),
                                new XElement("Status", t.Status.ToString())
                                )
                                )
                            )
                        )
                    )
                );

            xmlDocument.Save("_delayed_projects.xml");
        }

        public void SeedDb()
        {
            dp.SeedDb();
        }
    }
}

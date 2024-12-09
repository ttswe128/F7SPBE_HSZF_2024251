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

        public Project ListAndSelectProjects(List<Project> projects)
        {
            Console.WriteLine("List of Projects: \n");
            for (int i = 0; i < projects.Count; i++)
            {
                string names = null;
                if (projects[i].Participants != null)
                {
                    names = string.Join(", ", projects[i].Participants.Select(participant => participant.Name));
                }
                else
                {
                    names = "none";
                }
                Console.WriteLine($"{i + 1} - {projects[i].Name} - Status: {projects[i].Status} - Participants: {names}");
            }
            Console.Write("\nEnter the Project's index: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int projectIndex))
            {
                var selectedProject = projects[projectIndex - 1];
                if (selectedProject != null)
                {
                    return selectedProject;
                }
            }

            Console.WriteLine("Invalid selection. Returning to the main menu.");
            return null;

        }

        public void CloseProject(int id)
        {
            var project = dp.GetProject(id);

            if (project.Status == EStatus.CLOSED) Console.WriteLine("This Project is already closed");

            if (project.Tasks.Any(task => task.Status != EStatus.CLOSED))
            {
                Console.WriteLine("The project cannot be closed because not all tasks are completed.");
                return;
            }

            project.Status = EStatus.CLOSED;
            project.EndDate = DateTime.Now;


            dp.UpdateProject(id, project);


            Console.WriteLine($"\nProject with ID {id} successfully closed.");
        }

        public void UpdateProject(int id, Project project)
        {
            dp.UpdateProject(id, project);
        }

        public List<Project> GetClosableProjects() => dp.GetClosableProjects();

        public void CreateProject()
        {
            Console.WriteLine("Creating Project...\n");

            // Name
            Console.Write("Enter the project's name: ");
            string name = Console.ReadLine();

            // Description
            Console.Write("Enter the project's description: ");
            string description = Console.ReadLine();

            // StartDate
            Console.Write("Enter the project's start date (yyyy-MM-dd): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.WriteLine("Invalid date format. Please try again.");
            }

            // Deadlines
            List<DateTime> deadlines = new List<DateTime>();
            Console.WriteLine("Enter project deadlines (yyyy-MM-dd). Type 'done' to finish:");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "done")
                    break;

                if (DateTime.TryParse(input, out DateTime deadline))
                {
                    deadlines.Add(deadline);
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                }
            }

            // Status
            Console.Write("Enter the project's status (STARTED, IN_PROGRESS, CLOSED): ");
            EStatus status;
            while (!Enum.TryParse(Console.ReadLine(), true, out status))
            {
                Console.WriteLine("Invalid status. Please try again.");
            }

            var newProject = new Project
            {
                Name = name,
                Description = description,
                StartDate = startDate,
                Deadlines = deadlines,
                Status = status
            };

            dp.CreateProject(newProject);

            Console.WriteLine("Project added successfully.");
        }

        public Project AssignProgrammersToProject(List<Programmer> programmers)
        {
            List<Project> projectsWithoutProgrammers = GetProjectsWithoutProgrammers();
            //Console.Clear();

            // List Projects
            Console.WriteLine("Projects without Programmers:\n");
            for (int i = 0; i < projectsWithoutProgrammers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {projectsWithoutProgrammers[i].Name} - Description: {projectsWithoutProgrammers[i].Description} - Start: {projectsWithoutProgrammers[i].StartDate} - End: {projectsWithoutProgrammers[i].EndDate} - Status: {projectsWithoutProgrammers[i].Status}");

            }

            // Project selection
            Console.Write("\nEnter the Project's number here: ");
            Project projectSelected = null;
            while (projectSelected == null)
            {
                if (int.TryParse(Console.ReadLine(), out int projectIndex) && projectIndex > 0 && projectIndex <= projectsWithoutProgrammers.Count)
                {
                    projectSelected = projectsWithoutProgrammers[projectIndex - 1];
                }
                else
                {
                    Console.WriteLine("\nThe number entered is incorrect. Please try again.");
                }
            }

            // List Programmers
            Console.WriteLine("\nList of Programmers:\n");
            for (int i = 0; i < programmers.Count; i++)
            {
                Console.WriteLine($"{programmers[i].Id}. {programmers[i].Name} - {programmers[i].Role} (Year of joining: {programmers[i].DateOfJoining})");
            }

            // Programmers selection
            List<int> ids = new List<int>();
            Console.WriteLine("\nEnter the Programmers' IDs . Type 'done' to finish: ");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "done")
                    break;

                if (int.TryParse(input, out int programmerId) && programmerId > 0 && programmerId <= programmers.Count)
                {
                    if (!ids.Contains(programmerId))
                        ids.Add(programmerId);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter at least one valid programmer number.");
                }
            }

            List<Programmer> programmersToAssign = programmers
                .Where(p => ids.Contains(p.Id))
                .ToList();


            projectSelected.Participants = programmersToAssign;

            UpdateProject(projectSelected.Id, projectSelected);

            Console.WriteLine("\nProgrammers successfully assigned to the project.");

            return projectSelected;

        }

        public List<Project> GetProjectsOfProgrammer(Programmer programmer)
        {

            List<Project> projects = dp.GetProjectsOfProgrammer(programmer);
            if (!projects.Any()) Console.WriteLine($"{programmer.Name} is not assigned to any projects.");

            return projects;

        }

        public Project AddTask(Project project, Programmer programmer)
        {

            Console.WriteLine($"\nAdding a task to project: {project.Name}");
            Console.Write("Enter Task Name: ");
            string taskName = Console.ReadLine();

            Console.Write("Enter Task Description: ");
            string taskDescription = Console.ReadLine();

            Console.Write("Enter Task Size: ");
            string taskSize = Console.ReadLine();

            EStatus taskStatus;
            while (true)
            {
                Console.Write("Enter Task Status (STARTED, IN_PROGRESS, CLOSED): ");
                if (Enum.TryParse(Console.ReadLine(), true, out taskStatus))
                {
                    break;
                }
                Console.WriteLine("Invalid status. Please try again.");
            }

            var newTask = new Task
            {
                Name = taskName,
                Description = taskDescription,
                Responsible = programmer,
                Size = taskSize,
                Status = taskStatus
            };

            Project projectToReturn = dp.AddTask(project, programmer, newTask);
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

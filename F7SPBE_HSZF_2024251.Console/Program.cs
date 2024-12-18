﻿using F7SPBE_HSZF_2024251.Application;
using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddScoped<AppDbContext>();
                services.AddSingleton<IProgrammerDataProvider, ProgrammerDataProvider>();
                services.AddSingleton<IProgrammerService, ProgrammerService>();
                services.AddSingleton<ITaskDataProvider, TaskDataProvider>();
                services.AddSingleton<ITaskService, TaskService>();
                services.AddSingleton<IProjectDataProvider, ProjectDataProvider>();
                services.AddSingleton<IProjectService, ProjectService>();


            })
            .Build();
            host.Start();

            using IServiceScope serviceScope = host.Services.CreateScope();
            var programmerService = host.Services.GetRequiredService<IProgrammerService>();
            var taskService = host.Services.GetRequiredService<ITaskService>();
            var projectService = host.Services.GetRequiredService<IProjectService>();

            projectService.SeedDb();

            List<Programmer> programmersList = programmerService.GetProgrammers();
            Programmer programmerSignedIn = null;
            Console.Clear();

            WelcomeMessage();

            while (programmerSignedIn == null)
            {
                programmerSignedIn = SignIn(programmersList);
                Console.Clear();
            }
            Console.WriteLine($"Welcome {programmerSignedIn.Name} ({programmerSignedIn.Role})!\n");


            string? button = null;
            while (button != "exit")
            {
                button = Menu();
                Console.Clear();
                switch (button)
                {
                    case "1":
                        CreateProject(projectService);

                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        break;
                    case "2":

                        AssignProgrammersToProject(projectService, programmerService);

                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        break;
                    case "3":
                        string input3 = null;
                        while(input3 == null)
                        {
                            List<Project> projectToSelect = projectService.GetProjectsOfProgrammer(programmerSignedIn);

                            Console.WriteLine($"Projects assigned to {programmerSignedIn.Name}:");
                            for (int i = 0; i < projectToSelect.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. Name: {projectToSelect[i].Name} - Description: {projectToSelect[i].Description}");
                            }


                            Project selectedProject = null;
                            while (selectedProject == null)
                            {
                                Console.Write("\nEnter the Project's number here: ");
                                if (int.TryParse(Console.ReadLine(), out int projectIndex) && projectIndex > 0 && projectIndex <= projectToSelect.Count)
                                {
                                    selectedProject = projectToSelect[projectIndex - 1];
                                }
                                else
                                {
                                    Console.WriteLine("Invalid selection. Please try again.");
                                }
                            }


                            input3 = AddOrModifyTaskMenu();
                            if (input3.Equals("1"))
                            {
                                AddTask(projectService, selectedProject, programmerSignedIn);
                            }
                            if (input3.Equals("2"))
                            {

                                ModifyTask(projectService, selectedProject, programmerSignedIn);

                            }
                        }

                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        break;
                    case "4":
                        ChangeStatus(projectService, taskService, programmerSignedIn);

                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        break;

                    case "5":
                        var projects5 = projectService.GetClosableProjects();
                        var selectedP = ListAndSelectProjects(projects5);
                        var project = projectService.GetProject(selectedP.Id);

                        if (project.Status == EStatus.CLOSED) Console.WriteLine("This Project is already closed");

                        if (project.Tasks.Any(task => task.Status != EStatus.CLOSED)) Console.WriteLine("The project cannot be closed because not all tasks are completed.");

                        projectService.CloseProject(project);

                        Console.WriteLine($"\nProject with ID {selectedP.Id} successfully closed.");

                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        break;
                    case "6":
                        string input6 = null;
                        while (input6 == null)
                        {
                            input6 = ExportSelector();
                            if (input6.Equals("1"))
                            {
                                projectService.ExportDelayedProjects();
                                Console.WriteLine($"\nDelayed projects exported successfully");

                            }
                            if (input6.Equals("2"))
                            {
                                programmerService.ExportProgrammersPerformance();
                                Console.WriteLine("\nProgrammers' performance successfully exported to JSON.");
                            }
                        }

                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        break;
                    case "exit":
                        Console.WriteLine("\nExiting application...");
                        break;
                    default:
                        Console.WriteLine("This shouldn't have been possible...");
                        break;
                }

                Console.Clear();
            }


        }

        static void AssignProgrammersToProject(IProjectService projectService, IProgrammerService programmerService)
        {
            List<Programmer> programmersForProjects = programmerService.GetProgrammers();
            List<Project> projectsWithoutProgrammers = projectService.GetProjectsWithoutProgrammers();

            Console.WriteLine("Projects without Programmers:\n");
            for (int i = 0; i < projectsWithoutProgrammers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {projectsWithoutProgrammers[i].Name} - Description: {projectsWithoutProgrammers[i].Description} - Start: {projectsWithoutProgrammers[i].StartDate} - End: {projectsWithoutProgrammers[i].EndDate} - Status: {projectsWithoutProgrammers[i].Status}");

            }

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

            Console.WriteLine("\nList of Programmers:\n");
            for (int i = 0; i < programmersForProjects.Count; i++)
            {
                Console.WriteLine($"{programmersForProjects[i].Id}. {programmersForProjects[i].Name} - {programmersForProjects[i].Role} (Year of joining: {programmersForProjects[i].DateOfJoining})");
            }

            List<int> ids = new List<int>();
            Console.WriteLine("\nEnter the Programmers' IDs . Type 'done' to finish: ");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "done")
                    break;

                if (int.TryParse(input, out int programmerId) && programmerId > 0 && programmerId <= programmersForProjects.Count)
                {
                    if (!ids.Contains(programmerId))
                        ids.Add(programmerId);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter at least one valid programmer number.");
                }
            }

            List<Programmer> programmersToAssign = programmersForProjects
                .Where(p => ids.Contains(p.Id))
                .ToList();


            projectSelected.Participants = programmersToAssign;

            projectService.AssignProgrammersToProject(projectSelected.Id, projectSelected);

            Console.WriteLine("\nProgrammers successfully assigned to the project.");
        }

        static Project AddTask(IProjectService projectService, Project project, Programmer programmer)
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

            Project projectToReturn = projectService.AddTask(project, programmer, newTask);
            return projectToReturn;
        }

        static void CreateProject(IProjectService projectService)
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

            projectService.CreateProject(newProject);

            Console.WriteLine("Project added successfully.");
        }

        static void ChangeStatus(IProjectService projectService, ITaskService taskService, Programmer programmerSignedIn)
        {
            List<Project> projects = projectService.GetProjects();
            List<Task> tasks = taskService.GetTasksWithProgrammer(projects, programmerSignedIn);



            if (!tasks.Any())
            {
                Console.WriteLine($"{programmerSignedIn.Name} has no tasks assigned.");
                return;
            }

            Console.WriteLine($"Tasks assigned to {programmerSignedIn.Name}:");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i].Name} - {tasks[i].Description} - Size: {tasks[i].Size} - Status: {tasks[i].Status}");
            }

            Task selectedTask = null;
            while (selectedTask == null)
            {
                Console.Write("\nEnter the Task's number here: ");
                if (int.TryParse(Console.ReadLine(), out int taskIndex) && taskIndex > 0 && taskIndex <= tasks.Count)
                {
                    selectedTask = tasks[taskIndex - 1];
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                }
            }


            Console.WriteLine($"\nModifying status for task: {selectedTask.Name}");
            Console.WriteLine("Available statuses: STARTED, IN_PROGRESS, CLOSED");

            EStatus newStatus;
            while (true)
            {
                Console.Write("Enter new status: ");
                if (Enum.TryParse(Console.ReadLine(), true, out newStatus))
                {
                    selectedTask.Status = newStatus;
                    Console.WriteLine($"\nTask '{selectedTask.Name}' status updated to '{newStatus}' successfully!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid status. Please try again.");
                }
            }


            taskService.UpdateTaskStatus(selectedTask.Id, selectedTask);

        }

        static Project ListAndSelectProjects(List<Project> projects)
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

        static void ModifyTask(IProjectService projectService, Project selectedProject, Programmer programmerSignedIn)
        {
            List<Task> tasks = projectService.GetTasksForModifying(selectedProject, programmerSignedIn);

            Console.WriteLine("\nTasks in the project:");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i].Name} - {tasks[i].Description} - Size: {tasks[i].Size} - Status: {tasks[i].Status}");
            }
            Task selectedTask = null;
            while (selectedTask == null)
            {
                Console.Write("\nEnter the Task's number here: ");
                if (int.TryParse(Console.ReadLine(), out int taskIndex) && taskIndex > 0 && taskIndex <= tasks.Count)
                {
                    selectedTask = tasks[taskIndex - 1];
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                }
            }


            Console.WriteLine($"\nModifying task with ID: {selectedTask.Id}");
            Console.WriteLine("\nEnter new Task Name (leave blank to keep current): ");
            string newTaskName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTaskName))
            {
                selectedTask.Name = newTaskName;
            }

            Console.WriteLine("Enter new Task Description (leave blank to keep current): ");
            string newTaskDescription = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTaskDescription))
            {
                selectedTask.Description = newTaskDescription;
            }

            Console.WriteLine("Enter new Task Size (Leave blank to keep current): ");
            string newTaskSize = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTaskSize))
            {
                selectedTask.Size = newTaskSize;
            }

            Console.WriteLine("Enter new Task Status (STARTED, IN_PROGRESS, CLOSED, leave blank to keep current): ");
            string newTaskStatus = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTaskStatus) && Enum.TryParse(newTaskStatus, true, out EStatus taskStatus))
            {
                selectedTask.Status = taskStatus;
            }


            projectService.ModifyTask(selectedProject, selectedTask, programmerSignedIn);

            Console.WriteLine($"\nTask '{selectedTask.Name}' modified successfully!");


        }

        static string ExportSelector()
        {
            Console.WriteLine("[1] Export delayed Projects to XML");
            Console.WriteLine("[2] Export Programmers' performance to JSON");
            Console.WriteLine("[3] Cancel export selection\n");

            string input = Console.ReadLine();

            if (input == "3")
            {
                return Menu();
            }

            if (!string.IsNullOrEmpty(input) && (input == "1" || input == "2"))
            {
                return input;
            }
            
            Console.WriteLine("Invalid input. Please try again.");
            return ExportSelector();
        }

        static string AddOrModifyTaskMenu()
        {
            Console.WriteLine("[1] Add Task to Project");
            Console.WriteLine("[2] Modify Task of a Project\n");

            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) { Console.WriteLine("Input not received"); }
            if (!string.IsNullOrEmpty(input) && (input == "1" || input == "2")) 
            {
                return input;
            }
            else
            {
                throw new Exception("Input is incorrect.");
            }
        }

        static Programmer SignIn(List<Programmer> programmers)
        {
            Console.WriteLine("Please select the programmer you want sign in as!\n");
            for (int i = 0; i < programmers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {programmers[i].Name} - {programmers[i].Role} (Year of joining: {programmers[i].DateOfJoining})");
            }

            Console.Write("\nEnter your number here: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= programmers.Count)
            {
                return programmers[index - 1];
            }
            else
            {
                Console.WriteLine("\nThe number entered is incorrect. Please try again.");
                return null;
            }
        }


        static string Menu()
        {
            string button = null;

            Console.WriteLine("[1] Create Project");
            Console.WriteLine("[2] Assign Participants to Projects");
            Console.WriteLine("[3] Create or modify Task");
            Console.WriteLine("[4] List Tasks and change the status of a Task");
            Console.WriteLine("[5] Close Projects");
            Console.WriteLine("[6] Export statements");
            Console.WriteLine("[exit] Exit program");
            Console.Write("\nEnter number here: ");

            button = Console.ReadLine();
            if (new[] { "1", "2", "3", "4", "5", "6", "exit" }.Contains(button))
            {
                return button;
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again");
            }

            Console.WriteLine("Unexpected error");
            return Menu();
        }

        static void WelcomeMessage()
        {
            Console.SetWindowSize(84, 16);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(new string('=', 84));
            Console.WriteLine(@"
             ░▒▓█▓▒░▒▓█▓▒░▒▓███████▓▒░ ░▒▓██████▓▒░ ░▒▓███████▓▒░▒▓█▓▒░░▒▓█▓▒░ 
             ░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ 
             ░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ 
             ░▒▓█▓▒░▒▓█▓▒░▒▓███████▓▒░░▒▓█▓▒░░▒▓█▓▒░░▒▓██████▓▒░░▒▓████████▓▒░ 
      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░ 
      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░ 
       ░▒▓██████▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓██████▓▒░░▒▓███████▓▒░░▒▓█▓▒░░▒▓█▓▒░ 
                                                                              
");
            Console.WriteLine($"{new string(' ', 20)}--  Ticketing System by BscDroid::F7SPBE  --");    // 45 char
            Console.WriteLine(new string('=', 84));
            Console.WriteLine();

            Thread.Sleep(3000);
            Console.Clear();
            Console.SetWindowSize(200, 50);
        }
    }
}

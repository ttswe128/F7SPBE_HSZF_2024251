using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;


namespace F7SPBE_HSZF_2024251.Application
{
    public class TaskService : ITaskService
    {
        private readonly ITaskDataProvider dp;

        public TaskService(ITaskDataProvider taskDataProvider)
        {
            dp = taskDataProvider;
        }

        public void ChangeStatus(int id, Model.EStatus status)
        {
            dp.ChangeStatus(id, status);
        }

        public void CreateTask(Task task)
        {
            dp.CreateTask(task);
        }

        public void CreateTasks(List<Task> tasks)
        {
            dp.CreateTasks(tasks);
        }

        public Task GetTask(int id)
        {
            return dp.GetTask(id);
        }

        public List<Task> GetTasks()
        {
            return dp.GetTasks();
        }

        public void UpdateTask(int id, Task task)
        {
            dp.UpdateTask(id, task);
        }
        
        public void UpdateTaskStatus(List<Project> projects, Programmer programmer)
        {
            var tasks = dp.GetTasksWithProgrammer(projects, programmer);

            if (!tasks.Any())
            {
                Console.WriteLine($"{programmer.Name} has no tasks assigned.");
                return;
            }

            Console.WriteLine($"Tasks assigned to {programmer.Name}:");
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

            dp.UpdateTask(selectedTask.Id, selectedTask);
        }
    }
}

using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using Task = F7SPBE_HSZF_2024251.Model.Task;
using EStatus = F7SPBE_HSZF_2024251.Model.EStatus;


namespace F7SPBE_HSZF_2024251.Persistence.MsSql
{
    public class TaskDataProvider : ITaskDataProvider
    {
        private readonly AppDbContext ctx;

        public TaskDataProvider(AppDbContext appDbContext) { ctx = appDbContext; }

        public void ChangeStatus(int id, EStatus status)
        {
            try
            {

                Task task = ctx.Tasks
                    .Include(t => t.Responsible)
                    .First(t => t.Id == id);

                task.Status = status;
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task with ID {id} couldn't be found");
            }
        }

        public void CreateTask(Task task)
        {
            ctx.Tasks.Add(task);
            ctx.SaveChanges();
        }

        public void CreateTasks(List<Task> tasks)
        {
            ctx.Tasks.AddRange(tasks);
            ctx.SaveChanges();
        }

        public Task GetTask(int id)
        {
            return ctx.Tasks
                .Include(t => t.Responsible)
                .First(t => t.Id == id);
        }

        public List<Task> GetTasks()
        {
            return ctx.Tasks
                .Include(t => t.Responsible)
                .ToList();
        }

        public void UpdateTask(int id, Task task)
        {
            var t = ctx.Tasks
                .Include(t => t.Responsible)
                .First(t => t.Id == id);

            if (t != null || !t.Id.Equals(task.Id)) throw new Exception("Task Update failed.");

            t = task;

            ctx.SaveChanges();
        }

        public void UpdateTaskStatus(List<Project> projects, Programmer programmer)
        {
            var tasks = projects
                .SelectMany(p => p.Tasks)
                .Where(t => t.Responsible == programmer)
                .ToList();

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
                Console.WriteLine("Enter new status: ");
                if (Enum.TryParse(Console.ReadLine(), true, out newStatus))
                {
                    selectedTask.Status = newStatus;
                    ctx.SaveChanges();
                    Console.WriteLine($"\nTask '{selectedTask.Name}' status updated to '{newStatus}' successfully!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid status. Please try again.");
                }
            }
        }
    }
}

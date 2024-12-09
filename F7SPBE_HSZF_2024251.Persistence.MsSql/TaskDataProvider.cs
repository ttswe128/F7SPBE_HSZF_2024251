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
                throw new Exception($"Task with ID {id} couldn't be found");
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

            if (t == null || !t.Id.Equals(task.Id)) throw new Exception("Task Update failed.");

            t = task;

            ctx.SaveChanges();
        }
        
        public List<Task> GetTasksWithProgrammer(List<Project> projects, Programmer programmer)
        {
            return ctx.Programmers
            .SelectMany(p => p.Tasks)
            .Where(t => t.Responsible == programmer)
            .ToList();
        }
    }
}

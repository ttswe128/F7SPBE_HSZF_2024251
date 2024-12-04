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
            dp.UpdateTaskStatus(projects, programmer);
        }
    }
}

using F7SPBE_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;


namespace F7SPBE_HSZF_2024251.Application
{
    public interface ITaskService
    {
        Task GetTask(int id);
        List<Task> GetTasks();
        void CreateTask(Task task);
        void CreateTasks(List<Task> tasks);
        void UpdateTask(int id, Task task);
        void ChangeStatus(int id, EStatus status);
        List<Task> GetTasksWithProgrammer(List<Project> projects, Programmer programmer);
        void UpdateTaskStatus(int id, Task task);
    }
}

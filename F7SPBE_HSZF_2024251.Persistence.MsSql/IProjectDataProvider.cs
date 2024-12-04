using F7SPBE_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project = F7SPBE_HSZF_2024251.Model.Project;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251.Persistence.MsSql
{
    public interface IProjectDataProvider
    {
        Project GetProject(int id);
        List<Project> GetProjects();
        List<Project> GetProjectsWithoutProgrammers();
        void CreateProject(Project project);
        void CreateProjects(List<Project> projects);
        void AssignProgrammersToProject(int projectId, List<int> programmerIds);
        List<Project> GetClosableProjects();
        void UpdateProject(int id, Project project);
        Project GetProjectOfProgrammer(Programmer programmer);
        Project AddTask(Project project, Programmer programmer, Task task);
        Project ModifyTask(Project project, Programmer programmer);
        List<Project> GetDelayedProjects();
        void SeedDb();
    }
}

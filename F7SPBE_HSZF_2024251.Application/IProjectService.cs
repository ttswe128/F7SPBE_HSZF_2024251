﻿using F7SPBE_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251.Application
{
    public interface IProjectService
    {
        Project GetProject(int id);
        List<Project> GetProjects();
        List<Project> GetProjectsWithoutProgrammers();
        void CreateProject(Project project);
        void CreateProjects(List<Project> projects);
        void AssignProgrammersToProject(int projectId, List<int> programmerIds);
        Project AssignProgrammersToProject(int id, Project projectSelected);
        List<Project> GetClosableProjects();
        void CloseProject(Project project);
        void UpdateProject(int id, Project project);
        List<Project> GetProjectsOfProgrammer(Programmer programmer);
        Project AddTask(Project project, Programmer programmer, Task task);
        Project ModifyTask(Project project, Task task, Programmer programmer);
        void ExportDelayedProjects();
        List<Task> GetTasksForModifying(Project project, Programmer programmer);
        void SeedDb();
    }
}

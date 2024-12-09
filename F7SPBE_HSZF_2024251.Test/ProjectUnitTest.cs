using F7SPBE_HSZF_2024251.Application;
using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251.Test
{
    [TestFixture]
    class ProjectUnitTest
    {
        IProjectService service;
        Mock<IProjectDataProvider> dp;

        [SetUp]
        public void Init()
        {
            var projects = Seeder.SeedProjects();

            dp = new Mock<IProjectDataProvider>();
            dp.Setup(m => m.GetProjects()).Returns(projects);
            service = new ProjectService(dp.Object);
        }

        [Test]
        public void GetProject()
        {
            // Arrange
            int id = 1;
            var projects = Seeder.SeedProjects();

            dp.Setup(m => m.GetProject(id)).Returns(projects[0]);

            // Act
            var p = service.GetProject(id);

            // Assert
            Assert.That(p, Is.EqualTo(projects[0]));
            dp.Verify(dp => dp.GetProject(id), Times.Once);
        }

        [Test]
        public void CreateProject()
        {
            // Arrange

            HashSet<DateTime> deadlines =
                [
                    new DateTime(2024, 12, 1),
                    new DateTime(2024, 12, 10),
                    new DateTime(2024, 12, 20)
                ];
            Project project = new("Project Test", "Test description", new DateTime(2024, 11, 1), deadlines, EStatus.STARTED);

            //dp.Setup(dp => dp.CreateProject(It.IsAny<Project>()))
            //                .Callback<Project>(p => capturedProject = p);

            // Act
            service.CreateProject(project);

            // Assert
            Assert.NotNull(project);
            Assert.That(project.Name, Is.EqualTo("Project Test"));
            Assert.That(project.Description, Is.EqualTo("Test description"));
            Assert.That(project.StartDate, Is.EqualTo(new DateTime(2024, 11, 1)));
            Assert.That(project.Deadlines, Is.EqualTo(deadlines));
            Assert.That(project.Status, Is.EqualTo(EStatus.STARTED));
        }

        [Test]
        public void AssignProgrammersToProject()
        {
            // Arrange
            var project = new Project(
                201,
                "Project Empty Test",
                "Assign tasks and programmers",
                EStatus.STARTED,
                DateTime.Now,
                new DateTime(2024, 12, 14),
                new HashSet<DateTime> { new DateTime(2024, 12, 10), new DateTime(2024, 10, 11) }
                );

            Project projectUpdate = project;


            List<Programmer> participants =
                [
                    new Programmer("Joe", "Intern", 2020),
                    new Programmer("Jane", "Lead Developer", 2019),
                    new Programmer("Bob", "Project Manager", 2020),
                ];

            projectUpdate.Participants = participants;

            // Act
            var result = service.AssignProgrammersToProject(projectUpdate.Id, projectUpdate);

            // Assert
            Assert.NotNull(result.Participants);
            Assert.That(result.Participants.Count() == 3);
        }

        [Test]
        public void CloseProject()
        {
            // Arrange
            var projects = Seeder.SeedProjects();
            var projectToClose = projects[4];

            dp.Setup(dp => dp.GetProject(5)).Returns(projectToClose);

            // Act
            service.CloseProject(projectToClose);

            // Assert
            Assert.That(projectToClose.Status, Is.EqualTo(EStatus.CLOSED));
            Assert.That(projectToClose.EndDate.Year, Is.EqualTo(DateTime.Now.Year));
        }

        [Test]
        public void AddTask()
        {
            // Arrange
            var projects = Seeder.SeedProjects();
            Project project = projects[2];
            var programmer = project.Participants.First(p => p.Name.Equals("Carmack"));

            Task newTask = new Task("Test Task", "Test description", programmer, "smol", EStatus.STARTED);;

            // Act
            var result = service.AddTask(project, programmer, newTask);

            // Assert
            Assert.IsNotNull(newTask);
            Assert.That(newTask.Name, Is.EqualTo("Test Task"));
            Assert.That(newTask.Description, Is.EqualTo("Test description"));
            Assert.That(newTask.Size, Is.EqualTo("smol"));
            Assert.That(newTask.Status, Is.EqualTo(EStatus.STARTED));
            Assert.That(newTask.Responsible, Is.EqualTo(programmer));

            dp.Verify(dp => dp.AddTask(project, programmer, It.IsAny<Task>()), Times.Once);

        }

        [Test]
        public void ModifyTask()
        {
            // Arrange
            var projects = Seeder.SeedProjects();
            Project project = projects[1];
            var programmer = project.Participants.First(p => p.Name.Equals("Julia"));
            var task = project.Tasks.First(t => t.Name == "Issue#57");

            Task modifiedTask = new Task("Issue#57", "Develop Backend tests", programmer, "smol", EStatus.STARTED);

            dp.Setup(dp => dp.ModifyTask(It.IsAny<Project>(), It.IsAny<Task>()))
                .Returns(project);

            // Act
            var result = service.ModifyTask(project, modifiedTask, programmer);

            // Assert
            var taskResult = result.Tasks.First(t => t.Name == "Issue#57");
            Assert.IsNotNull(taskResult);
            Assert.That(taskResult.Id, Is.EqualTo(task.Id));
            Assert.That(modifiedTask.Description, Is.EqualTo("Develop Backend tests"));
            Assert.That(modifiedTask.Size, Is.EqualTo("smol"));
            Assert.That(modifiedTask.Status, Is.EqualTo(EStatus.STARTED));
            Assert.That(modifiedTask.Responsible.Name, Is.EqualTo(programmer.Name));

            dp.Verify(dp => dp.ModifyTask(project, It.IsAny<Task>()), Times.Once);
        }
    }
}

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
            var input = @"Project Test
Test description
2024-12-01
2024-12-10
2024-12-20
done
IN_PROGRESS
";
            using var inputReader = new StringReader(input);
            using var outputWriter = new StringWriter();

            Console.SetIn(inputReader);
            Console.SetOut(outputWriter);

            Project capturedProject = null;
            dp.Setup(dp => dp.CreateProject(It.IsAny<Project>()))
                            .Callback<Project>(p => capturedProject = p);

            // Act
            service.CreateProject();

            // Assert
            Assert.NotNull(capturedProject);
            Assert.That(capturedProject.Name, Is.EqualTo("Project Test"));
            Assert.That(capturedProject.Description, Is.EqualTo("Test description"));
            Assert.That(capturedProject.StartDate, Is.EqualTo(new DateTime(2024, 12, 1)));
            Assert.That(capturedProject.Deadlines, Is.EqualTo(new List<DateTime>
            {
                new DateTime(2024, 12, 10),
                new DateTime(2024, 12, 20)
            }));
            Assert.That(capturedProject.Status, Is.EqualTo(EStatus.IN_PROGRESS));

            var output = outputWriter.ToString();
            Assert.IsTrue(output.Contains("Project added successfully."));
        }

        [Test]
        public void CreateProjectInvalidStartDate()
        {
            // Arrange

            var input = @"Project Test
Test description
NOT_A_DATE
2024-12-01
done
done
STARTED
";
            using var inputReader = new StringReader(input);
            using var outputWriter = new StringWriter();

            Console.SetIn(inputReader);
            Console.SetOut(outputWriter);

            // Act
            service.CreateProject();

            // Assert
            var output = outputWriter.ToString();
            Assert.IsTrue(output.Contains("Invalid date format. Please try again."));
        }

        [Test]
        public void AssignProgrammersToProject()
        {
            // Arrange
            var project = new Project(
            "Project Empty",
            "Assign tasks and programmers",
            EStatus.STARTED,
            DateTime.Now,
            new DateTime(2024, 12, 14),
            new HashSet<DateTime> { new DateTime(2024, 12, 10), new DateTime(2024, 10, 11) });

            List<Project> list = [project];

            dp.Setup(dp => dp.GetProjectsWithoutProgrammers()).Returns(list);

            var programmers = Seeder.SeedProgrammers();
            var input = @"1
1
2
done
";
            using var inputReader = new StringReader(input);
            using var outputWriter = new StringWriter();

            Console.SetIn(inputReader);
            Console.SetOut(outputWriter);

            // Act
            var result = service.AssignProgrammersToProject(programmers);

            // Assert
            Assert.NotNull(result.Participants);

            var output = outputWriter.ToString();
            Assert.IsTrue(output.Contains("Programmers successfully assigned to the project."));
        }

        [Test]
        public void CloseProject()
        {
            // Arrange
            var projects = Seeder.SeedProjects();
            var projectToClose = projects[4];

            dp.Setup(dp => dp.GetProject(5)).Returns(projectToClose);

            using var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            service.CloseProject(5);

            // Assert
            Assert.That(projectToClose.Status, Is.EqualTo(EStatus.CLOSED));
            Assert.That(projectToClose.EndDate.Year, Is.EqualTo(DateTime.Now.Year));

            dp.Verify(dp => dp.UpdateProject(5, projectToClose), Times.Once);
            var output = outputWriter.ToString();
            Assert.IsTrue(output.Contains("Project with ID 5 successfully closed."));
        }

        [Test]
        public void AddTask()
        {
            // Arrange
            var projects = Seeder.SeedProjects();
            Project project = projects[2];
            var programmer = project.Participants.First(p => p.Name.Equals("Carmack"));


            // Simulated user input
            var input = @"Test Task
Test description
smol
STARTED
";

            using var inputReader = new StringReader(input);
            using var outputWriter = new StringWriter();

            Console.SetIn(inputReader);
            Console.SetOut(outputWriter);

            Task newTask = null;
            dp.Setup(dp => dp.AddTask(It.IsAny<Project>(), It.IsAny<Programmer>(), It.IsAny<Task>()))
                .Callback<Project, Programmer, Task>((p, prog, task) => newTask = task)
                .Returns(project);

            // Act
            var result = service.AddTask(project, programmer);

            // Assert
            Assert.IsNotNull(newTask);
            Assert.That(newTask.Name, Is.EqualTo("Test Task"));
            Assert.That(newTask.Description, Is.EqualTo("Test description"));
            Assert.That(newTask.Size, Is.EqualTo("smol"));
            Assert.That(newTask.Status, Is.EqualTo(EStatus.STARTED));
            Assert.That(newTask.Responsible, Is.EqualTo(programmer));

            dp.Verify(dp => dp.AddTask(project, programmer, It.IsAny<Task>()), Times.Once);

            var output = outputWriter.ToString();
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

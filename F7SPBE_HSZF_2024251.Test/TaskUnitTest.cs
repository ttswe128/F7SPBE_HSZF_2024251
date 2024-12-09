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
    class TaskUnitTest
    {
        ITaskService service;
        Mock<ITaskDataProvider> dp;

        [SetUp]
        public void Init()
        {
            var tasks = Seeder.SeedTasks();

            dp = new Mock<ITaskDataProvider>();
            dp.Setup(dp => dp.GetTasks()).Returns(tasks);

            service = new TaskService(dp.Object);
        }

        [Test]
        public void GetTasks()
        {
            // Arrange
            int id = 1;
            Task task = new Task("Issue#1", "Project Setup", new Programmer(3, "Bob", "Project Manager", 2020), "Small", EStatus.STARTED);
            dp.Setup(m => m.GetTask(id)).Returns(task);

            // Act
            var t = service.GetTask(id);

            // Assert
            Assert.That(t, Is.EqualTo(task));
            dp.Verify(dp => dp.GetTask(id), Times.Once);
        }

        [Test]
        public void ChangeStatus()
        {
            // Arrange

            Programmer programmer = new Programmer(4, "Carmack", "Programmer", 1993);
            List<Task> tasks =
                [
                    new Task(2, "Issue#66", "Euclidian Geometry Setup", programmer, "Medium", EStatus.CLOSED),
                    new Task(5, "Issue#89", "Optimize Rendering Engine", programmer, "Large", EStatus.STARTED),
                    new Task(14, "Issue#666", "Launching Doom", programmer, "Diabolical", EStatus.CLOSED)
                ];

            var projects = Seeder.SeedProjects();

            var changedStatus = new Task(5, "Issue#89", "Optimize Rendering Engine", programmer, "Large", EStatus.CLOSED);

            // Act
            service.UpdateTaskStatus(5, changedStatus);

            // Assert
            dp.Verify(dp => dp.UpdateTask(5, It.Is<Task>(t => t.Status == EStatus.CLOSED)), Times.Once);
        }
    }
}

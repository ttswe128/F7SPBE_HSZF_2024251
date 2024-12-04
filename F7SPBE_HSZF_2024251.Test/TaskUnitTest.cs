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
            // Act
            // Assert
        }
    }
}

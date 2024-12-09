using F7SPBE_HSZF_2024251.Application;
using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F7SPBE_HSZF_2024251.Test
{
    [TestFixture]
    class ProgrammerUnitTest
    {
        IProgrammerService service;
        Mock<IProgrammerDataProvider> dp;

        [SetUp]
        public void Init()
        {
            var programmers = Seeder.SeedProgrammers();

            dp = new Mock<IProgrammerDataProvider>();
            dp.Setup(m => m.GetProgrammers()).Returns(programmers);

            service = new ProgrammerService(dp.Object);
        }

        [Test]
        public void GetProgrammer()
        {
            // Arrange
            int id = 1;
            Programmer programmer = new Programmer("Joe", "Intern", 2020);
            dp.Setup(m => m.GetProgrammer(id)).Returns(programmer);

            // Act
            var p = service.GetProgrammer(id);

            // Assert
            Assert.That(p, Is.EqualTo(programmer));
            dp.Verify(dp => dp.GetProgrammer(id), Times.Once);
        }

        [Test]
        public void GetProgrammers()
        {
            // Act
            List<Programmer> programmers = service.GetProgrammers();

            // Assert
            Assert.That(programmers.Count() == 10);
            dp.Verify(dp => dp.GetProgrammers(), Times.Once);
        }
    }
}

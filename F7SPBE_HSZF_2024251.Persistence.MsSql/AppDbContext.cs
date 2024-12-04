using F7SPBE_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251.Persistence.MsSql
{
    public class AppDbContext : DbContext
    {
        public DbSet<Programmer> Programmers { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }

        public AppDbContext() 
        {
            this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=jirosh_db;Integrated Security=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void Seeder(AppDbContext ctx)
        {
            List<Programmer> programmers =
            [
                new Programmer("Joe", "Intern", 2020),
                new Programmer("Jane", "Lead Developer", 2019),
                new Programmer("Bob", "Project Manager", 2020),
                new Programmer("Adam", "Team Lead", 2023),
                new Programmer("Carmack", "Programmer", 1993),
                new Programmer("Steven", "Junior Developer", 2011),
                new Programmer("Michael", "Software Engineer", 2005),
                new Programmer("John", "Developer", 2007),
                new Programmer("Julia", "Developer", 2018),
            ];


            List<Task> tasks =
            [
                new Task("Issue#1", "Project Setup", programmers[2], "Small" , EStatus.STARTED),
                new Task("Issue#34", "Write Jirosh Tasks", programmers[3], "Oversized", EStatus.IN_PROGRESS),
                new Task("Issue#666", "Euclidian Geometry Setup", programmers[4], "Medium", EStatus.CLOSED),
                new Task("Issue#12", "Create REST API for Task Management", programmers[1], "Large", EStatus.IN_PROGRESS),
                new Task("Issue#45", "Design Database Schema", programmers[5], "Medium", EStatus.STARTED),
                new Task("Issue#89", "Optimize Rendering Engine", programmers[4], "Large", EStatus.STARTED),
                new Task("Issue#23", "Fix Legacy Authentication Bug", programmers[7], "Small", EStatus.CLOSED),
                new Task("Issue#57", "Develop Frontend Dashboard", programmers[8], "Large", EStatus.IN_PROGRESS),
                new Task("Issue#102", "Write Unit Tests for API", programmers[6], "Small", EStatus.STARTED),
                new Task("Issue#321", "Document Project Requirements", programmers[2], "Medium", EStatus.CLOSED),
                new Task("Issue#404", "Resolve HTTP Error Handling", programmers[8], "Medium", EStatus.CLOSED),
                new Task("Issue#78", "Develop Mobile App Interface", programmers[3], "Large", EStatus.IN_PROGRESS),
                new Task("Issue#91", "Implement CI/CD Pipeline", programmers[6], "Medium", EStatus.STARTED)
            ];


            List<Project> projects =
            [
                new Project(
                    "Project Alpha",
                    "Develop a new authentication system",
                    EStatus.IN_PROGRESS,
                    new DateTime(2023, 1, 10),
                    new DateTime(2025, 2, 15),
                    new List<Programmer> { programmers[1], programmers[2] },
                    new List<Task> { tasks[0], tasks[1] },
                    new HashSet<DateTime> { new DateTime(2023, 6, 1), new DateTime(2023, 12, 1), new DateTime(2024, 2, 15) }
                ),

                new Project(
                    "Project Beta",
                    "Build a frontend dashboard for analytics",
                    EStatus.STARTED,
                    new DateTime(2023, 2, 5),
                    new DateTime(2023, 8, 30),
                    new List<Programmer> { programmers[3], programmers[8] },
                    new List<Task> { tasks[7], tasks[9] },
                    new HashSet<DateTime> { new DateTime(2023, 4, 1), new DateTime(2023, 7, 15) }
                ),

                new Project(
                    "Project Gamma",
                    "Optimize database performance",
                    EStatus.CLOSED,
                    new DateTime(2022, 9, 1),
                    new DateTime(2023, 1, 1),
                    new List<Programmer> { programmers[4], programmers[6] },
                    new List<Task> { tasks[4], tasks[8] },
                    new HashSet<DateTime> { new DateTime(2022, 10, 15), new DateTime(2022, 12, 15) }
                ),

                new Project(
                    "Project Delta",
                    "Develop a mobile application",
                    EStatus.IN_PROGRESS,
                    new DateTime(2024, 3, 1),
                    new DateTime(2024, 12, 15),
                    new List<Programmer> { programmers[5], programmers[7] },
                    new List<Task> { tasks[5], tasks[6] },
                    new HashSet<DateTime> { new DateTime(2024, 5, 1), new DateTime(2024, 9, 1) }
                ),

                new Project(
                    "Project Epsilon",
                    "Create an advanced AI model",
                    EStatus.STARTED,
                    new DateTime(2024, 5, 10),
                    new DateTime(2025, 2, 20),
                    new List<Programmer> { programmers[1], programmers[4] },
                    new List<Task> { tasks[2], tasks[9] },
                    new HashSet<DateTime> { new DateTime(2024, 9, 15), new DateTime(2025, 1, 15) }
                ),

                new Project(
                    "Project Zeta",
                    "Implement CI/CD pipelines",
                    EStatus.CLOSED,
                    new DateTime(2024, 6, 1),
                    new DateTime(2025, 11, 1),
                    new List<Programmer> { programmers[0], programmers[2] },
                    new List<Task> { tasks[1], tasks[4] },
                    new HashSet<DateTime> { new DateTime(2024, 8, 1), new DateTime(2024, 10, 1) }
                ),
            ];


            ctx.Programmers.AddRange(programmers);
            ctx.Tasks.AddRange(tasks);
            ctx.Projects.AddRange( projects );

            ctx.SaveChanges();
        }
    }
}

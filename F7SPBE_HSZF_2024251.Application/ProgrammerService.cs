using F7SPBE_HSZF_2024251.Model;
using F7SPBE_HSZF_2024251.Persistence.MsSql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F7SPBE_HSZF_2024251.Application
{
    public class ProgrammerService : IProgrammerService
    {
        private readonly IProgrammerDataProvider dp;

        public ProgrammerService(IProgrammerDataProvider provider)
        {
            this.dp = provider;
        }

        public List<Programmer> GetProgrammers()
        {
            return dp.GetProgrammers();
        }

        public Programmer GetProgrammer(int id)
        {
            return dp.GetProgrammer(id);
        }

        public void CreateProgrammer(Programmer programmer)
        {
            dp.CreateProgrammer(programmer);
        }

        public void CreateProgrammers(List<Programmer> programmers)
        {
            dp.CreateProgrammers(programmers);
        }

        public void UpdateProgrammer(int id, Programmer programmer)
        {
            dp.UpdateProgrammer(id, programmer);
        }

        public Programmer SignIn(List<Programmer> programmers)
        {
            Console.WriteLine("Please select the programmer you want sign in as!\n");
            for (int i = 0; i < programmers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {programmers[i].Name} - {programmers[i].Role} (Year of joining: {programmers[i].DateOfJoining})");
            }

            Console.Write("\nEnter your number here: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= programmers.Count)
            {
                return programmers[index - 1];
            }
            else
            {
                Console.WriteLine("\nThe number entered is incorrect. Please try again.");
                return null;
            }
        }

        public void ExportProgrammersPerformance()
        {

            var programmers = dp.GetProgrammers()
                .Where(p => p.Tasks.Any(t => t.Status == EStatus.CLOSED))
                .Select(p => new
                {
                    Name = p.Name,
                    Role = p.Role,
                    DateOfJoining = p.DateOfJoining,
                    Tasks = p.Tasks
                                .Where(t => t.Status == EStatus.CLOSED)
                                .Select(t => new
                                    {
                                        Name = t.Name,
                                        Description = t.Description,
                                        Size = t.Size,
                                        Status = t.Status.ToString()
                                    })
                                .ToList()
                })
                .ToList();

            string fileName = "_programmers_performance.json";

            File.WriteAllText(fileName, JsonConvert.SerializeObject(programmers));

            Console.WriteLine("\nProgrammers' performance successfully exported to JSON.");
        }
    }
}

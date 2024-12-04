using F7SPBE_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task = F7SPBE_HSZF_2024251.Model.Task;

namespace F7SPBE_HSZF_2024251.Persistence.MsSql
{
    public class ProgrammerDataProvider : IProgrammerDataProvider
    {
        private readonly AppDbContext ctx;

        public ProgrammerDataProvider(AppDbContext appDbContext)
        {
            ctx = appDbContext;
        }

        public List<Programmer> GetProgrammers()
        {
            return ctx.Programmers
                .Include(p => p.Projects)
                .Include(p => p.Tasks)
                .ToList();
        }

        public Programmer GetProgrammer(int id)
        {
            var p = ctx.Programmers
                .Include(p => p.Projects)
                .Include(p => p.Tasks)
                .First(p => p.Id == id);
            if (p == null)
            {
                throw new KeyNotFoundException($"Programmer with ID {id} not found.");
            }
            return p;
        }

        public void CreateProgrammer(Programmer programmer)
        {
            ctx.Programmers.Add(programmer);
            ctx.SaveChanges();
        }

        public void CreateProgrammers(List<Programmer> programmers)
        {
            ctx.Programmers.AddRange(programmers);
            ctx.SaveChanges();
        }

        public void UpdateProgrammer(int id, Programmer programmer)
        {
            Programmer toUpdate = ctx.Programmers.First(p => p.Id == id);

            if (toUpdate == null)
            {
                throw new KeyNotFoundException($"Programmer with ID {id} not found.");
            }

            toUpdate.Name = programmer.Name;
            toUpdate.Role = programmer.Role;
            toUpdate.DateOfJoining = programmer.DateOfJoining;

            ctx.SaveChanges();
        }

        public void DeleteProgrammer(int id)
        {

            Programmer toDelete = ctx.Programmers.FirstOrDefault(p => p.Id == id);
            if (toDelete == null)
            {
                throw new KeyNotFoundException($"Programmer with ID {id} not found.");
            }
            ctx.Programmers.Remove(toDelete);
            ctx.SaveChanges();
        }

    }
}
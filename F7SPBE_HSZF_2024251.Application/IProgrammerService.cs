﻿using F7SPBE_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F7SPBE_HSZF_2024251.Application
{
    public interface IProgrammerService
    {
        Programmer GetProgrammer(int id);
        List<Programmer> GetProgrammers();
        void CreateProgrammer(Programmer programmer);
        void CreateProgrammers(List<Programmer> programmers);
        void UpdateProgrammer(int id, Programmer programmer);
        Programmer SignIn(List<Programmer> programmers);
        void ExportProgrammersPerformance();
    }
}

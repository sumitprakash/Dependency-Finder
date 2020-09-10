using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DependencyFinderEngine;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();

            watch.Start();
            ExecuteSlnParser();
            watch.Stop();

            Console.WriteLine("First: " + watch.ElapsedMilliseconds);

            Console.ReadKey();
        }

        private static void ExecuteSlnParser()
        {
            SolutionParser parser1 = new SolutionParser(@"G:\Coding\Dotnet\ConsoleApp1\ConsoleApp1.sln");
            parser1.ParseSolution();
            var sln = parser1.Solution;
            CommonUtil.SlnToCSV(sln);
        }
    }
}

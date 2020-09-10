using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyFinderEngine.Model
{
    public class SolutionDetails
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<ProjectDetails> Projects { get; set; }

        public SolutionDetails(string slnPath)
        {
            Path = slnPath;
            Name = GetSlnName();
            Projects = new List<ProjectDetails>();
        }

        private string GetSlnName()
        {
            return new FileInfo(Path).Name;
        }
    }
}

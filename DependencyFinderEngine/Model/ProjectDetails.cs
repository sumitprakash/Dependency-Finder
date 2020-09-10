using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyFinderEngine.Model
{
    public class ProjectDetails
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<ReferenceDetails> References { get; set; }

        public ProjectDetails(string name, string path)
        {
            Name = name;
            Path = path;
            References = new List<ReferenceDetails>();
        }
    }
}

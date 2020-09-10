using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyFinderEngine.Model
{
    public class ReferenceDetails
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }

        public ReferenceDetails(string name, string path)
        {
            ExtractNameAndVersion(name);
            Path = path;
        }

        private void ExtractNameAndVersion(string name)
        {
            try
            {
                Version = string.Empty;
                Name = name;
                if (name.Contains(","))
                {
                    var parts = name.Split(new char[] { ',' }, 3);
                    Name = parts[0];
                    if (parts.Length > 1 && parts[1].ToLower().Contains("version"))
                        Version = parts[1].ToLower().Remove(0, 9);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

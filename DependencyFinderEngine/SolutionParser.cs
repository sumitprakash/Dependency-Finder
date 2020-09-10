using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP = Onion.SolutionParser.Parser;
using DependencyFinderEngine.Model;
using static DependencyFinderEngine.CommonUtil;

namespace DependencyFinderEngine
{
    public class SolutionParser
    {
        private SolutionDetails _solution;
        private string _slnPath;
        public string SlnPath { 
            get => _slnPath; 
            private set => _slnPath = value; 
        }
        
        private IEnumerable<SP.Model.Project> Projects { 
            get
            {
                if (IsSlnPathValid())
                    return SP.SolutionParser.Parse(SlnPath).Projects;
                else
                    return null;
            }
        }

        public SolutionDetails Solution 
        { 
            get => _solution; 
            private set => _solution = value; 
        }

        public SolutionParser(string slnPath)
        {
            this._slnPath = slnPath;
            if (IsSlnPathValid())
                Solution = new SolutionDetails(SlnPath);
            else
            {
                this._slnPath = string.Empty;
                throw new Exception("Invalid solution path");
            }
        }

        public void ParseSolution()
        {
            try
            {
                IterateOnProjects(Projects, new FileInfo(_slnPath).DirectoryName);
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error);
            }
        }

        private void IterateOnProjects(IEnumerable<SP.Model.Project> projects, string parentDir)
        {
            try
            {
                XmlNodeList refNodes = null;
                string projAbsPath = string.Empty;
                ProjectDetails project = null;

                foreach (var proj in projects)
                {
                    project = AddProjToSln(parentDir, proj, out projAbsPath);
                    refNodes = GetReferenceNodes(projAbsPath, proj);
                    ExtractReferenceInfo(refNodes, project);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ProjectDetails AddProjToSln(string parentDir, SP.Model.Project proj, out string projAbsPath)
        {
            try
            {
                projAbsPath = ResolvePath(parentDir, proj.Path);
                ProjectDetails project = new ProjectDetails(proj.Name, projAbsPath);
                Solution.Projects.Add(project);
                return project;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlNodeList GetReferenceNodes(string projAbsPath, SP.Model.Project proj)
        {
            try
            {
                var xml = File.ReadAllText(projAbsPath);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                return doc.DocumentElement.GetElementsByTagName("Reference");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ExtractReferenceInfo(XmlNodeList refNodes, ProjectDetails project)
        {
            try
            {
                var references = project.References;
                foreach (XmlElement refNode in refNodes)
                {
                    if (refNode.HasChildNodes)
                        references.Add(new ReferenceDetails(refNode.GetAttribute("Include"), ResolvePath(project.Path, refNode["HintPath"].InnerText)));
                    else
                        references.Add(new ReferenceDetails(refNode.GetAttribute("Include"), string.Empty));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ResolvePath(string path1, string path2)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path1))
                    path1 = string.Empty;
                else if (!path1.EndsWith("\\"))
                    path1 += "\\";

                if (string.IsNullOrWhiteSpace(path2))
                    path2 = string.Empty;

                return Path.GetFullPath(path1 + path2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsSlnPathValid()
        {
            try
            {
                return !string.IsNullOrWhiteSpace(_slnPath)
                       && File.Exists(_slnPath)
                       && _slnPath.ToLower().EndsWith(".sln");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

using DependencyFinderEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyFinderEngine
{
    public static class CommonUtil
    {
        private static string _logPath = Path.Combine(Environment.CurrentDirectory, "Log");
        private static string _outputPath = Path.Combine(Environment.CurrentDirectory, "Output");
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        public enum LogLevel
        {
            Information,
            Error
        }

        public static string OutputPath { get => _outputPath; private set => _outputPath = value; }
        public static string LogPath { get => _logPath; private set => _logPath = value; }

        static CommonUtil()
        {
            Directory.CreateDirectory(_logPath);
            Directory.CreateDirectory(OutputPath);
        }

        public static bool SlnToCSV(SolutionDetails solution)
        {
            try
            {
                using (StreamWriter stream = CreateOutputFile())
                {
                    stream.WriteLine(string.Format("{0},{1},{2},{3},{4}",
                                "Solution Name", "Project Name", "Reference Name", "Reference Version", "Reference Path"));
                    foreach (var project in solution.Projects)
                    {
                        foreach (var reference in project.References)
                        {
                            stream.WriteLine(string.Format("{0},{1},{2},{3},{4}", 
                                solution.Name, project.Name, reference.Name, reference.Version, reference.Path));
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error);
                return false;
            }
        }

        public static void Log(string message, LogLevel level)
        {
            _readWriteLock.EnterWriteLock();
            var log = DateTime.Now.ToString() + "\t" + level.ToString() + "\t" + message;
            using (StreamWriter sw = CreateLogFile())
            {
                sw.WriteLine(message);
            }
        }

        private static StreamWriter CreateLogFile()
        {
            try
            {
                var currentDate = DateTime.Today.ToString("d").Replace('/', '-');
                var logFileName = "Logs_" + currentDate + ".txt";
                return File.AppendText(Path.Combine(LogPath, logFileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static StreamWriter CreateOutputFile()
        {
            try
            {
                return File.CreateText(Path.Combine(OutputPath, "Report_" + GetUnixTimeStamp() + ".csv"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetUnixTimeStamp()
        {
            try
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return (DateTime.UtcNow - epoch).TotalSeconds.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

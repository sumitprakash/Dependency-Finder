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
            //ExecuteSlnParser();
            FilePermissionTest();
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

        private static void FilePermissionTest()
        {
            string FileLocation = @"D:\test1";
            DirectoryInfo info = new DirectoryInfo(FileLocation);
            CurrentUserSecurity currentUser = new CurrentUserSecurity();
            var x = currentUser.HasAccess(info, FileSystemRights.Read);
            //FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.Write, FileLocation);
            //var x = writePermission.ToXml();
            //var y = writePermission.ToString();
            //var z = writePermission.AllFiles;
            //HasWritePermissionOnDir(FileLocation);
        }

        private static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(Path.GetDirectoryName(path));
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true,
                                        typeof(SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                    continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    writeAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    writeDeny = true;
            }

            return writeAllow && !writeDeny;
        }

        public class CurrentUserSecurity
        {
            WindowsIdentity _currentUser;
            WindowsPrincipal _currentPrincipal;

            public CurrentUserSecurity()
            {
                _currentUser = WindowsIdentity.GetCurrent();
                _currentPrincipal = new WindowsPrincipal(_currentUser);
            }

            public bool HasAccess(DirectoryInfo directory, FileSystemRights right)
            {
                // Get the collection of authorization rules that apply to the directory.
                AuthorizationRuleCollection acl = directory.GetAccessControl()
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));
                return HasFileOrDirectoryAccess(right, acl);
            }

            public bool HasAccess(FileInfo file, FileSystemRights right)
            {
                // Get the collection of authorization rules that apply to the file.
                AuthorizationRuleCollection acl = file.GetAccessControl(AccessControlSections.Access)
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));
                return HasFileOrDirectoryAccess(right, acl);
            }

            private bool HasFileOrDirectoryAccess(FileSystemRights right,
                                                  AuthorizationRuleCollection acl)
            {
                bool allow = false;
                bool inheritedAllow = false;
                bool inheritedDeny = false;

                for (int i = 0; i < acl.Count; i++)
                {
                    var currentRule = (FileSystemAccessRule)acl[i];
                    // If the current rule applies to the current user.
                    if (_currentUser.User.Equals(currentRule.IdentityReference) ||
                        _currentPrincipal.IsInRole(
                                        (SecurityIdentifier)currentRule.IdentityReference))
                    {

                        if (currentRule.AccessControlType.Equals(AccessControlType.Deny))
                        {
                            if ((currentRule.FileSystemRights & right) == right)
                            {
                                if (currentRule.IsInherited)
                                {
                                    inheritedDeny = true;
                                }
                                else
                                { // Non inherited "deny" takes overall precedence.
                                    return false;
                                }
                            }
                        }
                        else if (currentRule.AccessControlType
                                                        .Equals(AccessControlType.Allow))
                        {
                            if ((currentRule.FileSystemRights & right) == right)
                            {
                                if (currentRule.IsInherited)
                                {
                                    inheritedAllow = true;
                                }
                                else
                                {
                                    allow = true;
                                }
                            }
                        }
                    }
                }

                if (allow)
                { // Non inherited "allow" takes precedence over inherited rules.
                    return true;
                }
                return inheritedAllow && !inheritedDeny;
            }
        }
    }
}

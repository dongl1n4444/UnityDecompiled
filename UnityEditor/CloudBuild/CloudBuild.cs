namespace UnityEditor.CloudBuild
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditor.Web;

    [InitializeOnLoad]
    internal class CloudBuild
    {
        static CloudBuild()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("unity/cloudbuild", new UnityEditor.CloudBuild.CloudBuild());
        }

        private Dictionary<string, string> DetectGit()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = this.RunCommand("git", "config --get remote.origin.url");
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            dictionary.Add("url", str);
            dictionary.Add("branch", this.RunCommand("git", "rev-parse --abbrev-ref HEAD"));
            dictionary.Add("root", this.RemoveProjectDirectory(this.RunCommand("git", "rev-parse --show-toplevel")));
            return dictionary;
        }

        private Dictionary<string, string> DetectMercurial()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = this.RunCommand("hg", "paths default");
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            dictionary.Add("url", str);
            dictionary.Add("branch", this.RunCommand("hg", "branch"));
            dictionary.Add("root", this.RemoveProjectDirectory(this.RunCommand("hg", "root")));
            return dictionary;
        }

        private Dictionary<string, string> DetectPerforce()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string environmentVariable = Environment.GetEnvironmentVariable("P4PORT");
            if (string.IsNullOrEmpty(environmentVariable))
            {
                return null;
            }
            dictionary.Add("url", environmentVariable);
            string str2 = Environment.GetEnvironmentVariable("P4CLIENT");
            if (!string.IsNullOrEmpty(str2))
            {
                dictionary.Add("workspace", str2);
            }
            return dictionary;
        }

        private Dictionary<string, string> DetectSubversion()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = this.RunCommand("svn", "info");
            if (str == null)
            {
                return null;
            }
            string[] strArray = str.Split(Environment.NewLine.ToCharArray());
            foreach (string str2 in strArray)
            {
                char[] separator = new char[] { ':' };
                string[] strArray3 = str2.Split(separator, 2);
                if (strArray3.Length == 2)
                {
                    if (strArray3[0].Equals("Repository Root"))
                    {
                        dictionary.Add("url", strArray3[1].Trim());
                    }
                    if (strArray3[0].Equals("URL"))
                    {
                        dictionary.Add("branch", strArray3[1].Trim());
                    }
                    if (strArray3[0].Equals("Working Copy Root Path"))
                    {
                        dictionary.Add("root", this.RemoveProjectDirectory(strArray3[1].Trim()));
                    }
                }
            }
            if (!dictionary.ContainsKey("url"))
            {
                return null;
            }
            return dictionary;
        }

        public Dictionary<string, Dictionary<string, string>> GetScmCandidates()
        {
            Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> dictionary2 = this.DetectGit();
            if (dictionary2 != null)
            {
                dictionary.Add("git", dictionary2);
            }
            Dictionary<string, string> dictionary3 = this.DetectMercurial();
            if (dictionary3 != null)
            {
                dictionary.Add("mercurial", dictionary3);
            }
            Dictionary<string, string> dictionary4 = this.DetectSubversion();
            if (dictionary4 != null)
            {
                dictionary.Add("subversion", dictionary4);
            }
            Dictionary<string, string> dictionary5 = this.DetectPerforce();
            if (dictionary5 != null)
            {
                dictionary.Add("perforce", dictionary5);
            }
            return dictionary;
        }

        private string RemoveProjectDirectory(string workingDirectory)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (currentDirectory.StartsWith(workingDirectory.Replace('/', '\\')))
            {
                workingDirectory = workingDirectory.Replace('/', '\\');
            }
            char[] trimChars = new char[] { Path.DirectorySeparatorChar };
            return currentDirectory.Replace(workingDirectory, "").Trim(trimChars);
        }

        private string RunCommand(string command, string arguments)
        {
            try
            {
                ProcessStartInfo si = new ProcessStartInfo(command) {
                    Arguments = arguments
                };
                Program program = new Program(si);
                program.Start();
                program.WaitForExit();
                if (program.ExitCode < 0)
                {
                    return null;
                }
                StringBuilder builder = new StringBuilder();
                foreach (string str2 in program.GetStandardOutput())
                {
                    builder.AppendLine(str2);
                }
                return builder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
            catch (Win32Exception)
            {
                return null;
            }
        }
    }
}


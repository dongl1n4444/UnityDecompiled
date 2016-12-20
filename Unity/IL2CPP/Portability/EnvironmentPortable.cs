﻿namespace Unity.IL2CPP.Portability
{
    using System;

    public class EnvironmentPortable
    {
        public static string GetApplicationDataDirectoryPortable()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static string[] GetCommandLineArgsPortable()
        {
            return Environment.GetCommandLineArgs();
        }

        public static string GetDesktopDirectoryPortable()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        public static string[] GetLogicalDrivesPortable()
        {
            return Environment.GetLogicalDrives();
        }

        public static string GetMyDocumentsPortable()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public static string GetPersonalFolderPortable()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public static string CommandLinePortable
        {
            get
            {
                return Environment.CommandLine;
            }
        }

        public static string CurrentDirectoryPortable
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        public static int ExitCodePortable
        {
            get
            {
                return Environment.ExitCode;
            }
            set
            {
                Environment.ExitCode = value;
            }
        }

        public static string MachineNamePortable
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public static Version OSVersionVersionPortable
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }

        public static string UserNamePortable
        {
            get
            {
                return Environment.UserName;
            }
        }
    }
}


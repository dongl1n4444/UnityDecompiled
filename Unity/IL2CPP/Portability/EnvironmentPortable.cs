namespace Unity.IL2CPP.Portability
{
    using System;

    public class EnvironmentPortable
    {
        public static string GetApplicationDataDirectoryPortable() => 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string[] GetCommandLineArgsPortable() => 
            Environment.GetCommandLineArgs();

        public static string GetDesktopDirectoryPortable() => 
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public static string[] GetLogicalDrivesPortable() => 
            Environment.GetLogicalDrives();

        public static string GetMyDocumentsPortable() => 
            Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static string GetPersonalFolderPortable() => 
            Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static string CommandLinePortable =>
            Environment.CommandLine;

        public static string CurrentDirectoryPortable =>
            Environment.CurrentDirectory;

        public static int ExitCodePortable
        {
            get => 
                Environment.ExitCode;
            set
            {
                Environment.ExitCode = value;
            }
        }

        public static string MachineNamePortable =>
            Environment.MachineName;

        public static Version OSVersionVersionPortable =>
            Environment.OSVersion.Version;

        public static string UserNamePortable =>
            Environment.UserName;
    }
}


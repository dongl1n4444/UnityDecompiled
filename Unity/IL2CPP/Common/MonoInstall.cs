namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;
    using System.Collections.Generic;

    public class MonoInstall
    {
        private readonly NPath _installRoot;
        public static MonoInstall BleedingEdge = new MonoInstall("MonoBleedingEdge");
        public static MonoInstall TwoSix = new MonoInstall("Mono");

        private MonoInstall(string installName)
        {
            this._installRoot = FindInstallRoot(installName);
        }

        private NPath _MonoExecutable(string program)
        {
            string[] append = new string[] { program + (!PlatformUtils.IsWindows() ? "" : ".bat") };
            return this._installRoot.Combine(append);
        }

        public static IEnumerable<string> ArgumentsForArchCommand(string monoExecutable, bool use64BitMono)
        {
            if (!PlatformUtils.IsOSX())
            {
                throw new InvalidOperationException("The platform should be OSX to call this method.");
            }
            return new List<string> { 
                (!use64BitMono ? "-i386" : "-x86_64"),
                monoExecutable
            };
        }

        private static NPath FindInstallRoot(string installName)
        {
            if (Il2CppDependencies.Available && !UnitySourceCode.Available)
            {
                return Il2CppDependencies.MonoInstall(installName);
            }
            string[] append = new string[] { installName };
            NPath path = CommonPaths.Il2CppRoot.Parent.Combine(append);
            if (path.DirectoryExists(""))
            {
                return path;
            }
            if (!UnitySourceCode.Available)
            {
                throw new InvalidOperationException("Unable to find mono install at: " + path);
            }
            string[] textArray2 = new string[] { "External/" + installName + "/builds/monodistribution" };
            return UnitySourceCode.Paths.UnityRoot.Combine(textArray2);
        }

        public static NPath MonoBleedingEdgeExecutableForArch(bool use64BitMono)
        {
            if (PlatformUtils.IsWindows() && use64BitMono)
            {
                string[] append = new string[] { "bin-x64", "mono.exe" };
                return BleedingEdge.Root.Combine(append);
            }
            if (PlatformUtils.IsOSX())
            {
                string[] textArray2 = new string[] { "bin", "mono" };
                return BleedingEdge.Root.Combine(textArray2);
            }
            return BleedingEdge.Cli;
        }

        private static string ProfileDirectoryName(DotNetProfile profile)
        {
            switch (profile)
            {
                case DotNetProfile.Unity:
                    return "unity";

                case DotNetProfile.Net20:
                    return "2.0";

                case DotNetProfile.Net45:
                    return "4.5";
            }
            throw new InvalidOperationException($"Unknown profile : {profile}");
        }

        public NPath ProfilePath(DotNetProfile profile)
        {
            string[] append = new string[] { "lib/mono" };
            string[] textArray2 = new string[] { ProfileDirectoryName(profile) };
            return this._installRoot.Combine(append).Combine(textArray2);
        }

        public static NPath SmartCompiler(DotNetProfile profile)
        {
            if (profile == DotNetProfile.Net45)
            {
                return BleedingEdge.Mcs;
            }
            return TwoSix._Gmcs;
        }

        public static MonoInstall SmartInstall(DotNetProfile profile)
        {
            if (profile == DotNetProfile.Net45)
            {
                return BleedingEdge;
            }
            return TwoSix;
        }

        public static NPath SmartProfilePath(DotNetProfile profile)
        {
            if (profile == DotNetProfile.Net45)
            {
                return BleedingEdge.ProfilePath(profile);
            }
            return TwoSix.ProfilePath(profile);
        }

        public NPath _Gmcs =>
            this._MonoExecutable("bin/gmcs");

        public NPath Cli =>
            this._MonoExecutable("bin/cli");

        public NPath ConfigPath
        {
            get
            {
                string[] append = new string[] { "etc" };
                return this._installRoot.Combine(append);
            }
        }

        public NPath Mcs =>
            this._MonoExecutable("bin/mcs");

        public NPath Root =>
            this._installRoot;
    }
}


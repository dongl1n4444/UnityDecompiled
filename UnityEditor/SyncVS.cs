namespace UnityEditor
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Utils;
    using UnityEditor.VisualStudioIntegration;
    using UnityEditorInternal;

    [InitializeOnLoad]
    internal class SyncVS : AssetPostprocessor
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<VisualStudioVersion, VisualStudioPath[]>, VisualStudioVersion> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<VisualStudioVersion, VisualStudioPath[]>, VisualStudioPath[]> <>f__am$cache1;
        [CompilerGenerated]
        private static System.Action <>f__mg$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private static Dictionary<VisualStudioVersion, VisualStudioPath[]> <InstalledVisualStudios>k__BackingField;
        private static bool s_AlreadySyncedThisDomainReload;
        private static readonly SolutionSynchronizer Synchronizer = new SolutionSynchronizer(Directory.GetParent(Application.dataPath).FullName, new SolutionSynchronizationSettings());

        static SyncVS()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new System.Action(SyncVS.SyncVisualStudioProjectIfItAlreadyExists);
            }
            EditorUserBuildSettings.activeBuildTargetChanged = (System.Action) Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, <>f__mg$cache0);
            try
            {
                InstalledVisualStudios = GetInstalledVisualStudios() as Dictionary<VisualStudioVersion, VisualStudioPath[]>;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error detecting Visual Studio installations: {0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
                InstalledVisualStudios = new Dictionary<VisualStudioVersion, VisualStudioPath[]>();
            }
            SetVisualStudioAsEditorIfNoEditorWasSet();
            UnityVSSupport.Initialize();
        }

        internal static bool CheckVisualStudioVersion(int major, int minor, int build)
        {
            int num = -1;
            int num2 = -1;
            if (major == 11)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing");
                if (key == null)
                {
                    return false;
                }
                foreach (string str in key.GetSubKeyNames())
                {
                    if (str.StartsWith("11.") && (str.Length > 3))
                    {
                        try
                        {
                            int num4 = Convert.ToInt32(str.Substring(3));
                            if (num4 > num)
                            {
                                num = num4;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (num < 0)
                {
                    return false;
                }
                RegistryKey key2 = key.OpenSubKey($"11.{num}\RuntimeDebug");
                if (key2 == null)
                {
                    return false;
                }
                string str2 = key2.GetValue("Version", null) as string;
                if (str2 == null)
                {
                    return false;
                }
                char[] separator = new char[] { '.' };
                string[] strArray2 = str2.Split(separator);
                if ((strArray2 == null) || (strArray2.Length < 3))
                {
                    return false;
                }
                try
                {
                    num2 = Convert.ToInt32(strArray2[2]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return ((num > minor) || ((num == minor) && (num2 >= build)));
        }

        public static void CreateIfDoesntExist()
        {
            if (!Synchronizer.SolutionExists())
            {
                Synchronizer.Sync();
            }
        }

        private static string DeriveProgramFilesSentinel()
        {
            char[] separator = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            string str = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Split(separator).LastOrDefault<string>();
            if (!string.IsNullOrEmpty(str))
            {
                int startIndex = str.LastIndexOf("(x86)");
                if (0 <= startIndex)
                {
                    str = str.Remove(startIndex);
                }
                return str.TrimEnd(new char[0]);
            }
            return "Program Files";
        }

        private static string DeriveVisualStudioPath(string debuggerPath)
        {
            string a = DeriveProgramFilesSentinel();
            string str2 = "Common7";
            bool flag = false;
            char[] separator = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            string[] strArray = debuggerPath.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            foreach (string str4 in strArray)
            {
                if (!flag && string.Equals(a, str4, StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                }
                else if (flag)
                {
                    folderPath = Path.Combine(folderPath, str4);
                    if (string.Equals(str2, str4, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
            string[] components = new string[] { folderPath, "IDE", "devenv.exe" };
            return Paths.Combine(components);
        }

        public static string FindBestVisualStudio()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = kvp => kvp.Key;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = kvp2 => kvp2.Value;
            }
            VisualStudioPath[] source = Enumerable.Select<KeyValuePair<VisualStudioVersion, VisualStudioPath[]>, VisualStudioPath[]>(Enumerable.OrderByDescending<KeyValuePair<VisualStudioVersion, VisualStudioPath[]>, VisualStudioVersion>(InstalledVisualStudios, <>f__am$cache0), <>f__am$cache1).FirstOrDefault<VisualStudioPath[]>();
            return ((source != null) ? source.Last<VisualStudioPath>().Path : null);
        }

        private static IDictionary<VisualStudioVersion, VisualStudioPath[]> GetInstalledVisualStudios()
        {
            Dictionary<VisualStudioVersion, VisualStudioPath[]> dictionary = new Dictionary<VisualStudioVersion, VisualStudioPath[]>();
            if (SolutionSynchronizationSettings.IsWindows)
            {
                IEnumerator enumerator = Enum.GetValues(typeof(VisualStudioVersion)).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        VisualStudioVersion current = (VisualStudioVersion) enumerator.Current;
                        if (current <= VisualStudioVersion.VisualStudio2015)
                        {
                            try
                            {
                                string environmentVariable = Environment.GetEnvironmentVariable($"VS{(int) current}0COMNTOOLS");
                                if (!string.IsNullOrEmpty(environmentVariable))
                                {
                                    string[] components = new string[] { environmentVariable, "..", "IDE", "devenv.exe" };
                                    string path = Paths.Combine(components);
                                    if (File.Exists(path))
                                    {
                                        dictionary[current] = new VisualStudioPath[] { new VisualStudioPath(path, "") };
                                        continue;
                                    }
                                }
                                environmentVariable = GetRegistryValue($"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\{(int) current}.0", "InstallDir");
                                if (string.IsNullOrEmpty(environmentVariable))
                                {
                                    environmentVariable = GetRegistryValue($"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\{(int) current}.0", "InstallDir");
                                }
                                if (!string.IsNullOrEmpty(environmentVariable))
                                {
                                    string[] textArray2 = new string[] { environmentVariable, "devenv.exe" };
                                    string str3 = Paths.Combine(textArray2);
                                    if (File.Exists(str3))
                                    {
                                        dictionary[current] = new VisualStudioPath[] { new VisualStudioPath(str3, "") };
                                        continue;
                                    }
                                }
                                environmentVariable = GetRegistryValue($"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\{(int) current}.0\Debugger", "FEQARuntimeImplDll");
                                if (!string.IsNullOrEmpty(environmentVariable))
                                {
                                    string str4 = DeriveVisualStudioPath(environmentVariable);
                                    if (!string.IsNullOrEmpty(str4) && File.Exists(str4))
                                    {
                                        dictionary[current] = new VisualStudioPath[] { new VisualStudioPath(DeriveVisualStudioPath(environmentVariable), "") };
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                string[] strArray = VisualStudioUtil.FindVisualStudioDevEnvPaths(15, "Microsoft.VisualStudio.Workload.ManagedGame");
                if ((strArray == null) || (strArray.Length <= 0))
                {
                    return dictionary;
                }
                VisualStudioPath[] pathArray = new VisualStudioPath[strArray.Length / 2];
                for (int i = 0; i < (strArray.Length / 2); i++)
                {
                    pathArray[i] = new VisualStudioPath(strArray[i * 2], strArray[(i * 2) + 1]);
                }
                dictionary[VisualStudioVersion.VisualStudio2017] = pathArray;
            }
            return dictionary;
        }

        private static string GetRegistryValue(string path, string key)
        {
            try
            {
                return (Registry.GetValue(path, key, null) as string);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static void OpenProjectFileUnlessInBatchMode()
        {
            if (!InternalEditorUtility.inBatchMode)
            {
                InternalEditorUtility.OpenFileAtLineExternal("", -1);
            }
        }

        private static bool PathsAreEquivalent(string aPath, string zPath)
        {
            if ((aPath == null) && (zPath == null))
            {
                return true;
            }
            if (string.IsNullOrEmpty(aPath) || string.IsNullOrEmpty(zPath))
            {
                return false;
            }
            aPath = Path.GetFullPath(aPath);
            zPath = Path.GetFullPath(zPath);
            StringComparison ordinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
            if (!SolutionSynchronizationSettings.IsOSX && !SolutionSynchronizationSettings.IsWindows)
            {
                ordinalIgnoreCase = StringComparison.Ordinal;
            }
            aPath = aPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            zPath = zPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return string.Equals(aPath, zPath, ordinalIgnoreCase);
        }

        public static void PostprocessSyncProject(string[] importedAssets, string[] addedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            Synchronizer.SyncIfNeeded(addedAssets.Union<string>(deletedAssets.Union<string>(movedAssets.Union<string>(movedFromAssetPaths))));
        }

        public static bool ProjectExists() => 
            Synchronizer.SolutionExists();

        private static void SetVisualStudioAsEditorIfNoEditorWasSet()
        {
            string str = EditorPrefs.GetString("kScriptsDefaultApp");
            string str2 = FindBestVisualStudio();
            if ((str == "") && (str2 != null))
            {
                EditorPrefs.SetString("kScriptsDefaultApp", str2);
            }
        }

        [UnityEditor.MenuItem("Assets/Open C# Project")]
        private static void SyncAndOpenSolution()
        {
            SyncSolution();
            OpenProjectFileUnlessInBatchMode();
        }

        public static void SyncIfFirstFileOpenSinceDomainLoad()
        {
            if (!s_AlreadySyncedThisDomainReload)
            {
                s_AlreadySyncedThisDomainReload = true;
                Synchronizer.Sync();
            }
        }

        public static void SyncSolution()
        {
            AssetDatabase.Refresh();
            Synchronizer.Sync();
        }

        public static void SyncVisualStudioProjectIfItAlreadyExists()
        {
            if (Synchronizer.SolutionExists())
            {
                Synchronizer.Sync();
            }
        }

        internal static Dictionary<VisualStudioVersion, VisualStudioPath[]> InstalledVisualStudios
        {
            [CompilerGenerated]
            get => 
                <InstalledVisualStudios>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <InstalledVisualStudios>k__BackingField = value;
            }
        }

        private class SolutionSynchronizationSettings : DefaultSolutionSynchronizationSettings
        {
            protected override string FrameworksPath() => 
                EditorApplication.applicationContentsPath;

            public override string GetProjectFooterTemplate(ScriptingLanguage language) => 
                EditorPrefs.GetString("VSProjectFooter", base.GetProjectFooterTemplate(language));

            public override string GetProjectHeaderTemplate(ScriptingLanguage language) => 
                EditorPrefs.GetString("VSProjectHeader", base.GetProjectHeaderTemplate(language));

            public override string[] Defines =>
                EditorUserBuildSettings.activeScriptCompilationDefines;

            public override string EditorAssemblyPath =>
                InternalEditorUtility.GetEditorAssemblyPath();

            public override string EngineAssemblyPath =>
                InternalEditorUtility.GetEngineAssemblyPath();

            internal static bool IsOSX =>
                (Environment.OSVersion.Platform == PlatformID.Unix);

            internal static bool IsWindows =>
                ((!IsOSX && (Path.DirectorySeparatorChar == '\\')) && (Environment.NewLine == "\r\n"));

            public override string SolutionTemplate =>
                EditorPrefs.GetString("VSSolutionText", base.SolutionTemplate);

            public override int VisualStudioVersion
            {
                get
                {
                    string externalScriptEditor = InternalEditorUtility.GetExternalScriptEditor();
                    if ((SyncVS.InstalledVisualStudios.ContainsKey(UnityEditor.VisualStudioVersion.VisualStudio2008) && (externalScriptEditor != string.Empty)) && SyncVS.PathsAreEquivalent(SyncVS.InstalledVisualStudios[UnityEditor.VisualStudioVersion.VisualStudio2008].Last<VisualStudioPath>().Path, externalScriptEditor))
                    {
                        return 9;
                    }
                    return 10;
                }
            }
        }
    }
}


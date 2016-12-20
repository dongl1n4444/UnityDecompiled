namespace Unity.IL2CPP.Building.ToolChains
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Common;

    public static class WinRTManifest
    {
        [CompilerGenerated]
        private static Func<NPath, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;

        public static void AddActivatableClasses(NPath manifestPath)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<NPath, string>(null, (IntPtr) <AddActivatableClasses>m__0);
            }
            string[] source = Enumerable.ToArray<string>(Enumerable.Select<NPath, string>(manifestPath.Parent.Files("*.winmd", false), <>f__am$cache0));
            if (Enumerable.Any<string>(source))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <AddActivatableClasses>m__1);
                }
                string str = Enumerable.Aggregate<string>(source, <>f__am$cache1);
                string contents = File.ReadAllText(manifestPath.ToString());
                int index = contents.IndexOf("<Extensions>");
                if (index != -1)
                {
                    int length = contents.IndexOf('\n', index) + 1;
                    contents = contents.Substring(0, length) + source + contents.Substring(length);
                }
                else
                {
                    int num3 = contents.IndexOf("</Package>");
                    if (num3 == -1)
                    {
                        throw new InvalidOperationException("Manifest is invalid: could not find end of Package element.");
                    }
                    StringBuilder builder = new StringBuilder();
                    builder.Append(contents.Substring(0, num3));
                    builder.AppendLine("  <Extensions>");
                    builder.Append(str);
                    builder.AppendLine("  </Extensions>");
                    builder.Append(contents.Substring(num3));
                    contents = builder.ToString();
                }
                File.WriteAllText(manifestPath.ToString(), contents);
            }
        }

        private static string ArchitectureToNameInManifest(Unity.IL2CPP.Building.Architecture architecture)
        {
            if (architecture is x86Architecture)
            {
                return "x86";
            }
            if (architecture is x64Architecture)
            {
                return "x64";
            }
            if (!(architecture is ARMv7Architecture))
            {
                throw new NotSupportedException(string.Format("Architecture {0} is not supported by WinRTManifest!", architecture));
            }
            return "arm";
        }

        private static string MakeActivatableExtensionElementForWinmd(NPath winmd)
        {
            ReaderParameters parameters = new ReaderParameters {
                ApplyWindowsRuntimeProjections = true
            };
            ModuleDefinition definition = ModuleDefinition.ReadModule(winmd.ToString(), parameters);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("    <Extension Category=\"windows.activatableClass.inProcessServer\">");
            builder.AppendLine("      <InProcessServer>");
            builder.AppendLine(string.Format("        <Path>{0}</Path>", winmd.ChangeExtension(".dll").FileName));
            foreach (TypeDefinition definition2 in definition.Types)
            {
                if (definition2.IsPublic && !definition2.IsValueType)
                {
                    builder.AppendLine(string.Format("        <ActivatableClass ActivatableClassId=\"{0}\" ThreadingModel=\"both\" />", definition2.FullName));
                }
            }
            builder.AppendLine("      </InProcessServer>");
            builder.AppendLine("    </Extension>");
            return builder.ToString();
        }

        public static void Write(NPath outputDirectory, string executableName, Unity.IL2CPP.Building.Architecture architecture)
        {
            string newValue = ArchitectureToNameInManifest(architecture);
            string[] append = new string[] { @"Unity.IL2CPP.WinRT\AppxManifest.xml" };
            NPath path = CommonPaths.Il2CppRoot.Combine(append);
            string[] textArray2 = new string[] { path.FileName };
            NPath path2 = outputDirectory.Combine(textArray2);
            string str2 = path.ReadAllText();
            path2.WriteAllText(str2.Replace("__ARCHITECTURE__", newValue).Replace("__EXECUTABLE_NAME__", executableName));
        }
    }
}


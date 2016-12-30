namespace UnityEditor.Scripting
{
    using ICSharpCode.NRefactory;
    using Mono.Cecil;
    using mscorlib;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity.DataContract;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Utils;
    using UnityEngine;
    using UnityEngine.Scripting.APIUpdating;

    internal class APIUpdaterHelper
    {
        private static string[] _ignoredAssemblies = new string[] { "^UnityScript$", @"^System\..*", "^mscorlib$" };
        [CompilerGenerated]
        private static Func<string, Exception, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, Exception, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<Type, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache7;

        private static string APIVersionArgument() => 
            (" --api-version " + Application.unityVersion + " ");

        private static string AssemblySearchPathArgument()
        {
            string[] textArray1 = new string[] { Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed"), ",+", Path.Combine(EditorApplication.applicationContentsPath, "UnityExtensions/Unity"), ",+", Application.dataPath };
            string str = string.Concat(textArray1);
            return (" -s \"" + str + "\"");
        }

        private static string ConfigurationProviderAssembliesPathArgument()
        {
            StringBuilder builder = new StringBuilder();
            foreach (PackageInfo info in ModuleManager.packageManager.unityExtensions)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = f => f.Value.type == PackageFileType.Dll;
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = pi => pi.Key;
                }
                foreach (string str in Enumerable.Select<KeyValuePair<string, PackageFileData>, string>(Enumerable.Where<KeyValuePair<string, PackageFileData>>(info.files, <>f__am$cache2), <>f__am$cache3))
                {
                    builder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(info.basePath, str)));
                }
            }
            string unityEditorManagedPath = GetUnityEditorManagedPath();
            builder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(unityEditorManagedPath, "UnityEngine.dll")));
            builder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(unityEditorManagedPath, "UnityEditor.dll")));
            return builder.ToString();
        }

        public static bool DoesAssemblyRequireUpgrade(string assemblyFullPath)
        {
            if (File.Exists(assemblyFullPath))
            {
                string str;
                string str2;
                if (!AssemblyHelper.IsManagedAssembly(assemblyFullPath))
                {
                    return false;
                }
                if (!MayContainUpdatableReferences(assemblyFullPath))
                {
                    return false;
                }
                int num = RunUpdatingProgram("AssemblyUpdater.exe", TimeStampArgument() + APIVersionArgument() + "--check-update-required -a " + CommandLineFormatter.PrepareFileName(assemblyFullPath) + AssemblySearchPathArgument() + ConfigurationProviderAssembliesPathArgument(), out str, out str2);
                Console.WriteLine("{0}{1}", str, str2);
                switch (num)
                {
                    case 0:
                    case 1:
                        return false;

                    case 2:
                        return true;
                }
                Debug.LogError(str + Environment.NewLine + str2);
            }
            return false;
        }

        private static Type FindExactTypeMatchingMovedType(string simpleOrQualifiedName)
        {
            <FindExactTypeMatchingMovedType>c__AnonStorey3 storey = new <FindExactTypeMatchingMovedType>c__AnonStorey3();
            Match match = Regex.Match(simpleOrQualifiedName, @"^(?:(?<namespace>.*)(?=\.)\.)?(?<typename>[a-zA-Z_0-9]+)$");
            if (!match.Success)
            {
                return null;
            }
            storey.typename = match.Groups["typename"].Value;
            storey.namespaceName = match.Groups["namespace"].Value;
            return FindTypeInLoadedAssemblies(new Func<Type, bool>(storey.<>m__0));
        }

        private static Type FindTypeInLoadedAssemblies(Func<Type, bool> predicate)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = assembly => !IsIgnoredAssembly(assembly.GetName());
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = a => GetValidTypesIn(a);
            }
            return Enumerable.FirstOrDefault<Type>(Enumerable.SelectMany<Assembly, Type>(Enumerable.Where<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), <>f__am$cache4), <>f__am$cache5), predicate);
        }

        private static Type FindTypeMatchingMovedTypeBasedOnNamespaceFromError(IEnumerable<string> lines)
        {
            string valueFromNormalizedMessage = GetValueFromNormalizedMessage(lines, "Line=");
            int line = (valueFromNormalizedMessage == null) ? -1 : int.Parse(valueFromNormalizedMessage);
            valueFromNormalizedMessage = GetValueFromNormalizedMessage(lines, "Column=");
            int column = (valueFromNormalizedMessage == null) ? -1 : int.Parse(valueFromNormalizedMessage);
            string path = GetValueFromNormalizedMessage(lines, "Script=");
            if (((line == -1) || (column == -1)) || (path == null))
            {
                return null;
            }
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StreamReader(stream));
                parser.Lexer.EvaluateConditionalCompilation = false;
                parser.Parse();
                string simpleOrQualifiedName = InvalidTypeOrNamespaceErrorTypeMapper.IsTypeMovedToNamespaceError(parser.CompilationUnit, line, column);
                if (simpleOrQualifiedName == null)
                {
                    return null;
                }
                return FindExactTypeMatchingMovedType(simpleOrQualifiedName);
            }
        }

        private static string GetUnityEditorManagedPath() => 
            Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed");

        private static IEnumerable<Type> GetValidTypesIn(Assembly a)
        {
            Type[] types;
            try
            {
                types = a.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                types = exception.Types;
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = t => t != null;
            }
            return Enumerable.Where<Type>(types, <>f__am$cache6);
        }

        private static string GetValueFromNormalizedMessage(IEnumerable<string> lines, string marker)
        {
            <GetValueFromNormalizedMessage>c__AnonStorey4 storey = new <GetValueFromNormalizedMessage>c__AnonStorey4 {
                marker = marker
            };
            string str = null;
            string str2 = Enumerable.FirstOrDefault<string>(lines, new Func<string, bool>(storey.<>m__0));
            if (str2 != null)
            {
                str = str2.Substring(storey.marker.Length).Trim();
            }
            return str;
        }

        private static bool IsError(int exitCode) => 
            ((exitCode & 0x80) != 0);

        private static bool IsIgnoredAssembly(AssemblyName assemblyName)
        {
            <IsIgnoredAssembly>c__AnonStorey1 storey = new <IsIgnoredAssembly>c__AnonStorey1 {
                name = assemblyName.Name
            };
            return Enumerable.Any<string>(_ignoredAssemblies, new Func<string, bool>(storey.<>m__0));
        }

        public static bool IsReferenceToMissingObsoleteMember(string namespaceName, string className)
        {
            bool flag;
            <IsReferenceToMissingObsoleteMember>c__AnonStorey0 storey = new <IsReferenceToMissingObsoleteMember>c__AnonStorey0 {
                className = className,
                namespaceName = namespaceName
            };
            try
            {
                flag = FindTypeInLoadedAssemblies(new Func<Type, bool>(storey.<>m__0)) != null;
            }
            catch (ReflectionTypeLoadException exception)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (acc, curr) => acc + "\r\n\t" + curr.Message;
                }
                throw new Exception(exception.Message + Enumerable.Aggregate<Exception, string>(exception.LoaderExceptions, "", <>f__am$cache0));
            }
            return flag;
        }

        public static bool IsReferenceToTypeWithChangedNamespace(string normalizedErrorMessage)
        {
            bool flag;
            try
            {
                Type type;
                char[] separator = new char[] { '\n' };
                string[] lines = normalizedErrorMessage.Split(separator);
                Type type1 = FindExactTypeMatchingMovedType(GetValueFromNormalizedMessage(lines, "EntityName="));
                if (type1 != null)
                {
                    type = type1;
                }
                else
                {
                    type = FindTypeMatchingMovedTypeBasedOnNamespaceFromError(lines);
                }
                flag = type != null;
            }
            catch (ReflectionTypeLoadException exception)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = (acc, curr) => acc + "\r\n\t" + curr.Message;
                }
                throw new Exception(exception.Message + Enumerable.Aggregate<Exception, string>(exception.LoaderExceptions, "", <>f__am$cache1));
            }
            return flag;
        }

        private static bool IsTargetFrameworkValidOnCurrentOS(AssemblyDefinition assembly)
        {
            System.Boolean ReflectorVariable0;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                if (assembly.HasCustomAttributes)
                {
                }
                ReflectorVariable0 = true;
            }
            else
            {
                ReflectorVariable0 = false;
            }
            return (ReflectorVariable0 ? !((<>f__am$cache7 == null) && Enumerable.Any<CustomAttribute>(assembly.CustomAttributes, <>f__am$cache7)) : true);
        }

        private static bool IsUpdateable(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (customAttributes.Length != 1)
            {
                return false;
            }
            ObsoleteAttribute attribute = (ObsoleteAttribute) customAttributes[0];
            return attribute.Message.Contains("UnityUpgradable");
        }

        internal static bool MayContainUpdatableReferences(string assemblyPath)
        {
            using (FileStream stream = File.Open(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stream);
                if (assembly.Name.IsWindowsRuntime)
                {
                    return false;
                }
                if (!IsTargetFrameworkValidOnCurrentOS(assembly))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool NamespaceHasChanged(Type type, string namespaceName)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(MovedFromAttribute), false);
            if (customAttributes.Length != 1)
            {
                return false;
            }
            if (string.IsNullOrEmpty(namespaceName))
            {
                return true;
            }
            MovedFromAttribute attribute = (MovedFromAttribute) customAttributes[0];
            return (attribute.Namespace == namespaceName);
        }

        private static string ResolveAssemblyPath(string assemblyPath) => 
            CommandLineFormatter.PrepareFileName(assemblyPath);

        public static void Run(string commaSeparatedListOfAssemblies)
        {
            char[] separator = new char[] { ',' };
            string[] source = commaSeparatedListOfAssemblies.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            object[] args = new object[] { source.Count<string>() };
            APIUpdaterLogger.WriteToFile("Started to update {0} assemblie(s)", args);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (string str in source)
            {
                if (AssemblyHelper.IsManagedAssembly(str))
                {
                    string str2;
                    string str3;
                    string str4 = ResolveAssemblyPath(str);
                    int exitCode = RunUpdatingProgram("AssemblyUpdater.exe", "-u -a " + str4 + APIVersionArgument() + AssemblySearchPathArgument() + ConfigurationProviderAssembliesPathArgument(), out str2, out str3);
                    if (str2.Length > 0)
                    {
                        object[] objArray2 = new object[] { str4, str2 };
                        APIUpdaterLogger.WriteToFile("Assembly update output ({0})\r\n{1}", objArray2);
                    }
                    if (IsError(exitCode))
                    {
                        object[] objArray3 = new object[] { exitCode, str3 };
                        APIUpdaterLogger.WriteErrorToConsole("Error {0} running AssemblyUpdater. Its output is: `{1}`", objArray3);
                    }
                }
            }
            object[] objArray4 = new object[] { stopwatch.Elapsed.TotalSeconds };
            APIUpdaterLogger.WriteToFile("Update finished in {0}s", objArray4);
        }

        private static int RunUpdatingProgram(string executable, string arguments, out string stdOut, out string stdErr)
        {
            string str = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
            ManagedProgram program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, str, arguments, false, null);
            program.LogProcessStartInfo();
            program.Start();
            program.WaitForExit();
            stdOut = program.GetStandardOutputAsString();
            stdErr = string.Join("\r\n", program.GetErrorOutput());
            return program.ExitCode;
        }

        private static bool TargetsWindowsSpecificFramework(CustomAttribute targetFrameworkAttr)
        {
            <TargetsWindowsSpecificFramework>c__AnonStorey2 storey = new <TargetsWindowsSpecificFramework>c__AnonStorey2();
            if (!targetFrameworkAttr.AttributeType.FullName.Contains("System.Runtime.Versioning.TargetFrameworkAttribute"))
            {
                return false;
            }
            storey.regex = new Regex(@"\.NETCore|\.NETPortable");
            return Enumerable.Any<CustomAttributeArgument>(targetFrameworkAttr.ConstructorArguments, new Func<CustomAttributeArgument, bool>(storey.<>m__0));
        }

        private static string TimeStampArgument() => 
            (" --timestamp " + DateTime.Now.Ticks + " ");

        [CompilerGenerated]
        private sealed class <FindExactTypeMatchingMovedType>c__AnonStorey3
        {
            internal string namespaceName;
            internal string typename;

            internal bool <>m__0(Type t) => 
                ((t.Name == this.typename) && APIUpdaterHelper.NamespaceHasChanged(t, this.namespaceName));
        }

        [CompilerGenerated]
        private sealed class <GetValueFromNormalizedMessage>c__AnonStorey4
        {
            internal string marker;

            internal bool <>m__0(string l) => 
                l.StartsWith(this.marker);
        }

        [CompilerGenerated]
        private sealed class <IsIgnoredAssembly>c__AnonStorey1
        {
            internal string name;

            internal bool <>m__0(string candidate) => 
                Regex.IsMatch(this.name, candidate);
        }

        [CompilerGenerated]
        private sealed class <IsReferenceToMissingObsoleteMember>c__AnonStorey0
        {
            internal string className;
            internal string namespaceName;

            internal bool <>m__0(Type t) => 
                (((t.Name == this.className) && (t.Namespace == this.namespaceName)) && APIUpdaterHelper.IsUpdateable(t));
        }

        [CompilerGenerated]
        private sealed class <TargetsWindowsSpecificFramework>c__AnonStorey2
        {
            internal Regex regex;

            internal bool <>m__0(CustomAttributeArgument arg) => 
                ((arg.Type.FullName == typeof(string).FullName) && this.regex.IsMatch((string) arg.Value));
        }
    }
}


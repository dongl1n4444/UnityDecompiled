namespace UnityEditor
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class AssemblyReferenceChecker
    {
        private HashSet<AssemblyDefinition> _assemblyDefinitions = new HashSet<AssemblyDefinition>();
        private readonly HashSet<string> _assemblyFileNames = new HashSet<string>();
        private readonly HashSet<string> _definedMethods = new HashSet<string>();
        private float _progressValue = 0f;
        private readonly HashSet<string> _referencedMethods = new HashSet<string>();
        private HashSet<string> _referencedTypes = new HashSet<string>();
        private DateTime _startTime = DateTime.MinValue;
        private Action _updateProgressAction;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <HasMouseEvent>k__BackingField;

        public AssemblyReferenceChecker()
        {
            this.HasMouseEvent = false;
            this._updateProgressAction = new Action(this, (IntPtr) this.DisplayProgress);
        }

        public static AssemblyReferenceChecker AssemblyReferenceCheckerWithUpdateProgressAction(Action action)
        {
            return new AssemblyReferenceChecker { _updateProgressAction = action };
        }

        private static DefaultAssemblyResolver AssemblyResolverFor(string path)
        {
            DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
            if (File.Exists(path) || Directory.Exists(path))
            {
                if ((File.GetAttributes(path) & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    path = Path.GetDirectoryName(path);
                }
                resolver.AddSearchDirectory(Path.GetFullPath(path));
            }
            return resolver;
        }

        internal void CollectReferencedAndDefinedMethods(TypeDefinition type)
        {
            if (this._updateProgressAction != null)
            {
                this._updateProgressAction.Invoke();
            }
            foreach (TypeDefinition definition in type.NestedTypes)
            {
                this.CollectReferencedAndDefinedMethods(definition);
            }
            foreach (MethodDefinition definition2 in type.Methods)
            {
                if (definition2.HasBody)
                {
                    foreach (Instruction instruction in definition2.Body.Instructions)
                    {
                        if (OpCodes.Call == instruction.OpCode)
                        {
                            this._referencedMethods.Add(instruction.Operand.ToString());
                        }
                    }
                    this._definedMethods.Add(definition2.ToString());
                    this.HasMouseEvent |= this.MethodIsMouseEvent(definition2);
                }
            }
        }

        private void CollectReferencedAndDefinedMethods(IEnumerable<AssemblyDefinition> assemblyDefinitions)
        {
            foreach (AssemblyDefinition definition in assemblyDefinitions)
            {
                foreach (TypeDefinition definition2 in definition.MainModule.Types)
                {
                    this.CollectReferencedAndDefinedMethods(definition2);
                }
            }
        }

        public void CollectReferences(string path, bool collectMethods, float progressValue, bool ignoreSystemDlls)
        {
            this._progressValue = progressValue;
            this._assemblyDefinitions = new HashSet<AssemblyDefinition>();
            string[] strArray = !Directory.Exists(path) ? new string[0] : Directory.GetFiles(path);
            DefaultAssemblyResolver resolver = AssemblyResolverFor(path);
            foreach (string str in strArray)
            {
                if (Path.GetExtension(str) == ".dll")
                {
                    ReaderParameters parameters = new ReaderParameters {
                        AssemblyResolver = resolver
                    };
                    AssemblyDefinition item = AssemblyDefinition.ReadAssembly(str, parameters);
                    if (!ignoreSystemDlls || !IsIgnoredSystemDll(item.Name.Name))
                    {
                        this._assemblyFileNames.Add(Path.GetFileName(str));
                        this._assemblyDefinitions.Add(item);
                    }
                }
            }
            AssemblyDefinition[] assemblies = Enumerable.ToArray<AssemblyDefinition>(this._assemblyDefinitions);
            this._referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(assemblies);
            if (collectMethods)
            {
                this.CollectReferencedAndDefinedMethods(assemblies);
            }
        }

        public void CollectReferencesFromRoots(string dir, IEnumerable<string> roots, bool collectMethods, float progressValue, bool ignoreSystemDlls)
        {
            this._progressValue = progressValue;
            this.CollectReferencesFromRootsRecursive(dir, roots, ignoreSystemDlls);
            AssemblyDefinition[] assemblies = Enumerable.ToArray<AssemblyDefinition>(this._assemblyDefinitions);
            this._referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(assemblies);
            if (collectMethods)
            {
                this.CollectReferencedAndDefinedMethods(assemblies);
            }
        }

        private void CollectReferencesFromRootsRecursive(string dir, IEnumerable<string> roots, bool ignoreSystemDlls)
        {
            DefaultAssemblyResolver resolver = AssemblyResolverFor(dir);
            foreach (string str in roots)
            {
                string fileName = Path.Combine(dir, str);
                if (!this._assemblyFileNames.Contains(str))
                {
                    ReaderParameters parameters = new ReaderParameters {
                        AssemblyResolver = resolver
                    };
                    AssemblyDefinition item = AssemblyDefinition.ReadAssembly(fileName, parameters);
                    if (!ignoreSystemDlls || !IsIgnoredSystemDll(item.Name.Name))
                    {
                        this._assemblyFileNames.Add(str);
                        this._assemblyDefinitions.Add(item);
                        foreach (AssemblyNameReference reference in item.MainModule.AssemblyReferences)
                        {
                            string str3 = reference.Name + ".dll";
                            if (!this._assemblyFileNames.Contains(str3))
                            {
                                string[] textArray1 = new string[] { str3 };
                                this.CollectReferencesFromRootsRecursive(dir, textArray1, ignoreSystemDlls);
                            }
                        }
                    }
                }
            }
        }

        private void DisplayProgress()
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - this._startTime);
            string[] strArray = new string[] { "Fetching assembly references", "Building list of referenced assemblies..." };
            if (span.TotalMilliseconds >= 100.0)
            {
                if (EditorUtility.DisplayCancelableProgressBar(strArray[0], strArray[1], this._progressValue))
                {
                    throw new OperationCanceledException();
                }
                this._startTime = DateTime.Now;
            }
        }

        public AssemblyDefinition[] GetAssemblyDefinitions()
        {
            return Enumerable.ToArray<AssemblyDefinition>(this._assemblyDefinitions);
        }

        public string[] GetAssemblyFileNames()
        {
            return Enumerable.ToArray<string>(this._assemblyFileNames);
        }

        public static bool GetScriptsHaveMouseEvents(string path)
        {
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            checker.CollectReferences(path, true, 0f, true);
            return checker.HasMouseEvent;
        }

        public bool HasDefinedMethod(string methodName)
        {
            <HasDefinedMethod>c__AnonStorey1 storey = new <HasDefinedMethod>c__AnonStorey1 {
                methodName = methodName
            };
            return Enumerable.Any<string>(this._definedMethods, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        public bool HasReferenceToMethod(string methodName)
        {
            <HasReferenceToMethod>c__AnonStorey0 storey = new <HasReferenceToMethod>c__AnonStorey0 {
                methodName = methodName
            };
            return Enumerable.Any<string>(this._referencedMethods, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        public bool HasReferenceToType(string typeName)
        {
            <HasReferenceToType>c__AnonStorey2 storey = new <HasReferenceToType>c__AnonStorey2 {
                typeName = typeName
            };
            return Enumerable.Any<string>(this._referencedTypes, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        private bool InheritsFromMonoBehaviour(TypeReference type)
        {
            if ((type.Namespace == "UnityEngine") && (type.Name == "MonoBehaviour"))
            {
                return true;
            }
            TypeDefinition definition = type.Resolve();
            return ((definition.BaseType != null) && this.InheritsFromMonoBehaviour(definition.BaseType));
        }

        public static bool IsIgnoredSystemDll(string name)
        {
            return (((name.StartsWith("System") || name.Equals("UnityEngine")) || name.Equals("UnityEngine.Networking")) || name.Equals("Mono.Posix"));
        }

        private bool MethodIsMouseEvent(MethodDefinition method)
        {
            if (((((method.Name != "OnMouseDown") && (method.Name != "OnMouseDrag")) && ((method.Name != "OnMouseEnter") && (method.Name != "OnMouseExit"))) && ((method.Name != "OnMouseOver") && (method.Name != "OnMouseUp"))) && (method.Name != "OnMouseUpAsButton"))
            {
                return false;
            }
            if (method.Parameters.Count != 0)
            {
                return false;
            }
            if (!this.InheritsFromMonoBehaviour(method.DeclaringType))
            {
                return false;
            }
            return true;
        }

        public string WhoReferencesClass(string klass, bool ignoreSystemDlls)
        {
            <WhoReferencesClass>c__AnonStorey3 storey = new <WhoReferencesClass>c__AnonStorey3 {
                klass = klass
            };
            foreach (AssemblyDefinition definition in this._assemblyDefinitions)
            {
                if (!ignoreSystemDlls || !IsIgnoredSystemDll(definition.Name.Name))
                {
                    AssemblyDefinition[] assemblies = new AssemblyDefinition[] { definition };
                    if (Enumerable.Any<string>(MonoAOTRegistration.BuildReferencedTypeList(assemblies), new Func<string, bool>(storey, (IntPtr) this.<>m__0)))
                    {
                        return definition.Name.Name;
                    }
                }
            }
            return null;
        }

        public bool HasMouseEvent { get; private set; }

        [CompilerGenerated]
        private sealed class <HasDefinedMethod>c__AnonStorey1
        {
            internal string methodName;

            internal bool <>m__0(string item)
            {
                return item.Contains(this.methodName);
            }
        }

        [CompilerGenerated]
        private sealed class <HasReferenceToMethod>c__AnonStorey0
        {
            internal string methodName;

            internal bool <>m__0(string item)
            {
                return item.Contains(this.methodName);
            }
        }

        [CompilerGenerated]
        private sealed class <HasReferenceToType>c__AnonStorey2
        {
            internal string typeName;

            internal bool <>m__0(string item)
            {
                return item.StartsWith(this.typeName);
            }
        }

        [CompilerGenerated]
        private sealed class <WhoReferencesClass>c__AnonStorey3
        {
            internal string klass;

            internal bool <>m__0(string item)
            {
                return item.StartsWith(this.klass);
            }
        }
    }
}


namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class AssemblyDependencyComparer : IComparer<AssemblyDefinition>
    {
        private readonly Dictionary<string, int> _depthForAssembly;
        private static HashSet<string> _specialAssemblies;
        [Inject]
        public static IAssemblyDependencies AssemblyDependencies;

        static AssemblyDependencyComparer()
        {
            HashSet<string> set = new HashSet<string> { 
                "Assembly-CSharp",
                "Assembly-CSharp-firstpass",
                "Assembly-UnityScript",
                "Assembly-UnityScript-firstpass"
            };
            _specialAssemblies = set;
        }

        public AssemblyDependencyComparer(Dictionary<string, int> depthForAssembly)
        {
            this._depthForAssembly = depthForAssembly;
        }

        public int Compare(AssemblyDefinition left, AssemblyDefinition right)
        {
            if (left == right)
            {
                return 0;
            }
            if (this.FirstRecursivelyReferencesSecond(left, right, new HashSet<string>()))
            {
                return 1;
            }
            if (this.FirstRecursivelyReferencesSecond(right, left, new HashSet<string>()))
            {
                return -1;
            }
            if (this.IsSpecialAssembly(right) && !this.IsSpecialAssembly(left))
            {
                return -1;
            }
            if (this.IsSpecialAssembly(left) && !this.IsSpecialAssembly(right))
            {
                return 1;
            }
            if (this._depthForAssembly.ContainsKey(left.Name.Name) && this._depthForAssembly.ContainsKey(right.Name.Name))
            {
                int num2 = this._depthForAssembly[right.Name.Name] - this._depthForAssembly[left.Name.Name];
                if (num2 != 0)
                {
                    return num2;
                }
            }
            return string.Compare(left.Name.Name, right.Name.Name, StringComparison.Ordinal);
        }

        private bool FirstRecursivelyReferencesSecond(AssemblyDefinition first, AssemblyDefinition second, HashSet<string> assemblyNamesAlreadyVisited)
        {
            if (first.References(second))
            {
                return true;
            }
            if (!assemblyNamesAlreadyVisited.Contains(first.Name.Name))
            {
                foreach (AssemblyDefinition definition in AssemblyDependencies.GetReferencedAssembliesFor(first))
                {
                    assemblyNamesAlreadyVisited.Add(first.Name.Name);
                    bool flag2 = this.FirstRecursivelyReferencesSecond(definition, second, assemblyNamesAlreadyVisited);
                    assemblyNamesAlreadyVisited.Remove(first.Name.Name);
                    if (flag2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsSpecialAssembly(AssemblyDefinition assembly) => 
            _specialAssemblies.Contains(assembly.Name.Name);

        public static Dictionary<string, int> MaximumDepthForEachAssembly(IEnumerable<AssemblyDefinition> assemblies)
        {
            Dictionary<string, int> depth = new Dictionary<string, int>();
            foreach (AssemblyDefinition definition in assemblies)
            {
                MaximumDepthForEachAssemblyRecursive(definition, new HashSet<string>(), depth, 0);
            }
            return depth;
        }

        private static void MaximumDepthForEachAssemblyRecursive(AssemblyDefinition assembly, HashSet<string> assemblyNamesAlreadyVisited, Dictionary<string, int> depth, int currentDepth)
        {
            if (!assemblyNamesAlreadyVisited.Contains(assembly.Name.Name))
            {
                if (depth.ContainsKey(assembly.Name.Name))
                {
                    depth[assembly.Name.Name] = Math.Max(depth[assembly.Name.Name], currentDepth);
                }
                else
                {
                    depth[assembly.Name.Name] = currentDepth;
                }
                foreach (AssemblyDefinition definition in AssemblyDependencies.GetReferencedAssembliesFor(assembly))
                {
                    assemblyNamesAlreadyVisited.Add(assembly.Name.Name);
                    MaximumDepthForEachAssemblyRecursive(definition, assemblyNamesAlreadyVisited, depth, currentDepth + 1);
                    assemblyNamesAlreadyVisited.Remove(assembly.Name.Name);
                }
            }
        }
    }
}


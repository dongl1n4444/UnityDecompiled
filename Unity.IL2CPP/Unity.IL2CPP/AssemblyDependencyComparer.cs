using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP
{
	public class AssemblyDependencyComparer : IComparer<AssemblyDefinition>
	{
		private static HashSet<string> _specialAssemblies = new HashSet<string>
		{
			"Assembly-CSharp",
			"Assembly-CSharp-firstpass",
			"Assembly-UnityScript",
			"Assembly-UnityScript-firstpass"
		};

		private readonly Dictionary<string, int> _depthForAssembly;

		public AssemblyDependencyComparer(Dictionary<string, int> depthForAssembly)
		{
			this._depthForAssembly = depthForAssembly;
		}

		public static Dictionary<string, int> MaximumDepthForEachAssembly(IEnumerable<AssemblyDefinition> assemblies)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (AssemblyDefinition current in assemblies)
			{
				AssemblyDependencyComparer.MaximumDepthForEachAssemblyRecursive(current, new HashSet<string>(), dictionary, 0);
			}
			return dictionary;
		}

		public int Compare(AssemblyDefinition left, AssemblyDefinition right)
		{
			int result;
			if (left == right)
			{
				result = 0;
			}
			else if (this.FirstRecursivelyReferencesSecond(left, right, new HashSet<string>()))
			{
				result = 1;
			}
			else if (this.FirstRecursivelyReferencesSecond(right, left, new HashSet<string>()))
			{
				result = -1;
			}
			else if (this.IsSpecialAssembly(right) && !this.IsSpecialAssembly(left))
			{
				result = -1;
			}
			else if (this.IsSpecialAssembly(left) && !this.IsSpecialAssembly(right))
			{
				result = 1;
			}
			else
			{
				if (this._depthForAssembly.ContainsKey(left.Name.Name) && this._depthForAssembly.ContainsKey(right.Name.Name))
				{
					int num = this._depthForAssembly[right.Name.Name] - this._depthForAssembly[left.Name.Name];
					if (num != 0)
					{
						result = num;
						return result;
					}
				}
				result = string.Compare(left.Name.Name, right.Name.Name, StringComparison.Ordinal);
			}
			return result;
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
				foreach (AssemblyNameReference current in assembly.MainModule.AssemblyReferences)
				{
					try
					{
						AssemblyDefinition assembly2 = assembly.MainModule.AssemblyResolver.Resolve(current);
						assemblyNamesAlreadyVisited.Add(assembly.Name.Name);
						AssemblyDependencyComparer.MaximumDepthForEachAssemblyRecursive(assembly2, assemblyNamesAlreadyVisited, depth, currentDepth + 1);
						assemblyNamesAlreadyVisited.Remove(assembly.Name.Name);
					}
					catch (AssemblyResolutionException)
					{
					}
				}
			}
		}

		private bool FirstRecursivelyReferencesSecond(AssemblyDefinition first, AssemblyDefinition second, HashSet<string> assemblyNamesAlreadyVisited)
		{
			bool result;
			if (first.References(second))
			{
				result = true;
			}
			else if (assemblyNamesAlreadyVisited.Contains(first.Name.Name))
			{
				result = false;
			}
			else
			{
				foreach (AssemblyNameReference current in first.MainModule.AssemblyReferences)
				{
					try
					{
						AssemblyDefinition first2 = first.MainModule.AssemblyResolver.Resolve(current);
						assemblyNamesAlreadyVisited.Add(first.Name.Name);
						bool flag = this.FirstRecursivelyReferencesSecond(first2, second, assemblyNamesAlreadyVisited);
						assemblyNamesAlreadyVisited.Remove(first.Name.Name);
						if (flag)
						{
							result = true;
							return result;
						}
					}
					catch (AssemblyResolutionException)
					{
					}
				}
				result = false;
			}
			return result;
		}

		private bool IsSpecialAssembly(AssemblyDefinition assembly)
		{
			return AssemblyDependencyComparer._specialAssemblies.Contains(assembly.Name.Name);
		}
	}
}

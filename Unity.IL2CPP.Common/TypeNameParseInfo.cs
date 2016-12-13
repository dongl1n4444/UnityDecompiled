using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Common
{
	public class TypeNameParseInfo
	{
		public const int ByRefModifierMarker = 0;

		public const int PointerModifierMarker = -1;

		public const int BoundedModifierMarker = -2;

		public string Name
		{
			get;
			internal set;
		}

		public List<int> Modifiers
		{
			get;
			internal set;
		}

		public List<string> Nested
		{
			get;
			internal set;
		}

		public List<int> Arities
		{
			get;
			internal set;
		}

		public string Namespace
		{
			get;
			internal set;
		}

		public AssemblyNameParseInfo Assembly
		{
			get;
			internal set;
		}

		public List<TypeNameParseInfo> TypeArguments
		{
			get;
			internal set;
		}

		public string ElementTypeName
		{
			get
			{
				string seed = this.Name;
				if (!string.IsNullOrEmpty(this.Namespace))
				{
					seed = this.Namespace + "." + this.Name;
				}
				return this.Nested.Aggregate(seed, (string c, string n) => c + "+" + n);
			}
		}

		public int[] Ranks
		{
			get
			{
				return (from m in this.Modifiers
				where m > 0
				select m).ToArray<int>();
			}
		}

		public int ArrayDimension
		{
			get
			{
				return this.Modifiers.Count((int modifier) => modifier > 0);
			}
		}

		public bool IsByRef
		{
			get
			{
				return this.Modifiers.IndexOf(0) >= 0;
			}
		}

		public bool IsPointer
		{
			get
			{
				return this.Modifiers.IndexOf(-1) >= 0;
			}
		}

		public bool IsBounded
		{
			get
			{
				return this.Modifiers.IndexOf(-2) >= 0;
			}
		}

		public bool IsArray
		{
			get
			{
				return this.Modifiers.Any((int m) => m > 0);
			}
		}

		public bool IsNested
		{
			get
			{
				return this.Nested.Count > 0;
			}
		}

		public bool HasGenericArguments
		{
			get
			{
				return this.TypeArguments.Count > 0;
			}
		}

		public TypeNameParseInfo()
		{
			this.Modifiers = new List<int>();
			this.Nested = new List<string>();
			this.Arities = new List<int>();
			this.Assembly = new AssemblyNameParseInfo();
			this.TypeArguments = new List<TypeNameParseInfo>();
		}
	}
}

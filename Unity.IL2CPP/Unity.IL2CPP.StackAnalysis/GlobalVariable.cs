using Mono.Cecil;
using System;

namespace Unity.IL2CPP.StackAnalysis
{
	public class GlobalVariable
	{
		public int BlockIndex
		{
			get;
			internal set;
		}

		public int Index
		{
			get;
			internal set;
		}

		public TypeReference Type
		{
			get;
			internal set;
		}

		public string VariableName
		{
			get
			{
				return string.Format("G_B{0}_{1}", this.BlockIndex, this.Index);
			}
		}
	}
}

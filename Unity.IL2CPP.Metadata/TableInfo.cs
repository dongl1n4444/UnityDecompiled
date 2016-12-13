using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public struct TableInfo
	{
		public readonly int Count;

		public readonly string Type;

		public readonly string Name;

		[Inject]
		public static INamingService Naming;

		public static TableInfo Empty
		{
			get
			{
				return new TableInfo(0, TableInfo.Naming.Null, TableInfo.Naming.Null);
			}
		}

		public TableInfo(int count, string type, string name)
		{
			this.Count = count;
			this.Type = type;
			this.Name = name;
		}
	}
}

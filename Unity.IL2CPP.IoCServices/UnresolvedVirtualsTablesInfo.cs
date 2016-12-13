using System;
using System.Collections.ObjectModel;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.IoCServices
{
	public struct UnresolvedVirtualsTablesInfo
	{
		public TableInfo MethodPointersInfo;

		public ReadOnlyCollection<Range> SignatureRangesInfo;

		public ReadOnlyCollection<int> SignatureTypesInfo;
	}
}

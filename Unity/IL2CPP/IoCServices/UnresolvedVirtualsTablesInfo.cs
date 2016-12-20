namespace Unity.IL2CPP.IoCServices
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Metadata;

    [StructLayout(LayoutKind.Sequential)]
    public struct UnresolvedVirtualsTablesInfo
    {
        public TableInfo MethodPointersInfo;
        public ReadOnlyCollection<Range> SignatureRangesInfo;
        public ReadOnlyCollection<int> SignatureTypesInfo;
    }
}


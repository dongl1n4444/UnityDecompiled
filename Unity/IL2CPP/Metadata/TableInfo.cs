namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TableInfo
    {
        public readonly int Count;
        public readonly string Type;
        public readonly string Name;
        [Inject]
        public static INamingService Naming;
        public TableInfo(int count, string type, string name)
        {
            this.Count = count;
            this.Type = type;
            this.Name = name;
        }

        public static TableInfo Empty
        {
            get
            {
                return new TableInfo(0, Naming.Null, Naming.Null);
            }
        }
    }
}


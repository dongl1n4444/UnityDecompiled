namespace Unity.BindingsGenerator.Core.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class)]
    internal class NativeTypeAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <Header>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <Name>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private ScriptingObjectType <ObjectType>k__BackingField;

        public NativeTypeAttribute()
        {
            this.ObjectType = ScriptingObjectType.UnityEngineObject;
        }

        public string Header { get; set; }

        public string Name { get; set; }

        public ScriptingObjectType ObjectType { get; set; }
    }
}


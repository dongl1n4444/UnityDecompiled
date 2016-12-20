namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [XmlType("ivy-repository")]
    public class ModuleRepository
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IvyModules <Modules>k__BackingField;

        [XmlElement("ivy-module")]
        public IvyModules Modules { get; set; }
    }
}


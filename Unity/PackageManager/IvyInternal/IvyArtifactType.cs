namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Xml.Serialization;

    [XmlType("type")]
    public enum IvyArtifactType
    {
        [XmlEnum("debug")]
        DebugSymbols = 5,
        [XmlEnum("dll")]
        Dll = 3,
        [XmlEnum("ivy")]
        Ivy = 2,
        [XmlEnum("none")]
        None = 0,
        [XmlEnum("package")]
        Package = 1,
        [XmlEnum("notes")]
        ReleaseNotes = 4
    }
}


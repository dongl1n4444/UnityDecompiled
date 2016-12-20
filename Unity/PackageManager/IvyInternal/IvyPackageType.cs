namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Xml.Serialization;

    [XmlType("packageType")]
    public enum IvyPackageType
    {
        [XmlEnum("PackageManager")]
        PackageManager = 3,
        [XmlEnum("PlaybackEngine")]
        PlaybackEngine = 1,
        [XmlEnum("UnityExtension")]
        UnityExtension = 2,
        [XmlEnum("Unknown")]
        Unknown = 0
    }
}


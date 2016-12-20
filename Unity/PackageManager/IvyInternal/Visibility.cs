namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Xml.Serialization;

    [XmlType("visibility")]
    public enum Visibility
    {
        [XmlEnum("private")]
        Private = 0,
        [XmlEnum("public")]
        Public = 1
    }
}


namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Xml.Serialization;

    [XmlType("repository")]
    public class IvyRepository
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("url")]
        public string Url;
    }
}


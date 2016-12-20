namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Xml.Serialization;

    [XmlType("artifact")]
    public class IvyArtifact
    {
        [XmlAttribute("ext")]
        public string Extension;
        [XmlAttribute("guid", Namespace="http://ant.apache.org/ivy/extra")]
        public string Guid;
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("type")]
        public IvyArtifactType Type;
        [XmlAttribute("url")]
        public string Url;
    }
}


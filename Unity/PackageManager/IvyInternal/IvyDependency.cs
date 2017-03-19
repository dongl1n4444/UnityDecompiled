namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Xml.Serialization;

    [XmlType("dependency")]
    public class IvyDependency
    {
        [XmlAttribute("branch")]
        public string Branch;
        [XmlAttribute("force")]
        public bool Force;
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("org")]
        public string Organisation;
        [XmlAttribute("rev")]
        public string Revision;
        [XmlAttribute("revConstraint")]
        public string RevisionConstraint;
    }
}


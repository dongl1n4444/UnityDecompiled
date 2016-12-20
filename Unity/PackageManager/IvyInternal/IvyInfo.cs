namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using Unity.PackageManager.Ivy;

    [XmlType("info")]
    public class IvyInfo : XmlSerializable
    {
        [XmlAttribute("branch")]
        public string Branch;
        [XmlElement("description")]
        public string Description;
        [XmlAttribute("module")]
        public string Module;
        [XmlAttribute("organisation")]
        public string Organisation;
        [XmlAttribute("publication")]
        public string Publication;
        [XmlAttribute("published", Namespace="http://ant.apache.org/ivy/extra"), DefaultValue(false)]
        public bool Published;
        [XmlIgnore]
        private List<Unity.PackageManager.IvyInternal.IvyRepository> repositories = new List<Unity.PackageManager.IvyInternal.IvyRepository>();
        [XmlAttribute("revision")]
        public string Revision;
        [XmlAttribute("status")]
        public string Status;
        [XmlAttribute("title", Namespace="http://ant.apache.org/ivy/extra")]
        public string Title;
        [XmlAttribute("packageType", Namespace="http://ant.apache.org/ivy/extra")]
        public IvyPackageType Type;
        [XmlAttribute("unityVersion", Namespace="http://ant.apache.org/ivy/extra")]
        public string UnityVersion;
        [XmlAttribute("version")]
        public string Version;

        [XmlIgnore]
        public List<Unity.PackageManager.IvyInternal.IvyRepository> Repositories
        {
            get
            {
                return this.repositories;
            }
        }

        [XmlElement("repository")]
        private List<Unity.PackageManager.IvyInternal.IvyRepository> xmlRepositories
        {
            get
            {
                if (this.repositories.Count == 0)
                {
                    return null;
                }
                return this.repositories;
            }
            set
            {
                if (value == null)
                {
                    this.repositories.Clear();
                }
                else
                {
                    this.repositories = value;
                }
            }
        }
    }
}


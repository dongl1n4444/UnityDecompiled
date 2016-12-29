namespace Unity.PackageManager.IvyInternal
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using Unity.PackageManager.Ivy;

    [XmlRoot("ivy-module")]
    public class IvyModule : XmlSerializable
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Unity.PackageManager.IvyInternal.IvyInfo <Info>k__BackingField;
        [XmlIgnore]
        private Unity.PackageManager.IvyInternal.IvyArtifacts artifacts = new Unity.PackageManager.IvyInternal.IvyArtifacts();
        [XmlAttribute("basepath", Namespace="http://ant.apache.org/ivy/extra")]
        public string BasePath;
        [XmlIgnore]
        private Unity.PackageManager.IvyInternal.IvyDependencies dependencies = new Unity.PackageManager.IvyInternal.IvyDependencies();
        [XmlAttribute("selected", Namespace="http://ant.apache.org/ivy/extra"), DefaultValue(false)]
        public bool Selected;
        [XmlAttribute("timestamp", Namespace="http://ant.apache.org/ivy/extra")]
        public string Timestamp;
        [XmlAttribute("version")]
        private string Version = "2.0";

        [XmlIgnore]
        public Unity.PackageManager.IvyInternal.IvyArtifacts Artifacts =>
            this.artifacts;

        [XmlIgnore]
        public Unity.PackageManager.IvyInternal.IvyDependencies Dependencies =>
            this.dependencies;

        [XmlElement("info", Order=1)]
        public Unity.PackageManager.IvyInternal.IvyInfo Info { get; set; }

        [XmlElement("publications", Order=2)]
        private Unity.PackageManager.IvyInternal.IvyArtifacts xmlArtifacts
        {
            get
            {
                if (this.artifacts.Count == 0)
                {
                    return null;
                }
                return this.artifacts;
            }
            set
            {
                if (value == null)
                {
                    this.artifacts.Clear();
                }
                else
                {
                    this.artifacts = value;
                }
            }
        }

        [XmlElement("dependencies", Order=3)]
        private Unity.PackageManager.IvyInternal.IvyDependencies xmlDependencies
        {
            get
            {
                if (this.dependencies.Count == 0)
                {
                    return null;
                }
                return this.dependencies;
            }
            set
            {
                if (value == null)
                {
                    this.dependencies.Clear();
                }
                else
                {
                    this.dependencies = value;
                }
            }
        }
    }
}


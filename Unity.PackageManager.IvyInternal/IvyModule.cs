using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Unity.PackageManager.Ivy;

namespace Unity.PackageManager.IvyInternal
{
	[XmlRoot("ivy-module")]
	public class IvyModule : XmlSerializable
	{
		[XmlAttribute("version")]
		private string Version = "2.0";

		[XmlAttribute("timestamp", Namespace = "http://ant.apache.org/ivy/extra")]
		public string Timestamp;

		[XmlAttribute("basepath", Namespace = "http://ant.apache.org/ivy/extra")]
		public string BasePath;

		[DefaultValue(false), XmlAttribute("selected", Namespace = "http://ant.apache.org/ivy/extra")]
		public bool Selected;

		[XmlIgnore]
		private IvyArtifacts artifacts = new IvyArtifacts();

		[XmlIgnore]
		private IvyDependencies dependencies = new IvyDependencies();

		[XmlElement("info", Order = 1)]
		public IvyInfo Info
		{
			get;
			set;
		}

		[XmlElement("publications", Order = 2)]
		private IvyArtifacts xmlArtifacts
		{
			get
			{
				IvyArtifacts result;
				if (this.artifacts.Count == 0)
				{
					result = null;
				}
				else
				{
					result = this.artifacts;
				}
				return result;
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

		[XmlElement("dependencies", Order = 3)]
		private IvyDependencies xmlDependencies
		{
			get
			{
				IvyDependencies result;
				if (this.dependencies.Count == 0)
				{
					result = null;
				}
				else
				{
					result = this.dependencies;
				}
				return result;
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

		[XmlIgnore]
		public IvyArtifacts Artifacts
		{
			get
			{
				return this.artifacts;
			}
		}

		[XmlIgnore]
		public IvyDependencies Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}
	}
}

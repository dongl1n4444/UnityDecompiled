namespace Unity.PackageManager.IvyInternal
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("publications")]
    public class IvyArtifacts : List<IvyArtifact>
    {
    }
}


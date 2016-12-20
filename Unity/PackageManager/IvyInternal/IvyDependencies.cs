namespace Unity.PackageManager.IvyInternal
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("dependencies")]
    public class IvyDependencies : List<IvyDependency>
    {
    }
}


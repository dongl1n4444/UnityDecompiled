namespace Unity.PackageManager.IvyInternal
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("modules")]
    public class IvyModules : List<IvyModule>
    {
    }
}


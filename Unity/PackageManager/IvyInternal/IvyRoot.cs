namespace Unity.PackageManager.IvyInternal
{
    using System.Xml.Serialization;
    using Unity.PackageManager.Ivy;

    [XmlRoot("root")]
    public class IvyRoot : XmlSerializable
    {
        [XmlElement("ivy-module")]
        public Unity.PackageManager.IvyInternal.IvyModule Module;
        [XmlElement("ivy-repository", Namespace="http://ant.apache.org/ivy/extra")]
        public Unity.PackageManager.IvyInternal.ModuleRepository Repository;
    }
}


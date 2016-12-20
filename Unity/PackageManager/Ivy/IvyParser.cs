namespace Unity.PackageManager.Ivy
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Unity.PackageManager.IvyInternal;

    public static class IvyParser
    {
        public static Exception ErrorException;
        public static string ErrorMessage;
        public static bool HasErrors;
        public static XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces();

        static IvyParser()
        {
            Namespaces.Add("e", "http://ant.apache.org/ivy/extra");
        }

        public static T Deserialize<T>(string xml) where T: class
        {
            HasErrors = false;
            ErrorMessage = null;
            ErrorException = null;
            if (xml.Length <= 0)
            {
                HasErrors = true;
                ErrorMessage = "Cannot deserialize empty xml document.";
                return null;
            }
            Type targetType = typeof(T);
            int startIndex = 0;
            int index = xml.IndexOf("<ivy-module");
            int num3 = xml.IndexOf("<ivy-repository");
            if (typeof(T) == typeof(Unity.PackageManager.Ivy.IvyModule))
            {
                if (index < 0)
                {
                    return null;
                }
                startIndex = index;
                targetType = typeof(Unity.PackageManager.Ivy.IvyModule);
            }
            else if (typeof(T) == typeof(Unity.PackageManager.Ivy.ModuleRepository))
            {
                if (num3 < 0)
                {
                    return null;
                }
                startIndex = num3;
                targetType = typeof(Unity.PackageManager.Ivy.ModuleRepository);
            }
            else if (typeof(T) == typeof(object))
            {
                if (num3 >= 0)
                {
                    startIndex = num3;
                    targetType = typeof(Unity.PackageManager.Ivy.ModuleRepository);
                }
                else if (index >= 0)
                {
                    startIndex = index;
                    targetType = typeof(Unity.PackageManager.Ivy.IvyModule);
                }
            }
            xml = xml.Insert(startIndex, "<root>") + "</root>";
            try
            {
                XmlTextReader xmlReader = new XmlTextReader(xml, XmlNodeType.Document, null);
                IvyRoot root = XmlSerializable.GetSerializer(typeof(IvyRoot)).Deserialize(xmlReader) as IvyRoot;
                if (targetType == typeof(Unity.PackageManager.Ivy.IvyModule))
                {
                    return (Cloner.CloneObject(root.Module, targetType) as T);
                }
                return (Cloner.CloneObject(root.Repository, targetType) as T);
            }
            catch (Exception exception)
            {
                HasErrors = true;
                ErrorMessage = "Deserialization failed.";
                ErrorException = exception;
            }
            return null;
        }

        public static T Parse<T>(string xml) where T: class
        {
            object obj2 = Deserialize<T>(xml);
            if ((obj2 != null) && (typeof(T) == typeof(Unity.PackageManager.Ivy.IvyModule)))
            {
                ((Unity.PackageManager.Ivy.IvyModule) obj2).IvyFile = "ivy.xml";
            }
            return (obj2 as T);
        }

        public static T ParseFile<T>(string path) where T: class
        {
            HasErrors = false;
            ErrorMessage = null;
            ErrorException = null;
            if (!File.Exists(path))
            {
                HasErrors = true;
                ErrorMessage = string.Format("File does not exist: {0}", path);
                return null;
            }
            object obj2 = Parse<T>(File.ReadAllText(path, Encoding.UTF8));
            if ((obj2 != null) && (typeof(T) == typeof(Unity.PackageManager.Ivy.IvyModule)))
            {
                ((Unity.PackageManager.Ivy.IvyModule) obj2).BasePath = Path.GetDirectoryName(path);
                ((Unity.PackageManager.Ivy.IvyModule) obj2).IvyFile = Path.GetFileName(path);
            }
            return (obj2 as T);
        }

        public static string Serialize(Unity.PackageManager.Ivy.IvyModule module)
        {
            HasErrors = false;
            ErrorMessage = null;
            ErrorException = null;
            Unity.PackageManager.IvyInternal.IvyModule o = Cloner.CloneObject<Unity.PackageManager.IvyInternal.IvyModule>(module);
            IvyRoot root = new IvyRoot {
                Module = o
            };
            StringBuilder builder = new StringBuilder();
            using (UTF8StringWriter writer = new UTF8StringWriter(builder))
            {
                XmlSerializable.GetSerializer(o.GetType()).Serialize(writer, o, Namespaces);
            }
            return builder.ToString().Replace("<root>", "").Replace("</root>", "");
        }

        public static string Serialize(Unity.PackageManager.Ivy.ModuleRepository repo)
        {
            HasErrors = false;
            ErrorMessage = null;
            ErrorException = null;
            Unity.PackageManager.IvyInternal.ModuleRepository repository = Cloner.CloneObject<Unity.PackageManager.IvyInternal.ModuleRepository>(repo);
            IvyRoot o = new IvyRoot {
                Repository = repository
            };
            StringBuilder builder = new StringBuilder();
            using (UTF8StringWriter writer = new UTF8StringWriter(builder))
            {
                XmlSerializable.GetSerializer(o.GetType()).Serialize(writer, o, Namespaces);
            }
            return builder.ToString().Replace("<root>", "").Replace("</root>", "");
        }
    }
}


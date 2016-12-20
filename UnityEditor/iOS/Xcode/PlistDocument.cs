namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    public class PlistDocument
    {
        [CompilerGenerated]
        private static Func<XText, string> <>f__am$cache0;
        public PlistElementDict root = new PlistElementDict();
        public string version = "1.0";

        internal static string CleanDtdToString(XDocument doc)
        {
            if (doc.DocumentType != null)
            {
                object[] objArray1 = new object[] { new XDocumentType(doc.DocumentType.Name, doc.DocumentType.PublicId, doc.DocumentType.SystemId, null), new XElement(doc.Root.Name) };
                XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null), objArray1);
                object[] objArray2 = new object[] { "", document.Declaration, "\n", document.DocumentType, "\n", doc.Root };
                return string.Concat(objArray2);
            }
            object[] content = new object[] { new XElement(doc.Root.Name) };
            XDocument document2 = new XDocument(new XDeclaration("1.0", "utf-8", null), content);
            object[] objArray4 = new object[] { "", document2.Declaration, Environment.NewLine, doc.Root };
            return string.Concat(objArray4);
        }

        private static string GetText(XElement xml)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<XText, string>(null, (IntPtr) <GetText>m__0);
            }
            return string.Join("", Enumerable.ToArray<string>(Enumerable.Select<XText, string>(Enumerable.OfType<XText>(xml.Nodes()), <>f__am$cache0)));
        }

        internal static XDocument ParseXmlNoDtd(string text)
        {
            XmlReaderSettings settings = new XmlReaderSettings {
                ProhibitDtd = false,
                XmlResolver = null
            };
            return XDocument.Load(XmlReader.Create(new StringReader(text), settings));
        }

        private static PlistElement ReadElement(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "dict":
                {
                    List<XElement> list = Enumerable.ToList<XElement>(xml.Elements());
                    PlistElementDict dict = new PlistElementDict();
                    if ((list.Count % 2) == 1)
                    {
                        throw new Exception("Malformed plist file");
                    }
                    for (int i = 0; i < (list.Count - 1); i++)
                    {
                        if (list[i].Name != "key")
                        {
                            throw new Exception("Malformed plist file");
                        }
                        string str2 = GetText(list[i]).Trim();
                        PlistElement element = ReadElement(list[i + 1]);
                        if (element != null)
                        {
                            i++;
                            dict[str2] = element;
                        }
                    }
                    return dict;
                }
                case "array":
                {
                    List<XElement> list2 = Enumerable.ToList<XElement>(xml.Elements());
                    PlistElementArray array = new PlistElementArray();
                    foreach (XElement element3 in list2)
                    {
                        PlistElement item = ReadElement(element3);
                        if (item != null)
                        {
                            array.values.Add(item);
                        }
                    }
                    return array;
                }
                case "string":
                    return new PlistElementString(GetText(xml));

                case "integer":
                    int num2;
                    if (int.TryParse(GetText(xml), out num2))
                    {
                        return new PlistElementInteger(num2);
                    }
                    return null;

                case "true":
                    return new PlistElementBoolean(true);

                case "false":
                    return new PlistElementBoolean(false);
            }
            return null;
        }

        public void ReadFromFile(string path)
        {
            this.ReadFromString(File.ReadAllText(path));
        }

        public void ReadFromStream(TextReader tr)
        {
            this.ReadFromString(tr.ReadToEnd());
        }

        public void ReadFromString(string text)
        {
            XDocument node = ParseXmlNoDtd(text);
            this.version = (string) node.Root.Attribute("version");
            PlistElement element2 = ReadElement(System.Xml.XPath.Extensions.XPathSelectElement(node, "plist/dict"));
            if (element2 == null)
            {
                throw new Exception("Error parsing plist file");
            }
            this.root = element2 as PlistElementDict;
            if (this.root == null)
            {
                throw new Exception("Malformed plist file");
            }
        }

        private static XElement WriteElement(PlistElement el)
        {
            if (el is PlistElementBoolean)
            {
                PlistElementBoolean flag = el as PlistElementBoolean;
                return new XElement(!flag.value ? "false" : "true");
            }
            if (el is PlistElementInteger)
            {
                PlistElementInteger integer = el as PlistElementInteger;
                return new XElement("integer", integer.value.ToString());
            }
            if (el is PlistElementString)
            {
                PlistElementString str = el as PlistElementString;
                return new XElement("string", str.value);
            }
            if (el is PlistElementDict)
            {
                PlistElementDict dict = el as PlistElementDict;
                XElement element2 = new XElement("dict");
                foreach (KeyValuePair<string, PlistElement> pair in dict.values)
                {
                    XElement content = new XElement("key", pair.Key);
                    XElement element4 = WriteElement(pair.Value);
                    if (element4 != null)
                    {
                        element2.Add(content);
                        element2.Add(element4);
                    }
                }
                return element2;
            }
            if (el is PlistElementArray)
            {
                PlistElementArray array = el as PlistElementArray;
                XElement element5 = new XElement("array");
                foreach (PlistElement element6 in array.values)
                {
                    XElement element7 = WriteElement(element6);
                    if (element7 != null)
                    {
                        element5.Add(element7);
                    }
                }
                return element5;
            }
            return null;
        }

        public void WriteToFile(string path)
        {
            File.WriteAllText(path, this.WriteToString());
        }

        public void WriteToStream(TextWriter tw)
        {
            tw.Write(this.WriteToString());
        }

        public string WriteToString()
        {
            XElement content = WriteElement(this.root);
            XElement element2 = new XElement("plist");
            element2.Add(new XAttribute("version", this.version));
            element2.Add(content);
            XDocument doc = new XDocument();
            doc.Add(element2);
            return CleanDtdToString(doc);
        }
    }
}


namespace UnityEditor
{
    using System;
    using System.Text;
    using System.Xml;

    internal class AndroidXmlDocument : XmlDocument
    {
        public const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        private string m_Path;
        protected XmlNamespaceManager nsMgr;

        public AndroidXmlDocument(string path)
        {
            this.m_Path = path;
            XmlTextReader reader = new XmlTextReader(this.m_Path);
            try
            {
                reader.Read();
                this.Load(reader);
            }
            finally
            {
                reader.Close();
            }
            this.nsMgr = new XmlNamespaceManager(base.NameTable);
            this.nsMgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");
        }

        protected XmlElement AppendElement(XmlElement node, string tag, string attribute)
        {
            if (node.SelectSingleNode(string.Format(".//{0}[@{1}]", tag, attribute), this.nsMgr) != null)
            {
                return null;
            }
            return (XmlElement) node.AppendChild(base.CreateElement(tag));
        }

        protected XmlElement AppendElement(XmlElement node, string tag, string attribute, string attributeValue)
        {
            if (node.SelectSingleNode(string.Format(".//{0}[@{1}='{2}']", tag, attribute, attributeValue), this.nsMgr) != null)
            {
                return null;
            }
            return (XmlElement) node.AppendChild(base.CreateElement(tag));
        }

        public XmlAttribute CreateAttribute(string prefix, string localName, string namezpace, string value)
        {
            XmlAttribute attribute = this.CreateAttribute(prefix, localName, namezpace);
            attribute.Value = value;
            return attribute;
        }

        public void PatchStringRes(string tag, string attrib, string value)
        {
            XmlNode node = base.SelectSingleNode(string.Format("//{0}[@name='{1}']", tag, attrib), this.nsMgr);
            if (node == null)
            {
                node = base.DocumentElement.AppendChild(base.CreateElement(tag));
                node.Attributes.Append(this.CreateAttribute("", "name", "", attrib));
            }
            value = value.Replace(@"\", @"\\");
            value = value.Replace("'", @"\'");
            value = value.Replace("\"", "\\\"");
            node.InnerText = value;
        }

        public string Save()
        {
            return this.SaveAs(this.m_Path);
        }

        public string SaveAs(string path)
        {
            XmlTextWriter w = new XmlTextWriter(path, new UTF8Encoding(false));
            try
            {
                w.Formatting = Formatting.Indented;
                this.Save(w);
            }
            finally
            {
                w.Close();
            }
            return path;
        }
    }
}


namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.XPath;

    internal class LinkXmlReader
    {
        private readonly List<string> _assembliesInALinkXmlFile = new List<string>();

        public LinkXmlReader()
        {
            foreach (string str in AssemblyStripper.GetUserBlacklistFiles())
            {
                XPathNavigator navigator = new XPathDocument(str).CreateNavigator();
                navigator.MoveToFirstChild();
                XPathNodeIterator iterator = navigator.SelectChildren("assembly", string.Empty);
                while (iterator.MoveNext())
                {
                    this._assembliesInALinkXmlFile.Add(iterator.Current.GetAttribute("fullname", string.Empty));
                }
            }
        }

        public bool IsDLLUsed(string assemblyFileName) => 
            this._assembliesInALinkXmlFile.Contains(Path.GetFileNameWithoutExtension(assemblyFileName));
    }
}


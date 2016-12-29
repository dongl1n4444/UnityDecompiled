namespace UnityEditor.iOS
{
    using System;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using UnityEditor;
    using UnityEditor.iOS.Xcode;

    internal class XcSchemeUpdater
    {
        public static string UpdateString(string contents, iOSBuildType buildType)
        {
            XDocument doc = PlistDocument.ParseXmlNoDtd(contents);
            string str = "";
            if (buildType != iOSBuildType.Debug)
            {
                if (buildType == iOSBuildType.Release)
                {
                    str = "ReleaseForRunning";
                }
            }
            else
            {
                str = "Debug";
            }
            XElement element = doc.Root.XPathSelectElement("./LaunchAction");
            if (element == null)
            {
                return contents;
            }
            element.SetAttributeValue("buildConfiguration", str);
            return PlistDocument.CleanDtdToString(doc);
        }
    }
}


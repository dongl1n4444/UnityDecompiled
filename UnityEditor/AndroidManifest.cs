namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;

    internal class AndroidManifest : AndroidXmlDocument
    {
        public static readonly string AndroidConfigChanges;
        public const string AndroidManifestFile = "AndroidManifest.xml";
        public readonly XmlElement ApplicationElement;

        static AndroidManifest()
        {
            string[] textArray1 = new string[] { "mcc", "mnc", "locale", "touchscreen", "keyboard", "keyboardHidden", "navigation", "orientation", "screenLayout", "uiMode", "screenSize", "smallestScreenSize", "fontScale", "layoutDirection" };
            AndroidConfigChanges = string.Join("|", textArray1);
        }

        public AndroidManifest(string path) : base(path)
        {
            this.ApplicationElement = (XmlElement) this.GetElementsByTagName("application")[0];
        }

        public void AddApplicationMetaDataAttribute(string name, string value)
        {
            List<XmlAttribute> attributes = new List<XmlAttribute> {
                this.CreateAndroidAttribute("value", value)
            };
            this.AppendApplicationAndroidNameTag("meta-data", name, attributes);
        }

        public void AddGLESVersion(string glEsVersion)
        {
            XmlElement element = base.AppendElement(base.DocumentElement, "uses-feature", "android:glEsVersion");
            if (element != null)
            {
                element.Attributes.Append(this.CreateAndroidAttribute("glEsVersion", glEsVersion));
            }
        }

        public bool AddIntentFilterCategory(string category)
        {
            XmlNode node = base.SelectSingleNode("/manifest/application/activity/intent-filter[category/@android:name='android.intent.category.LAUNCHER']", base.nsMgr);
            if (node == null)
            {
                return false;
            }
            node.AppendChild(base.CreateElement("category")).Attributes.Append(this.CreateAndroidAttribute("name", category));
            return true;
        }

        public bool AddLeanbackLauncherActivity() => 
            this.AddIntentFilterCategory("android.intent.category.LEANBACK_LAUNCHER");

        public bool AddResourceToLaunchActivity(string name, string resource)
        {
            XmlNode node = base.SelectSingleNode("/manifest/application/activity[intent-filter/category/@android:name='android.intent.category.LAUNCHER']", base.nsMgr);
            if (node == null)
            {
                return false;
            }
            XmlNode node2 = node.AppendChild(base.CreateElement("meta-data"));
            node2.Attributes.Append(this.CreateAndroidAttribute("name", name));
            node2.Attributes.Append(this.CreateAndroidAttribute("resource", resource));
            return true;
        }

        public void AddSupportsGLTexture(string format)
        {
            this.AppendTopAndroidNameTag("supports-gl-texture", format);
        }

        public void AddUsesFeature(string feature, bool required)
        {
            XmlElement element = this.AppendTopAndroidNameTag("uses-feature", feature);
            if ((element != null) && !required)
            {
                element.Attributes.Append(this.CreateAndroidAttribute("required", "false"));
            }
        }

        public void AddUsesPermission(string permission)
        {
            this.AppendTopAndroidNameTag("uses-permission", permission);
        }

        public void AddUsesPermission(string permission, int maxSdkVersion)
        {
            List<XmlAttribute> attributes = new List<XmlAttribute> {
                this.CreateAndroidAttribute("maxSdkVersion", maxSdkVersion.ToString())
            };
            this.AppendTopAndroidNameTag("uses-permission", permission, attributes);
        }

        public void AddUsesSDK(int minSdkVersion, int targetSdkVersion)
        {
            XmlElement element = null;
            XmlElement element2 = (XmlElement) base.SelectSingleNode("/manifest/uses-sdk[@android:minSdkVersion]", base.nsMgr);
            XmlElement element3 = (XmlElement) base.SelectSingleNode("/manifest/uses-sdk[@android:targetSdkVersion]", base.nsMgr);
            if ((element2 == null) || (element3 == null))
            {
                if (element2 != null)
                {
                    element = element2;
                }
                else if (element3 != null)
                {
                    element = element3;
                }
                else
                {
                    element = (XmlElement) base.DocumentElement.AppendChild(base.CreateElement("uses-sdk"));
                }
                if (element2 == null)
                {
                    element.Attributes.Append(this.CreateAndroidAttribute("minSdkVersion", minSdkVersion.ToString()));
                }
                if (element3 == null)
                {
                    element.Attributes.Append(this.CreateAndroidAttribute("targetSdkVersion", targetSdkVersion.ToString()));
                }
            }
        }

        private XmlElement AppendAndroidNameTag(XmlElement element, string tag, string value, List<XmlAttribute> attributes)
        {
            XmlElement element2 = base.AppendElement(element, tag, "android:name", value);
            if (element2 != null)
            {
                element2.Attributes.Append(this.CreateAndroidAttribute("name", value));
                if (attributes == null)
                {
                    return element2;
                }
                foreach (XmlAttribute attribute in attributes)
                {
                    element2.Attributes.Append(attribute);
                }
            }
            return element2;
        }

        private XmlElement AppendApplicationAndroidNameTag(string tag, string value, List<XmlAttribute> attributes) => 
            this.AppendAndroidNameTag(this.ApplicationElement, tag, value, attributes);

        private XmlElement AppendTopAndroidNameTag(string tag, string value) => 
            this.AppendTopAndroidNameTag(tag, value, null);

        private XmlElement AppendTopAndroidNameTag(string tag, string value, List<XmlAttribute> attributes) => 
            this.AppendAndroidNameTag(base.DocumentElement, tag, value, attributes);

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attribute = this.CreateAttribute("android", key, "http://schemas.android.com/apk/res/android");
            attribute.Value = value;
            return attribute;
        }

        public bool EnableVrMode(string activity) => 
            this.SetActivityAndroidAttribute(activity, "enableVrMode", "@string/gvr_vr_mode_component");

        public XmlElement GetActivity(string name) => 
            ((XmlElement) base.SelectSingleNode($"/manifest/application/activity[@android:name='{name}']", base.nsMgr));

        public string GetActivityWithLaunchIntent()
        {
            XmlNode node = base.SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and intent-filter/category/@android:name='android.intent.category.LAUNCHER']", base.nsMgr);
            return ((node == null) ? "" : node.Attributes["android:name"].Value);
        }

        public bool HasLeanbackLauncherActivity() => 
            (base.SelectSingleNode("/manifest/application/activity/intent-filter/category[@android:name='android.intent.category.LEANBACK_LAUNCHER']", base.nsMgr) != null);

        public void OverrideTheme(string theme)
        {
            this.ApplicationElement.Attributes.Append(this.CreateAndroidAttribute("theme", theme));
        }

        public void RemoveApplicationFlag(string name)
        {
            this.ApplicationElement.Attributes.RemoveNamedItem("android:" + name);
        }

        public bool RenameActivity(string src, string dst)
        {
            XmlElement activity = this.GetActivity(src);
            if (activity == null)
            {
                return false;
            }
            activity.Attributes.Append(this.CreateAndroidAttribute("name", dst));
            return true;
        }

        public bool SetActivityAndroidAttribute(string activity, string name, string val)
        {
            XmlElement element = this.GetActivity(activity);
            if (element == null)
            {
                return false;
            }
            element.Attributes.Append(this.CreateAndroidAttribute(name, val));
            return true;
        }

        public void SetApplicationBanner(string name)
        {
            this.ApplicationElement.Attributes.Append(this.CreateAndroidAttribute("banner", name));
        }

        public void SetApplicationFlag(string name, bool value)
        {
            this.ApplicationElement.Attributes.Append(this.CreateAndroidAttribute(name, !value ? "false" : "true"));
        }

        public bool SetConfigChanges(string activity, string configChanges) => 
            this.SetActivityAndroidAttribute(activity, "configChanges", configChanges);

        public void SetDebuggable(bool debuggable)
        {
            this.SetApplicationFlag("debuggable", debuggable);
        }

        public void SetInstallLocation(string location)
        {
            base.DocumentElement.Attributes.Append(this.CreateAndroidAttribute("installLocation", location));
        }

        public bool SetLaunchMode(string activity, string launchMode) => 
            this.SetActivityAndroidAttribute(activity, "launchMode", launchMode);

        public bool SetOrientation(string activity, string orientation) => 
            this.SetActivityAndroidAttribute(activity, "screenOrientation", orientation);

        public bool SetResizableActivity(string activity, bool value) => 
            this.SetActivityAndroidAttribute(activity, "resizeableActivity", !value ? "false" : "true");

        public void SetVersion(string versionName, int versionCode)
        {
            base.DocumentElement.Attributes.Append(this.CreateAndroidAttribute("versionName", versionName));
            base.DocumentElement.Attributes.Append(this.CreateAndroidAttribute("versionCode", versionCode.ToString()));
        }

        public void StripUnityLibEntryForNativeActitivy()
        {
            IEnumerator enumerator = base.SelectNodes("//meta-data[@android:name='android.app.lib_name' and @android:value='unity']", base.nsMgr).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode) enumerator.Current;
                    current.ParentNode.RemoveChild(current);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public string packageName
        {
            get => 
                base.DocumentElement.GetAttribute("package");
            set
            {
                base.DocumentElement.SetAttribute("package", value);
            }
        }
    }
}


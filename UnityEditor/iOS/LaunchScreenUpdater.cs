namespace UnityEditor.iOS
{
    using System;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using UnityEditor.iOS.Xcode;
    using UnityEngine;

    internal class LaunchScreenUpdater
    {
        private static bool RemoveElement(XElement el)
        {
            if (el == null)
            {
                return false;
            }
            el.Remove();
            return true;
        }

        internal static string RemoveiPhonePortraitViewImpl(string contents, string[] mainViewXpaths)
        {
            bool flag = true;
            XDocument doc = PlistDocument.ParseXmlNoDtd(contents);
            XElement node = System.Xml.XPath.Extensions.XPathSelectElement(doc.Root, "./objects/view[@id='iN0-l3-epB']");
            if (node == null)
            {
                flag = false;
            }
            else
            {
                foreach (string str in mainViewXpaths)
                {
                    flag &= RemoveElement(System.Xml.XPath.Extensions.XPathSelectElement(node, str));
                }
            }
            if (!flag)
            {
                Debug.Log("Could not delete portrait lauch screen view: launch screen XIB file is malformed");
            }
            return PlistDocument.CleanDtdToString(doc);
        }

        internal static string RemoveiPhonePortraitViewInConstant(string contents)
        {
            string[] mainViewXpaths = new string[] { "./subviews/view[@id='eQG-9T-8bq']", "./constraints/constraint[@id='ABe-25-nSQ']", "./constraints/constraint[@id='Bmb-Ri-oSH']", "./variation[@key='default']/mask[@key='subviews']/exclude[@reference='eQG-9T-8bq']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='ABe-25-nSQ']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='Bmb-Ri-oSH']", "./variation[@key='widthClass=compact']" };
            return RemoveiPhonePortraitViewImpl(contents, mainViewXpaths);
        }

        internal static string RemoveiPhonePortraitViewInRelative(string contents)
        {
            string[] mainViewXpaths = new string[] { "./subviews/view[@id='eQG-9T-8bq']", "./constraints/constraint[@id='0kh-ex-StW']", "./constraints/constraint[@id='ABe-25-nSQ']", "./constraints/constraint[@id='e2k-A9-5oM']", "./constraints/constraint[@id='omh-z1-Che']", "./constraints/constraint[@id='rQ6-gW-Rdm']", "./variation[@key='default']/mask[@key='subviews']/exclude[@reference='eQG-9T-8bq']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='0kh-ex-StW']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='ABe-25-nSQ']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='e2k-A9-5oM']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='omh-z1-Che']", "./variation[@key='default']/mask[@key='constraints']/exclude[@reference='rQ6-gW-Rdm']", "./variation[@key='widthClass=compact']" };
            return RemoveiPhonePortraitViewImpl(contents, mainViewXpaths);
        }

        private static bool SetAttribute(XElement el, string attr, string value)
        {
            if (el == null)
            {
                return false;
            }
            el.SetAttributeValue(attr, value);
            return true;
        }

        private static bool SetBackgroundColorElement(XElement parentEl, Color color)
        {
            XElement element = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./color[@key='backgroundColor']");
            if (element == null)
            {
                return false;
            }
            element.SetAttributeValue("red", color.r.ToString());
            element.SetAttributeValue("green", color.g.ToString());
            element.SetAttributeValue("blue", color.b.ToString());
            return true;
        }

        private static bool SetConstantForConstraint(XElement el, float constant)
        {
            if (el == null)
            {
                return false;
            }
            return SetAttribute(el, "constant", constant.ToString().ToLower());
        }

        private static bool SetImageSize(XElement el, int width, int height)
        {
            if (el == null)
            {
                return false;
            }
            el.SetAttributeValue("width", width.ToString());
            el.SetAttributeValue("height", height.ToString());
            return true;
        }

        private static bool SetMultiplierForConstraint(XElement el, float multiplier)
        {
            multiplier = Math.Max(multiplier, 0.001f);
            return SetAttribute(el, "multiplier", multiplier.ToString().ToLower());
        }

        private static bool SetRelativeSizeConstraints(float size, XElement first, XElement second, bool vertical)
        {
            string str = !vertical ? "leading" : "top";
            string str2 = !vertical ? "trailing" : "bottom";
            bool flag = true;
            if ((1f - size) > 0.001)
            {
                flag &= SetMultiplierForConstraint(first, 1f - size);
                return (flag & SetMultiplierForConstraint(second, 1f + size));
            }
            flag &= SetMultiplierForConstraint(first, 1f);
            flag &= SetAttribute(first, "secondAttribute", str);
            flag &= SetMultiplierForConstraint(second, 1f);
            return (flag & SetAttribute(second, "secondAttribute", str2));
        }

        private static bool UpdateLaunchScreenAllImageSizes(XElement rootEl, XibWithBackgroundData data, UnityEditor.iOS.DeviceType device)
        {
            bool flag = true;
            if (device == UnityEditor.iOS.DeviceType.iPhone)
            {
                XElement el = System.Xml.XPath.Extensions.XPathSelectElement(rootEl, "./resources/image[@name='LaunchScreen-iPhonePortrait.png']");
                flag &= SetImageSize(el, data.iPhonePortraitWidth, data.iPhonePortraitHeight);
                el = System.Xml.XPath.Extensions.XPathSelectElement(rootEl, "./resources/image[@name='LaunchScreen-iPhoneLandscape.png']");
                flag &= SetImageSize(el, data.iPhoneLandscapeWidth, data.iPhoneLandscapeHeight);
            }
            if (device == UnityEditor.iOS.DeviceType.iPad)
            {
                XElement element2 = System.Xml.XPath.Extensions.XPathSelectElement(rootEl, "./resources/image[@name='LaunchScreen-iPad.png']");
                flag &= SetImageSize(element2, data.iPadWidth, data.iPadHeight);
            }
            return flag;
        }

        internal static string UpdateStringForBackgroundConstant(string contents, XibWithBackgroundData data, UnityEditor.iOS.DeviceType device)
        {
            bool flag = true;
            XDocument doc = PlistDocument.ParseXmlNoDtd(contents);
            flag &= UpdateLaunchScreenAllImageSizes(doc.Root, data, device);
            XElement parentEl = System.Xml.XPath.Extensions.XPathSelectElement(doc.Root, "./objects/view[@id='iN0-l3-epB']");
            if (parentEl == null)
            {
                flag = false;
            }
            else
            {
                flag &= SetBackgroundColorElement(parentEl, data.background);
                if (device == UnityEditor.iOS.DeviceType.iPhone)
                {
                    XElement el = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./subviews/view[@id='eQG-9T-8bq']/constraints/constraint[@id='rU5-7V-ukh']");
                    flag &= SetConstantForConstraint(el, data.iPhonePortraitHorizontalSize);
                    el = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./subviews/view[@id='gVh-4A-1je']/constraints/constraint[@id='NjH-A4-Wg3']");
                    flag &= SetConstantForConstraint(el, data.iPhoneLandscapeVerticalSize);
                }
                if (device == UnityEditor.iOS.DeviceType.iPad)
                {
                    XElement element3 = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./subviews/view[@id='7bp-Iv-3mp']/constraints/constraint[@id='0Cy-oZ-PWZ']");
                    flag &= SetConstantForConstraint(element3, data.iPadVerticalSize);
                }
            }
            if (!flag)
            {
                Debug.Log("Launch screen XIB file is malformed");
            }
            return PlistDocument.CleanDtdToString(doc);
        }

        internal static string UpdateStringForBackgroundRelative(string contents, XibWithBackgroundData data, UnityEditor.iOS.DeviceType device)
        {
            bool flag = true;
            XDocument doc = PlistDocument.ParseXmlNoDtd(contents);
            flag &= UpdateLaunchScreenAllImageSizes(doc.Root, data, device);
            XElement parentEl = System.Xml.XPath.Extensions.XPathSelectElement(doc.Root, "./objects/view[@id='iN0-l3-epB']");
            if (parentEl == null)
            {
                flag = false;
            }
            else
            {
                flag &= SetBackgroundColorElement(parentEl, data.background);
                if (device == UnityEditor.iOS.DeviceType.iPhone)
                {
                    XElement first = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./constraints/constraint[@id='VWh-bq-Pfv']");
                    XElement second = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./constraints/constraint[@id='S6m-R2-QBg']");
                    XElement element4 = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./constraints/constraint[@id='e2k-A9-5oM']");
                    XElement element5 = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./constraints/constraint[@id='omh-z1-Che']");
                    flag &= SetRelativeSizeConstraints(data.iPhoneLandscapeRelativeVerticalSize, first, second, true);
                    flag &= SetRelativeSizeConstraints(data.iPhonePortraitRelativeHorizontalSize, element4, element5, false);
                }
                if (device == UnityEditor.iOS.DeviceType.iPad)
                {
                    XElement element6 = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./constraints/constraint[@id='wWy-Vq-I9X']");
                    XElement element7 = System.Xml.XPath.Extensions.XPathSelectElement(parentEl, "./constraints/constraint[@id='FGm-8j-iPJ']");
                    flag &= SetRelativeSizeConstraints(data.iPadRelativeVerticalSize, element6, element7, true);
                }
            }
            if (!flag)
            {
                Debug.Log("Launch screen XIB file is malformed");
            }
            return PlistDocument.CleanDtdToString(doc);
        }

        internal class XibWithBackgroundData
        {
            public Color background;
            public int iPadHeight = 0;
            public float iPadRelativeVerticalSize = 1f;
            public float iPadVerticalSize = 1f;
            public int iPadWidth = 0;
            public int iPhoneLandscapeHeight = 0;
            public float iPhoneLandscapeRelativeVerticalSize = 1f;
            public float iPhoneLandscapeVerticalSize = 1f;
            public int iPhoneLandscapeWidth = 0;
            public int iPhonePortraitHeight = 0;
            public float iPhonePortraitHorizontalSize = 1f;
            public float iPhonePortraitRelativeHorizontalSize = 1f;
            public int iPhonePortraitWidth = 0;
        }
    }
}


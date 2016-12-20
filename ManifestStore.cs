using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;

internal abstract class ManifestStore : ManifestWSA
{
    private readonly XNamespace _defaultNamespace = "http://schemas.microsoft.com/appx/2010/manifest";
    protected readonly List<UIOrientation> supportedOrientations;

    protected ManifestStore()
    {
        List<UIOrientation> list = new List<UIOrientation> {
            UIOrientation.LandscapeLeft,
            UIOrientation.LandscapeRight,
            UIOrientation.Portrait,
            UIOrientation.PortraitUpsideDown
        };
        this.supportedOrientations = list;
    }

    protected override XElement CreatePrerequisitesElement()
    {
        Version content = new Version(6, 2, 1);
        return new XElement((XName) (this.DefaultNamespace + "Prerequisites"), new object[] { new XElement((XName) (this.DefaultNamespace + "OSMinVersion"), content), new XElement((XName) (this.DefaultNamespace + "OSMaxVersionTested"), content) });
    }

    protected XElement GetSplashScreenElement(XNamespace @namespace)
    {
        return base.GetSplashScreenElement(@namespace, base.Images.storeSplashScreenImage);
    }

    protected override XNamespace DefaultNamespace
    {
        get
        {
            return this._defaultNamespace;
        }
    }

    protected override string StoreLogo
    {
        get
        {
            return base.Images.storeStoreLogo;
        }
    }
}


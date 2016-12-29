using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEditor;

internal class ManifestPhone81 : ManifestWSA
{
    private readonly XNamespace _defaultNamespace = "http://schemas.microsoft.com/appx/2010/manifest";
    private readonly XNamespace _m2Namespace = "http://schemas.microsoft.com/appx/2013/manifest";
    private readonly XNamespace _m3Namespace = "http://schemas.microsoft.com/appx/2014/manifest";
    private readonly XNamespace _mpNamespace = "http://schemas.microsoft.com/appx/2014/phone/manifest";
    private readonly List<UIOrientation> supportedOrientations;

    public ManifestPhone81()
    {
        List<UIOrientation> list = new List<UIOrientation> {
            UIOrientation.LandscapeLeft,
            UIOrientation.LandscapeRight,
            UIOrientation.Portrait
        };
        this.supportedOrientations = list;
    }

    protected override XElement CreateInitialRotationPreferenceElement() => 
        base.CreateInitialRotationPreferenceElement(this._m3Namespace, this.supportedOrientations);

    protected override XElement CreatePackageElement()
    {
        XElement element = base.CreatePackageElement();
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "m2"), this._m2Namespace));
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "m3"), this._m3Namespace));
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "mp"), this._mpNamespace));
        return element;
    }

    protected override XElement CreatePhoneIdentityElement() => 
        new XElement((XName) (this._mpNamespace + "PhoneIdentity"), new object[] { new XAttribute("PhoneProductId", base.ProductId), new XAttribute("PhonePublisherId", Guid.Empty.ToString("D", CultureInfo.InvariantCulture)) });

    protected override XElement CreatePrerequisitesElement()
    {
        Version content = new Version(6, 2, 1);
        return new XElement((XName) (this.DefaultNamespace + "Prerequisites"), new object[] { new XElement((XName) (this.DefaultNamespace + "OSMinVersion"), content), new XElement((XName) (this.DefaultNamespace + "OSMaxVersionTested"), content) });
    }

    protected override XElement CreateVisualElementsElement()
    {
        object[] content = new object[] { new XAttribute("DisplayName", PlayerSettings.productName), new XAttribute("Square150x150Logo", base.Images.phoneMediumTile), new XAttribute("Square44x44Logo", base.Images.phoneAppIcon), new XAttribute("Description", PlayerSettings.WSA.applicationDescription), new XAttribute("ForegroundText", base.ForegroundText), new XAttribute("BackgroundColor", "transparent") };
        XElement element = new XElement((XName) (this._m3Namespace + "VisualElements"), content);
        XElement defaultTileElement = this.GetDefaultTileElement();
        if (defaultTileElement != null)
        {
            element.Add(defaultTileElement);
        }
        XElement splashScreenElement = base.GetSplashScreenElement(this._m3Namespace, base.Images.phoneSplashScreenImage);
        element.Add(splashScreenElement);
        return element;
    }

    private XElement GetDefaultTileElement()
    {
        XElement element = new XElement((XName) (this._m3Namespace + "DefaultTile"));
        string str = null;
        switch (PlayerSettings.WSA.defaultTileSize)
        {
            case PlayerSettings.WSADefaultTileSize.NotSet:
                break;

            case PlayerSettings.WSADefaultTileSize.Medium:
                str = "square150x150Logo";
                break;

            case PlayerSettings.WSADefaultTileSize.Wide:
                str = "wide310x150Logo";
                break;

            default:
                throw new Exception($"Invalid WSADefaultTileSize value ({PlayerSettings.WSA.defaultTileSize}).");
        }
        if (!string.IsNullOrEmpty(str))
        {
            element.Add(new XAttribute("DefaultSize", str));
        }
        string tileShortName = PlayerSettings.WSA.tileShortName;
        if (!string.IsNullOrEmpty(tileShortName))
        {
            element.Add(new XAttribute("ShortName", tileShortName));
        }
        if (!string.IsNullOrEmpty(base.Images.phoneWideTile))
        {
            element.Add(new XAttribute("Wide310x150Logo", base.Images.phoneWideTile));
        }
        if (!string.IsNullOrEmpty(base.Images.phoneSmallTile))
        {
            element.Add(new XAttribute("Square71x71Logo", base.Images.phoneSmallTile));
        }
        XElement showNameOnTilesElement = this.GetShowNameOnTilesElement();
        if (showNameOnTilesElement != null)
        {
            element.Add(showNameOnTilesElement);
        }
        return ((!element.HasAttributes && !element.HasElements) ? null : element);
    }

    private XElement GetShowNameOnTilesElement()
    {
        XElement element = new XElement((XName) (this._m3Namespace + "ShowNameOnTiles"));
        if (PlayerSettings.WSA.mediumTileShowName)
        {
            element.Add(new XElement((XName) (this._m3Namespace + "ShowOn"), new XAttribute("Tile", "square150x150Logo")));
        }
        if (PlayerSettings.WSA.wideTileShowName && !string.IsNullOrEmpty(base.Images.phoneWideTile))
        {
            element.Add(new XElement((XName) (this._m3Namespace + "ShowOn"), new XAttribute("Tile", "wide310x150Logo")));
        }
        return (!element.HasElements ? null : element);
    }

    protected override XNamespace DefaultNamespace =>
        this._defaultNamespace;

    protected override string PackageName =>
        PlayerSettings.productGUID.ToString("D", CultureInfo.InvariantCulture);

    protected override string StoreLogo =>
        base.Images.phoneStoreLogo;
}


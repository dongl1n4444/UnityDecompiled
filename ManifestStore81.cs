using System;
using System.Xml.Linq;
using UnityEditor;

internal class ManifestStore81 : ManifestStore
{
    private readonly XNamespace _m2Namespace = "http://schemas.microsoft.com/appx/2013/manifest";

    protected override XElement CreateHumanInterfaceDeviceCapabilityElement()
    {
        return this.CreateHumanInterfaceDeviceCapabilityElement(this._m2Namespace);
    }

    protected override XElement CreateInitialRotationPreferenceElement()
    {
        return base.CreateInitialRotationPreferenceElement(this._m2Namespace, base.supportedOrientations);
    }

    protected override XElement CreatePackageElement()
    {
        XElement element = base.CreatePackageElement();
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "m2"), this._m2Namespace));
        return element;
    }

    protected override XElement CreateVisualElementsElement()
    {
        object[] content = new object[] { new XAttribute("DisplayName", PlayerSettings.productName), new XAttribute("Square150x150Logo", base.Images.storeTileLogo), new XAttribute("Square30x30Logo", base.Images.storeSmallLogo), new XAttribute("Description", PlayerSettings.WSA.applicationDescription), new XAttribute("ForegroundText", base.ForegroundText), new XAttribute("BackgroundColor", base.BackgroundColor) };
        XElement element = new XElement((XName) (this._m2Namespace + "VisualElements"), content);
        XElement defaultTileElement = this.GetDefaultTileElement();
        if (defaultTileElement != null)
        {
            element.Add(defaultTileElement);
        }
        XElement splashScreenElement = base.GetSplashScreenElement(this._m2Namespace);
        element.Add(splashScreenElement);
        return element;
    }

    private XElement GetDefaultTileElement()
    {
        XElement element = new XElement((XName) (this._m2Namespace + "DefaultTile"));
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
                throw new Exception(string.Format("Invalid WSADefaultTileSize value ({0}).", PlayerSettings.WSA.defaultTileSize));
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
        if (!string.IsNullOrEmpty(base.Images.storeTileWideLogo))
        {
            element.Add(new XAttribute("Wide310x150Logo", base.Images.storeTileWideLogo));
        }
        if (!string.IsNullOrEmpty(base.Images.storeSmallTile))
        {
            element.Add(new XAttribute("Square70x70Logo", base.Images.storeSmallTile));
        }
        if (!string.IsNullOrEmpty(base.Images.storeLargeTile))
        {
            element.Add(new XAttribute("Square310x310Logo", base.Images.storeLargeTile));
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
        XElement element = new XElement((XName) (this._m2Namespace + "ShowNameOnTiles"));
        if (PlayerSettings.WSA.mediumTileShowName)
        {
            element.Add(new XElement((XName) (this._m2Namespace + "ShowOn"), new XAttribute("Tile", "square150x150Logo")));
        }
        if (PlayerSettings.WSA.largeTileShowName && !string.IsNullOrEmpty(base.Images.storeLargeTile))
        {
            element.Add(new XElement((XName) (this._m2Namespace + "ShowOn"), new XAttribute("Tile", "square310x310Logo")));
        }
        if (PlayerSettings.WSA.wideTileShowName && !string.IsNullOrEmpty(base.Images.storeTileWideLogo))
        {
            element.Add(new XElement((XName) (this._m2Namespace + "ShowOn"), new XAttribute("Tile", "wide310x150Logo")));
        }
        return (!element.HasElements ? null : element);
    }
}


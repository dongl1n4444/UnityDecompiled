using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEditor;

internal class ManifestUAP : ManifestWSA
{
    private readonly XNamespace _defaultNamespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
    private readonly XNamespace _mpNamespace = "http://schemas.microsoft.com/appx/2014/phone/manifest";
    private readonly XNamespace _uap2Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10/2";
    private readonly XNamespace _uapNamespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
    private readonly XNamespace _uapRescapNamespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities";
    protected readonly List<UIOrientation> supportedOrientations;

    public ManifestUAP()
    {
        List<UIOrientation> list = new List<UIOrientation> {
            UIOrientation.LandscapeLeft,
            UIOrientation.LandscapeRight,
            UIOrientation.Portrait,
            UIOrientation.PortraitUpsideDown
        };
        this.supportedOrientations = list;
    }

    protected override XElement CreateDependenciesElement()
    {
        Version version = new Version(10, 0, 0x2800, 0);
        object[] content = new object[] { new XAttribute("Name", "Windows.Universal"), new XAttribute("MinVersion", version), new XAttribute("MaxVersionTested", MetroVisualStudioSolutionHelper.GetUWPSDKVersion()) };
        return new XElement((XName) (this.DefaultNamespace + "Dependencies"), new XElement((XName) (this.DefaultNamespace + "TargetDeviceFamily"), content));
    }

    protected override XElement CreateHumanInterfaceDeviceCapabilityElement()
    {
        return this.CreateHumanInterfaceDeviceCapabilityElement(this.DefaultNamespace);
    }

    protected override XElement CreateInitialRotationPreferenceElement()
    {
        return base.CreateInitialRotationPreferenceElement(this._uapNamespace, this.supportedOrientations);
    }

    protected override XElement CreatePackageElement()
    {
        XElement element = base.CreatePackageElement();
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "mp"), this._mpNamespace));
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "uap"), this._uapNamespace));
        element.Add(new XAttribute((XName) (XNamespace.Xmlns + "uap2"), this._uap2Namespace));
        string str = "uap uap2 mp";
        if (PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.InputInjectionBrokered))
        {
            element.Add(new XAttribute((XName) (XNamespace.Xmlns + "rescap"), this._uapRescapNamespace));
            str = str + " rescap";
        }
        element.Add(new XAttribute("IgnorableNamespaces", str));
        return element;
    }

    protected override XElement CreatePhoneIdentityElement()
    {
        return new XElement((XName) (this._mpNamespace + "PhoneIdentity"), new object[] { new XAttribute("PhoneProductId", base.ProductId), new XAttribute("PhonePublisherId", Guid.Empty.ToString("D", CultureInfo.InvariantCulture)) });
    }

    protected override XElement CreateVisualElementsElement()
    {
        object[] content = new object[] { new XAttribute("DisplayName", PlayerSettings.productName), new XAttribute("Square150x150Logo", base.Images.uwpSquare150x150Logo), new XAttribute("Square44x44Logo", base.Images.uwpSquare44x44Logo), new XAttribute("Description", PlayerSettings.WSA.applicationDescription), new XAttribute("BackgroundColor", base.BackgroundColor) };
        XElement element = new XElement((XName) (this._uapNamespace + "VisualElements"), content);
        XElement defaultTileElement = this.GetDefaultTileElement();
        if (defaultTileElement != null)
        {
            element.Add(defaultTileElement);
        }
        XElement splashScreenElement = base.GetSplashScreenElement(this._uapNamespace, base.Images.uwpSplashScreen);
        element.Add(splashScreenElement);
        return element;
    }

    private XElement GetDefaultTileElement()
    {
        XElement element = new XElement((XName) (this._uapNamespace + "DefaultTile"));
        string tileShortName = PlayerSettings.WSA.tileShortName;
        if (!string.IsNullOrEmpty(tileShortName))
        {
            element.Add(new XAttribute("ShortName", tileShortName));
        }
        if (!string.IsNullOrEmpty(base.Images.uwpWide310x150Logo))
        {
            element.Add(new XAttribute("Wide310x150Logo", base.Images.uwpWide310x150Logo));
        }
        if (!string.IsNullOrEmpty(base.Images.uwpSquare71x71Logo))
        {
            element.Add(new XAttribute("Square71x71Logo", base.Images.uwpSquare71x71Logo));
        }
        if (!string.IsNullOrEmpty(base.Images.uwpSquare310x310Logo))
        {
            element.Add(new XAttribute("Square310x310Logo", base.Images.uwpSquare310x310Logo));
        }
        XElement showNameOnTilesElement = this.GetShowNameOnTilesElement();
        if (showNameOnTilesElement != null)
        {
            element.Add(showNameOnTilesElement);
        }
        return ((!element.HasAttributes && !element.HasElements) ? null : element);
    }

    protected override XNamespace GetNamespaceForCapability(PlayerSettings.WSACapability cap)
    {
        switch (cap)
        {
            case PlayerSettings.WSACapability.BlockedChatMessages:
            case PlayerSettings.WSACapability.Chat:
            case PlayerSettings.WSACapability.Objects3D:
            case PlayerSettings.WSACapability.PhoneCall:
            case PlayerSettings.WSACapability.UserAccountInformation:
            case PlayerSettings.WSACapability.VoipCall:
                break;

            case PlayerSettings.WSACapability.SpatialPerception:
                return this._uap2Namespace;

            case PlayerSettings.WSACapability.InputInjectionBrokered:
                return this._uapRescapNamespace;

            default:
                switch (cap)
                {
                    case PlayerSettings.WSACapability.EnterpriseAuthentication:
                    case PlayerSettings.WSACapability.MusicLibrary:
                    case PlayerSettings.WSACapability.PicturesLibrary:
                    case PlayerSettings.WSACapability.RemovableStorage:
                    case PlayerSettings.WSACapability.SharedUserCertificates:
                    case PlayerSettings.WSACapability.VideosLibrary:
                        break;

                    case PlayerSettings.WSACapability.InternetClient:
                    case PlayerSettings.WSACapability.InternetClientServer:
                    case PlayerSettings.WSACapability.PrivateNetworkClientServer:
                        goto Label_0085;

                    default:
                        goto Label_0085;
                }
                break;
        }
        return this._uapNamespace;
    Label_0085:
        return this.DefaultNamespace;
    }

    protected override XNamespace GetNamespaceForFileTypeAssociationExtension()
    {
        return this._uapNamespace;
    }

    protected override XNamespace GetNamespaceForProtocolExtension()
    {
        return this._uapNamespace;
    }

    private XElement GetShowNameOnTilesElement()
    {
        XElement element = new XElement((XName) (this._uapNamespace + "ShowNameOnTiles"));
        if (PlayerSettings.WSA.mediumTileShowName)
        {
            element.Add(new XElement((XName) (this._uapNamespace + "ShowOn"), new XAttribute("Tile", "square150x150Logo")));
        }
        if (PlayerSettings.WSA.largeTileShowName && !string.IsNullOrEmpty(base.Images.uwpSquare310x310Logo))
        {
            element.Add(new XElement((XName) (this._uapNamespace + "ShowOn"), new XAttribute("Tile", "square310x310Logo")));
        }
        if (PlayerSettings.WSA.wideTileShowName && !string.IsNullOrEmpty(base.Images.uwpWide310x150Logo))
        {
            element.Add(new XElement((XName) (this._uapNamespace + "ShowOn"), new XAttribute("Tile", "wide310x150Logo")));
        }
        return (!element.HasElements ? null : element);
    }

    protected override bool IsCapabilitySupported(PlayerSettings.WSACapability cap)
    {
        switch (cap)
        {
            case PlayerSettings.WSACapability.EnterpriseAuthentication:
            case PlayerSettings.WSACapability.InternetClient:
            case PlayerSettings.WSACapability.InternetClientServer:
            case PlayerSettings.WSACapability.MusicLibrary:
            case PlayerSettings.WSACapability.PicturesLibrary:
            case PlayerSettings.WSACapability.PrivateNetworkClientServer:
            case PlayerSettings.WSACapability.RemovableStorage:
            case PlayerSettings.WSACapability.SharedUserCertificates:
            case PlayerSettings.WSACapability.VideosLibrary:
            case PlayerSettings.WSACapability.WebCam:
            case PlayerSettings.WSACapability.Proximity:
            case PlayerSettings.WSACapability.Microphone:
            case PlayerSettings.WSACapability.Location:
            case PlayerSettings.WSACapability.HumanInterfaceDevice:
            case PlayerSettings.WSACapability.AllJoyn:
            case PlayerSettings.WSACapability.BlockedChatMessages:
            case PlayerSettings.WSACapability.Chat:
            case PlayerSettings.WSACapability.CodeGeneration:
            case PlayerSettings.WSACapability.Objects3D:
            case PlayerSettings.WSACapability.PhoneCall:
            case PlayerSettings.WSACapability.UserAccountInformation:
            case PlayerSettings.WSACapability.VoipCall:
            case PlayerSettings.WSACapability.Bluetooth:
            case PlayerSettings.WSACapability.SpatialPerception:
            case PlayerSettings.WSACapability.InputInjectionBrokered:
                return true;
        }
        return false;
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
            return base.Images.uwpStoreLogo;
        }
    }
}


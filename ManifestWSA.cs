using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UnityEditor;

internal abstract class ManifestWSA
{
    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ProjectImages <Images>k__BackingField;

    protected ManifestWSA()
    {
    }

    private void AddRotationIfSupported(List<string> orientations, List<UIOrientation> supportedOrientations, UIOrientation orientation)
    {
        if (supportedOrientations.Contains(orientation))
        {
            switch (orientation)
            {
                case UIOrientation.Portrait:
                    orientations.Add("portrait");
                    break;

                case UIOrientation.PortraitUpsideDown:
                    orientations.Add("portraitFlipped");
                    break;

                case UIOrientation.LandscapeRight:
                    orientations.Add("landscapeFlipped");
                    break;

                case UIOrientation.LandscapeLeft:
                    orientations.Add("landscape");
                    break;
            }
        }
    }

    public void Create(string path, ProjectImages images)
    {
        this.Images = images;
        XDeclaration declaration = new XDeclaration("1.0", "utf-8", null);
        XDocument document = new XDocument(declaration, new object[0]);
        XElement content = this.CreatePackageElement();
        document.Add(content);
        XElement element2 = this.CreateIdentityElement();
        content.Add(element2);
        XElement element3 = this.CreatePhoneIdentityElement();
        if (element3 != null)
        {
            content.Add(element3);
        }
        XElement element4 = this.CreatePropertiesElement();
        content.Add(element4);
        XElement element5 = this.CreatePrerequisitesElement();
        if (element5 != null)
        {
            content.Add(element5);
        }
        XElement element6 = this.CreateDependenciesElement();
        if (element6 != null)
        {
            content.Add(element6);
        }
        XElement element7 = this.CreateResourcesElement();
        content.Add(element7);
        XElement element8 = this.CreateApplicationsElement();
        content.Add(element8);
        XElement element9 = this.CreateApplicationElement();
        element8.Add(element9);
        XElement element10 = this.CreateVisualElementsElement();
        element9.Add(element10);
        XElement element11 = this.CreateInitialRotationPreferenceElement();
        if (element11 != null)
        {
            element10.Add(element11);
        }
        XElement element12 = this.CreateExtensionsElement();
        if (element12 != null)
        {
            element9.Add(element12);
        }
        XElement element13 = this.CreateCapabilitiesElement();
        if (element13 != null)
        {
            content.Add(element13);
        }
        using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            document.Save(writer);
        }
        this.Images = null;
    }

    protected virtual XElement CreateApplicationElement()
    {
        return new XElement((XName) (this.DefaultNamespace + "Application"), new object[] { new XAttribute("Id", "App"), new XAttribute("Executable", "$targetnametoken$.exe"), new XAttribute("EntryPoint", this.PackageName + ".App") });
    }

    protected virtual XElement CreateApplicationsElement()
    {
        return new XElement((XName) (this.DefaultNamespace + "Applications"));
    }

    protected virtual XElement CreateCapabilitiesElement()
    {
        XElement element = new XElement((XName) (this.DefaultNamespace + "Capabilities"));
        List<XElement> list = new List<XElement>();
        List<XElement> list2 = new List<XElement>();
        PlayerSettings.WSACapability[] values = (PlayerSettings.WSACapability[]) Enum.GetValues(typeof(PlayerSettings.WSACapability));
        foreach (PlayerSettings.WSACapability capability in values)
        {
            if (PlayerSettings.WSA.GetCapability(capability) && this.IsCapabilitySupported(capability))
            {
                string str = capability.ToString();
                switch (capability)
                {
                    case PlayerSettings.WSACapability.WebCam:
                    case PlayerSettings.WSACapability.Proximity:
                    case PlayerSettings.WSACapability.Microphone:
                    case PlayerSettings.WSACapability.Location:
                    case PlayerSettings.WSACapability.Bluetooth:
                    {
                        str = str.ToLowerInvariant();
                        list2.Add(new XElement((XName) (this.DefaultNamespace + "DeviceCapability"), new XAttribute("Name", str)));
                        continue;
                    }
                    case PlayerSettings.WSACapability.HumanInterfaceDevice:
                    {
                        XElement item = this.CreateHumanInterfaceDeviceCapabilityElement();
                        if (item != null)
                        {
                            list2.Add(item);
                        }
                        continue;
                    }
                }
                list.Add(this.CreateCapabilityElement(capability));
            }
        }
        foreach (XElement element3 in list)
        {
            element.Add(element3);
        }
        foreach (XElement element4 in list2)
        {
            element.Add(element4);
        }
        return (!element.HasElements ? null : element);
    }

    protected virtual XElement CreateCapabilityElement(PlayerSettings.WSACapability cap)
    {
        string str = cap.ToString();
        str = char.ToLowerInvariant(str[0]) + str.Substring(1);
        return new XElement((XName) (this.GetNamespaceForCapability(cap) + "Capability"), new XAttribute("Name", str));
    }

    protected virtual XElement CreateDependenciesElement()
    {
        return null;
    }

    protected virtual XElement CreateExtensionsElement()
    {
        XElement element = new XElement((XName) (this.DefaultNamespace + "Extensions"));
        XElement content = this.CreateFileTypeAssociationExtensionElement();
        if (content != null)
        {
            element.Add(content);
        }
        XElement element3 = this.CreateProtocolExtensionElement();
        if (element3 != null)
        {
            element.Add(element3);
        }
        return (!element.HasElements ? null : element);
    }

    protected virtual XElement CreateFileTypeAssociationExtensionElement()
    {
        PlayerSettings.WSAFileTypeAssociations fileTypeAssociations = PlayerSettings.WSA.Declarations.fileTypeAssociations;
        if (string.IsNullOrEmpty(fileTypeAssociations.name))
        {
            return null;
        }
        XElement element2 = new XElement((XName) (this.GetNamespaceForFileTypeAssociationExtension() + "FileTypeAssociation"), new XAttribute("Name", fileTypeAssociations.name));
        XElement content = new XElement((XName) (this.GetNamespaceForFileTypeAssociationExtension() + "SupportedFileTypes"));
        foreach (XElement element4 in this.GetFileTypeAssociationItems(fileTypeAssociations.supportedFileTypes))
        {
            content.Add(element4);
        }
        element2.Add(content);
        return new XElement((XName) (this.GetNamespaceForFileTypeAssociationExtension() + "Extension"), new object[] { new XAttribute("Category", "windows.fileTypeAssociation"), element2 });
    }

    private XElement CreateHIDDevice(XNamespace @namespace, string usageDesc)
    {
        return new XElement((XName) (@namespace + "Device"), new object[] { new XAttribute("Id", "any"), new XElement((XName) (@namespace + "Function"), new XAttribute("Type", usageDesc)) });
    }

    protected virtual XElement CreateHumanInterfaceDeviceCapabilityElement()
    {
        return null;
    }

    protected virtual XElement CreateHumanInterfaceDeviceCapabilityElement(XNamespace @namespace)
    {
        object[] content = new object[2];
        content[0] = new XAttribute("Name", "humaninterfacedevice");
        XElement[] elementArray1 = new XElement[] { this.CreateHIDDevice(@namespace, "usage:0001 0004"), this.CreateHIDDevice(@namespace, "usage:0001 0005") };
        content[1] = elementArray1;
        return new XElement((XName) (@namespace + "DeviceCapability"), content);
    }

    protected virtual XElement CreateIdentityElement()
    {
        return new XElement((XName) (this.DefaultNamespace + "Identity"), new object[] { new XAttribute("Name", this.PackageName), new XAttribute("Publisher", "CN=" + PlayerSettings.WSA.certificateSubject), new XAttribute("Version", PlayerSettings.WSA.packageVersion) });
    }

    protected virtual XElement CreateInitialRotationPreferenceElement()
    {
        return null;
    }

    protected XElement CreateInitialRotationPreferenceElement(XNamespace elementNamespace, List<UIOrientation> supportedOrientations)
    {
        List<string> orientations = new List<string>();
        UIOrientation defaultInterfaceOrientation = PlayerSettings.defaultInterfaceOrientation;
        switch (defaultInterfaceOrientation)
        {
            case UIOrientation.Portrait:
            case UIOrientation.PortraitUpsideDown:
            case UIOrientation.LandscapeRight:
            case UIOrientation.LandscapeLeft:
                this.AddRotationIfSupported(orientations, supportedOrientations, defaultInterfaceOrientation);
                break;

            case UIOrientation.AutoRotation:
                if (PlayerSettings.allowedAutorotateToLandscapeLeft)
                {
                    this.AddRotationIfSupported(orientations, supportedOrientations, UIOrientation.LandscapeLeft);
                }
                if (PlayerSettings.allowedAutorotateToLandscapeRight)
                {
                    this.AddRotationIfSupported(orientations, supportedOrientations, UIOrientation.LandscapeRight);
                }
                if (PlayerSettings.allowedAutorotateToPortrait)
                {
                    this.AddRotationIfSupported(orientations, supportedOrientations, UIOrientation.Portrait);
                }
                if (PlayerSettings.allowedAutorotateToPortraitUpsideDown)
                {
                    this.AddRotationIfSupported(orientations, supportedOrientations, UIOrientation.PortraitUpsideDown);
                }
                break;
        }
        if (orientations.Count > 0)
        {
            XElement element = new XElement((XName) (elementNamespace + "InitialRotationPreference"));
            foreach (string str in orientations)
            {
                element.Add(new XElement((XName) (elementNamespace + "Rotation"), new XAttribute("Preference", str)));
            }
            return element;
        }
        return null;
    }

    protected virtual XElement CreatePackageElement()
    {
        return new XElement((XName) (this.DefaultNamespace + "Package"));
    }

    protected virtual XElement CreatePhoneIdentityElement()
    {
        return null;
    }

    protected virtual XElement CreatePrerequisitesElement()
    {
        return null;
    }

    protected virtual XElement CreatePropertiesElement()
    {
        return new XElement((XName) (this.DefaultNamespace + "Properties"), new object[] { new XElement((XName) (this.DefaultNamespace + "DisplayName"), PlayerSettings.productName), new XElement((XName) (this.DefaultNamespace + "PublisherDisplayName"), PlayerSettings.companyName), new XElement((XName) (this.DefaultNamespace + "Logo"), this.StoreLogo) });
    }

    protected virtual XElement CreateProtocolExtensionElement()
    {
        string stringValue = PlayerSettings.FindProperty("metroProtocolName").stringValue;
        if (string.IsNullOrEmpty(stringValue))
        {
            return null;
        }
        XElement element2 = new XElement((XName) (this.GetNamespaceForProtocolExtension() + "Protocol"), new XAttribute("Name", stringValue));
        return new XElement((XName) (this.GetNamespaceForProtocolExtension() + "Extension"), new object[] { new XAttribute("Category", "windows.protocol"), element2 });
    }

    protected virtual XElement CreateResourcesElement()
    {
        return new XElement((XName) (this.DefaultNamespace + "Resources"), new XElement((XName) (this.DefaultNamespace + "Resource"), new XAttribute("Language", "x-generate")));
    }

    protected abstract XElement CreateVisualElementsElement();
    [DebuggerHidden]
    protected IEnumerable<XElement> GetFileTypeAssociationItems(PlayerSettings.WSASupportedFileType[] types)
    {
        return new <GetFileTypeAssociationItems>c__Iterator0 { 
            types = types,
            $this = this,
            $PC = -2
        };
    }

    protected virtual XNamespace GetNamespaceForCapability(PlayerSettings.WSACapability cap)
    {
        return this.DefaultNamespace;
    }

    protected virtual XNamespace GetNamespaceForFileTypeAssociationExtension()
    {
        return this.DefaultNamespace;
    }

    protected virtual XNamespace GetNamespaceForProtocolExtension()
    {
        return this.DefaultNamespace;
    }

    protected XElement GetSplashScreenElement(XNamespace @namespace, string image)
    {
        XElement element = new XElement((XName) (@namespace + "SplashScreen"), new XAttribute("Image", image));
        Color32? splashScreenBackgroundColor = MetroVisualStudioSolutionHelper.GetSplashScreenBackgroundColor();
        if (splashScreenBackgroundColor.HasValue)
        {
            element.Add(new XAttribute("BackgroundColor", MetroVisualStudioSolutionHelper.ColorToXAMLAttribute(splashScreenBackgroundColor.Value)));
        }
        return element;
    }

    protected virtual bool IsCapabilitySupported(PlayerSettings.WSACapability cap)
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
                return true;
        }
        return false;
    }

    protected string BackgroundColor
    {
        get
        {
            return MetroVisualStudioSolutionHelper.ColorToXAMLAttribute(PlayerSettings.WSA.tileBackgroundColor);
        }
    }

    protected abstract XNamespace DefaultNamespace { get; }

    protected string ForegroundText
    {
        get
        {
            PlayerSettings.WSAApplicationForegroundText tileForegroundText = PlayerSettings.WSA.tileForegroundText;
            if (tileForegroundText != PlayerSettings.WSAApplicationForegroundText.Light)
            {
                if (tileForegroundText != PlayerSettings.WSAApplicationForegroundText.Dark)
                {
                    throw new Exception(string.Format("Invalid WSAApplicationForegroundText value ({0}).", PlayerSettings.WSA.tileForegroundText));
                }
            }
            else
            {
                return "light";
            }
            return "dark";
        }
    }

    protected ProjectImages Images { get; private set; }

    protected virtual string PackageName
    {
        get
        {
            return Utility.GetPackageName(true);
        }
    }

    protected string ProductId
    {
        get
        {
            return PlayerSettings.productGUID.ToString("D", CultureInfo.InvariantCulture);
        }
    }

    protected abstract string StoreLogo { get; }

    [CompilerGenerated]
    private sealed class <GetFileTypeAssociationItems>c__Iterator0 : IEnumerable, IEnumerable<XElement>, IEnumerator, IDisposable, IEnumerator<XElement>
    {
        internal XElement $current;
        internal bool $disposing;
        internal PlayerSettings.WSASupportedFileType[] $locvar0;
        internal int $locvar1;
        internal int $PC;
        internal ManifestWSA $this;
        internal string <contentType>__2;
        internal XElement <fileTypeElement>__1;
        internal PlayerSettings.WSASupportedFileType <type>__0;
        internal PlayerSettings.WSASupportedFileType[] types;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$disposing = true;
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$locvar0 = this.types;
                    this.$locvar1 = 0;
                    break;

                case 1:
                    this.$locvar1++;
                    break;

                default:
                    goto Label_010D;
            }
            if (this.$locvar1 < this.$locvar0.Length)
            {
                this.<type>__0 = this.$locvar0[this.$locvar1];
                this.<fileTypeElement>__1 = new XElement((XName) (this.$this.GetNamespaceForFileTypeAssociationExtension() + "FileType"), this.<type>__0.fileType);
                this.<contentType>__2 = this.<type>__0.contentType;
                if (!string.IsNullOrEmpty(this.<contentType>__2))
                {
                    this.<fileTypeElement>__1.Add(new XAttribute("ContentType", this.<contentType>__2));
                }
                this.$current = this.<fileTypeElement>__1;
                if (!this.$disposing)
                {
                    this.$PC = 1;
                }
                return true;
            }
            this.$PC = -1;
        Label_010D:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<XElement> IEnumerable<XElement>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ManifestWSA.<GetFileTypeAssociationItems>c__Iterator0 { 
                $this = this.$this,
                types = this.types
            };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<System.Xml.Linq.XElement>.GetEnumerator();
        }

        XElement IEnumerator<XElement>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}


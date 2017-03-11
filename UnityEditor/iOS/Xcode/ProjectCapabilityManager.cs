namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.IO;

    public class ProjectCapabilityManager
    {
        private readonly string m_BuildPath;
        private readonly string m_EntitlementFilePath;
        private PlistDocument m_Entitlements;
        private PlistDocument m_InfoPlist;
        private readonly string m_PBXProjectPath;
        private readonly string m_TargetGuid;
        protected internal PBXProject project;

        public ProjectCapabilityManager(string pbxProjectPath, string entitlementFilePath, string targetName)
        {
            this.m_BuildPath = Directory.GetParent(Path.GetDirectoryName(pbxProjectPath)).FullName;
            this.m_EntitlementFilePath = entitlementFilePath;
            this.m_PBXProjectPath = pbxProjectPath;
            this.project = new PBXProject();
            this.project.ReadFromString(File.ReadAllText(this.m_PBXProjectPath));
            this.m_TargetGuid = this.project.TargetGuidByName(targetName);
        }

        public void AddAppGroups(string[] groups)
        {
            PlistElement element = new PlistElementArray();
            this.GetOrCreateEntitlementDoc().root[AppGroupsEntitlements.Key] = element;
            PlistElementArray array = element as PlistElementArray;
            for (int i = 0; i < groups.Length; i++)
            {
                array.values.Add(new PlistElementString(groups[i]));
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.AppGroups, this.m_EntitlementFilePath, false);
        }

        public void AddApplePay(string[] merchants)
        {
            PlistElement element = new PlistElementArray();
            this.GetOrCreateEntitlementDoc().root[ApplePayEntitlements.Key] = element;
            PlistElementArray array = element as PlistElementArray;
            for (int i = 0; i < merchants.Length; i++)
            {
                array.values.Add(new PlistElementString(merchants[i]));
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.ApplePay, this.m_EntitlementFilePath, false);
        }

        public void AddAssociatedDomains(string[] domains)
        {
            PlistElement element = new PlistElementArray();
            this.GetOrCreateEntitlementDoc().root[AssociatedDomainsEntitlements.Key] = element;
            PlistElementArray array = element as PlistElementArray;
            for (int i = 0; i < domains.Length; i++)
            {
                array.values.Add(new PlistElementString(domains[i]));
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.AssociatedDomains, this.m_EntitlementFilePath, false);
        }

        public void AddBackgroundModes(BackgroundModesOptions options)
        {
            PlistElement element;
            PlistElement element1 = this.GetOrCreateInfoDoc().root[BackgroundInfo.Key];
            if (element1 == null)
            {
                element = new PlistElementArray();
                this.GetOrCreateInfoDoc().root[BackgroundInfo.Key] = element;
            }
            PlistElementArray root = element as PlistElementArray;
            if ((options & BackgroundModesOptions.ActsAsABluetoothLEAccessory) == BackgroundModesOptions.ActsAsABluetoothLEAccessory)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeActsBluetoothValue);
            }
            if ((options & BackgroundModesOptions.AudioAirplayPiP) == BackgroundModesOptions.AudioAirplayPiP)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeAudioValue);
            }
            if ((options & BackgroundModesOptions.BackgroundFetch) == BackgroundModesOptions.BackgroundFetch)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeFetchValue);
            }
            if ((options & BackgroundModesOptions.ExternalAccessoryCommunication) == BackgroundModesOptions.ExternalAccessoryCommunication)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeExtAccessoryValue);
            }
            if ((options & BackgroundModesOptions.LocationUpdates) == BackgroundModesOptions.LocationUpdates)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeLocationValue);
            }
            if ((options & BackgroundModesOptions.NewsstandDownloads) == BackgroundModesOptions.NewsstandDownloads)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeNewsstandValue);
            }
            if ((options & BackgroundModesOptions.RemoteNotifications) == BackgroundModesOptions.RemoteNotifications)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModePushValue);
            }
            if ((options & BackgroundModesOptions.VoiceOverIP) == BackgroundModesOptions.VoiceOverIP)
            {
                this.GetOrCreateStringElementInArray(root, BackgroundInfo.ModeVOIPValue);
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.BackgroundModes, null, false);
        }

        public void AddDataProtection()
        {
            this.GetOrCreateEntitlementDoc().root[DataProtectionEntitlements.Key] = new PlistElementString(DataProtectionEntitlements.Value);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.DataProtection, this.m_EntitlementFilePath, false);
        }

        public void AddGameCenter()
        {
            PlistElement element;
            PlistElement element1 = this.GetOrCreateInfoDoc().root[GameCenterInfo.Key];
            if (element1 == null)
            {
                element = new PlistElementArray();
                this.GetOrCreateInfoDoc().root[GameCenterInfo.Key] = element;
            }
            PlistElementArray array = element as PlistElementArray;
            array.values.Add(new PlistElementString(GameCenterInfo.Value));
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.GameCenter, null, false);
        }

        public void AddHealthKit()
        {
            PlistElement element;
            PlistElement element1 = this.GetOrCreateInfoDoc().root[HealthInfo.Key];
            if (element1 == null)
            {
                element = new PlistElementArray();
                this.GetOrCreateInfoDoc().root[HealthInfo.Key] = element;
            }
            PlistElementArray root = element as PlistElementArray;
            this.GetOrCreateStringElementInArray(root, HealthInfo.Value);
            this.GetOrCreateEntitlementDoc().root[HealthKitEntitlements.Key] = new PlistElementBoolean(true);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.HealthKit, this.m_EntitlementFilePath, false);
        }

        public void AddHomeKit()
        {
            this.GetOrCreateEntitlementDoc().root[HomeKitEntitlements.Key] = new PlistElementBoolean(true);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.HomeKit, this.m_EntitlementFilePath, false);
        }

        public void AddiCloud(bool keyValueStorage, bool iCloudDocument, string[] customContainers)
        {
            PlistDocument orCreateEntitlementDoc = this.GetOrCreateEntitlementDoc();
            PlistElement element = new PlistElementArray();
            orCreateEntitlementDoc.root[ICloudEntitlements.ContainerIdValue] = element;
            PlistElementArray array = element as PlistElementArray;
            if (iCloudDocument)
            {
                array.values.Add(new PlistElementString(ICloudEntitlements.ContainerIdValue));
                element = new PlistElementArray();
                orCreateEntitlementDoc.root[ICloudEntitlements.ServicesKey] = element;
                PlistElementArray array2 = element as PlistElementArray;
                array2.values.Add(new PlistElementString(ICloudEntitlements.ServicesKitValue));
                array2.values.Add(new PlistElementString(ICloudEntitlements.ServicesDocValue));
                element = new PlistElementArray();
                orCreateEntitlementDoc.root[ICloudEntitlements.UbiquityContainerIdKey] = element;
                PlistElementArray array3 = element as PlistElementArray;
                array3.values.Add(new PlistElementString(ICloudEntitlements.UbiquityContainerIdValue));
                for (int i = 0; i < customContainers.Length; i++)
                {
                    array2.values.Add(new PlistElementString(customContainers[i]));
                }
            }
            if (keyValueStorage)
            {
                orCreateEntitlementDoc.root[ICloudEntitlements.KeyValueStoreKey] = new PlistElementString(ICloudEntitlements.KeyValueStoreValue);
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.iCloud, this.m_EntitlementFilePath, iCloudDocument);
        }

        public void AddInAppPurchase()
        {
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.InAppPurchase, null, false);
        }

        public void AddInterAppAudio()
        {
            this.GetOrCreateEntitlementDoc().root[AudioEntitlements.Key] = new PlistElementBoolean(true);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.InterAppAudio, this.m_EntitlementFilePath, false);
        }

        public void AddKeychainSharing(string[] accessGroups)
        {
            PlistElement element = new PlistElementArray();
            this.GetOrCreateEntitlementDoc().root[KeyChainEntitlements.Key] = element;
            PlistElementArray array = element as PlistElementArray;
            if (accessGroups != null)
            {
                for (int i = 0; i < accessGroups.Length; i++)
                {
                    array.values.Add(new PlistElementString(accessGroups[i]));
                }
            }
            else
            {
                array.values.Add(new PlistElementString(KeyChainEntitlements.DefaultValue));
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.KeychainSharing, this.m_EntitlementFilePath, false);
        }

        public void AddMaps(MapsOptions options)
        {
            PlistElement element;
            PlistElement element1 = this.GetOrCreateInfoDoc().root[MapsInfo.BundleKey];
            if (element1 == null)
            {
                element = new PlistElementArray();
                this.GetOrCreateInfoDoc().root[MapsInfo.BundleKey] = element;
            }
            PlistElementArray root = element as PlistElementArray;
            root.values.Add(new PlistElementDict());
            PlistElementDict orCreateUniqueDictElementInArray = this.GetOrCreateUniqueDictElementInArray(root);
            orCreateUniqueDictElementInArray[MapsInfo.BundleNameKey] = new PlistElementString(MapsInfo.BundleNameValue);
            PlistElement element2 = orCreateUniqueDictElementInArray[MapsInfo.BundleTypeKey];
            if (element2 == null)
            {
                element = new PlistElementArray();
                orCreateUniqueDictElementInArray[MapsInfo.BundleTypeKey] = element;
            }
            PlistElementArray array2 = element as PlistElementArray;
            this.GetOrCreateStringElementInArray(array2, MapsInfo.BundleTypeValue);
            PlistElement element3 = this.GetOrCreateInfoDoc().root[MapsInfo.ModeKey];
            if (element3 == null)
            {
                element = new PlistElementArray();
                this.GetOrCreateInfoDoc().root[MapsInfo.ModeKey] = element;
            }
            PlistElementArray array3 = element as PlistElementArray;
            if ((options & MapsOptions.Airplane) == MapsOptions.Airplane)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModePlaneValue);
            }
            if ((options & MapsOptions.Bike) == MapsOptions.Bike)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeBikeValue);
            }
            if ((options & MapsOptions.Bus) == MapsOptions.Bus)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeBusValue);
            }
            if ((options & MapsOptions.Car) == MapsOptions.Car)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeCarValue);
            }
            if ((options & MapsOptions.Ferry) == MapsOptions.Ferry)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeFerryValue);
            }
            if ((options & MapsOptions.Other) == MapsOptions.Other)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeOtherValue);
            }
            if ((options & MapsOptions.Pedestrian) == MapsOptions.Pedestrian)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModePedestrianValue);
            }
            if ((options & MapsOptions.RideSharing) == MapsOptions.RideSharing)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeRideShareValue);
            }
            if ((options & MapsOptions.StreetCar) == MapsOptions.StreetCar)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeStreetCarValue);
            }
            if ((options & MapsOptions.Subway) == MapsOptions.Subway)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeSubwayValue);
            }
            if ((options & MapsOptions.Taxi) == MapsOptions.Taxi)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeTaxiValue);
            }
            if ((options & MapsOptions.Train) == MapsOptions.Train)
            {
                this.GetOrCreateStringElementInArray(array3, MapsInfo.ModeTrainValue);
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.Maps, null, false);
        }

        public void AddPersonalVPN()
        {
            PlistElement element = new PlistElementArray();
            this.GetOrCreateEntitlementDoc().root[VPNEntitlements.Key] = element;
            PlistElementArray array = element as PlistElementArray;
            array.values.Add(new PlistElementString(VPNEntitlements.Value));
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.PersonalVPN, this.m_EntitlementFilePath, false);
        }

        public void AddPushNotifications(bool development)
        {
            this.GetOrCreateEntitlementDoc().root[PushNotificationEntitlements.Key] = new PlistElementString(!development ? PushNotificationEntitlements.ProductionValue : PushNotificationEntitlements.DevelopmentValue);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.PushNotifications, this.m_EntitlementFilePath, false);
        }

        public void AddSiri()
        {
            this.GetOrCreateEntitlementDoc().root[SiriEntitlements.Key] = new PlistElementBoolean(true);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.Siri, this.m_EntitlementFilePath, false);
        }

        public void AddWallet(string[] passSubset)
        {
            PlistElement element = new PlistElementArray();
            this.GetOrCreateEntitlementDoc().root[WalletEntitlements.Key] = element;
            PlistElementArray array = element as PlistElementArray;
            if (((passSubset == null) || (passSubset.Length == 0)) && (array != null))
            {
                array.values.Add(new PlistElementString(WalletEntitlements.BaseValue + WalletEntitlements.BaseValue));
            }
            else
            {
                for (int i = 0; i < passSubset.Length; i++)
                {
                    if (array != null)
                    {
                        array.values.Add(new PlistElementString(WalletEntitlements.BaseValue + passSubset[i]));
                    }
                }
            }
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.Wallet, this.m_EntitlementFilePath, false);
        }

        public void AddWirelessAccessoryConfiguration()
        {
            this.GetOrCreateEntitlementDoc().root[WirelessAccessoryConfigurationEntitlements.Key] = new PlistElementBoolean(true);
            this.project.AddCapability(this.m_TargetGuid, PBXCapabilityType.WirelessAccessoryConfiguration, this.m_EntitlementFilePath, false);
        }

        private PlistDocument GetOrCreateEntitlementDoc()
        {
            if (this.m_Entitlements == null)
            {
                this.m_Entitlements = new PlistDocument();
                string[] files = Directory.GetFiles(this.m_BuildPath, this.m_EntitlementFilePath);
                if (files.Length > 0)
                {
                    this.m_Entitlements.ReadFromFile(files[0]);
                }
                else
                {
                    this.m_Entitlements.Create();
                }
            }
            return this.m_Entitlements;
        }

        private PlistDocument GetOrCreateInfoDoc()
        {
            if (this.m_InfoPlist == null)
            {
                this.m_InfoPlist = new PlistDocument();
                string[] files = Directory.GetFiles(this.m_BuildPath + "/", "Info.plist");
                if (files.Length > 0)
                {
                    this.m_InfoPlist.ReadFromFile(files[0]);
                }
                else
                {
                    this.m_InfoPlist.Create();
                }
            }
            return this.m_InfoPlist;
        }

        private PlistElementString GetOrCreateStringElementInArray(PlistElementArray root, string value)
        {
            PlistElementString item = null;
            int count = root.values.Count;
            bool flag = false;
            for (int i = 0; i < count; i++)
            {
                if ((root.values[i] is PlistElementString) && ((root.values[i] as PlistElementString).value == value))
                {
                    item = root.values[i] as PlistElementString;
                    flag = true;
                }
            }
            if (!flag)
            {
                item = new PlistElementString(value);
                root.values.Add(item);
            }
            return item;
        }

        private PlistElementDict GetOrCreateUniqueDictElementInArray(PlistElementArray root)
        {
            if (root.values.Count == 0)
            {
                return (root.values[0] as PlistElementDict);
            }
            PlistElementDict item = new PlistElementDict();
            root.values.Add(item);
            return item;
        }

        public void WriteToFile()
        {
            File.WriteAllText(this.m_PBXProjectPath, this.project.WriteToString());
            if (this.m_Entitlements != null)
            {
                this.m_Entitlements.WriteToFile(PBXPath.Combine(this.m_BuildPath, this.m_EntitlementFilePath));
            }
            if (this.m_InfoPlist != null)
            {
                this.m_InfoPlist.WriteToFile(PBXPath.Combine(this.m_BuildPath, "Info.plist"));
            }
        }
    }
}


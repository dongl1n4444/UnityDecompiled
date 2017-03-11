namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class PBXCapabilityType
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        public static readonly PBXCapabilityType AppGroups = new PBXCapabilityType("com.apple.ApplicationGroups.iOS", true, "", false);
        public static readonly PBXCapabilityType ApplePay = new PBXCapabilityType("com.apple.ApplePay", true, "", false);
        public static readonly PBXCapabilityType AssociatedDomains = new PBXCapabilityType("com.apple.SafariKeychain", true, "", false);
        public static readonly PBXCapabilityType BackgroundModes = new PBXCapabilityType("com.apple.BackgroundModes", false, "", false);
        public static readonly PBXCapabilityType DataProtection = new PBXCapabilityType("com.apple.DataProtection", true, "", false);
        public static readonly PBXCapabilityType GameCenter = new PBXCapabilityType("com.apple.GameCenter", false, "GameKit.framework", false);
        public static readonly PBXCapabilityType HealthKit = new PBXCapabilityType("com.apple.HealthKit", true, "HealthKit.framework", false);
        public static readonly PBXCapabilityType HomeKit = new PBXCapabilityType("com.apple.HomeKit", true, "HomeKit.framework", false);
        public static readonly PBXCapabilityType iCloud = new PBXCapabilityType("com.apple.iCloud", true, "CloudKit.framework", true);
        public static readonly PBXCapabilityType InAppPurchase = new PBXCapabilityType("com.apple.InAppPurchase", false, "", false);
        public static readonly PBXCapabilityType InterAppAudio = new PBXCapabilityType("com.apple.InterAppAudio", true, "AudioToolbox.framework", false);
        public static readonly PBXCapabilityType KeychainSharing = new PBXCapabilityType("com.apple.KeychainSharing", true, "", false);
        private readonly string m_Framework;
        private readonly string m_ID;
        private readonly bool m_OptionalFramework;
        private readonly bool m_RequiresEntitlements;
        public static readonly PBXCapabilityType Maps = new PBXCapabilityType("com.apple.Maps.iOS", false, "MapKit.framework", false);
        public static readonly PBXCapabilityType PersonalVPN = new PBXCapabilityType("com.apple.VPNLite", true, "NetworkExtension.framework", false);
        public static readonly PBXCapabilityType PushNotifications = new PBXCapabilityType("com.apple.Push", true, "", false);
        public static readonly PBXCapabilityType Siri = new PBXCapabilityType("com.apple.Siri", true, "", false);
        public static readonly PBXCapabilityType Wallet = new PBXCapabilityType("com.apple.Wallet", true, "PassKit.framework", false);
        public static readonly PBXCapabilityType WirelessAccessoryConfiguration = new PBXCapabilityType("com.apple.WAC", true, "ExternalAccessory.framework", false);

        private PBXCapabilityType(string _id, bool _requiresEntitlements, string _framework = "", bool _optionalFramework = false)
        {
            this.m_ID = _id;
            this.m_RequiresEntitlements = _requiresEntitlements;
            this.m_Framework = _framework;
            this.m_OptionalFramework = _optionalFramework;
        }

        public static PBXCapabilityType StringToPBXCapabilityType(string cap)
        {
            if (cap != null)
            {
                int num;
                if (<>f__switch$map0 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(0x12) {
                        { 
                            "com.apple.ApplePay",
                            0
                        },
                        { 
                            "com.apple.ApplicationGroups.iOS",
                            1
                        },
                        { 
                            "com.apple.SafariKeychain",
                            2
                        },
                        { 
                            "com.apple.BackgroundModes",
                            3
                        },
                        { 
                            "com.apple.DataProtection",
                            4
                        },
                        { 
                            "com.apple.GameCenter",
                            5
                        },
                        { 
                            "com.apple.HealthKit",
                            6
                        },
                        { 
                            "com.apple.HomeKit",
                            7
                        },
                        { 
                            "com.apple.iCloud",
                            8
                        },
                        { 
                            "com.apple.InAppPurchase",
                            9
                        },
                        { 
                            "com.apple.InterAppAudio",
                            10
                        },
                        { 
                            "com.apple.KeychainSharing",
                            11
                        },
                        { 
                            "com.apple.Maps.iOS",
                            12
                        },
                        { 
                            "com.apple.VPNLite",
                            13
                        },
                        { 
                            "com.apple.Push",
                            14
                        },
                        { 
                            "com.apple.Siri",
                            15
                        },
                        { 
                            "com.apple.Wallet",
                            0x10
                        },
                        { 
                            "WAC",
                            0x11
                        }
                    };
                    <>f__switch$map0 = dictionary;
                }
                if (<>f__switch$map0.TryGetValue(cap, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return ApplePay;

                        case 1:
                            return AppGroups;

                        case 2:
                            return AssociatedDomains;

                        case 3:
                            return BackgroundModes;

                        case 4:
                            return DataProtection;

                        case 5:
                            return GameCenter;

                        case 6:
                            return HealthKit;

                        case 7:
                            return HomeKit;

                        case 8:
                            return iCloud;

                        case 9:
                            return InAppPurchase;

                        case 10:
                            return InterAppAudio;

                        case 11:
                            return KeychainSharing;

                        case 12:
                            return Maps;

                        case 13:
                            return PersonalVPN;

                        case 14:
                            return PushNotifications;

                        case 15:
                            return Siri;

                        case 0x10:
                            return Wallet;

                        case 0x11:
                            return WirelessAccessoryConfiguration;
                    }
                }
            }
            return null;
        }

        public string framework =>
            this.m_Framework;

        public string id =>
            this.m_ID;

        public bool optionalFramework =>
            this.m_OptionalFramework;

        public bool requiresEntitlements =>
            this.m_RequiresEntitlements;

        [StructLayout(LayoutKind.Sequential)]
        public struct TargetCapabilityPair
        {
            public string targetGuid;
            public PBXCapabilityType capability;
            public TargetCapabilityPair(string guid, PBXCapabilityType type)
            {
                this.targetGuid = guid;
                this.capability = type;
            }
        }
    }
}


namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class ICloudEntitlements
    {
        internal static readonly string ContainerIdKey = "com.apple.developer.icloud-container-identifiers";
        internal static readonly string ContainerIdValue = "iCloud.$(CFBundleIdentifier)";
        internal static readonly string KeyValueStoreKey = "com.apple.developer.ubiquity-kvstore-identifier";
        internal static readonly string KeyValueStoreValue = "$(TeamIdentifierPrefix)$(CFBundleIdentifier)";
        internal static readonly string ServicesDocValue = "CloudDocuments";
        internal static readonly string ServicesKey = "com.apple.developer.icloud-services";
        internal static readonly string ServicesKitValue = "CloudKit";
        internal static readonly string UbiquityContainerIdKey = "com.apple.developer.ubiquity-container-identifiers";
        internal static readonly string UbiquityContainerIdValue = "iCloud.$(CFBundleIdentifier)";
    }
}


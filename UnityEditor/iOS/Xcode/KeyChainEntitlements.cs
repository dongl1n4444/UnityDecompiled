namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class KeyChainEntitlements
    {
        internal static readonly string DefaultValue = "$(AppIdentifierPrefix)$(CFBundleIdentifier)";
        internal static readonly string Key = "keychain-access-groups";
    }
}


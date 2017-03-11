namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class WalletEntitlements
    {
        internal static readonly string BaseValue = "$(TeamIdentifierPrefix)";
        internal static readonly string DefaultValue = "*";
        internal static readonly string Key = "com.apple.developer.pass-type-identifiers";
    }
}


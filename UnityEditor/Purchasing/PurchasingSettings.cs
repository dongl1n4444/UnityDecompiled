namespace UnityEditor.Purchasing
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Editor API for the Unity Services editor feature. Normally Purchasing is enabled from the Services window, but if writing your own editor extension, this API can be used.</para>
    /// </summary>
    public static class PurchasingSettings
    {
        /// <summary>
        /// <para>This Boolean field will cause the Purchasing feature in Unity to be enabled if true, or disabled if false.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}


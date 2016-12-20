namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>What to checkout when starting the Checkout task through the version control Provider.</para>
    /// </summary>
    [Flags]
    public enum CheckoutMode
    {
        /// <summary>
        /// <para>Checkout the asset only.</para>
        /// </summary>
        Asset = 1,
        /// <summary>
        /// <para>Checkout both asset and .meta file.</para>
        /// </summary>
        Both = 3,
        /// <summary>
        /// <para>Checkout.</para>
        /// </summary>
        Exact = 4,
        /// <summary>
        /// <para>Checkout .meta file only.</para>
        /// </summary>
        Meta = 2
    }
}


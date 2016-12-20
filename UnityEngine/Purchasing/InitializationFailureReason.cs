namespace UnityEngine.Purchasing
{
    using System;

    /// <summary>
    /// <para>The various reasons Unity IAP initialization can fail.</para>
    /// </summary>
    public enum InitializationFailureReason
    {
        PurchasingUnavailable,
        NoProductsAvailable,
        AppNotKnown
    }
}


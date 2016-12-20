namespace UnityEngine.Purchasing
{
    using System;

    /// <summary>
    /// <para>The various reasons a purchase can fail.</para>
    /// </summary>
    public enum PurchaseFailureReason
    {
        PurchasingUnavailable,
        ExistingPurchasePending,
        ProductUnavailable,
        SignatureInvalid,
        UserCancelled,
        PaymentDeclined,
        Unknown
    }
}


namespace UnityEngine.Purchasing.Extension
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Purchasing;

    /// <summary>
    /// <para>Represents a failed purchase as described by a purchasing service.</para>
    /// </summary>
    public class PurchaseFailureDescription
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <message>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <productId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PurchaseFailureReason <reason>k__BackingField;

        /// <summary>
        /// <para>Creates a PurchaseFailureDescription.</para>
        /// </summary>
        /// <param name="productId">The store-specific product ID which failed to purchase.</param>
        /// <param name="reason">The reason for the purchase failure.</param>
        /// <param name="message">More information about the failure from Unity IAP or the platform store, if available.</param>
        public PurchaseFailureDescription(string productId, PurchaseFailureReason reason, string message)
        {
            this.productId = productId;
            this.reason = reason;
            this.message = message;
        }

        /// <summary>
        /// <para>More information about the purchase failure from Unity IAP or the platform store, if available.</para>
        /// </summary>
        public string message { get; private set; }

        /// <summary>
        /// <para>The store-specific product ID which failed to purchase.</para>
        /// </summary>
        public string productId { get; private set; }

        /// <summary>
        /// <para>The reason for the purchase failure.</para>
        /// </summary>
        public PurchaseFailureReason reason { get; private set; }
    }
}


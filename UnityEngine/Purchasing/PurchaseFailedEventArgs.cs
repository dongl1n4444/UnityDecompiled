namespace UnityEngine.Purchasing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A purchase failed event containing diagnostic data.</para>
    /// </summary>
    public class PurchaseFailedEventArgs
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <message>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Product <purchasedProduct>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PurchaseFailureReason <reason>k__BackingField;

        internal PurchaseFailedEventArgs(Product purchasedProduct, PurchaseFailureReason reason, string message)
        {
            this.purchasedProduct = purchasedProduct;
            this.reason = reason;
            this.message = message;
        }

        /// <summary>
        /// <para>More information about the purchase failure, if available. Otherwise null.</para>
        /// </summary>
        public string message { get; private set; }

        /// <summary>
        /// <para>The product that failed to purchase.</para>
        /// </summary>
        public Product purchasedProduct { get; private set; }

        /// <summary>
        /// <para>The reason for the purchase failure.</para>
        /// </summary>
        public PurchaseFailureReason reason { get; private set; }
    }
}


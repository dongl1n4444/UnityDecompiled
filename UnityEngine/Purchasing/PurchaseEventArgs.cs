namespace UnityEngine.Purchasing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A purchase that succeeded, including the purchased product along with its purchase receipt.</para>
    /// </summary>
    public class PurchaseEventArgs
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Product <purchasedProduct>k__BackingField;

        internal PurchaseEventArgs(Product purchasedProduct)
        {
            this.purchasedProduct = purchasedProduct;
        }

        /// <summary>
        /// <para>The Product that was purchased.</para>
        /// </summary>
        public Product purchasedProduct { get; private set; }
    }
}


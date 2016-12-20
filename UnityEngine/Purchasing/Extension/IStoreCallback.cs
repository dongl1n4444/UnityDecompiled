namespace UnityEngine.Purchasing.Extension
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Purchasing;

    public interface IStoreCallback
    {
        void OnProductsRetrieved(List<ProductDescription> products);
        /// <summary>
        /// <para>Call to indicate to Unity IAP that a purchase failed.</para>
        /// </summary>
        /// <param name="desc">Details of the purchase failure.</param>
        void OnPurchaseFailed(PurchaseFailureDescription desc);
        /// <summary>
        /// <para>Inform Unity IAP of a purchase.</para>
        /// </summary>
        /// <param name="storeSpecificId">Product that was purchased.</param>
        /// <param name="receipt">Purchase receipt.</param>
        /// <param name="transactionIdentifier">Transaction ID.</param>
        void OnPurchaseSucceeded(string storeSpecificId, string receipt, string transactionIdentifier);
        /// <summary>
        /// <para>Indicate that IAP is unavailable for a specific reason, such as IAP being disabled in device settings.</para>
        /// </summary>
        /// <param name="reason">The reason purchasing is unavailable.</param>
        void OnSetupFailed(InitializationFailureReason reason);

        /// <summary>
        /// <para>Gets the item with local identifier.</para>
        /// </summary>
        ProductCollection products { get; }

        /// <summary>
        /// <para>Toggle use of Unity IAP's transaction log.</para>
        /// </summary>
        bool useTransactionLog { get; set; }
    }
}


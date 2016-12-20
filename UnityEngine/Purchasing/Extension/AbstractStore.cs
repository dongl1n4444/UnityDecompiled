namespace UnityEngine.Purchasing.Extension
{
    using System;
    using System.Collections.ObjectModel;
    using UnityEngine.Purchasing;

    /// <summary>
    /// <para>Extension point for purchasing plugins.</para>
    /// </summary>
    public abstract class AbstractStore : IStore
    {
        protected AbstractStore()
        {
        }

        /// <summary>
        /// <para>Called when Unity IAP has finished processing a purchase.</para>
        /// </summary>
        /// <param name="transactionId">The transaction ID for the purchase.</param>
        /// <param name="product">The product that was purchased.</param>
        public abstract void FinishTransaction(ProductDefinition product, string transactionId);
        /// <summary>
        /// <para>Called when Unity IAP is initializing.</para>
        /// </summary>
        /// <param name="callback">Callback for stores to interact with Unity IAP.</param>
        public abstract void Initialize(IStoreCallback callback);
        /// <summary>
        /// <para>Called when a user wants to buy the specified Product.</para>
        /// </summary>
        /// <param name="developerPayload">Any additional developer-supplied data.</param>
        /// <param name="product">The product to purchase.</param>
        public abstract void Purchase(ProductDefinition product, string developerPayload);
        public abstract void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products);
    }
}


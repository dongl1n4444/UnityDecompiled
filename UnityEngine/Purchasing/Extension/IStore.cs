namespace UnityEngine.Purchasing.Extension
{
    using System;
    using System.Collections.ObjectModel;
    using UnityEngine.Purchasing;

    public interface IStore
    {
        /// <summary>
        /// <para>Called by Unity IAP when a transaction has been recorded.</para>
        /// </summary>
        /// <param name="product"></param>
        /// <param name="transactionId"></param>
        void FinishTransaction(ProductDefinition product, string transactionId);
        /// <summary>
        /// <para>Initialize the store.</para>
        /// </summary>
        /// <param name="callback">Used by stores to interact with Unity Purchasing.</param>
        void Initialize(IStoreCallback callback);
        /// <summary>
        /// <para>Handle a purchase request from a user.</para>
        /// </summary>
        /// <param name="developerPayload">Any additional developer-supplied data.</param>
        /// <param name="product">The product to purchase.</param>
        void Purchase(ProductDefinition product, string developerPayload);
        void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products);
    }
}


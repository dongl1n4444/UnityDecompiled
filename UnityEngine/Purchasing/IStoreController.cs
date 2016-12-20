namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;

    public interface IStoreController
    {
        /// <summary>
        /// <para>Confirm a pending purchase.</para>
        /// </summary>
        /// <param name="product">The product to confirm the conclusion of its purchase transaction.</param>
        void ConfirmPendingPurchase(Product product);
        void FetchAdditionalProducts(HashSet<ProductDefinition> products, Action successCallback, Action<InitializationFailureReason> failCallback);
        /// <summary>
        /// <para>Initiate a purchase for a specific product.</para>
        /// </summary>
        /// <param name="product">The product to purchase.</param>
        /// <param name="payload">Any additional developer information to associate with the purchase.</param>
        /// <param name="productId">The identifier for the product to purchase. This may differ from the store-specific product ID.</param>
        void InitiatePurchase(string productId);
        /// <summary>
        /// <para>Initiate a purchase for a specific product.</para>
        /// </summary>
        /// <param name="product">The product to purchase.</param>
        /// <param name="payload">Any additional developer information to associate with the purchase.</param>
        /// <param name="productId">The identifier for the product to purchase. This may differ from the store-specific product ID.</param>
        void InitiatePurchase(Product product);
        /// <summary>
        /// <para>Initiate a purchase for a specific product.</para>
        /// </summary>
        /// <param name="product">The product to purchase.</param>
        /// <param name="payload">Any additional developer information to associate with the purchase.</param>
        /// <param name="productId">The identifier for the product to purchase. This may differ from the store-specific product ID.</param>
        void InitiatePurchase(string productId, string payload);
        /// <summary>
        /// <para>Initiate a purchase for a specific product.</para>
        /// </summary>
        /// <param name="product">The product to purchase.</param>
        /// <param name="payload">Any additional developer information to associate with the purchase.</param>
        /// <param name="productId">The identifier for the product to purchase. This may differ from the store-specific product ID.</param>
        void InitiatePurchase(Product product, string payload);

        /// <summary>
        /// <para>Store products including metadata and purchase receipts.</para>
        /// </summary>
        ProductCollection products { get; }
    }
}


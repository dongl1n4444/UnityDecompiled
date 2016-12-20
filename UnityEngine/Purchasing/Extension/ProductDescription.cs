namespace UnityEngine.Purchasing.Extension
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Purchasing;

    /// <summary>
    /// <para>A common format which store subsystems use to describe available products.</para>
    /// </summary>
    public class ProductDescription
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProductMetadata <metadata>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <receipt>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <storeSpecificId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <transactionId>k__BackingField;
        /// <summary>
        /// <para>The ProductType.</para>
        /// </summary>
        public ProductType type;

        /// <summary>
        /// <para>Create a ProductDescription.</para>
        /// </summary>
        /// <param name="id">The store-specific ID.</param>
        /// <param name="metadata">Localized metadata retrieved from the Store.</param>
        /// <param name="receipt">A purchase receipt, if owned. Otherwise null.</param>
        /// <param name="transactionId">The purchase transaction ID, if owned. Otherwise null.</param>
        /// <param name="type">The product type (optional for products queried by Unity IAP).</param>
        public ProductDescription(string id, ProductMetadata metadata) : this(id, metadata, null, null)
        {
        }

        /// <summary>
        /// <para>Create a ProductDescription.</para>
        /// </summary>
        /// <param name="id">The store-specific ID.</param>
        /// <param name="metadata">Localized metadata retrieved from the Store.</param>
        /// <param name="receipt">A purchase receipt, if owned. Otherwise null.</param>
        /// <param name="transactionId">The purchase transaction ID, if owned. Otherwise null.</param>
        /// <param name="type">The product type (optional for products queried by Unity IAP).</param>
        public ProductDescription(string id, ProductMetadata metadata, string receipt, string transactionId)
        {
            this.storeSpecificId = id;
            this.metadata = metadata;
            this.receipt = receipt;
            this.transactionId = transactionId;
        }

        /// <summary>
        /// <para>Create a ProductDescription.</para>
        /// </summary>
        /// <param name="id">The store-specific ID.</param>
        /// <param name="metadata">Localized metadata retrieved from the Store.</param>
        /// <param name="receipt">A purchase receipt, if owned. Otherwise null.</param>
        /// <param name="transactionId">The purchase transaction ID, if owned. Otherwise null.</param>
        /// <param name="type">The product type (optional for products queried by Unity IAP).</param>
        public ProductDescription(string id, ProductMetadata metadata, string receipt, string transactionId, ProductType type) : this(id, metadata, receipt, transactionId)
        {
            this.type = type;
        }

        /// <summary>
        /// <para>Localized metadata retrieved from the Store.</para>
        /// </summary>
        public ProductMetadata metadata { get; private set; }

        /// <summary>
        /// <para>A purchase receipt, if owned. Otherwise null.</para>
        /// </summary>
        public string receipt { get; private set; }

        /// <summary>
        /// <para>The store-specific ID.</para>
        /// </summary>
        public string storeSpecificId { get; private set; }

        /// <summary>
        /// <para>The purchase transaction ID, if owned. Otherwise null.</para>
        /// </summary>
        public string transactionId { get; set; }
    }
}


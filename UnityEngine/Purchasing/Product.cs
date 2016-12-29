namespace UnityEngine.Purchasing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Represents a product that may be purchased as an In-App Purchase.</para>
    /// </summary>
    public class Product
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <availableToPurchase>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProductDefinition <definition>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProductMetadata <metadata>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <receipt>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <transactionID>k__BackingField;

        internal Product(ProductDefinition definition, ProductMetadata metadata) : this(definition, metadata, null)
        {
        }

        internal Product(ProductDefinition definition, ProductMetadata metadata, string receipt)
        {
            this.definition = definition;
            this.metadata = metadata;
            this.receipt = receipt;
        }

        /// <summary>
        /// <para>Equality defined for use in collections.</para>
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Product product = obj as Product;
            if (product == null)
            {
                return false;
            }
            return this.definition.Equals(product.definition);
        }

        /// <summary>
        /// <para>GetHashCode defined for use in collections.</para>
        /// </summary>
        /// <returns>
        /// <para>Hashcode.</para>
        /// </returns>
        public override int GetHashCode() => 
            this.definition.GetHashCode();

        /// <summary>
        /// <para>Determine if this product is available to purchase according to the store subsystem.</para>
        /// </summary>
        public bool availableToPurchase { get; internal set; }

        /// <summary>
        /// <para>Fundamental immutable product properties.</para>
        /// </summary>
        public ProductDefinition definition { get; private set; }

        /// <summary>
        /// <para>Owned Non Consumables and Subscriptions should always have receipts.</para>
        /// </summary>
        public bool hasReceipt =>
            !string.IsNullOrEmpty(this.receipt);

        /// <summary>
        /// <para>Localized metadata provided by the store system.</para>
        /// </summary>
        public ProductMetadata metadata { get; internal set; }

        /// <summary>
        /// <para>The purchase receipt for this product, if owned. Otherwise null.</para>
        /// </summary>
        public string receipt { get; internal set; }

        /// <summary>
        /// <para>A unique identifier for this product's transaction, if available. Otherwise null.</para>
        /// </summary>
        public string transactionID { get; internal set; }
    }
}


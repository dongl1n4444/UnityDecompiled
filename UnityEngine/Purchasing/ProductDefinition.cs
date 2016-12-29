namespace UnityEngine.Purchasing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Minimal product definition, used by apps declaring products for sale.</para>
    /// </summary>
    public class ProductDefinition
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <id>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <storeSpecificId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProductType <type>k__BackingField;

        private ProductDefinition()
        {
        }

        /// <summary>
        /// <para>Create a ProductDefinition where the Store-independent ID is the same as the store-specific ID. Use this when you don't need the two IDs to be different.</para>
        /// </summary>
        /// <param name="id">Store-independent ID and store-specific ID.</param>
        /// <param name="type">Product type.</param>
        public ProductDefinition(string id, ProductType type) : this(id, id, type)
        {
        }

        /// <summary>
        /// <para>Create a ProductDefinition with different Store-independent ID and Store-specific ID. Use this when you need to two IDs to be different.</para>
        /// </summary>
        /// <param name="id">Store-independent ID.</param>
        /// <param name="storeSpecificId">Store-specific ID.</param>
        /// <param name="type">Product type.</param>
        public ProductDefinition(string id, string storeSpecificId, ProductType type)
        {
            this.id = id;
            this.storeSpecificId = storeSpecificId;
            this.type = type;
        }

        /// <summary>
        /// <para>Compares id properties. Requires obj be a ProductDefinition.</para>
        /// </summary>
        /// <param name="obj">A ProductDefinition instance.</param>
        /// <returns>
        /// <para>true if this is equal to obj per the equality rules, false otherwise.</para>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            ProductDefinition definition = obj as ProductDefinition;
            if (definition == null)
            {
                return false;
            }
            return (this.id == definition.id);
        }

        /// <summary>
        /// <para>Gets the application-domain-specific hash code of id.</para>
        /// </summary>
        /// <returns>
        /// <para>Hash code of id.</para>
        /// </returns>
        public override int GetHashCode() => 
            this.id.GetHashCode();

        /// <summary>
        /// <para>Unity IAP product ID. Potentially independent of store IDs.</para>
        /// </summary>
        public string id { get; private set; }

        /// <summary>
        /// <para>The ID this product has on a store.</para>
        /// </summary>
        public string storeSpecificId { get; private set; }

        /// <summary>
        /// <para>The product type.</para>
        /// </summary>
        public ProductType type { get; private set; }
    }
}


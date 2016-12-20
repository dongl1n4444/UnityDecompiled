namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Purchasing.Extension;

    /// <summary>
    /// <para>Builds configurations for Unity IAP.</para>
    /// </summary>
    public class ConfigurationBuilder
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <useCloudCatalog>k__BackingField;
        private PurchasingFactory m_Factory;
        private HashSet<ProductDefinition> m_Products = new HashSet<ProductDefinition>();

        internal ConfigurationBuilder(PurchasingFactory factory)
        {
            this.m_Factory = factory;
        }

        /// <summary>
        /// <para>Add a product with a Unity IAP ID, type and optional set of store-specific IDs.</para>
        /// </summary>
        /// <param name="id">The store independent ID.</param>
        /// <param name="type">The product type.</param>
        /// <param name="storeIDs">An optional set of store-specific identifiers, for when your product has different IDs on different stores.</param>
        /// <returns>
        /// <para>The referenced instance. Suitable for chaining.</para>
        /// </returns>
        public ConfigurationBuilder AddProduct(string id, ProductType type)
        {
            return this.AddProduct(id, type, null);
        }

        /// <summary>
        /// <para>Add a product with a Unity IAP ID, type and optional set of store-specific IDs.</para>
        /// </summary>
        /// <param name="id">The store independent ID.</param>
        /// <param name="type">The product type.</param>
        /// <param name="storeIDs">An optional set of store-specific identifiers, for when your product has different IDs on different stores.</param>
        /// <returns>
        /// <para>The referenced instance. Suitable for chaining.</para>
        /// </returns>
        public ConfigurationBuilder AddProduct(string id, ProductType type, IDs storeIDs)
        {
            string storeSpecificId = id;
            if (storeIDs != null)
            {
                storeSpecificId = storeIDs.SpecificIDForStore(this.factory.storeName, id);
            }
            ProductDefinition item = new ProductDefinition(id, storeSpecificId, type);
            this.m_Products.Add(item);
            return this;
        }

        public ConfigurationBuilder AddProducts(IEnumerable<ProductDefinition> products)
        {
            foreach (ProductDefinition definition in products)
            {
                this.m_Products.Add(definition);
            }
            return this;
        }

        public T Configure<T>() where T: IStoreConfiguration
        {
            return this.m_Factory.GetConfig<T>();
        }

        /// <summary>
        /// <para>Get an instance of ConfigurationBuilder.</para>
        /// </summary>
        /// <param name="first">The IAP module.</param>
        /// <param name="rest">Any additional modules.</param>
        /// <returns>
        /// <para>New instance.</para>
        /// </returns>
        public static ConfigurationBuilder Instance(IPurchasingModule first, params IPurchasingModule[] rest)
        {
            return new ConfigurationBuilder(new PurchasingFactory(first, rest));
        }

        internal PurchasingFactory factory
        {
            get
            {
                return this.m_Factory;
            }
        }

        /// <summary>
        /// <para>The products built so far.</para>
        /// </summary>
        public HashSet<ProductDefinition> products
        {
            get
            {
                return this.m_Products;
            }
        }

        /// <summary>
        /// <para>If set Unity IAP will retrieve your product catalog from Unity cloud services, allowing you to change your catalog dynamically without updating your App.</para>
        /// </summary>
        public bool useCloudCatalog { get; set; }
    }
}


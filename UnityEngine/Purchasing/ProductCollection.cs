namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Provides helper methods to retrieve products by ID.</para>
    /// </summary>
    public class ProductCollection
    {
        [CompilerGenerated]
        private static Func<Product, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Product, string> <>f__am$cache1;
        private Dictionary<string, Product> m_IdToProduct;
        private Product[] m_Products;
        private HashSet<Product> m_ProductSet = new HashSet<Product>();
        private Dictionary<string, Product> m_StoreSpecificIdToProduct;

        internal ProductCollection(Product[] products)
        {
            this.AddProducts(products);
        }

        internal void AddProducts(IEnumerable<Product> products)
        {
            this.m_ProductSet.UnionWith(products);
            this.m_Products = this.m_ProductSet.ToArray<Product>();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<Product, string>(null, (IntPtr) <AddProducts>m__0);
            }
            this.m_IdToProduct = Enumerable.ToDictionary<Product, string>(this.m_Products, <>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<Product, string>(null, (IntPtr) <AddProducts>m__1);
            }
            this.m_StoreSpecificIdToProduct = Enumerable.ToDictionary<Product, string>(this.m_Products, <>f__am$cache1);
        }

        /// <summary>
        /// <para>Get the product with store-independent Unity IAP ID.</para>
        /// </summary>
        /// <param name="id">The store-independent ID.</param>
        /// <returns>
        /// <para>A product reference, if found. Otherwise null.</para>
        /// </returns>
        public Product WithID(string id)
        {
            Product product = null;
            this.m_IdToProduct.TryGetValue(id, out product);
            return product;
        }

        /// <summary>
        /// <para>Get the product with the store-specific ID.</para>
        /// </summary>
        /// <param name="id">Get the product with store-specific ID.</param>
        /// <returns>
        /// <para>A product reference, if found. Otherwise null.</para>
        /// </returns>
        public Product WithStoreSpecificID(string id)
        {
            Product product = null;
            this.m_StoreSpecificIdToProduct.TryGetValue(id, out product);
            return product;
        }

        /// <summary>
        /// <para>All products.</para>
        /// </summary>
        public Product[] all =>
            this.m_Products;

        /// <summary>
        /// <para>The set of products.</para>
        /// </summary>
        public HashSet<Product> set =>
            this.m_ProductSet;
    }
}


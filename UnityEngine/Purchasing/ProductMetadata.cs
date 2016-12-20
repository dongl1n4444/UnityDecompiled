namespace UnityEngine.Purchasing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Localized information about a product, retrieved from a store.</para>
    /// </summary>
    public class ProductMetadata
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <isoCurrencyCode>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <localizedDescription>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private decimal <localizedPrice>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <localizedPriceString>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <localizedTitle>k__BackingField;

        /// <summary>
        /// <para>Create a ProductMetadata.</para>
        /// </summary>
        /// <param name="priceString">Formatted product price with currency symbols suitable for display to the user.</param>
        /// <param name="title">Localized product title.</param>
        /// <param name="description">Localized product description.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="localizedPrice">Numeric localized price.</param>
        public ProductMetadata()
        {
        }

        /// <summary>
        /// <para>Create a ProductMetadata.</para>
        /// </summary>
        /// <param name="priceString">Formatted product price with currency symbols suitable for display to the user.</param>
        /// <param name="title">Localized product title.</param>
        /// <param name="description">Localized product description.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="localizedPrice">Numeric localized price.</param>
        public ProductMetadata(string priceString, string title, string description, string currencyCode, decimal localizedPrice)
        {
            this.localizedPriceString = priceString;
            this.localizedTitle = title;
            this.localizedDescription = description;
            this.isoCurrencyCode = currencyCode;
            this.localizedPrice = localizedPrice;
        }

        /// <summary>
        /// <para>Product currency in ISO 4217 format; e.g. GBP or USD.</para>
        /// </summary>
        public string isoCurrencyCode { get; internal set; }

        /// <summary>
        /// <para>Localized product description as retrieved from the store subsystem; e.g. Apple or Google.</para>
        /// </summary>
        public string localizedDescription { get; internal set; }

        /// <summary>
        /// <para>Decimal product price denominated in the currency indicated by isoCurrencySymbol.</para>
        /// </summary>
        public decimal localizedPrice { get; internal set; }

        /// <summary>
        /// <para>Localized price string.</para>
        /// </summary>
        public string localizedPriceString { get; internal set; }

        /// <summary>
        /// <para>Localized product title as retrieved from the store subsystem; e.g. Apple or Google.</para>
        /// </summary>
        public string localizedTitle { get; internal set; }
    }
}


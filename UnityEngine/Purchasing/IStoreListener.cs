namespace UnityEngine.Purchasing
{
    using System;

    public interface IStoreListener
    {
        /// <summary>
        /// <para>Called when Unity IAP has retrieved all product metadata and is ready to make purchases.</para>
        /// </summary>
        /// <param name="controller">Access cross-platform Unity IAP functionality.</param>
        /// <param name="extensions">Access store-specific Unity IAP functionality.</param>
        void OnInitialized(IStoreController controller, IExtensionProvider extensions);
        /// <summary>
        /// <para>Note that Unity IAP will not call this method if the device is offline, but continually attempt initialization until online.</para>
        /// </summary>
        /// <param name="error">The reason IAP cannot initialize.</param>
        void OnInitializeFailed(InitializationFailureReason error);
        /// <summary>
        /// <para>Called when a purchase fails.</para>
        /// </summary>
        /// <param name="i">The product the purchase relates to.</param>
        /// <param name="p">The reason for the failure.</param>
        void OnPurchaseFailed(Product i, PurchaseFailureReason p);
        /// <summary>
        /// <para>Called when a purchase succeeds.</para>
        /// </summary>
        /// <param name="e">The purchase details.</param>
        /// <returns>
        /// <para>Applications should only return PurchaseProcessingResult.Complete when a permanent record of the purchase has been made. If PurchaseProcessingResult.Pending is returned Unity IAP will continue to notify the app of the purchase on startup, also via ProcessPurchase.</para>
        /// </returns>
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e);
    }
}


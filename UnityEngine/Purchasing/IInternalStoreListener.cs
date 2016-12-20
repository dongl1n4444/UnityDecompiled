namespace UnityEngine.Purchasing
{
    using System;

    internal interface IInternalStoreListener
    {
        void OnInitialized(IStoreController controller);
        void OnInitializeFailed(InitializationFailureReason error);
        void OnPurchaseFailed(Product i, PurchaseFailureReason p);
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e);
    }
}


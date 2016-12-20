namespace UnityEngine.Purchasing
{
    using System;

    internal class StoreListenerProxy : IInternalStoreListener
    {
        private AnalyticsReporter m_Analytics;
        private IExtensionProvider m_Extensions;
        private IStoreListener m_ForwardTo;

        public StoreListenerProxy(IStoreListener forwardTo, AnalyticsReporter analytics, IExtensionProvider extensions)
        {
            this.m_ForwardTo = forwardTo;
            this.m_Analytics = analytics;
            this.m_Extensions = extensions;
        }

        public void OnInitialized(IStoreController controller)
        {
            this.m_ForwardTo.OnInitialized(controller, this.m_Extensions);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            this.m_ForwardTo.OnInitializeFailed(error);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            this.m_Analytics.OnPurchaseFailed(i, p);
            this.m_ForwardTo.OnPurchaseFailed(i, p);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            this.m_Analytics.OnPurchaseSucceeded(e.purchasedProduct);
            return this.m_ForwardTo.ProcessPurchase(e);
        }
    }
}


namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;

    internal class AnalyticsReporter
    {
        private IUnityAnalytics m_Analytics;

        public AnalyticsReporter(IUnityAnalytics analytics)
        {
            this.m_Analytics = analytics;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Dictionary<string, object> data = new Dictionary<string, object> {
                { 
                    "productID",
                    product.definition.storeSpecificId
                },
                { 
                    "reason",
                    reason
                },
                { 
                    "price",
                    product.metadata.localizedPrice
                },
                { 
                    "currency",
                    product.metadata.isoCurrencyCode
                }
            };
            this.m_Analytics.CustomEvent("unity.PurchaseFailed", data);
        }

        public void OnPurchaseSucceeded(Product product)
        {
            if (product.metadata.isoCurrencyCode != null)
            {
                this.m_Analytics.Transaction(product.definition.storeSpecificId, product.metadata.localizedPrice, product.metadata.isoCurrencyCode, product.receipt, null);
            }
        }
    }
}


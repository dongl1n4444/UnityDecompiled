﻿namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Analytics;

    internal class UnityAnalytics : IUnityAnalytics
    {
        public void CustomEvent(string name, Dictionary<string, object> data)
        {
            UnityEngine.Analytics.Analytics.CustomEvent(name, data);
        }

        public void Transaction(string productId, decimal price, string currency, string receipt, string signature)
        {
            UnityEngine.Analytics.Analytics.Transaction(productId, price, currency, receipt, signature, true);
        }
    }
}


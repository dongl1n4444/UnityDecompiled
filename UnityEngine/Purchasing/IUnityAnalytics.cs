namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;

    internal interface IUnityAnalytics
    {
        void CustomEvent(string name, Dictionary<string, object> data);
        void Transaction(string productId, decimal price, string currency, string receipt, string signature);
    }
}


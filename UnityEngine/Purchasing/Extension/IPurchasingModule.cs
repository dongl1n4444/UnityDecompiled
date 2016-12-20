namespace UnityEngine.Purchasing.Extension
{
    using System;

    public interface IPurchasingModule
    {
        /// <summary>
        /// <para>Called when Unity IAP is loading your module. Register stores and associated extensions using the IPurchasingBinder.</para>
        /// </summary>
        /// <param name="binder">Used to register store implementations, extensions and configuration.</param>
        void Configure(IPurchasingBinder binder);
    }
}


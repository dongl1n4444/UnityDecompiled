namespace UnityEngine.Purchasing.Extension
{
    using System;

    public interface IPurchasingBinder
    {
        void RegisterConfiguration<T>(T instance) where T: IStoreConfiguration;
        void RegisterExtension<T>(T instance) where T: IStoreExtension;
        /// <summary>
        /// <para>Informs Unity IAP that a store implementation exists, specifying its name.</para>
        /// </summary>
        /// <param name="name">The store's name, eg 'AppleAppStore'.</param>
        /// <param name="a">The store instance.</param>
        void RegisterStore(string name, IStore a);
    }
}


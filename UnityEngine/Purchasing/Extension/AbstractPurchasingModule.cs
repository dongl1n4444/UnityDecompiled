namespace UnityEngine.Purchasing.Extension
{
    using System;

    /// <summary>
    /// <para>Helper base class for IAP Modules.</para>
    /// </summary>
    public abstract class AbstractPurchasingModule : IPurchasingModule
    {
        protected IPurchasingBinder m_Binder;

        protected AbstractPurchasingModule()
        {
        }

        protected void BindConfiguration<T>(T instance) where T: IStoreConfiguration
        {
            this.m_Binder.RegisterConfiguration<T>(instance);
        }

        protected void BindExtension<T>(T instance) where T: IStoreExtension
        {
            this.m_Binder.RegisterExtension<T>(instance);
        }

        /// <summary>
        /// <para>Called when your module is loaded by Unity.</para>
        /// </summary>
        /// <param name="binder">Used to register store implementations, extensions and configuration.</param>
        public abstract void Configure();
        /// <summary>
        /// <para>Called when your module is loaded by Unity.</para>
        /// </summary>
        /// <param name="binder">Used to register store implementations, extensions and configuration.</param>
        public void Configure(IPurchasingBinder binder)
        {
            this.m_Binder = binder;
            this.Configure();
        }

        /// <summary>
        /// <para>Register a store implementation along with its name.</para>
        /// </summary>
        /// <param name="name">The store's name, eg 'AppleAppStore'.</param>
        /// <param name="a">The store implementation, or null if running on an unsupported platform.</param>
        protected void RegisterStore(string name, IStore a)
        {
            this.m_Binder.RegisterStore(name, a);
        }
    }
}


namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Purchasing.Extension;

    internal class PurchasingFactory : IPurchasingBinder, IExtensionProvider
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <storeName>k__BackingField;
        private Dictionary<Type, IStoreConfiguration> m_ConfigMap = new Dictionary<Type, IStoreConfiguration>();
        private Dictionary<Type, IStoreExtension> m_ExtensionMap = new Dictionary<Type, IStoreExtension>();
        private IStore m_Store;

        public PurchasingFactory(IPurchasingModule first, params IPurchasingModule[] remainingModules)
        {
            first.Configure(this);
            foreach (IPurchasingModule module in remainingModules)
            {
                module.Configure(this);
            }
        }

        public T GetConfig<T>() where T: IStoreConfiguration
        {
            if (this.service is T)
            {
                return (T) this.service;
            }
            Type key = typeof(T);
            if (!this.m_ConfigMap.ContainsKey(key))
            {
                throw new ArgumentException("No binding for config type " + key);
            }
            return (T) this.m_ConfigMap[key];
        }

        public T GetExtension<T>() where T: IStoreExtension
        {
            if (this.service is T)
            {
                return (T) this.service;
            }
            Type key = typeof(T);
            if (!this.m_ExtensionMap.ContainsKey(key))
            {
                throw new ArgumentException("No binding for type " + key);
            }
            return (T) this.m_ExtensionMap[key];
        }

        public void RegisterConfiguration<T>(T instance) where T: IStoreConfiguration
        {
            this.m_ConfigMap[typeof(T)] = instance;
        }

        public void RegisterExtension<T>(T instance) where T: IStoreExtension
        {
            this.m_ExtensionMap[typeof(T)] = instance;
        }

        public void RegisterStore(string name, IStore s)
        {
            if ((this.m_Store == null) && (s != null))
            {
                this.storeName = name;
                this.service = s;
            }
        }

        public IStore service
        {
            get
            {
                if (this.m_Store == null)
                {
                    throw new InvalidOperationException("No impl available!");
                }
                return this.m_Store;
            }
            set
            {
                this.m_Store = value;
            }
        }

        public string storeName { get; private set; }
    }
}


namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Entry point for Applications using Unity IAP.</para>
    /// </summary>
    public abstract class UnityPurchasing
    {
        private const string kCatalogURL = "https://catalog.iap.cloud.unity3d.com";

        protected UnityPurchasing()
        {
        }

        /// <summary>
        /// <para>Clears Unity IAP's internal transaction log.</para>
        /// </summary>
        public static void ClearTransactionLog()
        {
            new TransactionLog(Debug.logger, Application.persistentDataPath).Clear();
        }

        internal static void FetchAndMergeProducts(bool useCatalog, HashSet<ProductDefinition> localProductSet, CloudCatalogManager catalog, Action<HashSet<ProductDefinition>> callback)
        {
            <FetchAndMergeProducts>c__AnonStorey1 storey = new <FetchAndMergeProducts>c__AnonStorey1 {
                localProductSet = localProductSet,
                callback = callback
            };
            if (useCatalog)
            {
                catalog.FetchProducts(new Action<HashSet<ProductDefinition>>(storey.<>m__0), 0);
            }
            else
            {
                storey.callback(storey.localProductSet);
            }
        }

        /// <summary>
        /// <para>Initialize Unity IAP with the specified listener and configuration.</para>
        /// </summary>
        /// <param name="listener">Your Application's listener that processes purchasing related events.</param>
        /// <param name="builder">Unity IAP configuration.</param>
        public static void Initialize(IStoreListener listener, ConfigurationBuilder builder)
        {
            Initialize(listener, builder, Debug.logger, Application.persistentDataPath, new UnityAnalytics(), InstantiateCatalog(builder.factory.storeName));
        }

        internal static void Initialize(IStoreListener listener, ConfigurationBuilder builder, ILogger logger, string persistentDatapath, IUnityAnalytics analytics, CloudCatalogManager catalog)
        {
            <Initialize>c__AnonStorey0 storey = new <Initialize>c__AnonStorey0();
            TransactionLog tDb = new TransactionLog(logger, persistentDatapath);
            storey.manager = new PurchasingManager(tDb, logger, builder.factory.service, builder.factory.storeName);
            AnalyticsReporter reporter = new AnalyticsReporter(analytics);
            storey.proxy = new StoreListenerProxy(listener, reporter, builder.factory);
            FetchAndMergeProducts(builder.useCloudCatalog, builder.products, catalog, new Action<HashSet<ProductDefinition>>(storey.<>m__0));
        }

        private static CloudCatalogManager InstantiateCatalog(string storeName)
        {
            GameObject target = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(target);
            target.name = "Unity IAP";
            target.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AsyncUtil util = target.AddComponent<AsyncUtil>();
            string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Unity"), Path.Combine(Application.cloudProjectId, "IAP"));
            string cacheFile = null;
            try
            {
                Directory.CreateDirectory(path);
                cacheFile = Path.Combine(path, "catalog.json");
            }
            catch (Exception exception)
            {
                Debug.logger.LogError("Unable to cache IAP catalog", exception);
            }
            return new CloudCatalogManager(util, cacheFile, Debug.logger, $"{"https://catalog.iap.cloud.unity3d.com"}/{Application.cloudProjectId}", storeName);
        }

        [CompilerGenerated]
        private sealed class <FetchAndMergeProducts>c__AnonStorey1
        {
            internal Action<HashSet<ProductDefinition>> callback;
            internal HashSet<ProductDefinition> localProductSet;

            internal void <>m__0(HashSet<ProductDefinition> cloudProducts)
            {
                HashSet<ProductDefinition> set = new HashSet<ProductDefinition>(this.localProductSet);
                foreach (ProductDefinition definition in cloudProducts)
                {
                    set.Remove(definition);
                    set.Add(definition);
                }
                this.callback(set);
            }
        }

        [CompilerGenerated]
        private sealed class <Initialize>c__AnonStorey0
        {
            internal PurchasingManager manager;
            internal StoreListenerProxy proxy;

            internal void <>m__0(HashSet<ProductDefinition> response)
            {
                this.manager.Initialize(this.proxy, response);
            }
        }
    }
}


namespace UnityEngine.Purchasing
{
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using UnityEngine;

    internal class CloudCatalogManager
    {
        [CompilerGenerated]
        private static Func<char, int, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;
        private const int kMaxRetryDelayInSeconds = 300;
        private IAsyncUtil m_AsyncUtil;
        private string m_CacheFileName;
        private string m_CatalogURL;
        private ILogger m_Logger;
        private string m_StoreName;

        internal CloudCatalogManager(IAsyncUtil util, string cacheFile, ILogger logger, string catalogURL, string storeName)
        {
            this.m_AsyncUtil = util;
            this.m_CacheFileName = cacheFile;
            this.m_Logger = logger;
            this.m_CatalogURL = catalogURL;
            this.m_StoreName = storeName;
        }

        internal static string CamelCaseToSnakeCase(string s)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<char, int, string>(null, (IntPtr) <CamelCaseToSnakeCase>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<string, string, string>(null, (IntPtr) <CamelCaseToSnakeCase>m__1);
            }
            return Enumerable.Aggregate<string>(Enumerable.Select<char, string>(s, <>f__am$cache0), <>f__am$cache1);
        }

        internal void FetchProducts(Action<HashSet<ProductDefinition>> callback, int delayInSeconds)
        {
            <FetchProducts>c__AnonStorey0 storey = new <FetchProducts>c__AnonStorey0 {
                callback = callback,
                delayInSeconds = delayInSeconds,
                $this = this
            };
            this.m_Logger.Log("Fetching IAP cloud catalog...");
            this.m_AsyncUtil.Get(this.m_CatalogURL, new Action<string>(storey.<>m__0), new Action<string>(storey.<>m__1));
        }

        internal static HashSet<ProductDefinition> ParseProductsFromJSON(string json, string storeName)
        {
            HashSet<ProductDefinition> set2;
            HashSet<ProductDefinition> set = new HashSet<ProductDefinition>();
            try
            {
                object obj3;
                ((JsonObject) SimpleJson.SimpleJson.DeserializeObject(json)).TryGetValue("products", out obj3);
                string key = CamelCaseToSnakeCase(storeName);
                foreach (JsonObject obj4 in (JsonArray) obj3)
                {
                    object obj5;
                    object obj6;
                    object obj7;
                    obj4.TryGetValue("id", out obj5);
                    obj4.TryGetValue("store_ids", out obj6);
                    obj4.TryGetValue("type", out obj7);
                    JsonObject obj8 = (JsonObject) obj6;
                    string storeSpecificId = (string) obj5;
                    if ((obj8 != null) && obj8.ContainsKey(key))
                    {
                        object obj9 = null;
                        obj8.TryGetValue(key, out obj9);
                        if (obj9 != null)
                        {
                            storeSpecificId = (string) obj9;
                        }
                    }
                    ProductType type = (ProductType) Enum.Parse(typeof(ProductType), (string) obj7);
                    set.Add(new ProductDefinition((string) obj5, storeSpecificId, type));
                }
                set2 = set;
            }
            catch (Exception exception)
            {
                throw new SerializationException("Error parsing JSON", exception);
            }
            return set2;
        }

        private HashSet<ProductDefinition> TryLoadCachedCatalog()
        {
            if ((this.m_CacheFileName != null) && File.Exists(this.m_CacheFileName))
            {
                try
                {
                    return ParseProductsFromJSON(File.ReadAllText(this.m_CacheFileName), this.m_StoreName);
                }
                catch (Exception exception)
                {
                    this.m_Logger.LogError("Error loading cached catalog", exception);
                }
            }
            return new HashSet<ProductDefinition>();
        }

        private void TryPersistCatalog(string response)
        {
            if (this.m_CacheFileName != null)
            {
                try
                {
                    File.WriteAllText(this.m_CacheFileName, response);
                }
                catch (Exception exception)
                {
                    this.m_Logger.LogError("Failed persisting IAP catalog", exception);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FetchProducts>c__AnonStorey0
        {
            internal CloudCatalogManager $this;
            internal Action<HashSet<ProductDefinition>> callback;
            internal int delayInSeconds;

            internal void <>m__0(string response)
            {
                this.$this.m_Logger.Log("Fetched catalog");
                try
                {
                    HashSet<ProductDefinition> set = CloudCatalogManager.ParseProductsFromJSON(response, this.$this.m_StoreName);
                    this.$this.TryPersistCatalog(response);
                    this.callback(set);
                }
                catch (SerializationException exception)
                {
                    this.$this.m_Logger.LogError("Error parsing IAP catalog", exception);
                    this.$this.m_Logger.Log(response);
                    this.callback(this.$this.TryLoadCachedCatalog());
                }
            }

            internal void <>m__1(string error)
            {
                HashSet<ProductDefinition> set = this.$this.TryLoadCachedCatalog();
                if (set.Count > 0)
                {
                    this.$this.m_Logger.Log("Failed to fetch IAP catalog, using cache.");
                    this.callback(set);
                }
                else
                {
                    this.delayInSeconds = Math.Max(5, this.delayInSeconds * 2);
                    this.delayInSeconds = Math.Min(300, this.delayInSeconds);
                    this.$this.m_AsyncUtil.Schedule(new Action(this, (IntPtr) this.<>m__2), this.delayInSeconds);
                }
            }

            internal void <>m__2()
            {
                this.$this.FetchProducts(this.callback, this.delayInSeconds);
            }
        }
    }
}


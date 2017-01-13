namespace UnityEngine.Purchasing
{
    using SimpleJson;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Purchasing.Extension;

    internal class PurchasingManager : IStoreCallback, IStoreController
    {
        [CompilerGenerated]
        private static Func<ProductDefinition, Product> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<ProductDefinition, Product> <>f__am$cache1;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProductCollection <products>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <useTransactionLog>k__BackingField;
        private bool initialized;
        private Action m_AdditionalProductsCallback;
        private Action<InitializationFailureReason> m_AdditionalProductsFailCallback;
        private IInternalStoreListener m_Listener;
        private ILogger m_Logger;
        private IStore m_Store;
        private string m_StoreName;
        private TransactionLog m_TransactionLog;

        internal PurchasingManager(TransactionLog tDb, ILogger logger, IStore store, string storeName)
        {
            this.m_TransactionLog = tDb;
            this.m_Store = store;
            this.m_Logger = logger;
            this.m_StoreName = storeName;
            this.useTransactionLog = true;
        }

        private void CheckForInitialization()
        {
            if (!this.initialized)
            {
                bool flag = false;
                foreach (Product product in this.products.set)
                {
                    if (!product.availableToPurchase)
                    {
                        object[] args = new object[] { product.definition.id, product.definition.storeSpecificId };
                        this.m_Logger.LogFormat(LogType.Warning, "Unavailable product {0} -{1}", args);
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.m_Listener.OnInitialized(this);
                }
                else
                {
                    this.OnSetupFailed(InitializationFailureReason.NoProductsAvailable);
                }
                this.initialized = true;
            }
            else if (this.m_AdditionalProductsCallback != null)
            {
                this.m_AdditionalProductsCallback();
            }
        }

        public void ConfirmPendingPurchase(Product product)
        {
            if (product == null)
            {
                this.m_Logger.Log("Unable to confirm purchase with null Product");
            }
            else if (string.IsNullOrEmpty(product.transactionID))
            {
                this.m_Logger.Log("Unable to confirm purchase; Product has missing or empty transactionID");
            }
            else
            {
                if (this.useTransactionLog)
                {
                    this.m_TransactionLog.Record(product.transactionID);
                }
                this.m_Store.FinishTransaction(product.definition, product.transactionID);
            }
        }

        public void FetchAdditionalProducts(HashSet<ProductDefinition> products, Action successCallback, Action<InitializationFailureReason> failCallback)
        {
            this.m_AdditionalProductsCallback = successCallback;
            this.m_AdditionalProductsFailCallback = failCallback;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => new Product(x, new ProductMetadata());
            }
            this.products.AddProducts(Enumerable.Select<ProductDefinition, Product>(products, <>f__am$cache0));
            this.m_Store.RetrieveProducts(new ReadOnlyCollection<ProductDefinition>(products.ToList<ProductDefinition>()));
        }

        private string FormatUnifiedReceipt(string platformReceipt, string transactionId)
        {
            Dictionary<string, object> json = new Dictionary<string, object> {
                ["Store"] = this.m_StoreName
            };
            if (transactionId == null)
            {
            }
            json["TransactionID"] = string.Empty;
            if (platformReceipt == null)
            {
            }
            json["Payload"] = string.Empty;
            return SimpleJson.SimpleJson.SerializeObject(json);
        }

        public void Initialize(IInternalStoreListener listener, HashSet<ProductDefinition> products)
        {
            this.m_Listener = listener;
            this.m_Store.Initialize(this);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => new Product(x, new ProductMetadata());
            }
            Product[] productArray = Enumerable.Select<ProductDefinition, Product>(products, <>f__am$cache1).ToArray<Product>();
            this.products = new ProductCollection(productArray);
            ReadOnlyCollection<ProductDefinition> onlys = new ReadOnlyCollection<ProductDefinition>(products.ToList<ProductDefinition>());
            this.m_Store.RetrieveProducts(onlys);
        }

        public void InitiatePurchase(string productId)
        {
            this.InitiatePurchase(productId, string.Empty);
        }

        public void InitiatePurchase(Product product)
        {
            this.InitiatePurchase(product, string.Empty);
        }

        public void InitiatePurchase(string purchasableId, string developerPayload)
        {
            Product product = this.products.WithID(purchasableId);
            if (product == null)
            {
                this.m_Logger.LogWarning("Unable to purchase unknown product with id: {0}", purchasableId);
            }
            this.InitiatePurchase(product, developerPayload);
        }

        public void InitiatePurchase(Product product, string developerPayload)
        {
            if (product == null)
            {
                this.m_Logger.Log("Trying to purchase null Product");
            }
            else if (!product.availableToPurchase)
            {
                this.m_Listener.OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
            }
            else
            {
                this.m_Store.Purchase(product.definition, developerPayload);
                this.m_Logger.Log("purchase({0})", product.definition.id);
            }
        }

        public void OnProductsRetrieved(List<ProductDescription> products)
        {
            HashSet<Product> set = new HashSet<Product>();
            foreach (ProductDescription description in products)
            {
                Product item = this.products.WithStoreSpecificID(description.storeSpecificId);
                if (item == null)
                {
                    ProductDefinition definition = new ProductDefinition(description.storeSpecificId, description.storeSpecificId, description.type);
                    item = new Product(definition, description.metadata);
                    set.Add(item);
                }
                item.availableToPurchase = true;
                item.metadata = description.metadata;
                item.transactionID = description.transactionId;
                if (!string.IsNullOrEmpty(description.receipt))
                {
                    item.receipt = this.FormatUnifiedReceipt(description.receipt, description.transactionId);
                }
            }
            if (set.Count > 0)
            {
                this.products.AddProducts(set);
            }
            this.CheckForInitialization();
            foreach (Product product2 in this.products.set)
            {
                if (!string.IsNullOrEmpty(product2.receipt) && !string.IsNullOrEmpty(product2.transactionID))
                {
                    this.ProcessPurchaseIfNew(product2);
                }
            }
        }

        public void OnPurchaseFailed(PurchaseFailureDescription description)
        {
            Product i = this.products.WithStoreSpecificID(description.productId);
            if (i == null)
            {
                this.m_Logger.LogError("Failed to purchase unknown product {0}", description.productId);
            }
            else
            {
                this.m_Logger.Log("onPurchaseFailedEvent({0})", i.definition.id);
                this.m_Listener.OnPurchaseFailed(i, description.reason);
            }
        }

        public void OnPurchaseSucceeded(string id, string receipt, string transactionId)
        {
            Product product = this.products.WithStoreSpecificID(id);
            if (product == null)
            {
                ProductDefinition definition = new ProductDefinition(id, ProductType.NonConsumable);
                product = new Product(definition, new ProductMetadata());
            }
            string str = this.FormatUnifiedReceipt(receipt, transactionId);
            product.receipt = str;
            product.transactionID = transactionId;
            this.ProcessPurchaseIfNew(product);
        }

        public void OnSetupFailed(InitializationFailureReason reason)
        {
            if (this.initialized)
            {
                if (this.m_AdditionalProductsFailCallback != null)
                {
                    this.m_AdditionalProductsFailCallback(reason);
                }
            }
            else
            {
                this.m_Listener.OnInitializeFailed(reason);
            }
        }

        private void ProcessPurchaseIfNew(Product product)
        {
            if (this.useTransactionLog && this.m_TransactionLog.HasRecordOf(product.transactionID))
            {
                this.m_Logger.Log("Already recorded transaction " + product.transactionID);
                this.m_Store.FinishTransaction(product.definition, product.transactionID);
            }
            else
            {
                PurchaseEventArgs e = new PurchaseEventArgs(product);
                if (this.m_Listener.ProcessPurchase(e) == PurchaseProcessingResult.Complete)
                {
                    this.ConfirmPendingPurchase(product);
                }
            }
        }

        public ProductCollection products { get; private set; }

        public bool useTransactionLog { get; set; }
    }
}


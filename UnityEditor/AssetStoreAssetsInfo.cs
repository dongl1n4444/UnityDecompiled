namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;

    internal class AssetStoreAssetsInfo : AssetStoreResultBase<AssetStoreAssetsInfo>
    {
        internal Dictionary<int, AssetStoreAsset> assets;
        internal string currency;
        internal string message;
        internal string paymentMethodCard;
        internal string paymentMethodExpire;
        internal bool paymentTokenAvailable;
        internal float price;
        internal string priceText;
        internal Status status;
        internal float vat;
        internal string vatText;

        internal AssetStoreAssetsInfo(AssetStoreResultBase<AssetStoreAssetsInfo>.Callback c, List<AssetStoreAsset> assets) : base(c)
        {
            this.assets = new Dictionary<int, AssetStoreAsset>();
            foreach (AssetStoreAsset asset in assets)
            {
                this.assets[asset.id] = asset;
            }
        }

        protected override void Parse(Dictionary<string, JSONValue> dict)
        {
            Dictionary<string, JSONValue> dictionary = dict["purchase_info"].AsDict(true);
            JSONValue value3 = dictionary["status"];
            switch (value3.AsString(true))
            {
                case "basket-not-empty":
                    this.status = Status.BasketNotEmpty;
                    break;

                case "service-disabled":
                    this.status = Status.ServiceDisabled;
                    break;

                case "user-anonymous":
                    this.status = Status.AnonymousUser;
                    break;

                case "ok":
                    this.status = Status.Ok;
                    break;
            }
            this.paymentTokenAvailable = dictionary["payment_token_available"].AsBool();
            if (dictionary.ContainsKey("payment_method_card"))
            {
                this.paymentMethodCard = dictionary["payment_method_card"].AsString(true);
            }
            if (dictionary.ContainsKey("payment_method_expire"))
            {
                this.paymentMethodExpire = dictionary["payment_method_expire"].AsString(true);
            }
            this.price = dictionary["price"].AsFloat(true);
            this.vat = dictionary["vat"].AsFloat(true);
            this.priceText = dictionary["price_text"].AsString(true);
            this.vatText = dictionary["vat_text"].AsString(true);
            this.currency = dictionary["currency"].AsString(true);
            this.message = !dictionary.ContainsKey("message") ? null : dictionary["message"].AsString(true);
            List<JSONValue> list = dict["results"].AsList(true);
            foreach (JSONValue value14 in list)
            {
                AssetStoreAsset asset;
                int key = 0;
                JSONValue value15 = value14["id"];
                if (value15.IsString())
                {
                    key = int.Parse(value14["id"].AsString());
                }
                else
                {
                    JSONValue value17 = value14["id"];
                    key = (int) value17.AsFloat();
                }
                if (this.assets.TryGetValue(key, out asset))
                {
                    JSONValue value27;
                    if (asset.previewInfo == null)
                    {
                        asset.previewInfo = new AssetStoreAsset.PreviewInfo();
                    }
                    AssetStoreAsset.PreviewInfo previewInfo = asset.previewInfo;
                    JSONValue value18 = value14["class_names"];
                    asset.className = value18.AsString(true).Trim();
                    JSONValue value19 = value14["package_name"];
                    previewInfo.packageName = value19.AsString(true).Trim();
                    JSONValue value20 = value14["short_url"];
                    previewInfo.packageShortUrl = value20.AsString(true).Trim();
                    asset.price = !value14.ContainsKey("price_text") ? null : value14["price_text"].AsString(true).Trim();
                    previewInfo.packageSize = int.Parse(!value14.Get("package_size").IsNull() ? value14["package_size"].AsString(true) : "-1");
                    asset.packageID = int.Parse(value14["package_id"].AsString());
                    previewInfo.packageVersion = value14["package_version"].AsString();
                    if (!value14.Get("rating").IsNull())
                    {
                        value27 = value14["rating"];
                    }
                    previewInfo.packageRating = int.Parse((value27.AsString(true).Length != 0) ? value14["rating"].AsString(true) : "-1");
                    JSONValue value29 = value14["package_asset_count"];
                    previewInfo.packageAssetCount = int.Parse(!value29.IsNull() ? value14["package_asset_count"].AsString(true) : "-1");
                    previewInfo.isPurchased = value14.ContainsKey("purchased") && value14["purchased"].AsBool(true);
                    previewInfo.isDownloadable = previewInfo.isPurchased || (asset.price == null);
                    JSONValue value32 = value14["publisher_name"];
                    previewInfo.publisherName = value32.AsString(true).Trim();
                    previewInfo.packageUrl = !value14.Get("package_url").IsNull() ? value14["package_url"].AsString(true) : "";
                    previewInfo.encryptionKey = !value14.Get("encryption_key").IsNull() ? value14["encryption_key"].AsString(true) : "";
                    previewInfo.categoryName = !value14.Get("category_name").IsNull() ? value14["category_name"].AsString(true) : "";
                    previewInfo.buildProgress = -1f;
                    previewInfo.downloadProgress = -1f;
                }
            }
        }

        internal enum Status
        {
            BasketNotEmpty,
            ServiceDisabled,
            AnonymousUser,
            Ok
        }
    }
}


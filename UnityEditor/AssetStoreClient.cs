namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetStoreClient
    {
        [CompilerGenerated]
        private static Predicate<string> <>f__am$cache0;
        private const string kUnauthSessionID = "26c4202eb475d02864b40827dfff11a14657aa41";
        private static string s_AssetStoreSearchUrl = null;
        private static string s_AssetStoreUrl = null;
        private static string sLoginErrorMessage = null;
        private static LoginState sLoginState = LoginState.LOGGED_OUT;

        static AssetStoreClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AssetStoreClient.<AssetStoreClient>m__0);
        }

        [CompilerGenerated]
        private static bool <AssetStoreClient>m__0(object, X509Certificate, X509Chain, SslPolicyErrors) => 
            true;

        private static string APISearchUrl(string path) => 
            string.Format("{0}/public-api{2}.json?{1}", AssetStoreSearchUrl, VersionParams, path);

        private static string APIUrl(string path) => 
            string.Format("{0}/api{2}.json?{1}", AssetStoreUrl, VersionParams, path);

        internal static AsyncHTTPClient AssetsInfo(List<AssetStoreAsset> assets, AssetStoreResultBase<AssetStoreAssetsInfo>.Callback callback)
        {
            <AssetsInfo>c__AnonStorey3 storey = new <AssetsInfo>c__AnonStorey3();
            string url = APIUrl("/assets/list");
            foreach (AssetStoreAsset asset in assets)
            {
                url = url + "&id=" + asset.id.ToString();
            }
            storey.r = new AssetStoreAssetsInfo(callback, assets);
            return CreateJSONRequest(url, new DoneCallback(storey.<>m__0));
        }

        internal static AsyncHTTPClient BuildPackage(AssetStoreAsset asset, AssetStoreResultBase<BuildPackageResult>.Callback callback)
        {
            <BuildPackage>c__AnonStorey5 storey = new <BuildPackage>c__AnonStorey5();
            string url = APIUrl("/content/download/" + asset.packageID.ToString());
            storey.r = new BuildPackageResult(asset, callback);
            return CreateJSONRequest(url, new DoneCallback(storey.<>m__0));
        }

        private static AsyncHTTPClient CreateJSONRequest(string url, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url) {
                header = { ["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken() },
                doneCallback = WrapJsonCallback(callback)
            };
            client.Begin();
            return client;
        }

        private static AsyncHTTPClient CreateJSONRequestDelete(string url, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url, "DELETE") {
                header = { ["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken() },
                doneCallback = WrapJsonCallback(callback)
            };
            client.Begin();
            return client;
        }

        private static AsyncHTTPClient CreateJSONRequestPost(string url, Dictionary<string, string> param, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url) {
                header = { ["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken() },
                postDictionary = param,
                doneCallback = WrapJsonCallback(callback)
            };
            client.Begin();
            return client;
        }

        private static AsyncHTTPClient CreateJSONRequestPost(string url, string postData, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url) {
                header = { ["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken() },
                postData = postData,
                doneCallback = WrapJsonCallback(callback)
            };
            client.Begin();
            return client;
        }

        internal static AsyncHTTPClient DirectPurchase(int packageID, string password, AssetStoreResultBase<PurchaseResult>.Callback callback)
        {
            <DirectPurchase>c__AnonStorey4 storey = new <DirectPurchase>c__AnonStorey4();
            string url = APIUrl($"/purchase/direct/{packageID.ToString()}");
            storey.r = new PurchaseResult(callback);
            Dictionary<string, string> param = new Dictionary<string, string> {
                ["password"] = password
            };
            return CreateJSONRequestPost(url, param, new DoneCallback(storey.<>m__0));
        }

        private static string GetToken() => 
            InternalEditorUtility.GetAuthToken();

        public static bool LoggedIn() => 
            !string.IsNullOrEmpty(ActiveSessionID);

        public static bool LoggedOut() => 
            string.IsNullOrEmpty(ActiveSessionID);

        public static bool LoginError() => 
            (sLoginState == LoginState.LOGIN_ERROR);

        public static bool LoginInProgress() => 
            (sLoginState == LoginState.IN_PROGRESS);

        internal static void LoginWithCredentials(string username, string password, bool rememberMe, DoneLoginCallback callback)
        {
            if (sLoginState == LoginState.IN_PROGRESS)
            {
                Debug.LogError("Tried to login with credentials while already in progress of logging in");
            }
            else
            {
                sLoginState = LoginState.IN_PROGRESS;
                RememberSession = rememberMe;
                string str = AssetStoreUrl + "/login?skip_terms=1";
                sLoginErrorMessage = null;
                new AsyncHTTPClient(str.Replace("http://", "https://")) { 
                    postData = "user=" + username + "&pass=" + password,
                    header = { ["X-Unity-Session"] = "26c4202eb475d02864b40827dfff11a14657aa41" + GetToken() },
                    doneCallback = WrapLoginCallback(callback)
                }.Begin();
            }
        }

        internal static void LoginWithRememberedSession(DoneLoginCallback callback)
        {
            if (sLoginState == LoginState.IN_PROGRESS)
            {
                Debug.LogError("Tried to login with remembered session while already in progress of logging in");
            }
            else
            {
                sLoginState = LoginState.IN_PROGRESS;
                if (!RememberSession)
                {
                    SavedSessionID = "";
                }
                string str = AssetStoreUrl + "/login?skip_terms=1&reuse_session=" + SavedSessionID;
                sLoginErrorMessage = null;
                new AsyncHTTPClient(str) { 
                    header = { ["X-Unity-Session"] = "26c4202eb475d02864b40827dfff11a14657aa41" + GetToken() },
                    doneCallback = WrapLoginCallback(callback)
                }.Begin();
            }
        }

        public static void Logout()
        {
            ActiveSessionID = "";
            SavedSessionID = "";
            sLoginState = LoginState.LOGGED_OUT;
        }

        private static AssetStoreResponse ParseContent(AsyncHTTPClient job)
        {
            string str2;
            string str3;
            AssetStoreResponse response = new AssetStoreResponse {
                job = job,
                dict = null,
                ok = false
            };
            AsyncHTTPClient.State state = job.state;
            string text = job.text;
            if (!AsyncHTTPClient.IsSuccess(state))
            {
                Console.WriteLine(text);
                return response;
            }
            response.dict = ParseJSON(text, out str2, out str3);
            if (str2 == "error")
            {
                Debug.LogError("Request error (" + str2 + "): " + str3);
                return response;
            }
            response.ok = true;
            return response;
        }

        private static Dictionary<string, JSONValue> ParseJSON(string content, out string status, out string message)
        {
            JSONValue value2;
            Dictionary<string, JSONValue> dictionary2;
            message = null;
            status = null;
            try
            {
                value2 = new JSONParser(content).Parse();
            }
            catch (JSONParseException exception)
            {
                Debug.Log("Error parsing server reply: " + content);
                Debug.Log(exception.Message);
                return null;
            }
            try
            {
                dictionary2 = value2.AsDict(true);
                if (dictionary2 == null)
                {
                    Debug.Log("Error parsing server message: " + content);
                    return null;
                }
                if (dictionary2.ContainsKey("result"))
                {
                    JSONValue value3 = dictionary2["result"];
                    if (value3.IsDict())
                    {
                        dictionary2 = dictionary2["result"].AsDict(true);
                    }
                }
                if (dictionary2.ContainsKey("message"))
                {
                    message = dictionary2["message"].AsString(true);
                }
                if (dictionary2.ContainsKey("status"))
                {
                    status = dictionary2["status"].AsString(true);
                    return dictionary2;
                }
                if (dictionary2.ContainsKey("error"))
                {
                    status = dictionary2["error"].AsString(true);
                    if (status == "")
                    {
                        status = "ok";
                    }
                    return dictionary2;
                }
                status = "ok";
            }
            catch (JSONTypeException exception2)
            {
                Debug.Log("Error parsing server reply. " + content);
                Debug.Log(exception2.Message);
                return null;
            }
            return dictionary2;
        }

        internal static AsyncHTTPClient SearchAssets(string searchString, string[] requiredClassNames, string[] assetLabels, List<SearchCount> counts, AssetStoreResultBase<AssetStoreSearchResults>.Callback callback)
        {
            <SearchAssets>c__AnonStorey2 storey = new <SearchAssets>c__AnonStorey2();
            string str = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            foreach (SearchCount count in counts)
            {
                str = str + str4 + count.offset;
                str2 = str2 + str4 + count.limit;
                str3 = str3 + str4 + count.name;
                str4 = ",";
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => a.Equals("MonoScript", StringComparison.OrdinalIgnoreCase);
            }
            if (Array.Exists<string>(requiredClassNames, <>f__am$cache0))
            {
                Array.Resize<string>(ref requiredClassNames, requiredClassNames.Length + 1);
                requiredClassNames[requiredClassNames.Length - 1] = "Script";
            }
            string url = $"{APISearchUrl("/search/assets")}&q={Uri.EscapeDataString(searchString)}&c={Uri.EscapeDataString(string.Join(",", requiredClassNames))}&l={Uri.EscapeDataString(string.Join(",", assetLabels))}&O={str}&N={str2}&G={str3}";
            storey.r = new AssetStoreSearchResults(callback);
            return CreateJSONRequest(url, new DoneCallback(storey.<>m__0));
        }

        private static AsyncHTTPClient.DoneCallback WrapJsonCallback(DoneCallback callback)
        {
            <WrapJsonCallback>c__AnonStorey1 storey = new <WrapJsonCallback>c__AnonStorey1 {
                callback = callback
            };
            return new AsyncHTTPClient.DoneCallback(storey.<>m__0);
        }

        private static AsyncHTTPClient.DoneCallback WrapLoginCallback(DoneLoginCallback callback)
        {
            <WrapLoginCallback>c__AnonStorey0 storey = new <WrapLoginCallback>c__AnonStorey0 {
                callback = callback
            };
            return new AsyncHTTPClient.DoneCallback(storey.<>m__0);
        }

        private static string ActiveOrUnauthSessionID
        {
            get
            {
                string activeSessionID = ActiveSessionID;
                if (activeSessionID == "")
                {
                    return "26c4202eb475d02864b40827dfff11a14657aa41";
                }
                return activeSessionID;
            }
        }

        internal static string ActiveSessionID
        {
            get
            {
                if (AssetStoreContext.SessionHasString("kharma.active_sessionid"))
                {
                    return AssetStoreContext.SessionGetString("kharma.active_sessionid");
                }
                return "";
            }
            set
            {
                AssetStoreContext.SessionSetString("kharma.active_sessionid", value);
            }
        }

        private static string AssetStoreSearchUrl
        {
            get
            {
                if (s_AssetStoreSearchUrl == null)
                {
                    s_AssetStoreSearchUrl = AssetStoreUtils.GetAssetStoreSearchUrl();
                }
                return s_AssetStoreSearchUrl;
            }
        }

        private static string AssetStoreUrl
        {
            get
            {
                if (s_AssetStoreUrl == null)
                {
                    s_AssetStoreUrl = AssetStoreUtils.GetAssetStoreUrl();
                }
                return s_AssetStoreUrl;
            }
        }

        public static bool HasActiveSessionID =>
            !string.IsNullOrEmpty(ActiveSessionID);

        public static bool HasSavedSessionID =>
            !string.IsNullOrEmpty(SavedSessionID);

        public static string LoginErrorMessage =>
            sLoginErrorMessage;

        public static bool RememberSession
        {
            get => 
                (EditorPrefs.GetString("kharma.remember_session") == "1");
            set
            {
                EditorPrefs.SetString("kharma.remember_session", !value ? "0" : "1");
            }
        }

        private static string SavedSessionID
        {
            get
            {
                if (RememberSession)
                {
                    return EditorPrefs.GetString("kharma.sessionid", "");
                }
                return "";
            }
            set
            {
                EditorPrefs.SetString("kharma.sessionid", value);
            }
        }

        private static string VersionParams =>
            ("unityversion=" + Uri.EscapeDataString(Application.unityVersion) + "&skip_terms=1");

        [CompilerGenerated]
        private sealed class <AssetsInfo>c__AnonStorey3
        {
            internal AssetStoreAssetsInfo r;

            internal void <>m__0(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <BuildPackage>c__AnonStorey5
        {
            internal BuildPackageResult r;

            internal void <>m__0(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <DirectPurchase>c__AnonStorey4
        {
            internal PurchaseResult r;

            internal void <>m__0(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <SearchAssets>c__AnonStorey2
        {
            internal AssetStoreSearchResults r;

            internal void <>m__0(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <WrapJsonCallback>c__AnonStorey1
        {
            internal AssetStoreClient.DoneCallback callback;

            internal void <>m__0(AsyncHTTPClient job)
            {
                if (job.IsDone())
                {
                    try
                    {
                        AssetStoreResponse response = AssetStoreClient.ParseContent(job);
                        this.callback(response);
                    }
                    catch (Exception exception)
                    {
                        Debug.Log("Uncaught exception in async net callback: " + exception.Message);
                        Debug.Log(exception.StackTrace);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WrapLoginCallback>c__AnonStorey0
        {
            internal AssetStoreClient.DoneLoginCallback callback;

            internal void <>m__0(AsyncHTTPClient job)
            {
                string text = job.text;
                if (!job.IsSuccess())
                {
                    AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGIN_ERROR;
                    AssetStoreClient.sLoginErrorMessage = ((job.responseCode < 200) || (job.responseCode >= 300)) ? "Failed to login - please retry" : text;
                }
                else if (text.StartsWith("<!DOCTYPE"))
                {
                    AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGIN_ERROR;
                    AssetStoreClient.sLoginErrorMessage = "Failed to login";
                }
                else
                {
                    AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGGED_IN;
                    if (text.Contains("@"))
                    {
                        AssetStoreClient.ActiveSessionID = AssetStoreClient.SavedSessionID;
                    }
                    else
                    {
                        AssetStoreClient.ActiveSessionID = text;
                    }
                    if (AssetStoreClient.RememberSession)
                    {
                        AssetStoreClient.SavedSessionID = AssetStoreClient.ActiveSessionID;
                    }
                }
                this.callback(AssetStoreClient.sLoginErrorMessage);
            }
        }

        public delegate void DoneCallback(AssetStoreResponse response);

        public delegate void DoneLoginCallback(string errorMessage);

        internal enum LoginState
        {
            LOGGED_OUT,
            IN_PROGRESS,
            LOGGED_IN,
            LOGIN_ERROR
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SearchCount
        {
            public string name;
            public int offset;
            public int limit;
        }
    }
}


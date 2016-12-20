namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngineInternal;

    /// <summary>
    /// <para>The UnityWebRequest object is used to communicate with web servers.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class UnityWebRequest : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        /// <summary>
        /// <para>The string "GET", commonly used as the verb for an HTTP GET request.</para>
        /// </summary>
        public const string kHttpVerbGET = "GET";
        /// <summary>
        /// <para>The string "HEAD", commonly used as the verb for an HTTP HEAD request.</para>
        /// </summary>
        public const string kHttpVerbHEAD = "HEAD";
        /// <summary>
        /// <para>The string "POST", commonly used as the verb for an HTTP POST request.</para>
        /// </summary>
        public const string kHttpVerbPOST = "POST";
        /// <summary>
        /// <para>The string "PUT", commonly used as the verb for an HTTP PUT request.</para>
        /// </summary>
        public const string kHttpVerbPUT = "PUT";
        /// <summary>
        /// <para>The string "CREATE", commonly used as the verb for an HTTP CREATE request.</para>
        /// </summary>
        public const string kHttpVerbCREATE = "CREATE";
        /// <summary>
        /// <para>The string "DELETE", commonly used as the verb for an HTTP DELETE request.</para>
        /// </summary>
        public const string kHttpVerbDELETE = "DELETE";
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <disposeDownloadHandlerOnDispose>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private bool <disposeUploadHandlerOnDispose>k__BackingField;
        private static readonly string[] forbiddenHeaderKeys = new string[] { 
            "accept-charset", "access-control-request-headers", "access-control-request-method", "connection", "content-length", "date", "dnt", "expect", "host", "keep-alive", "origin", "referer", "te", "trailer", "transfer-encoding", "upgrade",
            "user-agent", "via", "x-unity-version"
        };
        /// <summary>
        /// <para>Creates a UnityWebRequest with the default options and no attached DownloadHandler or UploadHandler. Default method is GET.</para>
        /// </summary>
        /// <param name="url">The target URL with which this UnityWebRequest will communicate. Also accessible via the url property.</param>
        public UnityWebRequest()
        {
            this.InternalCreate();
            this.InternalSetDefaults();
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest with the default options and no attached DownloadHandler or UploadHandler. Default method is GET.</para>
        /// </summary>
        /// <param name="url">The target URL with which this UnityWebRequest will communicate. Also accessible via the url property.</param>
        public UnityWebRequest(string url)
        {
            this.InternalCreate();
            this.InternalSetDefaults();
            this.url = url;
        }

        public UnityWebRequest(string url, string method)
        {
            this.InternalCreate();
            this.InternalSetDefaults();
            this.url = url;
            this.method = method;
        }

        public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
        {
            this.InternalCreate();
            this.InternalSetDefaults();
            this.url = url;
            this.method = method;
            this.downloadHandler = downloadHandler;
            this.uploadHandler = uploadHandler;
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest configured for HTTP GET.</para>
        /// </summary>
        /// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
        /// <returns>
        /// <para>A UnityWebRequest object configured to retrieve data from uri.</para>
        /// </returns>
        public static UnityWebRequest Get(string uri)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest configured for HTTP DELETE.</para>
        /// </summary>
        /// <param name="uri">The URI to which a DELETE request should be sent.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to send an HTTP DELETE request.</para>
        /// </returns>
        public static UnityWebRequest Delete(string uri)
        {
            return new UnityWebRequest(uri, "DELETE");
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest configured to send a HTTP HEAD request.</para>
        /// </summary>
        /// <param name="uri">The URI to which to send a HTTP HEAD request.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to transmit a HTTP HEAD request.</para>
        /// </returns>
        public static UnityWebRequest Head(string uri)
        {
            return new UnityWebRequest(uri, "HEAD");
        }

        /// <summary>
        /// <para>Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
        /// </summary>
        /// <param name="uri">The URI of the image to download.</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <returns>
        /// <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
        /// </returns>
        public static UnityWebRequest GetTexture(string uri)
        {
            return GetTexture(uri, false);
        }

        /// <summary>
        /// <para>Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
        /// </summary>
        /// <param name="uri">The URI of the image to download.</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <returns>
        /// <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
        /// </returns>
        public static UnityWebRequest GetTexture(string uri, bool nonReadable)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerTexture(nonReadable), null);
        }

        /// <summary>
        /// <para>Create a UnityWebRequest intended to download an audio clip via HTTP GET and create an AudioClip based on the retrieved data.</para>
        /// </summary>
        /// <param name="uri">The URI of the audio clip to download.</param>
        /// <param name="audioType">The type of audio encoding for the downloaded audio clip. See AudioType.</param>
        /// <returns>
        /// <para>A UnityWebRequest properly configured to download an audio clip and convert it to an AudioClip.</para>
        /// </returns>
        public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAudioClip(uri, audioType), null);
        }

        public static UnityWebRequest GetAssetBundle(string uri)
        {
            return GetAssetBundle(uri, 0);
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
        /// </summary>
        /// <param name="uri">The URI of the asset bundle to download.</param>
        /// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
        /// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
        /// 
        /// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
        /// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
        /// </returns>
        public static UnityWebRequest GetAssetBundle(string uri, uint crc)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, crc), null);
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
        /// </summary>
        /// <param name="uri">The URI of the asset bundle to download.</param>
        /// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
        /// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
        /// 
        /// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
        /// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
        /// </returns>
        public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, version, crc), null);
        }

        /// <summary>
        /// <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
        /// </summary>
        /// <param name="uri">The URI of the asset bundle to download.</param>
        /// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
        /// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
        /// 
        /// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
        /// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
        /// </returns>
        public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, hash, crc), null);
        }

        /// <summary>
        /// <para>Create a UnityWebRequest configured to upload raw data to a remote server via HTTP PUT.</para>
        /// </summary>
        /// <param name="uri">The URI to which the data will be sent.</param>
        /// <param name="bodyData">The data to transmit to the remote server.
        /// 
        /// If a string, the string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to transmit bodyData to uri via HTTP PUT.</para>
        /// </returns>
        public static UnityWebRequest Put(string uri, byte[] bodyData)
        {
            return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
        }

        /// <summary>
        /// <para>Create a UnityWebRequest configured to upload raw data to a remote server via HTTP PUT.</para>
        /// </summary>
        /// <param name="uri">The URI to which the data will be sent.</param>
        /// <param name="bodyData">The data to transmit to the remote server.
        /// 
        /// If a string, the string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to transmit bodyData to uri via HTTP PUT.</para>
        /// </returns>
        public static UnityWebRequest Put(string uri, string bodyData)
        {
            return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
        }

        /// <summary>
        /// <para>Create a UnityWebRequest configured to send form data to a server via HTTP POST.</para>
        /// </summary>
        /// <param name="uri">The target URI to which form data will be transmitted.</param>
        /// <param name="postData">Form body data. Will be URLEncoded via WWWTranscoder.URLEncode prior to transmission.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to send form data to uri via POST.</para>
        /// </returns>
        public static UnityWebRequest Post(string uri, string postData)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            byte[] data = null;
            if (!string.IsNullOrEmpty(postData))
            {
                string s = WWWTranscoder.URLEncode(postData, Encoding.UTF8);
                data = Encoding.UTF8.GetBytes(s);
            }
            request.uploadHandler = new UploadHandlerRaw(data);
            request.uploadHandler.contentType = "application/x-www-form-urlencoded";
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        /// <summary>
        /// <para>Create a UnityWebRequest configured to send form data to a server via HTTP POST.</para>
        /// </summary>
        /// <param name="uri">The target URI to which form data will be transmitted.</param>
        /// <param name="formData">Form fields or files encapsulated in a WWWForm object, for formatting and transmission to the remote server.</param>
        /// <returns>
        /// <para>A UnityWebRequest configured to send form data to uri via POST.</para>
        /// </returns>
        public static UnityWebRequest Post(string uri, WWWForm formData)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            byte[] data = null;
            if (formData != null)
            {
                data = formData.data;
                if (data.Length == 0)
                {
                    data = null;
                }
            }
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            if (formData != null)
            {
                Dictionary<string, string> headers = formData.headers;
                foreach (KeyValuePair<string, string> pair in headers)
                {
                    request.SetRequestHeader(pair.Key, pair.Value);
                }
            }
            return request;
        }

        public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections)
        {
            byte[] boundary = GenerateBoundary();
            return Post(uri, multipartFormSections, boundary);
        }

        public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            byte[] data = null;
            if ((multipartFormSections != null) && (multipartFormSections.Count != 0))
            {
                data = SerializeFormSections(multipartFormSections, boundary);
            }
            UploadHandler handler = new UploadHandlerRaw(data) {
                contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length)
            };
            request.uploadHandler = handler;
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            byte[] data = null;
            if ((formFields != null) && (formFields.Count != 0))
            {
                data = SerializeSimpleForm(formFields);
            }
            UploadHandler handler = new UploadHandlerRaw(data) {
                contentType = "application/x-www-form-urlencoded"
            };
            request.uploadHandler = handler;
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
            int capacity = 0;
            foreach (IMultipartFormSection section in multipartFormSections)
            {
                capacity += 0x40 + section.sectionData.Length;
            }
            List<byte> list = new List<byte>(capacity);
            foreach (IMultipartFormSection section2 in multipartFormSections)
            {
                string str = "form-data";
                string sectionName = section2.sectionName;
                string fileName = section2.fileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    str = "file";
                }
                string s = "Content-Disposition: " + str;
                if (!string.IsNullOrEmpty(sectionName))
                {
                    s = s + "; name=\"" + sectionName + "\"";
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    s = s + "; filename=\"" + fileName + "\"";
                }
                s = s + "\r\n";
                string contentType = section2.contentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    s = s + "Content-Type: " + contentType + "\r\n";
                }
                list.AddRange(boundary);
                list.AddRange(bytes);
                list.AddRange(Encoding.UTF8.GetBytes(s));
                list.AddRange(bytes);
                list.AddRange(section2.sectionData);
            }
            list.TrimExcess();
            return list.ToArray();
        }

        /// <summary>
        /// <para>Generate a random 40-byte array for use as a multipart form boundary.</para>
        /// </summary>
        /// <returns>
        /// <para>40 random bytes, guaranteed to contain only printable ASCII values.</para>
        /// </returns>
        public static byte[] GenerateBoundary()
        {
            byte[] buffer = new byte[40];
            for (int i = 0; i < 40; i++)
            {
                int num2 = UnityEngine.Random.Range(0x30, 110);
                if (num2 > 0x39)
                {
                    num2 += 7;
                }
                if (num2 > 90)
                {
                    num2 += 6;
                }
                buffer[i] = (byte) num2;
            }
            return buffer;
        }

        public static byte[] SerializeSimpleForm(Dictionary<string, string> formFields)
        {
            string s = "";
            foreach (KeyValuePair<string, string> pair in formFields)
            {
                if (s.Length > 0)
                {
                    s = s + "&";
                }
                s = s + Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value);
            }
            return Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// <para>If true, any DownloadHandler attached to this UnityWebRequest will have DownloadHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
        /// </summary>
        public bool disposeDownloadHandlerOnDispose { get; set; }
        /// <summary>
        /// <para>If true, any UploadHandler attached to this UnityWebRequest will have UploadHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
        /// </summary>
        public bool disposeUploadHandlerOnDispose { get; set; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void InternalCreate();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        internal extern void InternalDestroy();
        private void InternalSetDefaults()
        {
            this.disposeDownloadHandlerOnDispose = true;
            this.disposeUploadHandlerOnDispose = true;
        }

        ~UnityWebRequest()
        {
            this.InternalDestroy();
        }

        /// <summary>
        /// <para>Signals that this [UnityWebRequest] is no longer being used, and should clean up any resources it is using.</para>
        /// </summary>
        public void Dispose()
        {
            if (this.disposeDownloadHandlerOnDispose)
            {
                DownloadHandler downloadHandler = this.downloadHandler;
                if (downloadHandler != null)
                {
                    downloadHandler.Dispose();
                }
            }
            if (this.disposeUploadHandlerOnDispose)
            {
                UploadHandler uploadHandler = this.uploadHandler;
                if (uploadHandler != null)
                {
                    uploadHandler.Dispose();
                }
            }
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern AsyncOperation InternalBegin();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void InternalAbort();
        /// <summary>
        /// <para>Begin communicating with the remote server.</para>
        /// </summary>
        /// <returns>
        /// <para>An AsyncOperation indicating the progress/completion state of the UnityWebRequest. Yield this object to wait until the UnityWebRequest is done.</para>
        /// </returns>
        public AsyncOperation Send()
        {
            return this.InternalBegin();
        }

        /// <summary>
        /// <para>If in progress, halts the UnityWebRequest as soon as possible.</para>
        /// </summary>
        public void Abort()
        {
            this.InternalAbort();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void InternalSetMethod(UnityWebRequestMethod methodType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void InternalSetCustomMethod(string customMethodName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int InternalGetMethod();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string InternalGetCustomMethod();
        /// <summary>
        /// <para>Defines the HTTP verb used by this UnityWebRequest, such as GET or POST.</para>
        /// </summary>
        public string method
        {
            get
            {
                switch (this.InternalGetMethod())
                {
                    case 0:
                        return "GET";

                    case 1:
                        return "POST";

                    case 2:
                        return "PUT";

                    case 3:
                        return "HEAD";
                }
                return this.InternalGetCustomMethod();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
                }
                switch (value.ToUpper())
                {
                    case "GET":
                        this.InternalSetMethod(UnityWebRequestMethod.Get);
                        return;

                    case "POST":
                        this.InternalSetMethod(UnityWebRequestMethod.Post);
                        return;

                    case "PUT":
                        this.InternalSetMethod(UnityWebRequestMethod.Put);
                        return;

                    case "HEAD":
                        this.InternalSetMethod(UnityWebRequestMethod.Head);
                        return;
                }
                this.InternalSetCustomMethod(value.ToUpper());
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int InternalGetError();
        /// <summary>
        /// <para>A human-readable string describing any system errors encountered by this UnityWebRequest object while handling HTTP requests or responses. (Read Only)</para>
        /// </summary>
        public string error { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Determines whether this UnityWebRequest will include Expect: 100-Continue in its outgoing request headers. (Default: true).</para>
        /// </summary>
        public bool useHttpContinue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Defines the target URL for the UnityWebRequest to communicate with.</para>
        /// </summary>
        public string url
        {
            get
            {
                return this.InternalGetUrl();
            }
            set
            {
                string localUrl = "http://localhost/";
                this.InternalSetUrl(WebRequestUtils.MakeInitialUrl(value, localUrl));
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string InternalGetUrl();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void InternalSetUrl(string url);
        /// <summary>
        /// <para>The numeric HTTP response code returned by the server, such as 200, 404 or 500. (Read Only)</para>
        /// </summary>
        public long responseCode { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns a floating-point value between 0.0 and 1.0, indicating the progress of uploading body data to the server.</para>
        /// </summary>
        public float uploadProgress { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns true while a UnityWebRequest’s configuration properties can be altered. (Read Only)</para>
        /// </summary>
        public bool isModifiable { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns true after the UnityWebRequest has finished communicating with the remote server. (Read Only)</para>
        /// </summary>
        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns true after this UnityWebRequest encounters a system error. (Read Only)</para>
        /// </summary>
        public bool isError { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns a floating-point value between 0.0 and 1.0, indicating the progress of downloading body data from the server. (Read Only)</para>
        /// </summary>
        public float downloadProgress { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns the number of bytes of body data the system has uploaded to the remote server. (Read Only)</para>
        /// </summary>
        public ulong uploadedBytes { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Returns the number of bytes of body data the system has downloaded from the remote server. (Read Only)</para>
        /// </summary>
        public ulong downloadedBytes { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>Indicates the number of redirects which this UnityWebRequest will follow before halting with a “Redirect Limit Exceeded” system error.</para>
        /// </summary>
        public int redirectLimit { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Indicates whether the UnityWebRequest system should employ the HTTP/1.1 chunked-transfer encoding method.</para>
        /// </summary>
        public bool chunkedTransfer { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Retrieves the value of a custom request header.</para>
        /// </summary>
        /// <param name="name">Name of the custom request header. Case-insensitive.</param>
        /// <returns>
        /// <para>The value of the custom request header. If no custom header with a matching name has been set, returns an empty string.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetRequestHeader(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void InternalSetRequestHeader(string name, string value);
        /// <summary>
        /// <para>Set a HTTP request header to a custom value.</para>
        /// </summary>
        /// <param name="name">The key of the header to be set. Case-sensitive.</param>
        /// <param name="value">The header's intended value.</param>
        public void SetRequestHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot set a Request Header with a null or empty name");
            }
            if (value == null)
            {
                throw new ArgumentException("Cannot set a Request header with a null");
            }
            if (!IsHeaderNameLegal(name))
            {
                throw new ArgumentException("Cannot set Request Header " + name + " - name contains illegal characters or is not user-overridable");
            }
            if (!IsHeaderValueLegal(value))
            {
                throw new ArgumentException("Cannot set Request Header - value contains illegal characters");
            }
            this.InternalSetRequestHeader(name, value);
        }

        /// <summary>
        /// <para>Retrieves the value of a response header from the latest HTTP response received.</para>
        /// </summary>
        /// <param name="name">The name of the HTTP header to retrieve. Case-insensitive.</param>
        /// <returns>
        /// <para>The value of the HTTP header from the latest HTTP response. If no header with a matching name has been received, or no responses have been received, returns null.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetResponseHeader(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string[] InternalGetResponseHeaderKeys();
        /// <summary>
        /// <para>Retrieves a dictionary containing all the response headers received by this UnityWebRequest in the latest HTTP response.</para>
        /// </summary>
        /// <returns>
        /// <para>A dictionary containing all the response headers received in the latest HTTP response. If no responses have been received, returns null.</para>
        /// </returns>
        public Dictionary<string, string> GetResponseHeaders()
        {
            string[] responseHeaderKeys = this.InternalGetResponseHeaderKeys();
            if (responseHeaderKeys == null)
            {
                return null;
            }
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>(responseHeaderKeys.Length, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < responseHeaderKeys.Length; i++)
            {
                string responseHeader = this.GetResponseHeader(responseHeaderKeys[i]);
                dictionary2.Add(responseHeaderKeys[i], responseHeader);
            }
            return dictionary2;
        }

        /// <summary>
        /// <para>Holds a reference to the UploadHandler object which manages body data to be uploaded to the remote server.</para>
        /// </summary>
        public UploadHandler uploadHandler { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Holds a reference to a DownloadHandler object, which manages body data received from the remote server by this UnityWebRequest.</para>
        /// </summary>
        public DownloadHandler downloadHandler { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        private static bool ContainsForbiddenCharacters(string s, int firstAllowedCharCode)
        {
            foreach (char ch in s)
            {
                if ((ch < firstAllowedCharCode) || (ch == '\x007f'))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsHeaderNameLegal(string headerName)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                return false;
            }
            headerName = headerName.ToLower();
            if (ContainsForbiddenCharacters(headerName, 0x21))
            {
                return false;
            }
            if (headerName.StartsWith("sec-") || headerName.StartsWith("proxy-"))
            {
                return false;
            }
            foreach (string str in forbiddenHeaderKeys)
            {
                if (string.Equals(headerName, str))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsHeaderValueLegal(string headerValue)
        {
            if (ContainsForbiddenCharacters(headerValue, 0x20))
            {
                return false;
            }
            return true;
        }

        private static string GetErrorDescription(UnityWebRequestError errorCode)
        {
            switch (errorCode)
            {
                case UnityWebRequestError.OK:
                    return "No Error";

                case UnityWebRequestError.SDKError:
                    return "Internal Error With Transport Layer";

                case UnityWebRequestError.UnsupportedProtocol:
                    return "Specified Transport Protocol is Unsupported";

                case UnityWebRequestError.MalformattedUrl:
                    return "URL is Malformatted";

                case UnityWebRequestError.CannotResolveProxy:
                    return "Unable to resolve specified proxy server";

                case UnityWebRequestError.CannotResolveHost:
                    return "Unable to resolve host specified in URL";

                case UnityWebRequestError.CannotConnectToHost:
                    return "Unable to connect to host specified in URL";

                case UnityWebRequestError.AccessDenied:
                    return "Remote server denied access to the specified URL";

                case UnityWebRequestError.GenericHTTPError:
                    return "Unknown/Generic HTTP Error - Check HTTP Error code";

                case UnityWebRequestError.WriteError:
                    return "Error when transmitting request to remote server - transmission terminated prematurely";

                case UnityWebRequestError.ReadError:
                    return "Error when reading response from remote server - transmission terminated prematurely";

                case UnityWebRequestError.OutOfMemory:
                    return "Out of Memory";

                case UnityWebRequestError.Timeout:
                    return "Timeout occurred while waiting for response from remote server";

                case UnityWebRequestError.HTTPPostError:
                    return "Error while transmitting HTTP POST body data";

                case UnityWebRequestError.SSLCannotConnect:
                    return "Unable to connect to SSL server at remote host";

                case UnityWebRequestError.Aborted:
                    return "Request was manually aborted by local code";

                case UnityWebRequestError.TooManyRedirects:
                    return "Redirect limit exceeded";

                case UnityWebRequestError.ReceivedNoData:
                    return "Received an empty response from remote host";

                case UnityWebRequestError.SSLNotSupported:
                    return "SSL connections are not supported on the local machine";

                case UnityWebRequestError.FailedToSendData:
                    return "Failed to transmit body data";

                case UnityWebRequestError.FailedToReceiveData:
                    return "Failed to receive response body data";

                case UnityWebRequestError.SSLCertificateError:
                    return "Failure to authenticate SSL certificate of remote host";

                case UnityWebRequestError.SSLCipherNotAvailable:
                    return "SSL cipher received from remote host is not supported on the local machine";

                case UnityWebRequestError.SSLCACertError:
                    return "Failure to authenticate Certificate Authority of the SSL certificate received from the remote host";

                case UnityWebRequestError.UnrecognizedContentEncoding:
                    return "Remote host returned data with an unrecognized/unparseable content encoding";

                case UnityWebRequestError.LoginFailed:
                    return "HTTP authentication failed";

                case UnityWebRequestError.SSLShutdownFailed:
                    return "Failure while shutting down SSL connection";
            }
            return "Unknown error";
        }
        internal enum UnityWebRequestError
        {
            OK,
            Unknown,
            SDKError,
            UnsupportedProtocol,
            MalformattedUrl,
            CannotResolveProxy,
            CannotResolveHost,
            CannotConnectToHost,
            AccessDenied,
            GenericHTTPError,
            WriteError,
            ReadError,
            OutOfMemory,
            Timeout,
            HTTPPostError,
            SSLCannotConnect,
            Aborted,
            TooManyRedirects,
            ReceivedNoData,
            SSLNotSupported,
            FailedToSendData,
            FailedToReceiveData,
            SSLCertificateError,
            SSLCipherNotAvailable,
            SSLCACertError,
            UnrecognizedContentEncoding,
            LoginFailed,
            SSLShutdownFailed
        }

        internal enum UnityWebRequestMethod
        {
            Get,
            Post,
            Put,
            Head,
            Custom
        }
    }
}


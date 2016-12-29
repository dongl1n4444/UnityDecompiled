namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Internal;

    internal sealed class AsyncHTTPClient
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<string, string>, string> <>f__am$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <responseCode>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private State <state>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <tag>k__BackingField;
        public DoneCallback doneCallback;
        public Dictionary<string, string> header;
        private string m_FromData;
        private IntPtr m_Handle;
        private string m_Method;
        private string m_ToUrl;
        public StatusCallback statusCallback;

        public AsyncHTTPClient(string _toUrl)
        {
            this.m_ToUrl = _toUrl;
            this.m_FromData = null;
            this.m_Method = "";
            this.state = State.INIT;
            this.header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.m_Handle = IntPtr.Zero;
            this.tag = "";
            this.statusCallback = null;
        }

        public AsyncHTTPClient(string _toUrl, string _method)
        {
            this.m_ToUrl = _toUrl;
            this.m_FromData = null;
            this.m_Method = _method;
            this.state = State.INIT;
            this.header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.m_Handle = IntPtr.Zero;
            this.tag = "";
            this.statusCallback = null;
        }

        public void Abort()
        {
            this.state = State.ABORTED;
            AbortByHandle(this.m_Handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void AbortByHandle(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void AbortByTag(string tag);
        public void Begin()
        {
            if (this.IsAborted())
            {
                this.state = State.ABORTED;
            }
            else
            {
                if (this.m_Method == "")
                {
                    this.m_Method = "GET";
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<KeyValuePair<string, string>, string>(null, (IntPtr) <Begin>m__1);
                }
                string[] headers = Enumerable.Select<KeyValuePair<string, string>, string>(this.header, <>f__am$cache0).ToArray<string>();
                this.m_Handle = SubmitClientRequest(this.tag, this.m_ToUrl, headers, this.m_Method, this.m_FromData, new RequestDoneCallback(this.Done), new RequestProgressCallback(this.Progress));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CurlRequestCheck();
        private void Done(State status, int i_ResponseCode)
        {
            this.state = status;
            this.responseCode = i_ResponseCode;
            if (this.doneCallback != null)
            {
                this.doneCallback(this);
            }
            this.m_Handle = IntPtr.Zero;
        }

        private string EscapeLong(string v)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < v.Length; i += 0x7ffe)
            {
                builder.Append(Uri.EscapeDataString(v.Substring(i, ((v.Length - i) <= 0x7ffe) ? (v.Length - i) : 0x7ffe)));
            }
            return builder.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern byte[] GetBytesByHandle(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Texture2D GetTextureByHandle(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SubmitClientRequest(string tag, string url, string[] headers, string method, string data, RequestDoneCallback doneDelegate, RequestProgressCallback progressDelegate, out IntPtr value);
        public bool IsAborted() => 
            (this.state == State.ABORTED);

        public bool IsDone() => 
            IsDone(this.state);

        public static bool IsDone(State state)
        {
            switch (state)
            {
                case State.DONE_OK:
                case State.DONE_FAILED:
                case State.ABORTED:
                case State.TIMEOUT:
                    return true;
            }
            return false;
        }

        public bool IsSuccess() => 
            (this.state == State.DONE_OK);

        public static bool IsSuccess(State state) => 
            (state == State.DONE_OK);

        private void Progress(State status, int bytesDone, int bytesTotal)
        {
            this.state = status;
            if (this.statusCallback != null)
            {
                this.statusCallback(status, bytesDone, bytesTotal);
            }
        }

        [ExcludeFromDocs]
        private static IntPtr SubmitClientRequest(string tag, string url, string[] headers, string method, string data, RequestDoneCallback doneDelegate)
        {
            RequestProgressCallback progressDelegate = null;
            IntPtr ptr;
            INTERNAL_CALL_SubmitClientRequest(tag, url, headers, method, data, doneDelegate, progressDelegate, out ptr);
            return ptr;
        }

        private static IntPtr SubmitClientRequest(string tag, string url, string[] headers, string method, string data, RequestDoneCallback doneDelegate, [DefaultValue("null")] RequestProgressCallback progressDelegate)
        {
            IntPtr ptr;
            INTERNAL_CALL_SubmitClientRequest(tag, url, headers, method, data, doneDelegate, progressDelegate, out ptr);
            return ptr;
        }

        public byte[] bytes =>
            GetBytesByHandle(this.m_Handle);

        public string postData
        {
            set
            {
                this.m_FromData = value;
                if (this.m_Method == "")
                {
                    this.m_Method = "POST";
                }
                if (!this.header.ContainsKey("Content-Type"))
                {
                    this.header["Content-Type"] = "application/x-www-form-urlencoded";
                }
            }
        }

        public Dictionary<string, string> postDictionary
        {
            set
            {
                this.postData = string.Join("&", Enumerable.Select<KeyValuePair<string, string>, string>(value, new Func<KeyValuePair<string, string>, string>(this, (IntPtr) this.<set_postDictionary>m__0)).ToArray<string>());
            }
        }

        public int responseCode { get; private set; }

        public State state { get; private set; }

        public string tag { get; set; }

        public string text
        {
            get
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = this.bytes;
                if (bytes == null)
                {
                    return null;
                }
                return encoding.GetString(bytes);
            }
        }

        public Texture2D texture =>
            GetTextureByHandle(this.m_Handle);

        public string url =>
            this.m_ToUrl;

        public delegate void DoneCallback(AsyncHTTPClient client);

        private delegate void RequestDoneCallback(AsyncHTTPClient.State status, int httpStatus);

        private delegate void RequestProgressCallback(AsyncHTTPClient.State status, int downloaded, int totalSize);

        internal enum State
        {
            INIT,
            CONNECTING,
            CONNECTED,
            UPLOADING,
            DOWNLOADING,
            CONFIRMING,
            DONE_OK,
            DONE_FAILED,
            ABORTED,
            TIMEOUT
        }

        public delegate void StatusCallback(AsyncHTTPClient.State status, int bytesDone, int bytesTotal);
    }
}


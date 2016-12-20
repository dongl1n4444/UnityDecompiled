namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Simple access to web pages.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class WWW : IDisposable
    {
        private static readonly char[] forbiddenCharacters = new char[] { 
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\x000e', '\x000f',
            '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', '\x001c', '\x001d', '\x001e', '\x001f',
            '\x007f', '\0'
        };
        private static readonly char[] forbiddenCharactersForNames = new char[] { ' ' };
        private static readonly string[] forbiddenHeaderKeys = new string[] { 
            "Accept-Charset", "Accept-Encoding", "Access-Control-Request-Headers", "Access-Control-Request-Method", "Connection", "Content-Length", "Cookie", "Cookie2", "Date", "DNT", "Expect", "Host", "Keep-Alive", "Origin", "Referer", "TE",
            "Trailer", "Transfer-Encoding", "Upgrade", "User-Agent", "Via", "X-Unity-Version"
        };
        [RequiredByNativeCode]
        internal IntPtr m_Ptr;

        /// <summary>
        /// <para>Creates a WWW request with the given URL.</para>
        /// </summary>
        /// <param name="url">The url to download. Must be '%' escaped.</param>
        /// <returns>
        /// <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
        /// </returns>
        public WWW(string url)
        {
            this.InitWWW(url, null, null);
        }

        /// <summary>
        /// <para>Creates a WWW request with the given URL.</para>
        /// </summary>
        /// <param name="url">The url to download. Must be '%' escaped.</param>
        /// <param name="form">A WWWForm instance containing the form data to post.</param>
        /// <returns>
        /// <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
        /// </returns>
        public WWW(string url, WWWForm form)
        {
            string[] headers = FlattenedHeadersFrom(form.headers);
            if (this.enforceWebSecurityRestrictions())
            {
                CheckSecurityOnHeaders(headers);
            }
            this.InitWWW(url, form.data, headers);
        }

        /// <summary>
        /// <para>Creates a WWW request with the given URL.</para>
        /// </summary>
        /// <param name="url">The url to download. Must be '%' escaped.</param>
        /// <param name="postData">A byte array of data to be posted to the url.</param>
        /// <returns>
        /// <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
        /// </returns>
        public WWW(string url, byte[] postData)
        {
            this.InitWWW(url, postData, null);
        }

        public WWW(string url, byte[] postData, Dictionary<string, string> headers)
        {
            string[] strArray = FlattenedHeadersFrom(headers);
            if (this.enforceWebSecurityRestrictions())
            {
                CheckSecurityOnHeaders(strArray);
            }
            this.InitWWW(url, postData, strArray);
        }

        internal WWW(string url, Hash128 hash, uint crc)
        {
            INTERNAL_CALL_WWW(this, url, ref hash, crc);
        }

        private static void CheckSecurityOnHeaders(string[] headers)
        {
            for (int i = 0; i < headers.Length; i += 2)
            {
                foreach (string str in forbiddenHeaderKeys)
                {
                    if (string.Equals(headers[i], str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new ArgumentException("Cannot overwrite header: " + headers[i]);
                    }
                }
                if (headers[i].StartsWith("Sec-") || headers[i].StartsWith("Proxy-"))
                {
                    throw new ArgumentException("Cannot overwrite header: " + headers[i]);
                }
                if (((headers[i].IndexOfAny(forbiddenCharacters) > -1) || (headers[i].IndexOfAny(forbiddenCharactersForNames) > -1)) || (headers[i + 1].IndexOfAny(forbiddenCharacters) > -1))
                {
                    throw new ArgumentException("Cannot include control characters in a HTTP header, either as key or value.");
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void DestroyWWW(bool cancel);
        /// <summary>
        /// <para>Disposes of an existing WWW object.</para>
        /// </summary>
        public void Dispose()
        {
            this.DestroyWWW(true);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool enforceWebSecurityRestrictions();
        /// <summary>
        /// <para>Escapes characters in a string to ensure they are URL-friendly.</para>
        /// </summary>
        /// <param name="s">A string with characters to be escaped.</param>
        /// <param name="e">The text encoding to use.</param>
        [ExcludeFromDocs]
        public static string EscapeURL(string s)
        {
            Encoding e = Encoding.UTF8;
            return EscapeURL(s, e);
        }

        /// <summary>
        /// <para>Escapes characters in a string to ensure they are URL-friendly.</para>
        /// </summary>
        /// <param name="s">A string with characters to be escaped.</param>
        /// <param name="e">The text encoding to use.</param>
        public static string EscapeURL(string s, [UnityEngine.Internal.DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
        {
            if (s == null)
            {
                return null;
            }
            if (s == "")
            {
                return "";
            }
            if (e == null)
            {
                return null;
            }
            return WWWTranscoder.URLEncode(s, e);
        }

        ~WWW()
        {
            this.DestroyWWW(false);
        }

        private static string[] FlattenedHeadersFrom(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                return null;
            }
            string[] strArray2 = new string[headers.Count * 2];
            int num = 0;
            foreach (KeyValuePair<string, string> pair in headers)
            {
                strArray2[num++] = pair.Key.ToString();
                strArray2[num++] = pair.Value.ToString();
            }
            return strArray2;
        }

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
        /// the .audioClip property defaults to 3D.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
        /// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
        /// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public AudioClip GetAudioClip(bool threeD)
        {
            return this.GetAudioClip(threeD, false);
        }

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
        /// the .audioClip property defaults to 3D.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
        /// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
        /// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public AudioClip GetAudioClip(bool threeD, bool stream)
        {
            return this.GetAudioClip(threeD, stream, AudioType.UNKNOWN);
        }

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
        /// the .audioClip property defaults to 3D.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
        /// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
        /// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType)
        {
            return this.GetAudioClipInternal(threeD, stream, false, audioType);
        }

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
        /// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public AudioClip GetAudioClipCompressed()
        {
            return this.GetAudioClipCompressed(true);
        }

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
        /// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public AudioClip GetAudioClipCompressed(bool threeD)
        {
            return this.GetAudioClipCompressed(threeD, AudioType.UNKNOWN);
        }

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
        /// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public AudioClip GetAudioClipCompressed(bool threeD, AudioType audioType)
        {
            return this.GetAudioClipInternal(threeD, false, true, audioType);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AudioClip GetAudioClipInternal(bool threeD, bool stream, bool compressed, AudioType audioType);
        private Encoding GetTextEncoder()
        {
            string str = null;
            if (this.responseHeaders.TryGetValue("CONTENT-TYPE", out str))
            {
                int index = str.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                {
                    int num2 = str.IndexOf('=', index);
                    if (num2 > -1)
                    {
                        char[] trimChars = new char[] { '\'', '"' };
                        string name = str.Substring(num2 + 1).Trim().Trim(trimChars).Trim();
                        int length = name.IndexOf(';');
                        if (length > -1)
                        {
                            name = name.Substring(0, length);
                        }
                        try
                        {
                            return Encoding.GetEncoding(name);
                        }
                        catch (Exception)
                        {
                            Debug.Log("Unsupported encoding: '" + name + "'");
                        }
                    }
                }
            }
            return Encoding.UTF8;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Texture2D GetTexture(bool markNonReadable);
        [Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
        public static Texture2D GetTextureFromURL(string url)
        {
            return new WWW(url).texture;
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
        public static extern string GetURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void InitWWW(string url, byte[] postData, string[] iHeaders);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_WWW(WWW self, string url, ref Hash128 hash, uint crc);
        /// <summary>
        /// <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
        /// </summary>
        /// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
        /// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
        /// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
        /// <returns>
        /// <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
        /// </returns>
        [ExcludeFromDocs]
        public static WWW LoadFromCacheOrDownload(string url, int version)
        {
            uint crc = 0;
            return LoadFromCacheOrDownload(url, version, crc);
        }

        [ExcludeFromDocs]
        public static WWW LoadFromCacheOrDownload(string url, Hash128 hash)
        {
            uint crc = 0;
            return LoadFromCacheOrDownload(url, hash, crc);
        }

        /// <summary>
        /// <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
        /// </summary>
        /// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
        /// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
        /// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
        /// <returns>
        /// <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
        /// </returns>
        public static WWW LoadFromCacheOrDownload(string url, int version, [UnityEngine.Internal.DefaultValue("0")] uint crc)
        {
            Hash128 hash = new Hash128(0, 0, 0, (uint) version);
            return LoadFromCacheOrDownload(url, hash, crc);
        }

        public static WWW LoadFromCacheOrDownload(string url, Hash128 hash, [UnityEngine.Internal.DefaultValue("0")] uint crc)
        {
            return new WWW(url, hash, crc);
        }

        /// <summary>
        /// <para>Replaces the contents of an existing Texture2D with an image from the downloaded data.</para>
        /// </summary>
        /// <param name="tex">An existing texture object to be overwritten with the image data.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void LoadImageIntoTexture(Texture2D tex);
        /// <summary>
        /// <para>Loads the new web player data file.</para>
        /// </summary>
        [Obsolete("LoadUnityWeb is no longer supported. Please use javascript to reload the web player on a different url instead", true)]
        public void LoadUnityWeb()
        {
        }

        internal static Dictionary<string, string> ParseHTTPHeaderString(string input)
        {
            if (input == null)
            {
                throw new ArgumentException("input was null to ParseHTTPHeaderString");
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            StringReader reader = new StringReader(input);
            int num = 0;
            while (true)
            {
                string str = reader.ReadLine();
                if (str == null)
                {
                    return dictionary;
                }
                if ((num++ == 0) && str.StartsWith("HTTP"))
                {
                    dictionary["STATUS"] = str;
                }
                else
                {
                    int index = str.IndexOf(": ");
                    if (index != -1)
                    {
                        string str4;
                        string key = str.Substring(0, index).ToUpper();
                        string str3 = str.Substring(index + 2);
                        if (dictionary.TryGetValue(key, out str4))
                        {
                            str3 = str4 + "," + str3;
                        }
                        dictionary[key] = str3;
                    }
                }
            }
        }

        /// <summary>
        /// <para>Converts URL-friendly escape sequences back to normal text.</para>
        /// </summary>
        /// <param name="s">A string containing escaped characters.</param>
        /// <param name="e">The text encoding to use.</param>
        [ExcludeFromDocs]
        public static string UnEscapeURL(string s)
        {
            Encoding e = Encoding.UTF8;
            return UnEscapeURL(s, e);
        }

        /// <summary>
        /// <para>Converts URL-friendly escape sequences back to normal text.</para>
        /// </summary>
        /// <param name="s">A string containing escaped characters.</param>
        /// <param name="e">The text encoding to use.</param>
        public static string UnEscapeURL(string s, [UnityEngine.Internal.DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
        {
            if (s == null)
            {
                return null;
            }
            if ((s.IndexOf('%') == -1) && (s.IndexOf('+') == -1))
            {
                return s;
            }
            return WWWTranscoder.URLDecode(s, e);
        }

        /// <summary>
        /// <para>Streams an AssetBundle that can contain any kind of asset from the project folder.</para>
        /// </summary>
        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns a AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        public AudioClip audioClip
        {
            get
            {
                return this.GetAudioClip(true);
            }
        }

        /// <summary>
        /// <para>Returns the contents of the fetched web page as a byte array (Read Only).</para>
        /// </summary>
        public byte[] bytes { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The number of bytes downloaded by this WWW query (read only).</para>
        /// </summary>
        public int bytesDownloaded { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Obsolete("Please use WWW.text instead")]
        public string data
        {
            get
            {
                return this.text;
            }
        }

        internal static Encoding DefaultEncoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }

        /// <summary>
        /// <para>Returns an error message if there was an error during the download (Read Only).</para>
        /// </summary>
        public string error { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is the download already finished? (Read Only)</para>
        /// </summary>
        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns a MovieTexture generated from the downloaded data (Read Only).</para>
        /// </summary>
        public MovieTexture movie { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Load an Ogg Vorbis file into the audio clip.</para>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property WWW.oggVorbis has been deprecated. Use WWW.audioClip instead (UnityUpgradable).", true)]
        public AudioClip oggVorbis
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// <para>How far has the download progressed (Read Only).</para>
        /// </summary>
        public float progress { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Dictionary of headers returned by the request.</para>
        /// </summary>
        public Dictionary<string, string> responseHeaders
        {
            get
            {
                if (!this.isDone)
                {
                    throw new UnityException("WWW is not finished downloading yet");
                }
                return ParseHTTPHeaderString(this.responseHeadersString);
            }
        }

        private string responseHeadersString { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int size { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the contents of the fetched web page as a string (Read Only).</para>
        /// </summary>
        public string text
        {
            get
            {
                if (!this.isDone)
                {
                    throw new UnityException("WWW is not ready downloading yet");
                }
                byte[] bytes = this.bytes;
                return this.GetTextEncoder().GetString(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// <para>Returns a Texture2D generated from the downloaded data (Read Only).</para>
        /// </summary>
        public Texture2D texture
        {
            get
            {
                return this.GetTexture(false);
            }
        }

        /// <summary>
        /// <para>Returns a non-readable Texture2D generated from the downloaded data (Read Only).</para>
        /// </summary>
        public Texture2D textureNonReadable
        {
            get
            {
                return this.GetTexture(true);
            }
        }

        /// <summary>
        /// <para>Priority of AssetBundle decompression thread.</para>
        /// </summary>
        public ThreadPriority threadPriority { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How far has the upload progressed (Read Only).</para>
        /// </summary>
        public float uploadProgress { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The URL of this WWW request (Read Only).</para>
        /// </summary>
        public string url { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}


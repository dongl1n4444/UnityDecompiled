﻿namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>Helpers for downloading image files into Textures using UnityWebRequest.</para>
    /// </summary>
    public static class UnityWebRequestTexture
    {
        /// <summary>
        /// <para>Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
        /// </summary>
        /// <param name="uri">The URI of the image to download.</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <returns>
        /// <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
        /// </returns>
        public static UnityWebRequest GetTexture(string uri) => 
            GetTexture(uri, false);

        /// <summary>
        /// <para>Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
        /// </summary>
        /// <param name="uri">The URI of the image to download.</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <returns>
        /// <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
        /// </returns>
        public static UnityWebRequest GetTexture(string uri, bool nonReadable) => 
            new UnityWebRequest(uri, "GET", new DownloadHandlerTexture(nonReadable), null);
    }
}


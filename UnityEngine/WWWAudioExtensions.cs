namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Provides methods to access AudioClip or MovieTexture objects from WWW streams.</para>
    /// </summary>
    public static class WWWAudioExtensions
    {
        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.
        /// The threeD parameter defaults to true.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false), or whether the stream can be played even if only part of the clip is downloaded (true).
        /// Setting this to true will disable seeking (with .time and/or .timeSamples) on the clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClip(this WWW www) => 
            www.GetAudioClip(true, false, AudioType.UNKNOWN);

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.
        /// The threeD parameter defaults to true.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false), or whether the stream can be played even if only part of the clip is downloaded (true).
        /// Setting this to true will disable seeking (with .time and/or .timeSamples) on the clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClip(this WWW www, bool threeD) => 
            www.GetAudioClip(threeD, false, AudioType.UNKNOWN);

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.
        /// The threeD parameter defaults to true.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false), or whether the stream can be played even if only part of the clip is downloaded (true).
        /// Setting this to true will disable seeking (with .time and/or .timeSamples) on the clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClip(this WWW www, bool threeD, bool stream) => 
            www.GetAudioClip(threeD, stream, AudioType.UNKNOWN);

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.
        /// The threeD parameter defaults to true.</param>
        /// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false), or whether the stream can be played even if only part of the clip is downloaded (true).
        /// Setting this to true will disable seeking (with .time and/or .timeSamples) on the clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClip(this WWW www, bool threeD, bool stream, AudioType audioType) => 
            ((AudioClip) www.GetAudioClipInternal(threeD, stream, false, audioType));

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClipCompressed(this WWW www) => 
            www.GetAudioClipCompressed(false, AudioType.UNKNOWN);

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClipCompressed(this WWW www, bool threeD) => 
            www.GetAudioClipCompressed(threeD, AudioType.UNKNOWN);

        /// <summary>
        /// <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
        /// </summary>
        /// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
        /// <param name="audioType">The AudioType of the content you are downloading. If this is not set Unity will try to determine the type from URL.</param>
        /// <param name="www"></param>
        /// <returns>
        /// <para>The returned AudioClip.</para>
        /// </returns>
        public static AudioClip GetAudioClipCompressed(this WWW www, bool threeD, AudioType audioType) => 
            ((AudioClip) www.GetAudioClipInternal(threeD, false, true, audioType));

        /// <summary>
        /// <para>Returns a MovieTexture generated from the downloaded data (Read Only).</para>
        /// </summary>
        /// <param name="www"></param>
        public static MovieTexture GetMovieTexture(this WWW www) => 
            ((MovieTexture) www.GetMovieTextureInternal());
    }
}


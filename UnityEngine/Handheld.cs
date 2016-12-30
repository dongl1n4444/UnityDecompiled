namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.iOS;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface into functionality unique to handheld devices.</para>
    /// </summary>
    public sealed class Handheld
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ClearShaderCache();
        /// <summary>
        /// <para>Gets the current activity indicator style.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetActivityIndicatorStyle();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_PlayFullScreenMovie(string path, ref Color bgColor, FullScreenMovieControlMode controlMode, FullScreenMovieScalingMode scalingMode);
        /// <summary>
        /// <para>Plays a full-screen movie.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the movie file.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="controlMode">How the playback controls are to be displayed.</param>
        /// <param name="scalingMode">How the movie is to be scaled to fit the screen.</param>
        [ExcludeFromDocs]
        public static bool PlayFullScreenMovie(string path)
        {
            FullScreenMovieScalingMode aspectFit = FullScreenMovieScalingMode.AspectFit;
            FullScreenMovieControlMode full = FullScreenMovieControlMode.Full;
            Color black = Color.black;
            return INTERNAL_CALL_PlayFullScreenMovie(path, ref black, full, aspectFit);
        }

        /// <summary>
        /// <para>Plays a full-screen movie.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the movie file.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="controlMode">How the playback controls are to be displayed.</param>
        /// <param name="scalingMode">How the movie is to be scaled to fit the screen.</param>
        [ExcludeFromDocs]
        public static bool PlayFullScreenMovie(string path, Color bgColor)
        {
            FullScreenMovieScalingMode aspectFit = FullScreenMovieScalingMode.AspectFit;
            FullScreenMovieControlMode full = FullScreenMovieControlMode.Full;
            return INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, full, aspectFit);
        }

        /// <summary>
        /// <para>Plays a full-screen movie.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the movie file.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="controlMode">How the playback controls are to be displayed.</param>
        /// <param name="scalingMode">How the movie is to be scaled to fit the screen.</param>
        [ExcludeFromDocs]
        public static bool PlayFullScreenMovie(string path, Color bgColor, FullScreenMovieControlMode controlMode)
        {
            FullScreenMovieScalingMode aspectFit = FullScreenMovieScalingMode.AspectFit;
            return INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, aspectFit);
        }

        /// <summary>
        /// <para>Plays a full-screen movie.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the movie file.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="controlMode">How the playback controls are to be displayed.</param>
        /// <param name="scalingMode">How the movie is to be scaled to fit the screen.</param>
        public static bool PlayFullScreenMovie(string path, [DefaultValue("Color.black")] Color bgColor, [DefaultValue("FullScreenMovieControlMode.Full")] FullScreenMovieControlMode controlMode, [DefaultValue("FullScreenMovieScalingMode.AspectFit")] FullScreenMovieScalingMode scalingMode) => 
            INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);

        /// <summary>
        /// <para>Sets the desired activity indicator style.</para>
        /// </summary>
        /// <param name="style"></param>
        public static void SetActivityIndicatorStyle(AndroidActivityIndicatorStyle style)
        {
            SetActivityIndicatorStyleImpl((int) style);
        }

        public static void SetActivityIndicatorStyle(ActivityIndicatorStyle style)
        {
            SetActivityIndicatorStyleImpl((int) style);
        }

        /// <summary>
        /// <para>Sets the desired activity indicator style.</para>
        /// </summary>
        /// <param name="style"></param>
        public static void SetActivityIndicatorStyle(TizenActivityIndicatorStyle style)
        {
            SetActivityIndicatorStyleImpl((int) style);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetActivityIndicatorStyleImpl(int style);
        /// <summary>
        /// <para>Starts os activity indicator.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void StartActivityIndicator();
        /// <summary>
        /// <para>Stops os activity indicator.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void StopActivityIndicator();
        /// <summary>
        /// <para>Triggers device vibration.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Vibrate();

        /// <summary>
        /// <para>Determines whether or not a 32-bit display buffer will be used.</para>
        /// </summary>
        [Obsolete("Property Handheld.use32BitDisplayBuffer has been deprecated. Modifying it has no effect, use PlayerSettings instead.")]
        public static bool use32BitDisplayBuffer { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}


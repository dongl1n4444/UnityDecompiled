namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Defines the default look of secondary tile.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SecondaryTileData
    {
        /// <summary>
        /// <para>Arguments to be passed for application when secondary tile is activated.</para>
        /// </summary>
        public string arguments;
        private Color32 background;
        /// <summary>
        /// <para>Defines, whether backgroundColor should be used.
        /// </para>
        /// </summary>
        public bool backgroundColorSet;
        /// <summary>
        /// <para>Display name for secondary tile.
        /// </para>
        /// </summary>
        public string displayName;
        /// <summary>
        /// <para>Defines the style for foreground text on a secondary tile.
        /// </para>
        /// </summary>
        public TileForegroundText foregroundText;
        /// <summary>
        /// <para>Uri to logo, shown for secondary tile on lock screen.
        /// </para>
        /// </summary>
        public string lockScreenBadgeLogo;
        /// <summary>
        /// <para>Whether to show secondary tile on lock screen.
        /// </para>
        /// </summary>
        public bool lockScreenDisplayBadgeAndTileText;
        /// <summary>
        /// <para>Phonetic name for secondary tile.
        /// </para>
        /// </summary>
        public string phoneticName;
        /// <summary>
        /// <para>Defines whether secondary tile is copied to another device when application is installed by the same users account.
        /// </para>
        /// </summary>
        public bool roamingEnabled;
        /// <summary>
        /// <para>Defines whether the displayName should be shown on a medium secondary tile.
        /// </para>
        /// </summary>
        public bool showNameOnSquare150x150Logo;
        /// <summary>
        /// <para>Defines whether the displayName should be shown on a large secondary tile.
        /// </para>
        /// </summary>
        public bool showNameOnSquare310x310Logo;
        /// <summary>
        /// <para>Defines whether the displayName should be shown on a wide secondary tile.
        /// </para>
        /// </summary>
        public bool showNameOnWide310x150Logo;
        /// <summary>
        /// <para>Uri to the logo for medium size tile.</para>
        /// </summary>
        public string square150x150Logo;
        /// <summary>
        /// <para>Uri to the logo shown on tile
        /// </para>
        /// </summary>
        public string square30x30Logo;
        /// <summary>
        /// <para>Uri to the logo for large size tile.
        /// </para>
        /// </summary>
        public string square310x310Logo;
        /// <summary>
        /// <para>Uri to the logo for small size tile.
        /// </para>
        /// </summary>
        public string square70x70Logo;
        /// <summary>
        /// <para>Unique identifier within application for a secondary tile.
        /// </para>
        /// </summary>
        public string tileId;
        /// <summary>
        /// <para>Uri to the logo for wide tile.</para>
        /// </summary>
        public string wide310x150Logo;
        /// <summary>
        /// <para>Constructor for SecondaryTileData, sets default values for all members.</para>
        /// </summary>
        /// <param name="id">Unique identifier for secondary tile.</param>
        /// <param name="displayName">A display name for a tile.</param>
        public SecondaryTileData(string id, string displayName)
        {
            this.arguments = "";
            this.background = new Color32(0, 0, 0, 0);
            this.backgroundColorSet = false;
            this.displayName = displayName;
            this.foregroundText = TileForegroundText.Default;
            this.lockScreenBadgeLogo = "";
            this.lockScreenDisplayBadgeAndTileText = false;
            this.phoneticName = "";
            this.roamingEnabled = true;
            this.showNameOnSquare150x150Logo = true;
            this.showNameOnSquare310x310Logo = false;
            this.showNameOnWide310x150Logo = false;
            this.square150x150Logo = "";
            this.square30x30Logo = "";
            this.square310x310Logo = "";
            this.square70x70Logo = "";
            this.tileId = id;
            this.wide310x150Logo = "";
        }

        /// <summary>
        /// <para>Defines background color for secondary tile.
        /// </para>
        /// </summary>
        public Color32 backgroundColor
        {
            get => 
                this.background;
            set
            {
                this.background = value;
                this.backgroundColorSet = true;
            }
        }
    }
}


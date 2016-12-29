namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>AssetPostprocessor lets you hook into the import pipeline and run scripts prior or after importing assets.</para>
    /// </summary>
    public class AssetPostprocessor
    {
        private string m_PathName;

        /// <summary>
        /// <para>Override the order in which importers are processed.</para>
        /// </summary>
        public virtual int GetPostprocessOrder() => 
            0;

        /// <summary>
        /// <para>Returns the version of the asset postprocessor.</para>
        /// </summary>
        public virtual uint GetVersion() => 
            0;

        /// <summary>
        /// <para>Logs an import error message to the console.</para>
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="context"></param>
        [ExcludeFromDocs]
        public void LogError(string warning)
        {
            Object context = null;
            this.LogError(warning, context);
        }

        /// <summary>
        /// <para>Logs an import error message to the console.</para>
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="context"></param>
        public void LogError(string warning, [DefaultValue("null")] Object context)
        {
            Debug.LogError(warning, context);
        }

        /// <summary>
        /// <para>Logs an import warning to the console.</para>
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="context"></param>
        [ExcludeFromDocs]
        public void LogWarning(string warning)
        {
            Object context = null;
            this.LogWarning(warning, context);
        }

        /// <summary>
        /// <para>Logs an import warning to the console.</para>
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="context"></param>
        public void LogWarning(string warning, [DefaultValue("null")] Object context)
        {
            Debug.LogWarning(warning, context);
        }

        /// <summary>
        /// <para>Reference to the asset importer.</para>
        /// </summary>
        public AssetImporter assetImporter =>
            AssetImporter.GetAtPath(this.assetPath);

        /// <summary>
        /// <para>The path name of the asset being imported.</para>
        /// </summary>
        public string assetPath
        {
            get => 
                this.m_PathName;
            set
            {
                this.m_PathName = value;
            }
        }

        [Obsolete("To set or get the preview, call EditorUtility.SetAssetPreview or AssetPreview.GetAssetPreview instead", true)]
        public Texture2D preview
        {
            get => 
                null;
            set
            {
            }
        }
    }
}


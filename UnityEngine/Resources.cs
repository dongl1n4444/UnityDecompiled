namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>The Resources class allows you to find and access Objects including assets.</para>
    /// </summary>
    public sealed class Resources
    {
        internal static T[] ConvertObjects<T>(UnityEngine.Object[] rawObjects) where T: UnityEngine.Object
        {
            if (rawObjects == null)
            {
                return null;
            }
            T[] localArray2 = new T[rawObjects.Length];
            for (int i = 0; i < localArray2.Length; i++)
            {
                localArray2[i] = (T) rawObjects[i];
            }
            return localArray2;
        }

        public static T[] FindObjectsOfTypeAll<T>() where T: UnityEngine.Object => 
            ConvertObjects<T>(FindObjectsOfTypeAll(typeof(T)));

        /// <summary>
        /// <para>Returns a list of all objects of Type type.</para>
        /// </summary>
        /// <param name="type">Type of the class to match while searching.</param>
        /// <returns>
        /// <para>An array of objects whose class is type or is derived from type.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument), GeneratedByOldBindingsGenerator]
        public static extern UnityEngine.Object[] FindObjectsOfTypeAll(System.Type type);
        public static T GetBuiltinResource<T>(string path) where T: UnityEngine.Object => 
            ((T) GetBuiltinResource(typeof(T), path));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static extern UnityEngine.Object GetBuiltinResource(System.Type type, string path);
        /// <summary>
        /// <para>Loads an asset stored at path in a Resources folder.</para>
        /// </summary>
        /// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
        /// <param name="systemTypeInstance">Type filter for objects returned.</param>
        public static UnityEngine.Object Load(string path) => 
            Load(path, typeof(UnityEngine.Object));

        public static T Load<T>(string path) where T: UnityEngine.Object => 
            ((T) Load(path, typeof(T)));

        /// <summary>
        /// <para>Loads an asset stored at path in a Resources folder.</para>
        /// </summary>
        /// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
        /// <param name="systemTypeInstance">Type filter for objects returned.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public static extern UnityEngine.Object Load(string path, System.Type systemTypeInstance);
        /// <summary>
        /// <para>Loads all assets in a folder or file at path in a Resources folder.</para>
        /// </summary>
        /// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
        /// <param name="systemTypeInstance">Type filter for objects returned.</param>
        public static UnityEngine.Object[] LoadAll(string path) => 
            LoadAll(path, typeof(UnityEngine.Object));

        public static T[] LoadAll<T>(string path) where T: UnityEngine.Object => 
            ConvertObjects<T>(LoadAll(path, typeof(T)));

        /// <summary>
        /// <para>Loads all assets in a folder or file at path in a Resources folder.</para>
        /// </summary>
        /// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
        /// <param name="systemTypeInstance">Type filter for objects returned.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern UnityEngine.Object[] LoadAll(string path, System.Type systemTypeInstance);
        [Obsolete("Use AssetDatabase.LoadAssetAtPath<T>() instead (UnityUpgradable) -> * [UnityEditor] UnityEditor.AssetDatabase.LoadAssetAtPath<T>(*)", true)]
        public static T LoadAssetAtPath<T>(string assetPath) where T: UnityEngine.Object => 
            null;

        /// <summary>
        /// <para>Returns a resource at an asset path (Editor Only).</para>
        /// </summary>
        /// <param name="assetPath">Pathname of the target asset.</param>
        /// <param name="type">Type filter for objects returned.</param>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument), Obsolete("Use AssetDatabase.LoadAssetAtPath instead (UnityUpgradable) -> * [UnityEditor] UnityEditor.AssetDatabase.LoadAssetAtPath(*)", true)]
        public static UnityEngine.Object LoadAssetAtPath(string assetPath, System.Type type) => 
            null;

        /// <summary>
        /// <para>Asynchronously loads an asset stored at path in a Resources folder.</para>
        /// </summary>
        /// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
        /// <param name="systemTypeInstance">Type filter for objects returned.</param>
        /// <param name="type"></param>
        public static ResourceRequest LoadAsync(string path) => 
            LoadAsync(path, typeof(UnityEngine.Object));

        public static ResourceRequest LoadAsync<T>(string path) where T: UnityEngine.Object => 
            LoadAsync(path, typeof(T));

        /// <summary>
        /// <para>Asynchronously loads an asset stored at path in a Resources folder.</para>
        /// </summary>
        /// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
        /// <param name="systemTypeInstance">Type filter for objects returned.</param>
        /// <param name="type"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern ResourceRequest LoadAsync(string path, System.Type type);
        /// <summary>
        /// <para>Unloads assetToUnload from memory.</para>
        /// </summary>
        /// <param name="assetToUnload"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UnloadAsset(UnityEngine.Object assetToUnload);
        /// <summary>
        /// <para>Unloads assets that are not used.</para>
        /// </summary>
        /// <returns>
        /// <para>Object on which you can yield to wait until the operation completes.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern AsyncOperation UnloadUnusedAssets();
    }
}


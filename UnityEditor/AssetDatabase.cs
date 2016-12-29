namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>An Interface for accessing assets and performing operations on assets.</para>
    /// </summary>
    public sealed class AssetDatabase
    {
        public static  event ImportPackageCallback importPackageCancelled;

        public static  event ImportPackageCallback importPackageCompleted;

        public static  event ImportPackageFailedCallback importPackageFailed;

        public static  event ImportPackageCallback importPackageStarted;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void AddInstanceIDToAssetWithRandomFileId(int instanceIDToAdd, Object assetObject, bool hide);
        /// <summary>
        /// <para>Adds objectToAdd to an existing asset at path.</para>
        /// </summary>
        /// <param name="objectToAdd">Object to add to the existing asset.</param>
        /// <param name="path">Filesystem path to the asset.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void AddObjectToAsset(Object objectToAdd, string path);
        /// <summary>
        /// <para>Adds objectToAdd to an existing asset identified by assetObject.</para>
        /// </summary>
        /// <param name="objectToAdd"></param>
        /// <param name="assetObject"></param>
        public static void AddObjectToAsset(Object objectToAdd, Object assetObject)
        {
            AddObjectToAsset_OBJ_Internal(objectToAdd, assetObject);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void AddObjectToAsset_OBJ_Internal(Object newAsset, Object sameAssetFile);
        /// <summary>
        /// <para>Get the GUID for the asset at path.</para>
        /// </summary>
        /// <param name="path">Filesystem path for the asset.</param>
        /// <returns>
        /// <para>GUID.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string AssetPathToGUID(string path);
        /// <summary>
        /// <para>Removes all labels attached to an asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearLabels(Object obj);
        /// <summary>
        /// <para>Is object an asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Contains(int instanceID);
        /// <summary>
        /// <para>Is object an asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool Contains(Object obj) => 
            Contains(obj.GetInstanceID());

        /// <summary>
        /// <para>Duplicates the asset at path and stores it at newPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path of the source asset.</param>
        /// <param name="newPath">Filesystem path of the new asset to create.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CopyAsset(string path, string newPath);
        /// <summary>
        /// <para>Creates a new asset at path.</para>
        /// </summary>
        /// <param name="asset">Object to use in creating the asset.</param>
        /// <param name="path">Filesystem path for the new asset.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CreateAsset(Object asset, string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CreateAssetFromObjects(Object[] assets, string path);
        /// <summary>
        /// <para>Create a new folder.</para>
        /// </summary>
        /// <param name="parentFolder">The name of the parent folder.</param>
        /// <param name="newFolderName">The name of the new folder.</param>
        /// <returns>
        /// <para>The GUID of the newly created folder.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string CreateFolder(string parentFolder, string newFolderName);
        /// <summary>
        /// <para>Deletes the asset file at path.</para>
        /// </summary>
        /// <param name="path">Filesystem path of the asset to be deleted.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool DeleteAsset(string path);
        /// <summary>
        /// <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="assetPathNames"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="assetPathName"></param>
        public static void ExportPackage(string assetPathName, string fileName)
        {
            ExportPackage(new string[] { assetPathName }, fileName, ExportPackageOptions.Default);
        }

        /// <summary>
        /// <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="assetPathNames"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="assetPathName"></param>
        [ExcludeFromDocs]
        public static void ExportPackage(string[] assetPathNames, string fileName)
        {
            ExportPackageOptions flags = ExportPackageOptions.Default;
            ExportPackage(assetPathNames, fileName, flags);
        }

        /// <summary>
        /// <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="assetPathNames"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="assetPathName"></param>
        public static void ExportPackage(string assetPathName, string fileName, ExportPackageOptions flags)
        {
            ExportPackage(new string[] { assetPathName }, fileName, flags);
        }

        /// <summary>
        /// <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="assetPathNames"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="assetPathName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ExportPackage(string[] assetPathNames, string fileName, [DefaultValue("ExportPackageOptions.Default")] ExportPackageOptions flags);
        /// <summary>
        /// <para>Search the asset database using a search filter string.</para>
        /// </summary>
        /// <param name="filter">The filter string can contain search data for: names, asset labels and types (class names).</param>
        /// <param name="searchInFolders">Specifying one or more folders will limit the searching to these folders and their child folders (and is faster than searching all assets).</param>
        /// <returns>
        /// <para>Array of matching asset GUIDs.</para>
        /// </returns>
        public static string[] FindAssets(string filter) => 
            FindAssets(filter, null);

        private static string[] FindAssets(SearchFilter searchFilter)
        {
            if ((searchFilter.folders != null) && (searchFilter.folders.Length > 0))
            {
                return SearchInFolders(searchFilter);
            }
            return SearchAllAssets(searchFilter);
        }

        /// <summary>
        /// <para>Search the asset database using a search filter string.</para>
        /// </summary>
        /// <param name="filter">The filter string can contain search data for: names, asset labels and types (class names).</param>
        /// <param name="searchInFolders">Specifying one or more folders will limit the searching to these folders and their child folders (and is faster than searching all assets).</param>
        /// <returns>
        /// <para>Array of matching asset GUIDs.</para>
        /// </returns>
        public static string[] FindAssets(string filter, string[] searchInFolders)
        {
            SearchFilter filter2 = new SearchFilter();
            SearchUtility.ParseSearchString(filter, filter2);
            if (searchInFolders != null)
            {
                filter2.folders = searchInFolders;
            }
            return FindAssets(filter2);
        }

        /// <summary>
        /// <para>Creates a new unique path for an asset.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GenerateUniqueAssetPath(string path);
        /// <summary>
        /// <para>Return all the AssetBundle names in the asset database.</para>
        /// </summary>
        /// <returns>
        /// <para>Array of asset bundle names.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetAllAssetBundleNames();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string[] GetAllAssetBundleNamesWithoutVariant();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string[] GetAllAssetBundleVariants();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetAllAssetPaths();
        internal static Dictionary<string, float> GetAllLabels()
        {
            string[] strArray;
            float[] numArray;
            INTERNAL_GetAllLabels(out strArray, out numArray);
            Dictionary<string, float> dictionary = new Dictionary<string, float>(strArray.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                dictionary[strArray[i]] = numArray[i];
            }
            return dictionary;
        }

        /// <summary>
        /// <para>Given an assetBundleName, returns the list of AssetBundles that it depends on.</para>
        /// </summary>
        /// <param name="assetBundleName">The name of the AssetBundle for which dependencies are required.</param>
        /// <param name="recursive">If false, returns only AssetBundles which are direct dependencies of the input; if true, includes all indirect dependencies of the input.</param>
        /// <returns>
        /// <para>The names of all AssetBundles that the input depends on.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetAssetBundleDependencies(string assetBundleName, bool recursive);
        [Obsolete("Method GetAssetBundleNames has been deprecated. Use GetAllAssetBundleNames instead.")]
        public string[] GetAssetBundleNames() => 
            GetAllAssetBundleNames();

        /// <summary>
        /// <para>Returns the hash of all the dependencies of an asset.</para>
        /// </summary>
        /// <param name="path">Path to the asset.</param>
        /// <returns>
        /// <para>Aggregate hash.</para>
        /// </returns>
        public static Hash128 GetAssetDependencyHash(string path)
        {
            Hash128 hash;
            INTERNAL_CALL_GetAssetDependencyHash(path, out hash);
            return hash;
        }

        /// <summary>
        /// <para>Returns the path name relative to the project folder where the asset is stored.</para>
        /// </summary>
        /// <param name="assetObject"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetAssetOrScenePath(Object assetObject);
        /// <summary>
        /// <para>Returns the path name relative to the project folder where the asset is stored.</para>
        /// </summary>
        /// <param name="instanceID">The instance ID of the asset.</param>
        /// <param name="assetObject">A reference to the asset.</param>
        /// <returns>
        /// <para>The asset path name, or null, or an empty string if the asset does not exist.</para>
        /// </returns>
        public static string GetAssetPath(int instanceID) => 
            GetAssetPathFromInstanceID(instanceID);

        /// <summary>
        /// <para>Returns the path name relative to the project folder where the asset is stored.</para>
        /// </summary>
        /// <param name="instanceID">The instance ID of the asset.</param>
        /// <param name="assetObject">A reference to the asset.</param>
        /// <returns>
        /// <para>The asset path name, or null, or an empty string if the asset does not exist.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetAssetPath(Object assetObject);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetAssetPathFromInstanceID(int instanceID);
        /// <summary>
        /// <para>Gets the path to the asset file associated with a text .meta file.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string GetAssetPathFromTextMetaFilePath(string path);
        /// <summary>
        /// <para>Get the paths of the assets which have been marked with the given assetBundle name.</para>
        /// </summary>
        /// <param name="assetBundleName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetAssetPathsFromAssetBundle(string assetBundleName);
        /// <summary>
        /// <para>Get the asset paths from the given assetBundle name and asset name.</para>
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetAssetPathsFromAssetBundleAndAssetName(string assetBundleName, string assetName);
        public static T GetBuiltinExtraResource<T>(string path) where T: Object => 
            ((T) GetBuiltinExtraResource(typeof(T), path));

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static extern Object GetBuiltinExtraResource(Type type, string path);
        /// <summary>
        /// <para>Retrieves an icon for the asset at the given asset path.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Texture GetCachedIcon(string path);
        /// <summary>
        /// <para>Given a pathName, returns the list of all assets that it depends on.</para>
        /// </summary>
        /// <param name="pathName">The path to the asset for which dependencies are required.</param>
        /// <param name="recursive">If false, return only assets which are direct dependencies of the input; if true, include all indirect dependencies of the input. Defaults to true.</param>
        /// <returns>
        /// <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string pathName) => 
            GetDependencies(pathName, true);

        /// <summary>
        /// <para>Given an array of pathNames, returns the list of all assets that the input depend on.</para>
        /// </summary>
        /// <param name="pathNames">The path to the assets for which dependencies are required.</param>
        /// <param name="recursive">If false, return only assets which are direct dependencies of the input; if true, include all indirect dependencies of the input. Defaults to true.</param>
        /// <returns>
        /// <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string[] pathNames) => 
            GetDependencies(pathNames, true);

        /// <summary>
        /// <para>Given a pathName, returns the list of all assets that it depends on.</para>
        /// </summary>
        /// <param name="pathName">The path to the asset for which dependencies are required.</param>
        /// <param name="recursive">If false, return only assets which are direct dependencies of the input; if true, include all indirect dependencies of the input. Defaults to true.</param>
        /// <returns>
        /// <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string pathName, bool recursive) => 
            GetDependencies(new string[] { pathName }, recursive);

        /// <summary>
        /// <para>Given an array of pathNames, returns the list of all assets that the input depend on.</para>
        /// </summary>
        /// <param name="pathNames">The path to the assets for which dependencies are required.</param>
        /// <param name="recursive">If false, return only assets which are direct dependencies of the input; if true, include all indirect dependencies of the input. Defaults to true.</param>
        /// <returns>
        /// <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetDependencies(string[] pathNames, bool recursive);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetInstanceIDFromGUID(string guid);
        /// <summary>
        /// <para>Returns all labels attached to a given asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetLabels(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetMainAssetInstanceID(string assetPath);
        /// <summary>
        /// <para>Returns the type of the main asset object at assetPath.</para>
        /// </summary>
        /// <param name="assetPath">Filesystem path of the asset to load.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Type GetMainAssetTypeAtPath(string assetPath);
        /// <summary>
        /// <para>Given an absolute path to a directory, this method will return an array of all it's subdirectories.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetSubFolders(string path);
        /// <summary>
        /// <para>Gets the path to the text .meta file associated with an asset.</para>
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        /// <returns>
        /// <para>The path to the .meta text file or empty string if the file does not exist.</para>
        /// </returns>
        [Obsolete("GetTextMetaDataPathFromAssetPath has been renamed to GetTextMetaFilePathFromAssetPath (UnityUpgradable) -> GetTextMetaFilePathFromAssetPath(*)")]
        public static string GetTextMetaDataPathFromAssetPath(string path) => 
            null;

        /// <summary>
        /// <para>Gets the path to the text .meta file associated with an asset.</para>
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        /// <returns>
        /// <para>The path to the .meta text file or empty string if the file does not exist.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern string GetTextMetaFilePathFromAssetPath(string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetUniquePathNameAtSelectedPath(string fileName);
        /// <summary>
        /// <para>Return all the unused assetBundle names in the asset database.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string[] GetUnusedAssetBundleNames();
        /// <summary>
        /// <para>Translate a GUID to its current asset path.</para>
        /// </summary>
        /// <param name="guid"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GUIDToAssetPath(string guid);
        /// <summary>
        /// <para>Import asset at path.</para>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        [ExcludeFromDocs]
        public static void ImportAsset(string path)
        {
            ImportAssetOptions options = ImportAssetOptions.Default;
            ImportAsset(path, options);
        }

        /// <summary>
        /// <para>Import asset at path.</para>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ImportAsset(string path, [DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);
        /// <summary>
        /// <para>Imports package at packagePath into the current project.</para>
        /// </summary>
        /// <param name="packagePath"></param>
        /// <param name="interactive"></param>
        public static void ImportPackage(string packagePath, bool interactive)
        {
            string str;
            bool flag;
            if (string.IsNullOrEmpty(packagePath))
            {
                throw new ArgumentException("Path can not be empty or null", "packagePath");
            }
            ImportPackageItem[] items = PackageUtility.ExtractAndPrepareAssetList(packagePath, out str, out flag);
            if (items != null)
            {
                if (interactive)
                {
                    PackageImport.ShowImportPackage(packagePath, items, str, flag);
                }
                else
                {
                    PackageUtility.ImportPackageAssets(Path.GetFileNameWithoutExtension(packagePath), items, false);
                }
            }
        }

        internal static void ImportPackageImmediately(string packagePath)
        {
            string str;
            bool flag;
            if (string.IsNullOrEmpty(packagePath))
            {
                throw new ArgumentException("Path can not be empty or null", "packagePath");
            }
            ImportPackageItem[] items = PackageUtility.ExtractAndPrepareAssetList(packagePath, out str, out flag);
            if ((items != null) && (items.Length != 0))
            {
                PackageUtility.ImportPackageAssetsImmediately(Path.GetFileNameWithoutExtension(packagePath), items, false);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetAssetDependencyHash(string path, out Hash128 value);
        [RequiredByNativeCode]
        private static void Internal_CallImportPackageCancelled(string packageName)
        {
            if (importPackageCancelled != null)
            {
                importPackageCancelled(packageName);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_CallImportPackageCompleted(string packageName)
        {
            if (importPackageCompleted != null)
            {
                importPackageCompleted(packageName);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_CallImportPackageFailed(string packageName, string errorMessage)
        {
            if (importPackageFailed != null)
            {
                importPackageFailed(packageName, errorMessage);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_CallImportPackageStarted(string packageName)
        {
            if (importPackageStarted != null)
            {
                importPackageStarted(packageName);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_GetAllLabels(out string[] labels, out float[] scores);
        /// <summary>
        /// <para>Is asset a foreign asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsForeignAsset(int instanceID);
        /// <summary>
        /// <para>Is asset a foreign asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsForeignAsset(Object obj) => 
            IsForeignAsset(obj.GetInstanceID());

        /// <summary>
        /// <para>Is asset a main asset in the project window?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsMainAsset(int instanceID);
        /// <summary>
        /// <para>Is asset a main asset in the project window?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsMainAsset(Object obj) => 
            IsMainAsset(obj.GetInstanceID());

        /// <summary>
        /// <para>Returns true if the main asset object at assetPath is loaded in memory.</para>
        /// </summary>
        /// <param name="assetPath">Filesystem path of the asset to load.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsMainAssetAtPathLoaded(string assetPath);
        public static bool IsMetaFileOpenForEdit(Object assetObject)
        {
            string str;
            return IsMetaFileOpenForEdit(assetObject, out str);
        }

        public static bool IsMetaFileOpenForEdit(Object assetObject, out string message) => 
            IsOpenForEdit(GetTextMetaFilePathFromAssetPath(GetAssetOrScenePath(assetObject)), out message);

        /// <summary>
        /// <para>Is asset a native asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsNativeAsset(int instanceID);
        /// <summary>
        /// <para>Is asset a native asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsNativeAsset(Object obj) => 
            IsNativeAsset(obj.GetInstanceID());

        /// <summary>
        /// <para>Use IsOpenForEdit to determine if the asset is open for edit by the version control.</para>
        /// </summary>
        /// <param name="assetPath">Is the path to the asset on disk relative to project folder.</param>
        /// <param name="message">Used to give reason for not open.</param>
        /// <param name="assetObject"></param>
        /// <param name="assetOrMetaFilePath"></param>
        /// <returns>
        /// <para>True is the asset can be edited.</para>
        /// </returns>
        public static bool IsOpenForEdit(string assetOrMetaFilePath)
        {
            string str;
            return IsOpenForEdit(assetOrMetaFilePath, out str);
        }

        /// <summary>
        /// <para>Use IsOpenForEdit to determine if the asset is open for edit by the version control.</para>
        /// </summary>
        /// <param name="assetPath">Is the path to the asset on disk relative to project folder.</param>
        /// <param name="message">Used to give reason for not open.</param>
        /// <param name="assetObject"></param>
        /// <param name="assetOrMetaFilePath"></param>
        /// <returns>
        /// <para>True is the asset can be edited.</para>
        /// </returns>
        public static bool IsOpenForEdit(Object assetObject) => 
            IsOpenForEdit(GetAssetOrScenePath(assetObject));

        public static bool IsOpenForEdit(string assetOrMetaFilePath, out string message) => 
            AssetModificationProcessorInternal.IsOpenForEdit(assetOrMetaFilePath, out message);

        public static bool IsOpenForEdit(Object assetObject, out string message) => 
            IsOpenForEdit(GetAssetOrScenePath(assetObject), out message);

        /// <summary>
        /// <para>Does the asset form part of another asset?</para>
        /// </summary>
        /// <param name="obj">The asset Object to query.</param>
        /// <param name="instanceID">Instance ID of the asset Object to query.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsSubAsset(int instanceID);
        /// <summary>
        /// <para>Does the asset form part of another asset?</para>
        /// </summary>
        /// <param name="obj">The asset Object to query.</param>
        /// <param name="instanceID">Instance ID of the asset Object to query.</param>
        public static bool IsSubAsset(Object obj) => 
            IsSubAsset(obj.GetInstanceID());

        /// <summary>
        /// <para>Given an absolute path to a folder, returns true if it exists, false otherwise.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsValidFolder(string path);
        /// <summary>
        /// <para>Returns all asset representations at assetPath.</para>
        /// </summary>
        /// <param name="assetPath"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Object[] LoadAllAssetRepresentationsAtPath(string assetPath);
        /// <summary>
        /// <para>Returns an array of all asset objects at assetPath.</para>
        /// </summary>
        /// <param name="assetPath">Filesystem path to the asset.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Object[] LoadAllAssetsAtPath(string assetPath);
        public static T LoadAssetAtPath<T>(string assetPath) where T: Object => 
            ((T) LoadAssetAtPath(assetPath, typeof(T)));

        /// <summary>
        /// <para>Returns the first asset object of type type at given path assetPath.</para>
        /// </summary>
        /// <param name="assetPath">Path of the asset to load.</param>
        /// <param name="type">Data type of the asset.</param>
        /// <returns>
        /// <para>The asset matching the parameters.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public static extern Object LoadAssetAtPath(string assetPath, Type type);
        /// <summary>
        /// <para>Returns the main asset object at assetPath.</para>
        /// </summary>
        /// <param name="assetPath">Filesystem path of the asset to load.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Object LoadMainAssetAtPath(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string[] MatchLabelsPartial(Object obj, string partial);
        /// <summary>
        /// <para>Move an asset file from one folder to another.</para>
        /// </summary>
        /// <param name="oldPath">The path where the asset currently resides.</param>
        /// <param name="newPath">The path which the asset should be moved to.</param>
        /// <returns>
        /// <para>An empty string if the asset has been successfully moved, otherwise an error message.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string MoveAsset(string oldPath, string newPath);
        /// <summary>
        /// <para>Moves the asset at path to the trash.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool MoveAssetToTrash(string path);
        /// <summary>
        /// <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="target"></param>
        [ExcludeFromDocs]
        public static bool OpenAsset(int instanceID)
        {
            int lineNumber = -1;
            return OpenAsset(instanceID, lineNumber);
        }

        /// <summary>
        /// <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="target"></param>
        [ExcludeFromDocs]
        public static bool OpenAsset(Object target)
        {
            int lineNumber = -1;
            return OpenAsset(target, lineNumber);
        }

        /// <summary>
        /// <para>Opens the asset(s) with associated application(s).</para>
        /// </summary>
        /// <param name="objects"></param>
        public static bool OpenAsset(Object[] objects)
        {
            bool flag = true;
            foreach (Object obj2 in objects)
            {
                if (!OpenAsset(obj2))
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="target"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool OpenAsset(int instanceID, [DefaultValue("-1")] int lineNumber);
        /// <summary>
        /// <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(Object target, [DefaultValue("-1")] int lineNumber) => 
            ((target != null) && OpenAsset(target.GetInstanceID(), lineNumber));

        [ExcludeFromDocs]
        public static void Refresh()
        {
            ImportAssetOptions options = ImportAssetOptions.Default;
            Refresh(options);
        }

        /// <summary>
        /// <para>Import any changed assets.</para>
        /// </summary>
        /// <param name="options"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Refresh([DefaultValue("ImportAssetOptions.Default")] ImportAssetOptions options);
        [Obsolete("Please use AssetDatabase.Refresh instead", true)]
        public static void RefreshDelayed()
        {
        }

        [Obsolete("Please use AssetDatabase.Refresh instead", true)]
        public static void RefreshDelayed(ImportAssetOptions options)
        {
        }

        /// <summary>
        /// <para>Remove the assetBundle name from the asset database. The forceRemove flag is used to indicate if you want to remove it even it's in use.</para>
        /// </summary>
        /// <param name="assetBundleName">The assetBundle name you want to remove.</param>
        /// <param name="forceRemove">Flag to indicate if you want to remove the assetBundle name even it's in use.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool RemoveAssetBundleName(string assetBundleName, bool forceRemove);
        /// <summary>
        /// <para>Remove all the unused assetBundle names in the asset database.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RemoveUnusedAssetBundleNames();
        /// <summary>
        /// <para>Rename an asset file.</para>
        /// </summary>
        /// <param name="pathName">The path where the asset currently resides.</param>
        /// <param name="newName">The new name which should be given to the asset.</param>
        /// <returns>
        /// <para>An empty string, if the asset has been successfully renamed, otherwise an error message.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string RenameAsset(string pathName, string newName);
        /// <summary>
        /// <para>Writes all unsaved asset changes to disk.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SaveAssets();
        private static string[] SearchAllAssets(SearchFilter searchFilter)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            property.SetSearchFilter(searchFilter);
            property.Reset();
            List<string> list = new List<string>();
            while (property.Next(null))
            {
                list.Add(property.guid);
            }
            return list.ToArray();
        }

        private static string[] SearchInFolders(SearchFilter searchFilter)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            List<string> list = new List<string>();
            foreach (string str in searchFilter.folders)
            {
                property.SetSearchFilter(new SearchFilter());
                int mainAssetInstanceID = GetMainAssetInstanceID(str);
                if (property.Find(mainAssetInstanceID, null))
                {
                    property.SetSearchFilter(searchFilter);
                    int depth = property.depth;
                    int[] expanded = null;
                    while (property.NextWithDepthCheck(expanded, depth + 1))
                    {
                        list.Add(property.guid);
                    }
                }
                else
                {
                    Debug.LogWarning("AssetDatabase.FindAssets: Folder not found: '" + str + "'");
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// <para>Replaces that list of labels on an asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="labels"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetLabels(Object obj, string[] labels);
        /// <summary>
        /// <para>Begin Asset importing. This lets you group several asset imports together into one larger import.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void StartAssetEditing();
        /// <summary>
        /// <para>Stop Asset importing. This lets you group several asset imports together into one larger import.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void StopAssetEditing();
        /// <summary>
        /// <para>Checks if an asset file can be moved from one folder to another. (Without actually moving the file).</para>
        /// </summary>
        /// <param name="oldPath">The path where the asset currently resides.</param>
        /// <param name="newPath">The path which the asset should be moved to.</param>
        /// <returns>
        /// <para>An empty string if the asset can be moved, otherwise an error message.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string ValidateMoveAsset(string oldPath, string newPath);
        /// <summary>
        /// <para>Writes the import settings to disk.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool WriteImportSettingsIfDirty(string path);

        internal static bool isLocked { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Delegate to be called from AssetDatabase.ImportPackage callbacks. packageName is the name of the package that raised the callback.</para>
        /// </summary>
        /// <param name="packageName"></param>
        public delegate void ImportPackageCallback(string packageName);

        /// <summary>
        /// <para>Delegate to be called from AssetDatabase.ImportPackage callbacks. packageName is the name of the package that raised the callback. errorMessage is the reason for the failure.</para>
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="errorMessage"></param>
        public delegate void ImportPackageFailedCallback(string packageName, string errorMessage);
    }
}


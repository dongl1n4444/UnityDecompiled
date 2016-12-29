namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>This class provides acces to the version control API.</para>
    /// </summary>
    public sealed class Provider
    {
        /// <summary>
        /// <para>Adds an assets or list of assets to version control.</para>
        /// </summary>
        /// <param name="assets">List of assets to add to version control system.</param>
        /// <param name="recursive">Set this true if adding should be done recursively into subfolders.</param>
        /// <param name="asset">Single asset to add to version control system.</param>
        public static Task Add(Asset asset, bool recursive)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Add(assets, recursive);
        }

        /// <summary>
        /// <para>Adds an assets or list of assets to version control.</para>
        /// </summary>
        /// <param name="assets">List of assets to add to version control system.</param>
        /// <param name="recursive">Set this true if adding should be done recursively into subfolders.</param>
        /// <param name="asset">Single asset to add to version control system.</param>
        public static Task Add(AssetList assets, bool recursive) => 
            Internal_Add(assets.ToArray(), recursive);

        /// <summary>
        /// <para>Given a list of assets this function returns true if Add is a valid task to perform.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        public static bool AddIsValid(AssetList assets) => 
            Internal_AddIsValid(assets.ToArray());

        internal static Asset CacheStatus(string assetPath) => 
            Internal_CacheStatus(assetPath);

        /// <summary>
        /// <para>Given a changeset only containing the changeset ID, this will start a task for quering the description of the changeset.</para>
        /// </summary>
        /// <param name="changeset">Changeset to query description of.</param>
        public static Task ChangeSetDescription(ChangeSet changeset) => 
            Internal_ChangeSetDescription(changeset);

        /// <summary>
        /// <para>Move an asset or list of assets from their current changeset to a new changeset.</para>
        /// </summary>
        /// <param name="assets">List of asset to move to changeset.</param>
        /// <param name="changeset">Changeset to move asset to.</param>
        /// <param name="asset">Asset to move to changeset.</param>
        /// <param name="changesetID">ChangesetID to move asset to.</param>
        public static Task ChangeSetMove(Asset asset, string changesetID)
        {
            ChangeSet target = new ChangeSet("", changesetID);
            Asset[] assets = new Asset[] { asset };
            return Internal_ChangeSetMove(assets, target);
        }

        /// <summary>
        /// <para>Move an asset or list of assets from their current changeset to a new changeset.</para>
        /// </summary>
        /// <param name="assets">List of asset to move to changeset.</param>
        /// <param name="changeset">Changeset to move asset to.</param>
        /// <param name="asset">Asset to move to changeset.</param>
        /// <param name="changesetID">ChangesetID to move asset to.</param>
        public static Task ChangeSetMove(Asset asset, ChangeSet changeset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_ChangeSetMove(assets, changeset);
        }

        /// <summary>
        /// <para>Move an asset or list of assets from their current changeset to a new changeset.</para>
        /// </summary>
        /// <param name="assets">List of asset to move to changeset.</param>
        /// <param name="changeset">Changeset to move asset to.</param>
        /// <param name="asset">Asset to move to changeset.</param>
        /// <param name="changesetID">ChangesetID to move asset to.</param>
        public static Task ChangeSetMove(AssetList assets, string changesetID)
        {
            ChangeSet target = new ChangeSet("", changesetID);
            return Internal_ChangeSetMove(assets.ToArray(), target);
        }

        /// <summary>
        /// <para>Move an asset or list of assets from their current changeset to a new changeset.</para>
        /// </summary>
        /// <param name="assets">List of asset to move to changeset.</param>
        /// <param name="changeset">Changeset to move asset to.</param>
        /// <param name="asset">Asset to move to changeset.</param>
        /// <param name="changesetID">ChangesetID to move asset to.</param>
        public static Task ChangeSetMove(AssetList assets, ChangeSet changeset) => 
            Internal_ChangeSetMove(assets.ToArray(), changeset);

        /// <summary>
        /// <para>Get a list of pending changesets owned by the current user.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Task ChangeSets();
        /// <summary>
        /// <para>Retrieves the list of assets belonging to a changeset.</para>
        /// </summary>
        /// <param name="changeset">Changeset to query for assets.</param>
        /// <param name="changesetID">ChangesetID to query for assets.</param>
        public static Task ChangeSetStatus(string changesetID)
        {
            ChangeSet changeset = new ChangeSet("", changesetID);
            return Internal_ChangeSetStatus(changeset);
        }

        /// <summary>
        /// <para>Retrieves the list of assets belonging to a changeset.</para>
        /// </summary>
        /// <param name="changeset">Changeset to query for assets.</param>
        /// <param name="changesetID">ChangesetID to query for assets.</param>
        public static Task ChangeSetStatus(ChangeSet changeset) => 
            Internal_ChangeSetStatus(changeset);

        /// <summary>
        /// <para>Checkout an asset or list of asset from the version control system.</para>
        /// </summary>
        /// <param name="assets">List of assets to checkout.</param>
        /// <param name="mode">Tell the Provider to checkout the asset, the .meta file or both.</param>
        /// <param name="asset">Asset to checkout.</param>
        public static Task Checkout(string asset, CheckoutMode mode)
        {
            string[] assets = new string[] { asset };
            return Internal_CheckoutStrings(assets, mode);
        }

        /// <summary>
        /// <para>Checkout an asset or list of asset from the version control system.</para>
        /// </summary>
        /// <param name="assets">List of assets to checkout.</param>
        /// <param name="mode">Tell the Provider to checkout the asset, the .meta file or both.</param>
        /// <param name="asset">Asset to checkout.</param>
        public static Task Checkout(Asset asset, CheckoutMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Checkout(assets, mode);
        }

        /// <summary>
        /// <para>Checkout an asset or list of asset from the version control system.</para>
        /// </summary>
        /// <param name="assets">List of assets to checkout.</param>
        /// <param name="mode">Tell the Provider to checkout the asset, the .meta file or both.</param>
        /// <param name="asset">Asset to checkout.</param>
        public static Task Checkout(AssetList assets, CheckoutMode mode) => 
            Internal_Checkout(assets.ToArray(), mode);

        /// <summary>
        /// <para>Checkout an asset or list of asset from the version control system.</para>
        /// </summary>
        /// <param name="assets">List of assets to checkout.</param>
        /// <param name="mode">Tell the Provider to checkout the asset, the .meta file or both.</param>
        /// <param name="asset">Asset to checkout.</param>
        public static Task Checkout(string[] assets, CheckoutMode mode) => 
            Internal_CheckoutStrings(assets, mode);

        /// <summary>
        /// <para>Checkout an asset or list of asset from the version control system.</para>
        /// </summary>
        /// <param name="assets">List of assets to checkout.</param>
        /// <param name="mode">Tell the Provider to checkout the asset, the .meta file or both.</param>
        /// <param name="asset">Asset to checkout.</param>
        public static Task Checkout(Object[] assets, CheckoutMode mode)
        {
            AssetList list = new AssetList();
            foreach (Object obj2 in assets)
            {
                Asset assetByPath = GetAssetByPath(AssetDatabase.GetAssetPath(obj2));
                list.Add(assetByPath);
            }
            return Internal_Checkout(list.ToArray(), mode);
        }

        /// <summary>
        /// <para>Checkout an asset or list of asset from the version control system.</para>
        /// </summary>
        /// <param name="assets">List of assets to checkout.</param>
        /// <param name="mode">Tell the Provider to checkout the asset, the .meta file or both.</param>
        /// <param name="asset">Asset to checkout.</param>
        public static Task Checkout(Object asset, CheckoutMode mode)
        {
            Asset assetByPath = GetAssetByPath(AssetDatabase.GetAssetPath(asset));
            Asset[] assets = new Asset[] { assetByPath };
            return Internal_Checkout(assets, mode);
        }

        /// <summary>
        /// <para>Given an asset or a  list of assets this function returns true if Checkout is a valid task to perform.</para>
        /// </summary>
        /// <param name="assets">List of assets.</param>
        /// <param name="asset">Single asset.</param>
        public static bool CheckoutIsValid(Asset asset) => 
            CheckoutIsValid(asset, CheckoutMode.Exact);

        /// <summary>
        /// <para>Given an asset or a  list of assets this function returns true if Checkout is a valid task to perform.</para>
        /// </summary>
        /// <param name="assets">List of assets.</param>
        /// <param name="asset">Single asset.</param>
        public static bool CheckoutIsValid(AssetList assets) => 
            CheckoutIsValid(assets, CheckoutMode.Exact);

        public static bool CheckoutIsValid(Asset asset, CheckoutMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_CheckoutIsValid(assets, mode);
        }

        public static bool CheckoutIsValid(AssetList assets, CheckoutMode mode) => 
            Internal_CheckoutIsValid(assets.ToArray(), mode);

        /// <summary>
        /// <para>This will invalidate the cached state information for all assets.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearCache();
        /// <summary>
        /// <para>This will statt a task for deleting an asset or assets both from disk and from version control system.</para>
        /// </summary>
        /// <param name="assetProjectPath">Project path of asset.</param>
        /// <param name="assets">List of assets to delete.</param>
        /// <param name="asset">Asset to delete.</param>
        public static Task Delete(string assetProjectPath) => 
            Internal_DeleteAtProjectPath(assetProjectPath);

        /// <summary>
        /// <para>This will statt a task for deleting an asset or assets both from disk and from version control system.</para>
        /// </summary>
        /// <param name="assetProjectPath">Project path of asset.</param>
        /// <param name="assets">List of assets to delete.</param>
        /// <param name="asset">Asset to delete.</param>
        public static Task Delete(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Delete(assets);
        }

        /// <summary>
        /// <para>This will statt a task for deleting an asset or assets both from disk and from version control system.</para>
        /// </summary>
        /// <param name="assetProjectPath">Project path of asset.</param>
        /// <param name="assets">List of assets to delete.</param>
        /// <param name="asset">Asset to delete.</param>
        public static Task Delete(AssetList assets) => 
            Internal_Delete(assets.ToArray());

        /// <summary>
        /// <para>Starts a task that will attempt to delete the given changeset.</para>
        /// </summary>
        /// <param name="changesets">List of changetsets.</param>
        public static Task DeleteChangeSets(UnityEditor.VersionControl.ChangeSets changesets) => 
            Internal_DeleteChangeSets(changesets.ToArray());

        /// <summary>
        /// <para>Test if deleting a changeset is a valid task to perform.</para>
        /// </summary>
        /// <param name="changesets">Changeset to test.</param>
        public static bool DeleteChangeSetsIsValid(UnityEditor.VersionControl.ChangeSets changesets) => 
            Internal_DeleteChangeSetsIsValid(changesets.ToArray());

        /// <summary>
        /// <para>Starts a task for showing a diff of the given assest versus their head revision.</para>
        /// </summary>
        /// <param name="assets">List of assets.</param>
        /// <param name="includingMetaFiles">Whether or not to include .meta.</param>
        public static Task DiffHead(AssetList assets, bool includingMetaFiles) => 
            Internal_DiffHead(assets.ToArray(), includingMetaFiles);

        /// <summary>
        /// <para>Return true is starting a Diff task is a valid operation.</para>
        /// </summary>
        /// <param name="assets">List of assets.</param>
        public static bool DiffIsValid(AssetList assets) => 
            Internal_DiffIsValid(assets.ToArray());

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GenerateID();
        /// <summary>
        /// <para>Returns the configuration fields for the currently active version control plugin.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern ConfigField[] GetActiveConfigFields();
        /// <summary>
        /// <para>Gets the currently user selected verson control plugin.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Plugin GetActivePlugin();
        /// <summary>
        /// <para>Returns version control information about an asset.</para>
        /// </summary>
        /// <param name="guid">GUID of asset.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Asset GetAssetByGUID(string guid);
        /// <summary>
        /// <para>Returns version control information about an asset.</para>
        /// </summary>
        /// <param name="unityPath">Path to asset.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Asset GetAssetByPath(string unityPath);
        /// <summary>
        /// <para>Return version control information about the currently selected assets.</para>
        /// </summary>
        public static AssetList GetAssetListFromSelection()
        {
            AssetList list = new AssetList();
            Asset[] assetArray = Internal_GetAssetArrayFromSelection();
            foreach (Asset asset in assetArray)
            {
                list.Add(asset);
            }
            return list;
        }

        internal static Rect GetAtlasRectForState(int state)
        {
            Rect rect;
            INTERNAL_CALL_GetAtlasRectForState(state, out rect);
            return rect;
        }

        /// <summary>
        /// <para>Start a task for getting the latest version of an asset from the version control server.</para>
        /// </summary>
        /// <param name="assets">List of assets to update.</param>
        /// <param name="asset">Asset to update.</param>
        public static Task GetLatest(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_GetLatest(assets);
        }

        /// <summary>
        /// <para>Start a task for getting the latest version of an asset from the version control server.</para>
        /// </summary>
        /// <param name="assets">List of assets to update.</param>
        /// <param name="asset">Asset to update.</param>
        public static Task GetLatest(AssetList assets) => 
            Internal_GetLatest(assets.ToArray());

        /// <summary>
        /// <para>Returns true if getting the latest version of an asset is a valid operation.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        /// <param name="asset">Asset to test.</param>
        public static bool GetLatestIsValid(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_GetLatestIsValid(assets);
        }

        /// <summary>
        /// <para>Returns true if getting the latest version of an asset is a valid operation.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        /// <param name="asset">Asset to test.</param>
        public static bool GetLatestIsValid(AssetList assets) => 
            Internal_GetLatestIsValid(assets.ToArray());

        /// <summary>
        /// <para>Start a task for quering the version control server for incoming changes.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Task Incoming();
        /// <summary>
        /// <para>Given an incoming changeset this will start a task to query the version control server for which assets are part of the changeset.</para>
        /// </summary>
        /// <param name="changeset">Incoming changeset.</param>
        /// <param name="changesetID">Incoming changesetid.</param>
        public static Task IncomingChangeSetAssets(string changesetID)
        {
            ChangeSet changeset = new ChangeSet("", changesetID);
            return Internal_IncomingChangeSetAssets(changeset);
        }

        /// <summary>
        /// <para>Given an incoming changeset this will start a task to query the version control server for which assets are part of the changeset.</para>
        /// </summary>
        /// <param name="changeset">Incoming changeset.</param>
        /// <param name="changesetID">Incoming changesetid.</param>
        public static Task IncomingChangeSetAssets(ChangeSet changeset) => 
            Internal_IncomingChangeSetAssets(changeset);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Add(Asset[] assets, bool recursive);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_AddIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Asset Internal_CacheStatus(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetAtlasRectForState(int state, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_ChangeSetDescription(ChangeSet changeset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_ChangeSetMove(Asset[] assets, ChangeSet target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_ChangeSetStatus(ChangeSet changeset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Checkout(Asset[] assets, CheckoutMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_CheckoutIsValid(Asset[] assets, CheckoutMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_CheckoutStrings(string[] assets, CheckoutMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Delete(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_DeleteAtProjectPath(string assetProjectPath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_DeleteChangeSets(ChangeSet[] changesets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_DeleteChangeSetsIsValid(ChangeSet[] changes);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_DiffHead(Asset[] assets, bool includingMetaFiles);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_DiffIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Asset[] Internal_GetAssetArrayFromSelection();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_GetLatest(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_GetLatestIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_IncomingChangeSetAssets(ChangeSet changeset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Lock(Asset[] assets, bool locked);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_LockIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Merge(Asset[] assets, MergeMethod method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_MoveAsStrings(string from, string to);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_PromptAndCheckoutIfNeeded(string[] assets, string promptIfCheckoutIsNeeded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Resolve(Asset[] assets, ResolveMethod resolveMethod);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_ResolveIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Revert(Asset[] assets, RevertMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_RevertChangeSets(ChangeSet[] changesets, RevertMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_RevertIsValid(Asset[] assets, RevertMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_SetFileMode(Asset[] assets, FileMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_SetFileModeStrings(string[] assets, FileMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Status(Asset[] assets, bool recursively);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_StatusAbsolutePath(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_StatusStrings(string[] assetsProjectPaths, bool recursively);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Task Internal_Submit(ChangeSet changeset, Asset[] assets, string description, bool saveOnly);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_SubmitIsValid(ChangeSet changeset, Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_UnlockIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InvalidateCache();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IsCustomCommandEnabled(string name);
        /// <summary>
        /// <para>Returns true if an asset can be edited.</para>
        /// </summary>
        /// <param name="asset">Asset to test.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsOpenForEdit(Asset asset);
        /// <summary>
        /// <para>Attempt to lock an asset for exclusive editing.</para>
        /// </summary>
        /// <param name="assets">List of assets to lock/unlock.</param>
        /// <param name="locked">True to lock assets, false to unlock assets.</param>
        /// <param name="asset">Asset to lock/unlock.</param>
        public static Task Lock(Asset asset, bool locked)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Lock(assets, locked);
        }

        /// <summary>
        /// <para>Attempt to lock an asset for exclusive editing.</para>
        /// </summary>
        /// <param name="assets">List of assets to lock/unlock.</param>
        /// <param name="locked">True to lock assets, false to unlock assets.</param>
        /// <param name="asset">Asset to lock/unlock.</param>
        public static Task Lock(AssetList assets, bool locked) => 
            Internal_Lock(assets.ToArray(), locked);

        /// <summary>
        /// <para>Return true if the task can be executed.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        /// <param name="asset">Asset to test.</param>
        public static bool LockIsValid(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_LockIsValid(assets);
        }

        /// <summary>
        /// <para>Return true if the task can be executed.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        /// <param name="asset">Asset to test.</param>
        public static bool LockIsValid(AssetList assets) => 
            Internal_LockIsValid(assets.ToArray());

        /// <summary>
        /// <para>This method will initiate a merge task handle merging of the conflicting assets.</para>
        /// </summary>
        /// <param name="assets">The list of conflicting assets to be merged.</param>
        /// <param name="method">How to merge the assets.</param>
        public static Task Merge(AssetList assets, MergeMethod method) => 
            Internal_Merge(assets.ToArray(), method);

        /// <summary>
        /// <para>Uses the version control plugin to move an asset from one path to another.</para>
        /// </summary>
        /// <param name="from">Path to source asset.</param>
        /// <param name="to">Path to destination.</param>
        public static Task Move(string from, string to) => 
            Internal_MoveAsStrings(from, to);

        internal static bool PromptAndCheckoutIfNeeded(string[] assets, string promptIfCheckoutIsNeeded) => 
            Internal_PromptAndCheckoutIfNeeded(assets, promptIfCheckoutIsNeeded);

        /// <summary>
        /// <para>Start a task that will resolve conflicting assets in version control.</para>
        /// </summary>
        /// <param name="assets">The list of asset to mark as resolved.</param>
        /// <param name="resolveMethod">How the assets should be resolved.</param>
        public static Task Resolve(AssetList assets, ResolveMethod resolveMethod) => 
            Internal_Resolve(assets.ToArray(), resolveMethod);

        /// <summary>
        /// <para>Tests if any of the assets in the list is resolvable.</para>
        /// </summary>
        /// <param name="assets">The list of asset to be resolved.</param>
        public static bool ResolveIsValid(AssetList assets) => 
            Internal_ResolveIsValid(assets.ToArray());

        /// <summary>
        /// <para>Reverts the specified assets by undoing any changes done since last time you synced.</para>
        /// </summary>
        /// <param name="assets">The list of assets to be reverted.</param>
        /// <param name="mode">How to revert the assets.</param>
        /// <param name="asset">The asset to be reverted.</param>
        public static Task Revert(Asset asset, RevertMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Revert(assets, mode);
        }

        /// <summary>
        /// <para>Reverts the specified assets by undoing any changes done since last time you synced.</para>
        /// </summary>
        /// <param name="assets">The list of assets to be reverted.</param>
        /// <param name="mode">How to revert the assets.</param>
        /// <param name="asset">The asset to be reverted.</param>
        public static Task Revert(AssetList assets, RevertMode mode) => 
            Internal_Revert(assets.ToArray(), mode);

        internal static Task RevertChangeSets(UnityEditor.VersionControl.ChangeSets changesets, RevertMode mode) => 
            Internal_RevertChangeSets(changesets.ToArray(), mode);

        /// <summary>
        /// <para>Return true if Revert is a valid task to perform.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        /// <param name="mode">Revert mode to test for.</param>
        /// <param name="asset">Asset to test.</param>
        public static bool RevertIsValid(Asset asset, RevertMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_RevertIsValid(assets, mode);
        }

        /// <summary>
        /// <para>Return true if Revert is a valid task to perform.</para>
        /// </summary>
        /// <param name="assets">List of assets to test.</param>
        /// <param name="mode">Revert mode to test for.</param>
        /// <param name="asset">Asset to test.</param>
        public static bool RevertIsValid(AssetList assets, RevertMode mode) => 
            Internal_RevertIsValid(assets.ToArray(), mode);

        internal static Task SetFileMode(AssetList assets, FileMode mode) => 
            Internal_SetFileMode(assets.ToArray(), mode);

        internal static Task SetFileMode(string[] assets, FileMode mode) => 
            Internal_SetFileModeStrings(assets, mode);

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Status(assets, true);
        }

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(AssetList assets) => 
            Internal_Status(assets.ToArray(), true);

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(string[] assets) => 
            Internal_StatusStrings(assets, true);

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(string asset)
        {
            string[] assetsProjectPaths = new string[] { asset };
            return Internal_StatusStrings(assetsProjectPaths, true);
        }

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(string[] assets, bool recursively) => 
            Internal_StatusStrings(assets, recursively);

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(string asset, bool recursively)
        {
            string[] assetsProjectPaths = new string[] { asset };
            return Internal_StatusStrings(assetsProjectPaths, recursively);
        }

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(Asset asset, bool recursively)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Status(assets, recursively);
        }

        /// <summary>
        /// <para>Start a task that will fetch the most recent status from revision control system.</para>
        /// </summary>
        /// <param name="assets">The assets fetch new state for.</param>
        /// <param name="asset">The asset path to fetch new state for.</param>
        /// <param name="recursively">If any assets specified are folders this flag will get status for all descendants of the folder as well.</param>
        public static Task Status(AssetList assets, bool recursively) => 
            Internal_Status(assets.ToArray(), recursively);

        /// <summary>
        /// <para>Start a task that submits the assets to version control.</para>
        /// </summary>
        /// <param name="changeset">The changeset to submit.</param>
        /// <param name="list">The list of assets to submit.</param>
        /// <param name="description">The description of the changeset.</param>
        /// <param name="saveOnly">If true then only save the changeset to be submitted later.</param>
        public static Task Submit(ChangeSet changeset, AssetList list, string description, bool saveOnly) => 
            Internal_Submit(changeset, list?.ToArray(), description, saveOnly);

        /// <summary>
        /// <para>Returns true if submitting the assets is a valid operation.</para>
        /// </summary>
        /// <param name="changeset">The changeset to submit.</param>
        /// <param name="assets">The asset to submit.</param>
        public static bool SubmitIsValid(ChangeSet changeset, AssetList assets) => 
            Internal_SubmitIsValid(changeset, assets?.ToArray());

        /// <summary>
        /// <para>Returns true if locking the assets is a valid operation.</para>
        /// </summary>
        /// <param name="assets">The assets to lock.</param>
        /// <param name="asset">The asset to lock.</param>
        public static bool UnlockIsValid(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_UnlockIsValid(assets);
        }

        /// <summary>
        /// <para>Returns true if locking the assets is a valid operation.</para>
        /// </summary>
        /// <param name="assets">The assets to lock.</param>
        /// <param name="asset">The asset to lock.</param>
        public static bool UnlockIsValid(AssetList assets) => 
            Internal_UnlockIsValid(assets.ToArray());

        /// <summary>
        /// <para>Start a task that sends the version control settings to the version control system.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Task UpdateSettings();

        /// <summary>
        /// <para>Gets the currently executing task.</para>
        /// </summary>
        public static Task activeTask { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static CustomCommand[] customCommands { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if the version control provider is enabled and a valid Unity Pro License was found.</para>
        /// </summary>
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool hasChangelistSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool hasCheckoutSupport { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if a version control plugin has been selected and configured correctly.</para>
        /// </summary>
        public static bool isActive { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool isVersioningFolders { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the reason for the version control provider being offline (if it is offline).</para>
        /// </summary>
        public static string offlineReason { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the OnlineState of the version control provider.</para>
        /// </summary>
        public static OnlineState onlineState { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static Texture2D overlayAtlas { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>This is true if a network connection is required by the currently selected version control plugin to perform any action.</para>
        /// </summary>
        public static bool requiresNetwork { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}


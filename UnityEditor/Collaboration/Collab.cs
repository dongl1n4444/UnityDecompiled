namespace UnityEditor.Collaboration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Connect;
    using UnityEditor.SceneManagement;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [InitializeOnLoad]
    internal sealed class Collab : AssetPostprocessor
    {
        [CompilerGenerated]
        private static UnityEditor.Connect.StateChangedDelegate <>f__mg$cache0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <projectBrowserSingleMetaSelectionPath>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <projectBrowserSingleSelectionPath>k__BackingField;
        public static string[] clientType = new string[] { "Cloud Server", "Mock Server" };
        public string[] currentProjectBrowserSelection;
        internal static string editorPrefCollabClientType = "CollabConfig_Client";
        private static Collab s_Instance = new Collab();
        private static bool s_IsFirstStateChange = true;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event UnityEditor.Collaboration.StateChangedDelegate StateChanged;

        static Collab()
        {
            s_Instance.projectBrowserSingleSelectionPath = string.Empty;
            s_Instance.projectBrowserSingleMetaSelectionPath = string.Empty;
            JSProxyMgr.GetInstance().AddGlobalObject("unity/collab", s_Instance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        public extern bool AnyJobRunning();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool AreTestsRunning();
        public void CancelJob(int jobId)
        {
            this.CancelJobByType(jobId);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void CancelJobByType(int jobType);
        public void CancelJobWithoutException(int jobType)
        {
            try
            {
                this.CancelJobByType(jobType);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.Log("Cannot cancel job, reason:" + exception.Message);
            }
        }

        private static void CannotPublishDialog(string infoMessage)
        {
            CollabCannotPublishDialog.ShowCollabWindow(infoMessage);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void CheckConflictsResolvedExternal();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearAllFailures();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool ClearConflictResolved(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool ClearConflictsResolved(string[] paths);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearError(int errorCode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearErrors();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearNextOperationFailure();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ClearNextOperationFailureForFile(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Disconnect();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void FailNextOperation(int operation, int code);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void FailNextOperationForFile(string path, int operation, int code);
        public CollabStates GetAssetState(string guid) => 
            ((CollabStates) this.GetAssetStateInternal(guid));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern long GetAssetStateInternal(string guid);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Change[] GetChangesToPublish();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Change[] GetCollabConflicts();
        public CollabInfo GetCollabInfo() => 
            this.collabInfo;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern CollabStateID GetCollabState();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool GetError(int filter, out int code, out int priority, out int behaviour, out string errorMsg, out string errorShortMsg);
        public ProgressInfo GetJobProgress(int jobId) => 
            this.GetJobProgressByType(jobId);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern ProgressInfo GetJobProgressByType(int jobType);
        public static string GetProjectClientType()
        {
            string configValue = EditorUserSettings.GetConfigValue(editorPrefCollabClientType);
            return (!string.IsNullOrEmpty(configValue) ? configValue : clientType[0]);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern string GetProjectPath();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Revision[] GetRevisions();
        public CollabStates GetSelectedAssetState() => 
            ((CollabStates) this.GetSelectedAssetStateInternal());

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern long GetSelectedAssetStateInternal();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern SoftLock[] GetSoftLocks(string assetGuid);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void GoBackToRevision(string revisionID, bool updateToRevision);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool IsCollabEnabledForCurrentProject();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern bool IsConnected();
        public static bool IsDiffToolsAvailable() => 
            (InternalEditorUtility.GetAvailableDiffTools().Length > 0);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        public extern bool JobRunning(int a_jobID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void LaunchConflictExternalMerge(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName);
        private static void OnStateChanged()
        {
            if (s_IsFirstStateChange)
            {
                s_IsFirstStateChange = false;
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new UnityEditor.Connect.StateChangedDelegate(Collab.OnUnityConnectStateChanged);
                }
                UnityConnect.instance.StateChanged += <>f__mg$cache0;
            }
            UnityEditor.Collaboration.StateChangedDelegate stateChanged = instance.StateChanged;
            if (stateChanged != null)
            {
                stateChanged(instance.collabInfo);
            }
        }

        private static void OnUnityConnectStateChanged(ConnectInfo state)
        {
            instance.SendNotification();
        }

        [ExcludeFromDocs]
        public void Publish(string comment)
        {
            bool useSelectedAssets = false;
            this.Publish(comment, useSelectedAssets);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Publish(string comment, [DefaultValue("false")] bool useSelectedAssets);
        private static void PublishDialog(string changelist)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                CollabPublishDialog dialog = CollabPublishDialog.ShowCollabWindow(changelist);
                if (dialog.Options.DoPublish)
                {
                    instance.Publish(dialog.Options.Comments, true);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ResyncSnapshot();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ResyncToRevision(string revisionID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RevertFile(string path, bool forceOverwrite);
        public void SaveAssets()
        {
            AssetDatabase.SaveAssets();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SendNotification();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetCollabEnabledForCurrentProject(bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool SetConflictResolvedMine(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool SetConflictResolvedTheirs(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool SetConflictsResolvedMine(string[] paths);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool SetConflictsResolvedTheirs(string[] paths);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetError(int errorCode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetTestsRunning(bool running);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ShowConflictDifferences(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void ShowDifferences(string path);
        public static void SwitchToDefaultMode()
        {
            bool flag = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;
            SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
            if ((lastActiveSceneView != null) && (lastActiveSceneView.in2DMode != flag))
            {
                lastActiveSceneView.in2DMode = flag;
            }
        }

        [UnityEditor.MenuItem("Window/Collab/Get Revisions", false, 0x3e8, true)]
        public static void TestGetRevisions()
        {
            Revision[] revisions = instance.GetRevisions();
            if (revisions.Length == 0)
            {
                UnityEngine.Debug.Log("No revisions");
            }
            else
            {
                int length = revisions.Length;
                foreach (Revision revision in revisions)
                {
                    UnityEngine.Debug.Log(string.Concat(new object[] { "Revision #", length, ": ", revision.revisionID }));
                    length--;
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void TimeOutNextOperation(int operation, int timeOutSec);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void TimeOutNextOperationForFile(string path, int operation, int timeOutSec);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Update(string revisionID, bool updateToRevision);
        public void UpdateEditorSelectionCache()
        {
            List<string> list = new List<string>();
            foreach (string str in Selection.assetGUIDsDeepSelection)
            {
                string item = AssetDatabase.GUIDToAssetPath(str);
                list.Add(item);
                string path = item + ".meta";
                if (File.Exists(path))
                {
                    list.Add(path);
                }
            }
            this.currentProjectBrowserSelection = list.ToArray();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool ValidateSelectiveCommit();

        public CollabInfo collabInfo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static Collab instance =>
            s_Instance;

        public string projectBrowserSingleMetaSelectionPath { get; set; }

        public string projectBrowserSingleSelectionPath { get; set; }

        internal enum CollabStateID
        {
            None,
            Uninitialized,
            Initialized
        }

        [Flags]
        public enum CollabStates : ulong
        {
            kAnyLocalChanged = 0x950L,
            kAnyLocalEdited = 0x910L,
            kCollabAddedLocal = 0x100L,
            kCollabAddedRemote = 0x200L,
            kCollabCheckedOutLocal = 0x10L,
            kCollabCheckedOutRemote = 0x20L,
            kCollabConflicted = 0x400L,
            KCollabContentChanged = 0x200000L,
            KCollabContentConflicted = 0x400000L,
            KCollabContentDeleted = 0x800000L,
            kCollabDeletedLocal = 0x40L,
            kCollabDeletedRemote = 0x80L,
            kCollabFolderMetaFile = 0x100000L,
            kCollabIgnored = 8L,
            kCollabInvalidState = 0x40000000L,
            kCollabLocal = 1L,
            kCollabMerged = 0x40000L,
            kCollabMetaFile = 0x8000L,
            kCollabMovedLocal = 0x800L,
            kCollabMovedRemote = 0x1000L,
            kCollabNone = 0L,
            kCollabOutOfSync = 4L,
            kCollabPendingMerge = 0x80000L,
            kCollabReadOnly = 0x4000L,
            kCollabSynced = 2L,
            kCollabUpdating = 0x2000L,
            kCollabUseMine = 0x10000L,
            kCollabUseTheir = 0x20000L
        }
    }
}


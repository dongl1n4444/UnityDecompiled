namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Collaboration;
    using UnityEditor.Connect;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class HierarchyProperty : IHierarchyProperty
    {
        private IntPtr m_Property;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern HierarchyProperty(HierarchyType hierarchytType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearSceneObjectsFilter();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int CountRemaining(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Dispose();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void FilterSingleSceneObject(int instanceID, bool otherVisibilityState);
        ~HierarchyProperty()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Find(int instanceID, int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int[] FindAllAncestors(int[] instanceIDs);
        public Scene GetScene()
        {
            Scene scene;
            INTERNAL_CALL_GetScene(this, out scene);
            return scene;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetScene(HierarchyProperty self, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsExpanded(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Next(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool NextWithDepthCheck(int[] expanded, int minDepth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Parent();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Previous(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Reset();
        internal void SetSearchFilter(SearchFilter filter)
        {
            if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
            {
                this.SetSearchFilterINTERNAL(SearchFilter.Split(filter.nameFilter), filter.classNames, filter.assetLabels, filter.assetBundleNames, filter.versionControlStates, filter.referencingInstanceIDs, filter.scenePaths, filter.showAllHits);
            }
            else
            {
                this.SetSearchFilterINTERNAL(SearchFilter.Split(filter.nameFilter), filter.classNames, filter.assetLabels, filter.assetBundleNames, new string[0], filter.referencingInstanceIDs, filter.scenePaths, filter.showAllHits);
            }
        }

        public void SetSearchFilter(string searchString, int mode)
        {
            SearchFilter filter = SearchableEditorWindow.CreateFilter(searchString, (SearchableEditorWindow.SearchMode) mode);
            this.SetSearchFilter(filter);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetSearchFilterINTERNAL(string[] nameFilters, string[] classNames, string[] assetLabels, string[] assetBundleNames, string[] versionControlStates, int[] referencingInstanceIDs, string[] scenePaths, bool showAllHits);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Skip(int count, int[] expanded);

        public bool alphaSorted { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public int[] ancestors { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int colorCode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int depth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string guid { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool hasChildren { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool hasFullPreviewImage { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public Texture2D icon { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public IconDrawStyle iconDrawStyle { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int instanceID { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool isFolder { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool isMainRepresentation { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public bool isValid { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public Object pptrValue { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public int row { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}


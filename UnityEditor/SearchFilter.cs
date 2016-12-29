﻿namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEditor.Collaboration;
    using UnityEditor.Connect;
    using UnityEngine;

    [Serializable]
    internal class SearchFilter
    {
        [SerializeField]
        private string[] m_AssetBundleNames = new string[0];
        [SerializeField]
        private string[] m_AssetLabels = new string[0];
        [SerializeField]
        private string[] m_ClassNames = new string[0];
        [SerializeField]
        private string[] m_Folders = new string[0];
        [SerializeField]
        private string m_NameFilter = "";
        [SerializeField]
        private int[] m_ReferencingInstanceIDs = new int[0];
        [SerializeField]
        private string[] m_ScenePaths;
        [SerializeField]
        private SearchArea m_SearchArea = SearchArea.AllAssets;
        [SerializeField]
        private bool m_ShowAllHits = false;
        [SerializeField]
        private string[] m_VersionControlStates = new string[0];

        private void AddToString<T>(string prefix, T[] list, ref string result)
        {
            if (list != null)
            {
                if (result == null)
                {
                    result = "";
                }
                foreach (T local in list)
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result + " ";
                    }
                    result = result + prefix + local;
                }
            }
        }

        public void ClearSearch()
        {
            this.m_NameFilter = "";
            this.m_ClassNames = new string[0];
            this.m_AssetLabels = new string[0];
            this.m_AssetBundleNames = new string[0];
            this.m_ReferencingInstanceIDs = new int[0];
            this.m_ScenePaths = new string[0];
            this.m_VersionControlStates = new string[0];
            this.m_ShowAllHits = false;
        }

        internal static SearchFilter CreateSearchFilterFromString(string searchText)
        {
            SearchFilter filter = new SearchFilter();
            SearchUtility.ParseSearchString(searchText, filter);
            return filter;
        }

        internal string FilterToSearchFieldString()
        {
            string result = "";
            if (!string.IsNullOrEmpty(this.m_NameFilter))
            {
                result = result + this.m_NameFilter;
            }
            this.AddToString<string>("t:", this.m_ClassNames, ref result);
            this.AddToString<string>("l:", this.m_AssetLabels, ref result);
            if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
            {
                this.AddToString<string>("v:", this.m_VersionControlStates, ref result);
            }
            this.AddToString<string>("b:", this.m_AssetBundleNames, ref result);
            return result;
        }

        public State GetState()
        {
            bool flag = ((!string.IsNullOrEmpty(this.m_NameFilter) || !this.IsNullOrEmpty<string>(this.m_AssetLabels)) || (!this.IsNullOrEmpty<string>(this.m_ClassNames) || !this.IsNullOrEmpty<string>(this.m_AssetBundleNames))) || !this.IsNullOrEmpty<int>(this.m_ReferencingInstanceIDs);
            if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
            {
                flag = flag || !this.IsNullOrEmpty<string>(this.m_VersionControlStates);
            }
            bool flag2 = !this.IsNullOrEmpty<string>(this.m_Folders);
            if (flag)
            {
                if (this.m_SearchArea == SearchArea.AssetStore)
                {
                    return State.SearchingInAssetStore;
                }
                if (flag2 && (this.m_SearchArea == SearchArea.SelectedFolders))
                {
                    return State.SearchingInFolders;
                }
                return State.SearchingInAllAssets;
            }
            if (flag2)
            {
                return State.FolderBrowsing;
            }
            return State.EmptySearchFilter;
        }

        private bool IsNullOrEmpty<T>(T[] list) => 
            ((list == null) || (list.Length == 0));

        public bool IsSearching()
        {
            State state = this.GetState();
            return (((state == State.SearchingInAllAssets) || (state == State.SearchingInFolders)) || (state == State.SearchingInAssetStore));
        }

        internal void SearchFieldStringToFilter(string searchString)
        {
            this.ClearSearch();
            if (!string.IsNullOrEmpty(searchString))
            {
                SearchUtility.ParseSearchString(searchString, this);
            }
        }

        public bool SetNewFilter(SearchFilter newFilter)
        {
            bool flag = false;
            if (newFilter.m_NameFilter != this.m_NameFilter)
            {
                this.m_NameFilter = newFilter.m_NameFilter;
                flag = true;
            }
            if (newFilter.m_ClassNames != this.m_ClassNames)
            {
                this.m_ClassNames = newFilter.m_ClassNames;
                flag = true;
            }
            if (newFilter.m_Folders != this.m_Folders)
            {
                this.m_Folders = newFilter.m_Folders;
                flag = true;
            }
            if ((UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted) && (newFilter.m_VersionControlStates != this.m_VersionControlStates))
            {
                this.m_VersionControlStates = newFilter.m_VersionControlStates;
                flag = true;
            }
            if (newFilter.m_AssetLabels != this.m_AssetLabels)
            {
                this.m_AssetLabels = newFilter.m_AssetLabels;
                flag = true;
            }
            if (newFilter.m_AssetBundleNames != this.m_AssetBundleNames)
            {
                this.m_AssetBundleNames = newFilter.m_AssetBundleNames;
                flag = true;
            }
            if (newFilter.m_ReferencingInstanceIDs != this.m_ReferencingInstanceIDs)
            {
                this.m_ReferencingInstanceIDs = newFilter.m_ReferencingInstanceIDs;
                flag = true;
            }
            if (newFilter.m_ScenePaths != this.m_ScenePaths)
            {
                this.m_ScenePaths = newFilter.m_ScenePaths;
                flag = true;
            }
            if (newFilter.m_SearchArea != this.m_SearchArea)
            {
                this.m_SearchArea = newFilter.m_SearchArea;
                flag = true;
            }
            this.m_ShowAllHits = newFilter.m_ShowAllHits;
            return flag;
        }

        public static string[] Split(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[0];
            }
            List<string> list = new List<string>();
            IEnumerator enumerator = Regex.Matches(text, "\".+?\"|\\S+").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    list.Add(current.Value.Replace("\"", ""));
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return list.ToArray();
        }

        public override string ToString()
        {
            string str2;
            string str = "SearchFilter: ";
            str = str + $"[Area: {this.m_SearchArea}, State: {this.GetState()}]";
            if (!string.IsNullOrEmpty(this.m_NameFilter))
            {
                str = str + "[Name: " + this.m_NameFilter + "]";
            }
            if ((this.m_AssetLabels != null) && (this.m_AssetLabels.Length > 0))
            {
                str = str + "[Labels: " + this.m_AssetLabels[0] + "]";
            }
            if ((UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted) && ((this.m_VersionControlStates != null) && (this.m_VersionControlStates.Length > 0)))
            {
                str = str + "[VersionStates: " + this.m_VersionControlStates[0] + "]";
            }
            if ((this.m_AssetBundleNames != null) && (this.m_AssetBundleNames.Length > 0))
            {
                str = str + "[AssetBundleNames: " + this.m_AssetBundleNames[0] + "]";
            }
            if ((this.m_ClassNames != null) && (this.m_ClassNames.Length > 0))
            {
                str2 = str;
                object[] objArray1 = new object[] { str2, "[Types: ", this.m_ClassNames[0], " (", this.m_ClassNames.Length, ")]" };
                str = string.Concat(objArray1);
            }
            if ((this.m_ReferencingInstanceIDs != null) && (this.m_ReferencingInstanceIDs.Length > 0))
            {
                str2 = str;
                object[] objArray2 = new object[] { str2, "[RefIDs: ", this.m_ReferencingInstanceIDs[0], "]" };
                str = string.Concat(objArray2);
            }
            if ((this.m_Folders != null) && (this.m_Folders.Length > 0))
            {
                str = str + "[Folders: " + this.m_Folders[0] + "]";
            }
            str2 = str;
            object[] objArray3 = new object[] { str2, "[ShowAllHits: ", this.showAllHits, "]" };
            return string.Concat(objArray3);
        }

        public string[] assetBundleNames
        {
            get => 
                this.m_AssetBundleNames;
            set
            {
                this.m_AssetBundleNames = value;
            }
        }

        public string[] assetLabels
        {
            get => 
                this.m_AssetLabels;
            set
            {
                this.m_AssetLabels = value;
            }
        }

        public string[] classNames
        {
            get => 
                this.m_ClassNames;
            set
            {
                this.m_ClassNames = value;
            }
        }

        public string[] folders
        {
            get => 
                this.m_Folders;
            set
            {
                this.m_Folders = value;
            }
        }

        public string nameFilter
        {
            get => 
                this.m_NameFilter;
            set
            {
                this.m_NameFilter = value;
            }
        }

        public int[] referencingInstanceIDs
        {
            get => 
                this.m_ReferencingInstanceIDs;
            set
            {
                this.m_ReferencingInstanceIDs = value;
            }
        }

        public string[] scenePaths
        {
            get => 
                this.m_ScenePaths;
            set
            {
                this.m_ScenePaths = value;
            }
        }

        public SearchArea searchArea
        {
            get => 
                this.m_SearchArea;
            set
            {
                this.m_SearchArea = value;
            }
        }

        public bool showAllHits
        {
            get => 
                this.m_ShowAllHits;
            set
            {
                this.m_ShowAllHits = value;
            }
        }

        public string[] versionControlStates
        {
            get => 
                this.m_VersionControlStates;
            set
            {
                this.m_VersionControlStates = value;
            }
        }

        public enum SearchArea
        {
            AllAssets,
            SelectedFolders,
            AssetStore
        }

        public enum State
        {
            EmptySearchFilter,
            FolderBrowsing,
            SearchingInAllAssets,
            SearchingInFolders,
            SearchingInAssetStore
        }
    }
}


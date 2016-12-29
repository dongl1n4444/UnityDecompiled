namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;

    internal class AudioProfilerClipInfoHelper
    {
        private static string[] m_InternalLoadStateNames = new string[] { "Pending", "Loaded", "Failed" };
        private static string[] m_LoadStateNames = new string[] { "Unloaded", "Loading Base", "Loading Sub", "Loaded", "Failed" };

        public static string GetColumnString(AudioProfilerClipInfoWrapper info, ColumnIndices index)
        {
            switch (index)
            {
                case ColumnIndices.AssetName:
                    return info.assetName;

                case ColumnIndices.LoadState:
                    return m_LoadStateNames[info.info.loadState];

                case ColumnIndices.InternalLoadState:
                    return m_InternalLoadStateNames[info.info.internalLoadState];

                case ColumnIndices.Age:
                    return info.info.age.ToString();

                case ColumnIndices.Disposed:
                    return ((info.info.disposed == 0) ? "NO" : "YES");

                case ColumnIndices.NumChannelInstances:
                    return info.info.numChannelInstances.ToString();
            }
            return "Unknown";
        }

        public static int GetLastColumnIndex() => 
            5;

        public class AudioProfilerClipInfoComparer : IComparer<AudioProfilerClipInfoWrapper>
        {
            public AudioProfilerClipInfoHelper.ColumnIndices primarySortKey;
            public AudioProfilerClipInfoHelper.ColumnIndices secondarySortKey;
            public bool sortByDescendingOrder;

            public AudioProfilerClipInfoComparer(AudioProfilerClipInfoHelper.ColumnIndices primarySortKey, AudioProfilerClipInfoHelper.ColumnIndices secondarySortKey, bool sortByDescendingOrder)
            {
                this.primarySortKey = primarySortKey;
                this.secondarySortKey = secondarySortKey;
                this.sortByDescendingOrder = sortByDescendingOrder;
            }

            public int Compare(AudioProfilerClipInfoWrapper a, AudioProfilerClipInfoWrapper b)
            {
                int num = this.CompareInternal(a, b, this.primarySortKey);
                return ((num != 0) ? num : this.CompareInternal(a, b, this.secondarySortKey));
            }

            private int CompareInternal(AudioProfilerClipInfoWrapper a, AudioProfilerClipInfoWrapper b, AudioProfilerClipInfoHelper.ColumnIndices key)
            {
                int num = 0;
                switch (key)
                {
                    case AudioProfilerClipInfoHelper.ColumnIndices.AssetName:
                        num = a.assetName.CompareTo(b.assetName);
                        break;

                    case AudioProfilerClipInfoHelper.ColumnIndices.LoadState:
                        num = a.info.loadState.CompareTo(b.info.loadState);
                        break;

                    case AudioProfilerClipInfoHelper.ColumnIndices.InternalLoadState:
                        num = a.info.internalLoadState.CompareTo(b.info.internalLoadState);
                        break;

                    case AudioProfilerClipInfoHelper.ColumnIndices.Age:
                        num = a.info.age.CompareTo(b.info.age);
                        break;

                    case AudioProfilerClipInfoHelper.ColumnIndices.Disposed:
                        num = a.info.disposed.CompareTo(b.info.disposed);
                        break;

                    case AudioProfilerClipInfoHelper.ColumnIndices.NumChannelInstances:
                        num = a.info.numChannelInstances.CompareTo(b.info.numChannelInstances);
                        break;
                }
                return (!this.sortByDescendingOrder ? num : -num);
            }
        }

        public enum ColumnIndices
        {
            AssetName,
            LoadState,
            InternalLoadState,
            Age,
            Disposed,
            NumChannelInstances,
            _LastColumn
        }
    }
}


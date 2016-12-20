namespace UnityEditor.VersionControl
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>A list of version control information about assets.</para>
    /// </summary>
    public class AssetList : List<Asset>
    {
        public AssetList()
        {
        }

        public AssetList(AssetList src)
        {
        }

        public AssetList Filter(bool includeFolder, params Asset.States[] states)
        {
            AssetList list = new AssetList();
            if (includeFolder || ((states != null) && (states.Length != 0)))
            {
                foreach (Asset asset in this)
                {
                    if (asset.isFolder)
                    {
                        if (includeFolder)
                        {
                            list.Add(asset);
                        }
                    }
                    else if (asset.IsOneOfStates(states))
                    {
                        list.Add(asset);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// <para>Create an optimised list of assets by removing children of folders in the same list.</para>
        /// </summary>
        public AssetList FilterChildren()
        {
            AssetList list = new AssetList();
            list.AddRange(this);
            using (List<Asset>.Enumerator enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <FilterChildren>c__AnonStorey0 storey = new <FilterChildren>c__AnonStorey0 {
                        asset = enumerator.Current
                    };
                    list.RemoveAll(new Predicate<Asset>(storey.<>m__0));
                }
            }
            return list;
        }

        public int FilterCount(bool includeFolder, params Asset.States[] states)
        {
            int num = 0;
            if (!includeFolder && (states == null))
            {
                return base.Count;
            }
            foreach (Asset asset in this)
            {
                if (asset.isFolder)
                {
                    num++;
                }
                else if (asset.IsOneOfStates(states))
                {
                    num++;
                }
            }
            return num;
        }

        [CompilerGenerated]
        private sealed class <FilterChildren>c__AnonStorey0
        {
            internal Asset asset;

            internal bool <>m__0(Asset p)
            {
                return p.IsChildOf(this.asset);
            }
        }
    }
}


namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowSelection
    {
        [CompilerGenerated]
        private static Func<AnimationWindowSelectionItem, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<AnimationWindowSelectionItem, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Action <>f__am$cache2;
        private bool m_BatchOperations = false;
        private List<AnimationWindowCurve> m_CurvesCache = null;
        [SerializeField]
        private List<AnimationWindowSelectionItem> m_Selection = new List<AnimationWindowSelectionItem>();
        private bool m_SelectionChanged = false;
        [NonSerialized]
        public Action onSelectionChanged;

        public AnimationWindowSelection()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Action(AnimationWindowSelection.<AnimationWindowSelection>m__2);
            }
            this.onSelectionChanged = (Action) Delegate.Combine(this.onSelectionChanged, <>f__am$cache2);
        }

        [CompilerGenerated]
        private static void <AnimationWindowSelection>m__2()
        {
        }

        public void Add(AnimationWindowSelectionItem newItem)
        {
            if (!this.m_Selection.Contains(newItem))
            {
                this.m_Selection.Add(newItem);
                this.Notify();
            }
        }

        public void BeginOperations()
        {
            if (this.m_BatchOperations)
            {
                Debug.LogWarning("AnimationWindowSelection: Already inside a BeginOperations/EndOperations block");
            }
            else
            {
                this.m_BatchOperations = true;
                this.m_SelectionChanged = false;
            }
        }

        public void Clear()
        {
            if (this.m_Selection.Count > 0)
            {
                foreach (AnimationWindowSelectionItem item in this.m_Selection)
                {
                    Object.DestroyImmediate(item);
                }
                this.m_Selection.Clear();
                this.Notify();
            }
        }

        public void ClearCache()
        {
            this.m_CurvesCache = null;
        }

        public void EndOperations()
        {
            if (this.m_BatchOperations)
            {
                if (this.m_SelectionChanged)
                {
                    this.onSelectionChanged();
                }
                this.m_SelectionChanged = false;
                this.m_BatchOperations = false;
            }
        }

        public bool Exists(Predicate<AnimationWindowSelectionItem> predicate) => 
            this.m_Selection.Exists(predicate);

        public bool Exists(AnimationWindowSelectionItem itemToFind) => 
            this.m_Selection.Contains(itemToFind);

        public AnimationWindowSelectionItem Find(Predicate<AnimationWindowSelectionItem> predicate) => 
            this.m_Selection.Find(predicate);

        public AnimationWindowSelectionItem First() => 
            this.m_Selection.First<AnimationWindowSelectionItem>();

        public int GetRefreshHash()
        {
            int num = 0;
            foreach (AnimationWindowSelectionItem item in this.m_Selection)
            {
                num ^= item.GetRefreshHash();
            }
            return num;
        }

        public void Notify()
        {
            if (this.m_BatchOperations)
            {
                this.m_SelectionChanged = true;
            }
            else
            {
                this.onSelectionChanged();
            }
        }

        public void RangeAdd(AnimationWindowSelectionItem[] newItemArray)
        {
            bool flag = false;
            foreach (AnimationWindowSelectionItem item in newItemArray)
            {
                if (!this.m_Selection.Contains(item))
                {
                    this.m_Selection.Add(item);
                    flag = true;
                }
            }
            if (flag)
            {
                this.Notify();
            }
        }

        public void Refresh()
        {
            this.ClearCache();
            foreach (AnimationWindowSelectionItem item in this.m_Selection)
            {
                item.ClearCache();
            }
        }

        public void Set(AnimationWindowSelectionItem newItem)
        {
            this.BeginOperations();
            this.Clear();
            this.Add(newItem);
            this.EndOperations();
        }

        public void Synchronize()
        {
            if (this.m_Selection.Count > 0)
            {
                foreach (AnimationWindowSelectionItem item in this.m_Selection)
                {
                    item.Synchronize();
                }
            }
        }

        public AnimationWindowSelectionItem[] ToArray() => 
            this.m_Selection.ToArray();

        public void UpdateClip(AnimationWindowSelectionItem itemToUpdate, AnimationClip newClip)
        {
            if (this.m_Selection.Contains(itemToUpdate))
            {
                itemToUpdate.animationClip = newClip;
                this.Notify();
            }
        }

        public void UpdateTimeOffset(AnimationWindowSelectionItem itemToUpdate, float timeOffset)
        {
            if (this.m_Selection.Contains(itemToUpdate))
            {
                itemToUpdate.timeOffset = timeOffset;
            }
        }

        public bool canAddCurves
        {
            get
            {
                if (this.m_Selection.Count > 0)
                {
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = item => !item.canAddCurves;
                    }
                    return !Enumerable.Any<AnimationWindowSelectionItem>(this.m_Selection, <>f__am$cache1);
                }
                return false;
            }
        }

        public bool canRecord
        {
            get
            {
                if (this.m_Selection.Count > 0)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = item => !item.canRecord;
                    }
                    return !Enumerable.Any<AnimationWindowSelectionItem>(this.m_Selection, <>f__am$cache0);
                }
                return false;
            }
        }

        public int count =>
            this.m_Selection.Count;

        public List<AnimationWindowCurve> curves
        {
            get
            {
                if (this.m_CurvesCache == null)
                {
                    this.m_CurvesCache = new List<AnimationWindowCurve>();
                    foreach (AnimationWindowSelectionItem item in this.m_Selection)
                    {
                        this.m_CurvesCache.AddRange(item.curves);
                    }
                }
                return this.m_CurvesCache;
            }
        }

        public bool disabled
        {
            get
            {
                if (this.m_Selection.Count > 0)
                {
                    foreach (AnimationWindowSelectionItem item in this.m_Selection)
                    {
                        if (item.animationClip != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}


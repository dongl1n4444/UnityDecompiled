namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class DopeLine
    {
        [CompilerGenerated]
        private static Predicate<AnimationWindowCurve> <>f__am$cache0;
        [CompilerGenerated]
        private static Comparison<AnimationWindowKeyframe> <>f__am$cache1;
        public static GUIStyle dopekeyStyle = "Dopesheetkeyframe";
        public bool hasChildren;
        public bool isMasterDopeline;
        private AnimationWindowCurve[] m_Curves;
        private int m_HierarchyNodeID;
        private List<AnimationWindowKeyframe> m_Keys;
        public Type objectType;
        public Rect position;
        public bool tallMode;

        public DopeLine(int hierarchyNodeID, AnimationWindowCurve[] curves)
        {
            this.m_HierarchyNodeID = hierarchyNodeID;
            this.m_Curves = curves;
        }

        public void InvalidateKeyframes()
        {
            this.m_Keys = null;
        }

        public AnimationWindowCurve[] curves
        {
            get
            {
                return this.m_Curves;
            }
        }

        public int hierarchyNodeID
        {
            get
            {
                return this.m_HierarchyNodeID;
            }
        }

        public bool isEditable
        {
            get
            {
                if (this.m_Curves.Length > 0)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = curve => !curve.animationIsEditable;
                    }
                    return !Array.Exists<AnimationWindowCurve>(this.m_Curves, <>f__am$cache0);
                }
                return false;
            }
        }

        public bool isPptrDopeline
        {
            get
            {
                if (this.m_Curves.Length > 0)
                {
                    for (int i = 0; i < this.m_Curves.Length; i++)
                    {
                        if (!this.m_Curves[i].isPPtrCurve)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public List<AnimationWindowKeyframe> keys
        {
            get
            {
                if (this.m_Keys == null)
                {
                    this.m_Keys = new List<AnimationWindowKeyframe>();
                    foreach (AnimationWindowCurve curve in this.m_Curves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            this.m_Keys.Add(keyframe);
                        }
                    }
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = (a, b) => a.time.CompareTo(b.time);
                    }
                    this.m_Keys.Sort(<>f__am$cache1);
                }
                return this.m_Keys;
            }
        }

        public Type valueType
        {
            get
            {
                if (this.m_Curves.Length > 0)
                {
                    Type valueType = this.m_Curves[0].valueType;
                    for (int i = 1; i < this.m_Curves.Length; i++)
                    {
                        if (this.m_Curves[i].valueType != valueType)
                        {
                            return null;
                        }
                    }
                    return valueType;
                }
                return null;
            }
        }
    }
}


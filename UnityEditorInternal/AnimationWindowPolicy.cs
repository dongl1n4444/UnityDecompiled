namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowPolicy
    {
        [CompilerGenerated]
        private static SynchronizeGeometryDelegate <>f__am$cache0;
        [CompilerGenerated]
        private static SynchronizeFrameRateDelegate <>f__am$cache1;
        [CompilerGenerated]
        private static SynchronizeCurrentTimeDelegate <>f__am$cache2;
        [CompilerGenerated]
        private static SynchronizeZoomableAreaDelegate <>f__am$cache3;
        [CompilerGenerated]
        private static OnGeometryChangeDelegate <>f__am$cache4;
        [CompilerGenerated]
        private static OnCurrentTimeChangeDelegate <>f__am$cache5;
        [CompilerGenerated]
        private static OnZoomableAreaChangeDelegate <>f__am$cache6;
        public OnCurrentTimeChangeDelegate OnCurrentTimeChange;
        public OnGeometryChangeDelegate OnGeometryChange;
        public OnZoomableAreaChangeDelegate OnZoomableAreaChange;
        public SynchronizeCurrentTimeDelegate SynchronizeCurrentTime;
        public SynchronizeFrameRateDelegate SynchronizeFrameRate;
        public SynchronizeGeometryDelegate SynchronizeGeometry;
        public SynchronizeZoomableAreaDelegate SynchronizeZoomableArea;
        [SerializeField]
        public bool triggerFramingOnSelection = true;
        [NonSerialized]
        public bool unitialized = true;

        public AnimationWindowPolicy()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new SynchronizeGeometryDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__0);
            }
            this.SynchronizeGeometry = (SynchronizeGeometryDelegate) Delegate.Combine(this.SynchronizeGeometry, <>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new SynchronizeFrameRateDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__1);
            }
            this.SynchronizeFrameRate = (SynchronizeFrameRateDelegate) Delegate.Combine(this.SynchronizeFrameRate, <>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new SynchronizeCurrentTimeDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__2);
            }
            this.SynchronizeCurrentTime = (SynchronizeCurrentTimeDelegate) Delegate.Combine(this.SynchronizeCurrentTime, <>f__am$cache2);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new SynchronizeZoomableAreaDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__3);
            }
            this.SynchronizeZoomableArea = (SynchronizeZoomableAreaDelegate) Delegate.Combine(this.SynchronizeZoomableArea, <>f__am$cache3);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new OnGeometryChangeDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__4);
            }
            this.OnGeometryChange = (OnGeometryChangeDelegate) Delegate.Combine(this.OnGeometryChange, <>f__am$cache4);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new OnCurrentTimeChangeDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__5);
            }
            this.OnCurrentTimeChange = (OnCurrentTimeChangeDelegate) Delegate.Combine(this.OnCurrentTimeChange, <>f__am$cache5);
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new OnZoomableAreaChangeDelegate(AnimationWindowPolicy.<AnimationWindowPolicy>m__6);
            }
            this.OnZoomableAreaChange = (OnZoomableAreaChangeDelegate) Delegate.Combine(this.OnZoomableAreaChange, <>f__am$cache6);
        }

        [CompilerGenerated]
        private static bool <AnimationWindowPolicy>m__0(ref int[] sizes, ref int[] minSizes)
        {
            return false;
        }

        [CompilerGenerated]
        private static bool <AnimationWindowPolicy>m__1(ref float frameRate)
        {
            return false;
        }

        [CompilerGenerated]
        private static bool <AnimationWindowPolicy>m__2(ref float time)
        {
            return false;
        }

        [CompilerGenerated]
        private static bool <AnimationWindowPolicy>m__3(ref float horizontalScale, ref float horizontalTranslation)
        {
            return false;
        }

        [CompilerGenerated]
        private static void <AnimationWindowPolicy>m__4(int[] sizes)
        {
        }

        [CompilerGenerated]
        private static void <AnimationWindowPolicy>m__5(float time)
        {
        }

        [CompilerGenerated]
        private static void <AnimationWindowPolicy>m__6(float horizontalScale, float horizontalTranslation)
        {
        }

        public delegate void OnCurrentTimeChangeDelegate(float time);

        public delegate void OnGeometryChangeDelegate(int[] sizes);

        public delegate void OnZoomableAreaChangeDelegate(float horizontalScale, float horizontalTranslation);

        public delegate bool SynchronizeCurrentTimeDelegate(ref float time);

        public delegate bool SynchronizeFrameRateDelegate(ref float frameRate);

        public delegate bool SynchronizeGeometryDelegate(ref int[] sizes, ref int[] minSizes);

        public delegate bool SynchronizeZoomableAreaDelegate(ref float horizontalScale, ref float horizontalTranslation);
    }
}


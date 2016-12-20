namespace UnityEngine.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI.Collections;

    /// <summary>
    /// <para>EditorOnly class for tracking all Graphics.</para>
    /// </summary>
    public static class GraphicRebuildTracker
    {
        [CompilerGenerated]
        private static CanvasRenderer.OnRequestRebuild <>f__mg$cache0;
        private static IndexedSet<Graphic> m_Tracked = new IndexedSet<Graphic>();
        private static bool s_Initialized;

        private static void OnRebuildRequested()
        {
            StencilMaterial.ClearAll();
            for (int i = 0; i < m_Tracked.Count; i++)
            {
                m_Tracked[i].OnRebuildRequested();
            }
        }

        /// <summary>
        /// <para>Track a Graphic.</para>
        /// </summary>
        /// <param name="g"></param>
        public static void TrackGraphic(Graphic g)
        {
            if (!s_Initialized)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new CanvasRenderer.OnRequestRebuild(GraphicRebuildTracker.OnRebuildRequested);
                }
                CanvasRenderer.onRequestRebuild += <>f__mg$cache0;
                s_Initialized = true;
            }
            m_Tracked.AddUnique(g);
        }

        /// <summary>
        /// <para>Untrack a Graphic.</para>
        /// </summary>
        /// <param name="g"></param>
        public static void UnTrackGraphic(Graphic g)
        {
            m_Tracked.Remove(g);
        }
    }
}


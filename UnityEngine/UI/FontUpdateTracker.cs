namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Utility class that is used to help with Text update.</para>
    /// </summary>
    public static class FontUpdateTracker
    {
        [CompilerGenerated]
        private static Action<Font> <>f__mg$cache0;
        [CompilerGenerated]
        private static Action<Font> <>f__mg$cache1;
        private static Dictionary<Font, HashSet<Text>> m_Tracked = new Dictionary<Font, HashSet<Text>>();

        private static void RebuildForFont(Font f)
        {
            HashSet<Text> set;
            m_Tracked.TryGetValue(f, out set);
            if (set != null)
            {
                foreach (Text text in set)
                {
                    text.FontTextureChanged();
                }
            }
        }

        /// <summary>
        /// <para>Register a Text element for receiving texture atlas rebuild calls.</para>
        /// </summary>
        /// <param name="t"></param>
        public static void TrackText(Text t)
        {
            if (t.font != null)
            {
                HashSet<Text> set;
                m_Tracked.TryGetValue(t.font, out set);
                if (set == null)
                {
                    if (m_Tracked.Count == 0)
                    {
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new Action<Font>(FontUpdateTracker.RebuildForFont);
                        }
                        Font.textureRebuilt += <>f__mg$cache0;
                    }
                    set = new HashSet<Text>();
                    m_Tracked.Add(t.font, set);
                }
                if (!set.Contains(t))
                {
                    set.Add(t);
                }
            }
        }

        /// <summary>
        /// <para>Deregister a Text element from receiving texture atlas rebuild calls.</para>
        /// </summary>
        /// <param name="t"></param>
        public static void UntrackText(Text t)
        {
            if (t.font != null)
            {
                HashSet<Text> set;
                m_Tracked.TryGetValue(t.font, out set);
                if (set != null)
                {
                    set.Remove(t);
                    if (set.Count == 0)
                    {
                        m_Tracked.Remove(t.font);
                        if (m_Tracked.Count == 0)
                        {
                            if (<>f__mg$cache1 == null)
                            {
                                <>f__mg$cache1 = new Action<Font>(FontUpdateTracker.RebuildForFont);
                            }
                            Font.textureRebuilt -= <>f__mg$cache1;
                        }
                    }
                }
            }
        }
    }
}


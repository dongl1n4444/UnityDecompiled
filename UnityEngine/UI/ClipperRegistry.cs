namespace UnityEngine.UI
{
    using System;
    using UnityEngine.UI.Collections;

    /// <summary>
    /// <para>Registry class to keep track of all IClippers that exist in the scene.</para>
    /// </summary>
    public class ClipperRegistry
    {
        private readonly IndexedSet<IClipper> m_Clippers = new IndexedSet<IClipper>();
        private static ClipperRegistry s_Instance;

        protected ClipperRegistry()
        {
        }

        /// <summary>
        /// <para>Perform the clipping on all registered IClipper.</para>
        /// </summary>
        public void Cull()
        {
            for (int i = 0; i < this.m_Clippers.Count; i++)
            {
                this.m_Clippers[i].PerformClipping();
            }
        }

        /// <summary>
        /// <para>Register an IClipper.</para>
        /// </summary>
        /// <param name="c"></param>
        public static void Register(IClipper c)
        {
            if (c != null)
            {
                instance.m_Clippers.AddUnique(c);
            }
        }

        /// <summary>
        /// <para>Unregister an IClipper.</para>
        /// </summary>
        /// <param name="c"></param>
        public static void Unregister(IClipper c)
        {
            instance.m_Clippers.Remove(c);
        }

        /// <summary>
        /// <para>Singleton instance.</para>
        /// </summary>
        public static ClipperRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ClipperRegistry();
                }
                return s_Instance;
            }
        }
    }
}


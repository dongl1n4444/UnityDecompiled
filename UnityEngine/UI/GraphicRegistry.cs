namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI.Collections;

    /// <summary>
    /// <para>Registry which maps a Graphic to the canvas it belongs to.</para>
    /// </summary>
    public class GraphicRegistry
    {
        private readonly Dictionary<Canvas, IndexedSet<Graphic>> m_Graphics = new Dictionary<Canvas, IndexedSet<Graphic>>();
        private static readonly List<Graphic> s_EmptyList = new List<Graphic>();
        private static GraphicRegistry s_Instance;

        protected GraphicRegistry()
        {
        }

        /// <summary>
        /// <para>Return a list of Graphics that are registered on the Canvas.</para>
        /// </summary>
        /// <param name="canvas">Input canvas.</param>
        /// <returns>
        /// <para>Graphics on the input canvas.</para>
        /// </returns>
        public static IList<Graphic> GetGraphicsForCanvas(Canvas canvas)
        {
            IndexedSet<Graphic> set;
            if (instance.m_Graphics.TryGetValue(canvas, out set))
            {
                return set;
            }
            return s_EmptyList;
        }

        /// <summary>
        /// <para>Store a link between the given canvas and graphic in the registry.</para>
        /// </summary>
        /// <param name="c">Canvas to register.</param>
        /// <param name="graphic">Graphic to register.</param>
        public static void RegisterGraphicForCanvas(Canvas c, Graphic graphic)
        {
            if (c != null)
            {
                IndexedSet<Graphic> set;
                instance.m_Graphics.TryGetValue(c, out set);
                if (set != null)
                {
                    set.AddUnique(graphic);
                }
                else
                {
                    set = new IndexedSet<Graphic> {
                        graphic
                    };
                    instance.m_Graphics.Add(c, set);
                }
            }
        }

        /// <summary>
        /// <para>Deregister the given Graphic from a Canvas.</para>
        /// </summary>
        /// <param name="c">Canvas.</param>
        /// <param name="graphic">Graphic to deregister.</param>
        public static void UnregisterGraphicForCanvas(Canvas c, Graphic graphic)
        {
            IndexedSet<Graphic> set;
            if ((c != null) && instance.m_Graphics.TryGetValue(c, out set))
            {
                set.Remove(graphic);
                if (set.Count == 0)
                {
                    instance.m_Graphics.Remove(c);
                }
            }
        }

        /// <summary>
        /// <para>Singleton instance.</para>
        /// </summary>
        public static GraphicRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new GraphicRegistry();
                }
                return s_Instance;
            }
        }
    }
}


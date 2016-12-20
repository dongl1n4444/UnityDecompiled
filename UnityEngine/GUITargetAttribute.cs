namespace UnityEngine
{
    using System;
    using System.Reflection;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Allows to control for which display the OnGUI is called.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GUITargetAttribute : Attribute
    {
        internal int displayMask;

        /// <summary>
        /// <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
        /// </summary>
        /// <param name="displayIndex">Display index.</param>
        /// <param name="displayIndex1">Display index.</param>
        /// <param name="displayIndexList">Display index list.</param>
        public GUITargetAttribute()
        {
            this.displayMask = -1;
        }

        /// <summary>
        /// <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
        /// </summary>
        /// <param name="displayIndex">Display index.</param>
        /// <param name="displayIndex1">Display index.</param>
        /// <param name="displayIndexList">Display index list.</param>
        public GUITargetAttribute(int displayIndex)
        {
            this.displayMask = ((int) 1) << displayIndex;
        }

        /// <summary>
        /// <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
        /// </summary>
        /// <param name="displayIndex">Display index.</param>
        /// <param name="displayIndex1">Display index.</param>
        /// <param name="displayIndexList">Display index list.</param>
        public GUITargetAttribute(int displayIndex, int displayIndex1)
        {
            this.displayMask = (((int) 1) << displayIndex) | (((int) 1) << displayIndex1);
        }

        /// <summary>
        /// <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
        /// </summary>
        /// <param name="displayIndex">Display index.</param>
        /// <param name="displayIndex1">Display index.</param>
        /// <param name="displayIndexList">Display index list.</param>
        public GUITargetAttribute(int displayIndex, int displayIndex1, params int[] displayIndexList)
        {
            this.displayMask = (((int) 1) << displayIndex) | (((int) 1) << displayIndex1);
            for (int i = 0; i < displayIndexList.Length; i++)
            {
                this.displayMask |= ((int) 1) << displayIndexList[i];
            }
        }

        [RequiredByNativeCode]
        private static int GetGUITargetAttrValue(System.Type klass, string methodName)
        {
            MethodInfo method = klass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                object[] customAttributes = method.GetCustomAttributes(true);
                if (customAttributes != null)
                {
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        if (customAttributes[i].GetType() == typeof(GUITargetAttribute))
                        {
                            GUITargetAttribute attribute = customAttributes[i] as GUITargetAttribute;
                            return attribute.displayMask;
                        }
                    }
                }
            }
            return -1;
        }
    }
}


namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>LayerMask allow you to display the LayerMask popup menu in the inspector.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct LayerMask
    {
        private int m_Mask;
        public static implicit operator int(LayerMask mask) => 
            mask.m_Mask;

        public static implicit operator LayerMask(int intVal)
        {
            LayerMask mask;
            mask.m_Mask = intVal;
            return mask;
        }

        /// <summary>
        /// <para>Converts a layer mask value to an integer value.</para>
        /// </summary>
        public int value
        {
            get => 
                this.m_Mask;
            set
            {
                this.m_Mask = value;
            }
        }
        /// <summary>
        /// <para>Given a layer number, returns the name of the layer as defined in either a Builtin or a User Layer in the.</para>
        /// </summary>
        /// <param name="layer"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string LayerToName(int layer);
        /// <summary>
        /// <para>Given a layer name, returns the layer index as defined by either a Builtin or a User Layer in the.</para>
        /// </summary>
        /// <param name="layerName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int NameToLayer(string layerName);
        /// <summary>
        /// <para>Given a set of layer names as defined by either a Builtin or a User Layer in the, returns the equivalent layer mask for all of them.</para>
        /// </summary>
        /// <param name="layerNames">List of layer names to convert to a layer mask.</param>
        /// <returns>
        /// <para>The layer mask created from the layerNames.</para>
        /// </returns>
        public static int GetMask(params string[] layerNames)
        {
            if (layerNames == null)
            {
                throw new ArgumentNullException("layerNames");
            }
            int num = 0;
            foreach (string str in layerNames)
            {
                int num3 = NameToLayer(str);
                if (num3 != -1)
                {
                    num |= ((int) 1) << num3;
                }
            }
            return num;
        }
    }
}


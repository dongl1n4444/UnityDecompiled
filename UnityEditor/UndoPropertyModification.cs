namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>See Also: Undo.postprocessModifications.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct UndoPropertyModification
    {
        public PropertyModification previousValue;
        public PropertyModification currentValue;
        private int m_KeepPrefabOverride;
        public bool keepPrefabOverride
        {
            get => 
                (this.m_KeepPrefabOverride != 0);
            set
            {
                this.m_KeepPrefabOverride = !value ? 0 : 1;
            }
        }
    }
}


namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A class you can derive from if you want to create objects that don't need to be attached to game objects.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class ScriptableObject : UnityEngine.Object
    {
        public ScriptableObject()
        {
            Internal_CreateScriptableObject(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateScriptableObject([Writable] ScriptableObject self);
        [Obsolete("Use EditorUtility.SetDirty instead")]
        public void SetDirty()
        {
            INTERNAL_CALL_SetDirty(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetDirty(ScriptableObject self);
        /// <summary>
        /// <para>Creates an instance of a scriptable object.</para>
        /// </summary>
        /// <param name="className">The type of the ScriptableObject to create, as the name of the type.</param>
        /// <param name="type">The type of the ScriptableObject to create, as a System.Type instance.</param>
        /// <returns>
        /// <para>The created ScriptableObject.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern ScriptableObject CreateInstance(string className);
        /// <summary>
        /// <para>Creates an instance of a scriptable object.</para>
        /// </summary>
        /// <param name="className">The type of the ScriptableObject to create, as the name of the type.</param>
        /// <param name="type">The type of the ScriptableObject to create, as a System.Type instance.</param>
        /// <returns>
        /// <para>The created ScriptableObject.</para>
        /// </returns>
        public static ScriptableObject CreateInstance(System.Type type) => 
            CreateInstanceFromType(type);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern ScriptableObject CreateInstanceFromType(System.Type type);
        public static T CreateInstance<T>() where T: ScriptableObject => 
            ((T) CreateInstance(typeof(T)));
    }
}


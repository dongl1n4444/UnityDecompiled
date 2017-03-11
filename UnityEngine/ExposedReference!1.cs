namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Creates a type whos value is resolvable at runtime.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential), UsedByNativeCode(Name="ExposedReference")]
    public struct ExposedReference<T> where T: UnityEngine.Object
    {
        [SerializeField]
        public PropertyName exposedName;
        [SerializeField]
        public UnityEngine.Object defaultValue;
        public T Resolve(ExposedPropertyResolver resolver)
        {
            bool flag;
            UnityEngine.Object obj2 = ExposedPropertyResolver.ResolveReferenceInternal(resolver.table, this.exposedName, out flag);
            if (flag)
            {
                return (obj2 as T);
            }
            return (this.defaultValue as T);
        }
    }
}


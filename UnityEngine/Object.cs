namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>Base class for all objects Unity can reference.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Object
    {
        private IntPtr m_CachedPtr;
        private int m_InstanceID;
        private string m_UnityRuntimeErrorString;
        internal static int OffsetOfInstanceIDInCPlusPlusObject = -1;
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern UnityEngine.Object Internal_CloneSingle(UnityEngine.Object data);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern UnityEngine.Object Internal_CloneSingleWithParent(UnityEngine.Object data, Transform parent, bool worldPositionStays);
        [ThreadAndSerializationSafe]
        private static UnityEngine.Object Internal_InstantiateSingle(UnityEngine.Object data, Vector3 pos, Quaternion rot) => 
            INTERNAL_CALL_Internal_InstantiateSingle(data, ref pos, ref rot);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern UnityEngine.Object INTERNAL_CALL_Internal_InstantiateSingle(UnityEngine.Object data, ref Vector3 pos, ref Quaternion rot);
        private static UnityEngine.Object Internal_InstantiateSingleWithParent(UnityEngine.Object data, Transform parent, Vector3 pos, Quaternion rot) => 
            INTERNAL_CALL_Internal_InstantiateSingleWithParent(data, parent, ref pos, ref rot);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern UnityEngine.Object INTERNAL_CALL_Internal_InstantiateSingleWithParent(UnityEngine.Object data, Transform parent, ref Vector3 pos, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void EnsureRunningOnMainThread();
        /// <summary>
        /// <para>Removes a gameobject, component or asset.</para>
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        /// <param name="t">The optional amount of time to delay before destroying the object.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Destroy(UnityEngine.Object obj, [DefaultValue("0.0F")] float t);
        /// <summary>
        /// <para>Removes a gameobject, component or asset.</para>
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        /// <param name="t">The optional amount of time to delay before destroying the object.</param>
        [ExcludeFromDocs]
        public static void Destroy(UnityEngine.Object obj)
        {
            float t = 0f;
            Destroy(obj, t);
        }

        /// <summary>
        /// <para>Destroys the object obj immediately.</para>
        /// </summary>
        /// <param name="obj">Object to be destroyed.</param>
        /// <param name="allowDestroyingAssets">Set to true to allow assets to be destoyed.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DestroyImmediate(UnityEngine.Object obj, [DefaultValue("false")] bool allowDestroyingAssets);
        /// <summary>
        /// <para>Destroys the object obj immediately.</para>
        /// </summary>
        /// <param name="obj">Object to be destroyed.</param>
        /// <param name="allowDestroyingAssets">Set to true to allow assets to be destoyed.</param>
        [ExcludeFromDocs]
        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            bool allowDestroyingAssets = false;
            DestroyImmediate(obj, allowDestroyingAssets);
        }

        /// <summary>
        /// <para>Returns a list of all active loaded objects of Type type.</para>
        /// </summary>
        /// <param name="type">The type of object to find.</param>
        /// <returns>
        /// <para>The array of objects found matching the type specified.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
        public static extern UnityEngine.Object[] FindObjectsOfType(System.Type type);
        /// <summary>
        /// <para>The name of the object.</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Makes the object target not be destroyed automatically when loading a new scene.</para>
        /// </summary>
        /// <param name="target"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DontDestroyOnLoad(UnityEngine.Object target);
        /// <summary>
        /// <para>Should the object be hidden, saved with the scene or modifiable by the user?</para>
        /// </summary>
        public HideFlags hideFlags { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DestroyObject(UnityEngine.Object obj, [DefaultValue("0.0F")] float t);
        [ExcludeFromDocs]
        public static void DestroyObject(UnityEngine.Object obj)
        {
            float t = 0f;
            DestroyObject(obj, t);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use Object.FindObjectsOfType instead.")]
        public static extern UnityEngine.Object[] FindSceneObjectsOfType(System.Type type);
        /// <summary>
        /// <para>Returns a list of all active and inactive loaded objects of Type type, including assets.</para>
        /// </summary>
        /// <param name="type">The type of object or asset to find.</param>
        /// <returns>
        /// <para>The array of objects and assets found matching the type specified.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
        public static extern UnityEngine.Object[] FindObjectsOfTypeIncludingAssets(System.Type type);
        /// <summary>
        /// <para>Returns a list of all active and inactive loaded objects of Type type.</para>
        /// </summary>
        /// <param name="type">The type of object to find.</param>
        /// <returns>
        /// <para>The array of objects found matching the type specified.</para>
        /// </returns>
        [Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
        public static UnityEngine.Object[] FindObjectsOfTypeAll(System.Type type) => 
            Resources.FindObjectsOfTypeAll(type);

        /// <summary>
        /// <para>Returns the name of the game object.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public override extern string ToString();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);
        /// <summary>
        /// <para>Returns the instance id of the object.</para>
        /// </summary>
        [SecuritySafeCritical]
        public int GetInstanceID()
        {
            this.EnsureRunningOnMainThread();
            return this.m_InstanceID;
        }

        public override int GetHashCode() => 
            this.m_InstanceID;

        public override bool Equals(object other)
        {
            UnityEngine.Object rhs = other as UnityEngine.Object;
            if (((rhs == null) && (other != null)) && !(other is UnityEngine.Object))
            {
                return false;
            }
            return CompareBaseObjects(this, rhs);
        }

        public static implicit operator bool(UnityEngine.Object exists) => 
            !CompareBaseObjects(exists, null);

        private static bool CompareBaseObjects(UnityEngine.Object lhs, UnityEngine.Object rhs)
        {
            bool flag = lhs == null;
            bool flag2 = rhs == null;
            if (flag2 && flag)
            {
                return true;
            }
            if (flag2)
            {
                return !IsNativeObjectAlive(lhs);
            }
            if (flag)
            {
                return !IsNativeObjectAlive(rhs);
            }
            return (lhs.m_InstanceID == rhs.m_InstanceID);
        }

        private static bool IsNativeObjectAlive(UnityEngine.Object o)
        {
            if (o.GetCachedPtr() != IntPtr.Zero)
            {
                return true;
            }
            if ((o is MonoBehaviour) || (o is ScriptableObject))
            {
                return false;
            }
            return DoesObjectWithInstanceIDExist(o.GetInstanceID());
        }

        private IntPtr GetCachedPtr() => 
            this.m_CachedPtr;

        /// <summary>
        /// <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <param name="instantiateInWorldSpace">If when assigning the parent the original world position should be maintained.</param>
        /// <returns>
        /// <para>The instantiated clone.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation)
        {
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            if (original is ScriptableObject)
            {
                throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
            }
            return Internal_InstantiateSingle(original, position, rotation);
        }

        /// <summary>
        /// <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <param name="instantiateInWorldSpace">If when assigning the parent the original world position should be maintained.</param>
        /// <returns>
        /// <para>The instantiated clone.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (parent == null)
            {
                return Internal_InstantiateSingle(original, position, rotation);
            }
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_InstantiateSingleWithParent(original, parent, position, rotation);
        }

        /// <summary>
        /// <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <param name="instantiateInWorldSpace">If when assigning the parent the original world position should be maintained.</param>
        /// <returns>
        /// <para>The instantiated clone.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original)
        {
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_CloneSingle(original);
        }

        /// <summary>
        /// <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <param name="instantiateInWorldSpace">If when assigning the parent the original world position should be maintained.</param>
        /// <returns>
        /// <para>The instantiated clone.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent) => 
            Instantiate(original, parent, false);

        /// <summary>
        /// <para>Clones the object original and returns the clone.</para>
        /// </summary>
        /// <param name="original">An existing object that you want to make a copy of.</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">Parent that will be assigned to the new object.</param>
        /// <param name="instantiateInWorldSpace">If when assigning the parent the original world position should be maintained.</param>
        /// <returns>
        /// <para>The instantiated clone.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent, bool instantiateInWorldSpace)
        {
            if (parent == null)
            {
                return Internal_CloneSingle(original);
            }
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
        }

        public static T Instantiate<T>(T original) where T: UnityEngine.Object
        {
            CheckNullArgument(original, "The Object you want to instantiate is null.");
            return (T) Internal_CloneSingle(original);
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T: UnityEngine.Object => 
            ((T) Instantiate(original, position, rotation));

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T: UnityEngine.Object => 
            ((T) Instantiate(original, position, rotation, parent));

        public static T Instantiate<T>(T original, Transform parent) where T: UnityEngine.Object => 
            Instantiate<T>(original, parent, false);

        public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T: UnityEngine.Object => 
            ((T) Instantiate(original, parent, worldPositionStays));

        public static T[] FindObjectsOfType<T>() where T: UnityEngine.Object => 
            Resources.ConvertObjects<T>(FindObjectsOfType(typeof(T)));

        public static T FindObjectOfType<T>() where T: UnityEngine.Object => 
            ((T) FindObjectOfType(typeof(T)));

        private static void CheckNullArgument(object arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// <para>Returns the first active loaded object of Type type.</para>
        /// </summary>
        /// <param name="type">The type of object to find.</param>
        /// <returns>
        /// <para>An array of objects which matched the specified type, cast as Object.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static UnityEngine.Object FindObjectOfType(System.Type type)
        {
            UnityEngine.Object[] objArray = FindObjectsOfType(type);
            if (objArray.Length > 0)
            {
                return objArray[0];
            }
            return null;
        }

        public static bool operator ==(UnityEngine.Object x, UnityEngine.Object y) => 
            CompareBaseObjects(x, y);

        public static bool operator !=(UnityEngine.Object x, UnityEngine.Object y) => 
            !CompareBaseObjects(x, y);
    }
}


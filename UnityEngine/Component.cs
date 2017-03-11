namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Security;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>Base class for everything attached to GameObjects.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class Component : UnityEngine.Object
    {
        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object or any of its children.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="parameter">Optional parameter to pass to the method (can be any value).</param>
        /// <param name="options">Should an error be raised if the method does not exist for a given target object?</param>
        [ExcludeFromDocs]
        public void BroadcastMessage(string methodName)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            object parameter = null;
            this.BroadcastMessage(methodName, parameter, requireReceiver);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object or any of its children.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="parameter">Optional parameter to pass to the method (can be any value).</param>
        /// <param name="options">Should an error be raised if the method does not exist for a given target object?</param>
        [ExcludeFromDocs]
        public void BroadcastMessage(string methodName, object parameter)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            this.BroadcastMessage(methodName, parameter, requireReceiver);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object or any of its children.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="parameter">Optional parameter to pass to the method (can be any value).</param>
        /// <param name="options">Should an error be raised if the method does not exist for a given target object?</param>
        public void BroadcastMessage(string methodName, SendMessageOptions options)
        {
            this.BroadcastMessage(methodName, null, options);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object or any of its children.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="parameter">Optional parameter to pass to the method (can be any value).</param>
        /// <param name="options">Should an error be raised if the method does not exist for a given target object?</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        /// <summary>
        /// <para>Is this game object tagged with tag ?</para>
        /// </summary>
        /// <param name="tag">The tag to compare.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool CompareTag(string tag);
        [SecuritySafeCritical]
        public unsafe T GetComponent<T>()
        {
            CastHelper<T> helper = new CastHelper<T>();
            this.GetComponentFastPath(typeof(T), new IntPtr((void*) &helper.onePointerFurtherThanT));
            return helper.t;
        }

        /// <summary>
        /// <para>Returns the component with name type if the game object has one attached, null if it doesn't.</para>
        /// </summary>
        /// <param name="type"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Component GetComponent(string type);
        /// <summary>
        /// <para>Returns the component of Type type if the game object has one attached, null if it doesn't.</para>
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponent(System.Type type) => 
            this.gameObject.GetComponent(type);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void GetComponentFastPath(System.Type type, IntPtr oneFurtherThanResultValue);
        [ExcludeFromDocs]
        public T GetComponentInChildren<T>()
        {
            bool includeInactive = false;
            return this.GetComponentInChildren<T>(includeInactive);
        }

        public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive) => 
            ((T) this.GetComponentInChildren(typeof(T), includeInactive));

        /// <summary>
        /// <para>Returns the component of Type type in the GameObject or any of its children using depth first search.</para>
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <returns>
        /// <para>A component of the matching type, if found.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponentInChildren(System.Type t) => 
            this.GetComponentInChildren(t, false);

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponentInChildren(System.Type t, bool includeInactive) => 
            this.gameObject.GetComponentInChildren(t, includeInactive);

        public T GetComponentInParent<T>() => 
            ((T) this.GetComponentInParent(typeof(T)));

        /// <summary>
        /// <para>Returns the component of Type type in the GameObject or any of its parents.</para>
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <returns>
        /// <para>A component of the matching type, if found.</para>
        /// </returns>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponentInParent(System.Type t) => 
            this.gameObject.GetComponentInParent(t);

        public T[] GetComponents<T>() => 
            this.gameObject.GetComponents<T>();

        public void GetComponents<T>(List<T> results)
        {
            this.GetComponentsForListInternal(typeof(T), results);
        }

        /// <summary>
        /// <para>Returns all components of Type type in the GameObject.</para>
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        public Component[] GetComponents(System.Type type) => 
            this.gameObject.GetComponents(type);

        public void GetComponents(System.Type type, List<Component> results)
        {
            this.GetComponentsForListInternal(type, results);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetComponentsForListInternal(System.Type searchType, object resultList);
        public T[] GetComponentsInChildren<T>() => 
            this.GetComponentsInChildren<T>(false);

        public T[] GetComponentsInChildren<T>(bool includeInactive) => 
            this.gameObject.GetComponentsInChildren<T>(includeInactive);

        public void GetComponentsInChildren<T>(List<T> results)
        {
            this.GetComponentsInChildren<T>(false, results);
        }

        /// <summary>
        /// <para>Returns all components of Type type in the GameObject or any of its children.</para>
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set?</param>
        [ExcludeFromDocs]
        public Component[] GetComponentsInChildren(System.Type t)
        {
            bool includeInactive = false;
            return this.GetComponentsInChildren(t, includeInactive);
        }

        public void GetComponentsInChildren<T>(bool includeInactive, List<T> result)
        {
            this.gameObject.GetComponentsInChildren<T>(includeInactive, result);
        }

        /// <summary>
        /// <para>Returns all components of Type type in the GameObject or any of its children.</para>
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set?</param>
        public Component[] GetComponentsInChildren(System.Type t, [DefaultValue("false")] bool includeInactive) => 
            this.gameObject.GetComponentsInChildren(t, includeInactive);

        public T[] GetComponentsInParent<T>() => 
            this.GetComponentsInParent<T>(false);

        public T[] GetComponentsInParent<T>(bool includeInactive) => 
            this.gameObject.GetComponentsInParent<T>(includeInactive);

        [ExcludeFromDocs]
        public Component[] GetComponentsInParent(System.Type t)
        {
            bool includeInactive = false;
            return this.GetComponentsInParent(t, includeInactive);
        }

        public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
        {
            this.gameObject.GetComponentsInParent<T>(includeInactive, results);
        }

        /// <summary>
        /// <para>Returns all components of Type type in the GameObject or any of its parents.</para>
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <param name="includeInactive">Should inactive Components be included in the found set?</param>
        public Component[] GetComponentsInParent(System.Type t, [DefaultValue("false")] bool includeInactive) => 
            this.gameObject.GetComponentsInParent(t, includeInactive);

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="value">Optional parameter for the method.</param>
        /// <param name="options">Should an error be raised if the target object doesn't implement the method for the message?</param>
        [ExcludeFromDocs]
        public void SendMessage(string methodName)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            object obj2 = null;
            this.SendMessage(methodName, obj2, requireReceiver);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="value">Optional parameter for the method.</param>
        /// <param name="options">Should an error be raised if the target object doesn't implement the method for the message?</param>
        [ExcludeFromDocs]
        public void SendMessage(string methodName, object value)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            this.SendMessage(methodName, value, requireReceiver);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="value">Optional parameter for the method.</param>
        /// <param name="options">Should an error be raised if the target object doesn't implement the method for the message?</param>
        public void SendMessage(string methodName, SendMessageOptions options)
        {
            this.SendMessage(methodName, null, options);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object.</para>
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="value">Optional parameter for the method.</param>
        /// <param name="options">Should an error be raised if the target object doesn't implement the method for the message?</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SendMessage(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object and on every ancestor of the behaviour.</para>
        /// </summary>
        /// <param name="methodName">Name of method to call.</param>
        /// <param name="value">Optional parameter value for the method.</param>
        /// <param name="options">Should an error be raised if the method does not exist on the target object?</param>
        [ExcludeFromDocs]
        public void SendMessageUpwards(string methodName)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            object obj2 = null;
            this.SendMessageUpwards(methodName, obj2, requireReceiver);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object and on every ancestor of the behaviour.</para>
        /// </summary>
        /// <param name="methodName">Name of method to call.</param>
        /// <param name="value">Optional parameter value for the method.</param>
        /// <param name="options">Should an error be raised if the method does not exist on the target object?</param>
        [ExcludeFromDocs]
        public void SendMessageUpwards(string methodName, object value)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            this.SendMessageUpwards(methodName, value, requireReceiver);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object and on every ancestor of the behaviour.</para>
        /// </summary>
        /// <param name="methodName">Name of method to call.</param>
        /// <param name="value">Optional parameter value for the method.</param>
        /// <param name="options">Should an error be raised if the method does not exist on the target object?</param>
        public void SendMessageUpwards(string methodName, SendMessageOptions options)
        {
            this.SendMessageUpwards(methodName, null, options);
        }

        /// <summary>
        /// <para>Calls the method named methodName on every MonoBehaviour in this game object and on every ancestor of the behaviour.</para>
        /// </summary>
        /// <param name="methodName">Name of method to call.</param>
        /// <param name="value">Optional parameter value for the method.</param>
        /// <param name="options">Should an error be raised if the method does not exist on the target object?</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SendMessageUpwards(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

        /// <summary>
        /// <para>The Animation attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)", true)]
        public Component animation
        {
            get
            {
                throw new NotSupportedException("animation property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The AudioSource attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)", true)]
        public Component audio
        {
            get
            {
                throw new NotSupportedException("audio property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Camera attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)", true)]
        public Component camera
        {
            get
            {
                throw new NotSupportedException("camera property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Collider attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)", true)]
        public Component collider
        {
            get
            {
                throw new NotSupportedException("collider property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Collider2D component attached to the object.</para>
        /// </summary>
        [Obsolete("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)", true)]
        public Component collider2D
        {
            get
            {
                throw new NotSupportedException("collider2D property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The ConstantForce attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)", true)]
        public Component constantForce
        {
            get
            {
                throw new NotSupportedException("constantForce property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The game object this component is attached to. A component is always attached to a game object.</para>
        /// </summary>
        public GameObject gameObject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        [Obsolete("Property guiElement has been deprecated. Use GetComponent<GUIElement>() instead. (UnityUpgradable)", true)]
        public Component guiElement
        {
            get
            {
                throw new NotSupportedException("guiElement property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The GUIText attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property guiText has been deprecated. Use GetComponent<GUIText>() instead. (UnityUpgradable)", true)]
        public Component guiText
        {
            get
            {
                throw new NotSupportedException("guiText property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The GUITexture attached to this GameObject (Read Only). (null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property guiTexture has been deprecated. Use GetComponent<GUITexture>() instead. (UnityUpgradable)", true)]
        public Component guiTexture
        {
            get
            {
                throw new NotSupportedException("guiTexture property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The HingeJoint attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)", true)]
        public Component hingeJoint
        {
            get
            {
                throw new NotSupportedException("hingeJoint property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Light attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)", true)]
        public Component light
        {
            get
            {
                throw new NotSupportedException("light property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The NetworkView attached to this GameObject (Read Only). (null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)", true)]
        public Component networkView
        {
            get
            {
                throw new NotSupportedException("networkView property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The ParticleEmitter attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property particleEmitter has been deprecated. Use GetComponent<ParticleEmitter>() instead. (UnityUpgradable)", true)]
        public Component particleEmitter
        {
            get
            {
                throw new NotSupportedException("particleEmitter property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The ParticleSystem attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)", true)]
        public Component particleSystem
        {
            get
            {
                throw new NotSupportedException("particleSystem property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Renderer attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)", true)]
        public Component renderer
        {
            get
            {
                throw new NotSupportedException("renderer property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Rigidbody attached to this GameObject. (Null if there is none attached).</para>
        /// </summary>
        [Obsolete("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)", true)]
        public Component rigidbody
        {
            get
            {
                throw new NotSupportedException("rigidbody property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The Rigidbody2D that is attached to the Component's GameObject.</para>
        /// </summary>
        [Obsolete("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)", true)]
        public Component rigidbody2D
        {
            get
            {
                throw new NotSupportedException("rigidbody2D property has been deprecated");
            }
        }

        /// <summary>
        /// <para>The tag of this game object.</para>
        /// </summary>
        public string tag
        {
            get => 
                this.gameObject.tag;
            set
            {
                this.gameObject.tag = value;
            }
        }

        /// <summary>
        /// <para>The Transform attached to this GameObject.</para>
        /// </summary>
        public Transform transform { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}


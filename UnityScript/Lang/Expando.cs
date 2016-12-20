namespace UnityScript.Lang
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [Serializable]
    internal class Expando
    {
        protected Dictionary<string, object> _attributes = new Dictionary<string, object>();
        protected WeakReference _target;

        public Expando(object target)
        {
            this._target = new WeakReference(target);
        }

        public object this[string key]
        {
            get
            {
                return this._attributes[key];
            }
            set
            {
                if (value == null)
                {
                    this._attributes.Remove(key);
                }
                else
                {
                    this._attributes[key] = value;
                }
            }
        }

        public object Target
        {
            get
            {
                return this._target.Target;
            }
        }
    }
}


namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class SerializedPropertyDataStore
    {
        private Data[] m_Elements;
        private GatherDelegate m_GatherDel;
        private Object[] m_Objects;
        private string[] m_PropNames;

        public SerializedPropertyDataStore(string[] propNames, GatherDelegate gatherDel)
        {
            this.m_PropNames = propNames;
            this.m_GatherDel = gatherDel;
            this.Repopulate();
        }

        private void Clear()
        {
            for (int i = 0; i < this.m_Elements.Length; i++)
            {
                this.m_Elements[i].Dispose();
            }
            this.m_Objects = null;
            this.m_Elements = null;
        }

        ~SerializedPropertyDataStore()
        {
            this.Clear();
        }

        public Data[] GetElements() => 
            this.m_Elements;

        public bool Repopulate()
        {
            Object[] lhs = this.m_GatherDel();
            if (this.m_Objects != null)
            {
                if ((lhs.Length == this.m_Objects.Length) && ArrayUtility.ArrayReferenceEquals<Object>(lhs, this.m_Objects))
                {
                    return false;
                }
                this.Clear();
            }
            this.m_Objects = lhs;
            this.m_Elements = new Data[lhs.Length];
            for (int i = 0; i < lhs.Length; i++)
            {
                this.m_Elements[i] = new Data(lhs[i], this.m_PropNames);
            }
            return true;
        }

        internal class Data
        {
            private Object m_Object;
            private SerializedProperty[] m_Props;
            private SerializedObject m_SerializedObject;

            public Data(Object obj, string[] props)
            {
                this.m_Object = obj;
                this.m_SerializedObject = new SerializedObject(obj);
                this.m_Props = new SerializedProperty[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    this.m_Props[i] = this.m_SerializedObject.FindProperty(props[i]);
                }
            }

            public void Dispose()
            {
                foreach (SerializedProperty property in this.m_Props)
                {
                    if (property != null)
                    {
                        property.Dispose();
                    }
                }
                this.m_SerializedObject.Dispose();
                this.m_Object = null;
                this.m_SerializedObject = null;
                this.m_Props = null;
            }

            public void Load()
            {
                if (this.m_Object != null)
                {
                    this.m_SerializedObject.UpdateIfRequiredOrScript();
                }
            }

            public int ObjectId()
            {
                if (this.m_Object == null)
                {
                    return 0;
                }
                Component component = this.m_Object as Component;
                return ((component == null) ? this.m_Object.GetInstanceID() : component.gameObject.GetInstanceID());
            }

            public SerializedProperty[] Props() => 
                this.m_Props;

            public bool Refresh() => 
                ((this.m_Object != null) && this.m_SerializedObject.UpdateIfRequiredOrScript());

            public void Store()
            {
                if (this.m_Object != null)
                {
                    this.m_SerializedObject.ApplyModifiedProperties();
                }
            }

            public void Store(SerializedProperty prop)
            {
                if (this.m_Object != null)
                {
                    this.m_SerializedObject.CopyFromSerializedProperty(prop);
                    this.m_SerializedObject.ApplyModifiedProperties();
                }
            }

            public string name =>
                ((this.m_Object == null) ? string.Empty : this.m_Object.name);
        }

        internal delegate Object[] GatherDelegate();
    }
}


namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class PropertyHandlerCache
    {
        protected Dictionary<int, PropertyHandler> m_PropertyHandlers = new Dictionary<int, PropertyHandler>();

        public void Clear()
        {
            this.m_PropertyHandlers.Clear();
        }

        internal PropertyHandler GetHandler(SerializedProperty property)
        {
            PropertyHandler handler;
            int propertyHash = GetPropertyHash(property);
            if (this.m_PropertyHandlers.TryGetValue(propertyHash, out handler))
            {
                return handler;
            }
            return null;
        }

        private static int GetPropertyHash(SerializedProperty property)
        {
            if (property.serializedObject.targetObject == null)
            {
                return 0;
            }
            int num2 = property.serializedObject.targetObject.GetInstanceID() ^ property.hashCodeForPropertyPathWithoutArrayIndex;
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                num2 ^= property.objectReferenceInstanceIDValue;
            }
            return num2;
        }

        internal void SetHandler(SerializedProperty property, PropertyHandler handler)
        {
            int propertyHash = GetPropertyHash(property);
            this.m_PropertyHandlers[propertyHash] = handler;
        }
    }
}


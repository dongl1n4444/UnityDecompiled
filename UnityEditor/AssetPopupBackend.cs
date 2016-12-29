namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetPopupBackend
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache1;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache2;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache3;

        public static void AssetPopup<T>(SerializedProperty serializedProperty, GUIContent label, string fileExtension) where T: Object, new()
        {
            AssetPopup<T>(serializedProperty, label, fileExtension, "Default");
        }

        public static void AssetPopup<T>(SerializedProperty serializedProperty, GUIContent label, string fileExtension, string defaultFieldName) where T: Object, new()
        {
            GUIContent mixedValueContent;
            Rect rect;
            bool showMixedValue = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = serializedProperty.hasMultipleDifferentValues;
            string objectReferenceTypeString = serializedProperty.objectReferenceTypeString;
            if (serializedProperty.hasMultipleDifferentValues)
            {
                mixedValueContent = EditorGUI.mixedValueContent;
            }
            else if (serializedProperty.objectReferenceValue != null)
            {
                mixedValueContent = GUIContent.Temp(serializedProperty.objectReferenceStringValue);
            }
            else
            {
                mixedValueContent = GUIContent.Temp(defaultFieldName);
            }
            if (AudioMixerEffectGUI.PopupButton(label, mixedValueContent, EditorStyles.popup, out rect, new GUILayoutOption[0]))
            {
                ShowAssetsPopupMenu<T>(rect, objectReferenceTypeString, serializedProperty, fileExtension, defaultFieldName);
            }
            EditorGUI.showMixedValue = showMixedValue;
        }

        private static void AssetPopupMenuCallback(object userData)
        {
            object[] objArray = userData as object[];
            int instanceID = (int) objArray[0];
            SerializedProperty property = (SerializedProperty) objArray[1];
            property.objectReferenceValue = EditorUtility.InstanceIDToObject(instanceID);
            property.m_SerializedObject.ApplyModifiedProperties();
        }

        private static void ShowAssetsPopupMenu<T>(Rect buttonRect, string typeName, SerializedProperty serializedProperty, string fileExtension) where T: Object, new()
        {
            ShowAssetsPopupMenu<T>(buttonRect, typeName, serializedProperty, fileExtension, "Default");
        }

        private static void ShowAssetsPopupMenu<T>(Rect buttonRect, string typeName, SerializedProperty serializedProperty, string fileExtension, string defaultFieldName) where T: Object, new()
        {
            <ShowAssetsPopupMenu>c__AnonStorey1<T> storey = new <ShowAssetsPopupMenu>c__AnonStorey1<T> {
                typeName = typeName,
                fileExtension = fileExtension,
                serializedProperty = serializedProperty
            };
            GenericMenu menu = new GenericMenu();
            int num = (storey.serializedProperty.objectReferenceValue == null) ? 0 : storey.serializedProperty.objectReferenceValue.GetInstanceID();
            bool flag = false;
            int classID = BaseObjectTools.StringToClassID(storey.typeName);
            BuiltinResource[] builtinResourceList = null;
            if (classID > 0)
            {
                builtinResourceList = EditorGUIUtility.GetBuiltinResourceList(classID);
                BuiltinResource[] resourceArray2 = builtinResourceList;
                for (int i = 0; i < resourceArray2.Length; i++)
                {
                    <ShowAssetsPopupMenu>c__AnonStorey0<T> storey2 = new <ShowAssetsPopupMenu>c__AnonStorey0<T> {
                        resource = resourceArray2[i]
                    };
                    if (storey2.resource.m_Name == defaultFieldName)
                    {
                        if (<>f__mg$cache0 == null)
                        {
                            <>f__mg$cache0 = new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback);
                        }
                        object[] userData = new object[] { storey2.resource.m_InstanceID, storey.serializedProperty };
                        menu.AddItem(new GUIContent(storey2.resource.m_Name), storey2.resource.m_InstanceID == num, <>f__mg$cache0, userData);
                        builtinResourceList = Enumerable.Where<BuiltinResource>(builtinResourceList, new Func<BuiltinResource, bool>(storey2, (IntPtr) this.<>m__0)).ToArray<BuiltinResource>();
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback);
                }
                object[] objArray2 = new object[] { 0, storey.serializedProperty };
                menu.AddItem(new GUIContent(defaultFieldName), num == 0, <>f__mg$cache1, objArray2);
            }
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            SearchFilter filter2 = new SearchFilter();
            filter2.classNames = new string[] { storey.typeName };
            SearchFilter filter = filter2;
            property.SetSearchFilter(filter);
            property.Reset();
            while (property.Next(null))
            {
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback);
                }
                object[] objArray3 = new object[] { property.instanceID, storey.serializedProperty };
                menu.AddItem(new GUIContent(property.name), property.instanceID == num, <>f__mg$cache2, objArray3);
            }
            if ((classID > 0) && (builtinResourceList != null))
            {
                foreach (BuiltinResource resource in builtinResourceList)
                {
                    if (<>f__mg$cache3 == null)
                    {
                        <>f__mg$cache3 = new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback);
                    }
                    object[] objArray4 = new object[] { resource.m_InstanceID, storey.serializedProperty };
                    menu.AddItem(new GUIContent(resource.m_Name), resource.m_InstanceID == num, <>f__mg$cache3, objArray4);
                }
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Create New..."), false, new GenericMenu.MenuFunction(storey.<>m__0));
            menu.DropDown(buttonRect);
        }

        [CompilerGenerated]
        private sealed class <ShowAssetsPopupMenu>c__AnonStorey0<T> where T: Object, new()
        {
            internal BuiltinResource resource;

            internal bool <>m__0(BuiltinResource x) => 
                (x != this.resource);
        }

        [CompilerGenerated]
        private sealed class <ShowAssetsPopupMenu>c__AnonStorey1<T> where T: Object, new()
        {
            internal string fileExtension;
            internal SerializedProperty serializedProperty;
            internal string typeName;

            internal void <>m__0()
            {
                T asset = Activator.CreateInstance<T>();
                ProjectWindowUtil.CreateAsset(asset, "New " + this.typeName + "." + this.fileExtension);
                this.serializedProperty.objectReferenceValue = asset;
                this.serializedProperty.m_SerializedObject.ApplyModifiedProperties();
            }
        }
    }
}


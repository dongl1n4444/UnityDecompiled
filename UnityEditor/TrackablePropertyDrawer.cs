namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Analytics;

    [CustomPropertyDrawer(typeof(TrackableProperty), true)]
    internal class TrackablePropertyDrawer : PropertyDrawer
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cache0;
        [CompilerGenerated]
        private static Predicate<MemberInfo> <>f__am$cache1;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;
        private const int kExtraSpacing = 9;
        private GUIContent kNoFieldContent = new GUIContent("No Field");
        private SerializedProperty m_FieldsArray;
        private int m_LastSelectedIndex;
        private ReorderableList m_ReorderableList;
        private Dictionary<string, State> m_States = new Dictionary<string, State>();

        private void AddParam(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            this.m_LastSelectedIndex = list.index;
            SerializedProperty arrayElementAtIndex = this.m_FieldsArray.GetArrayElementAtIndex(list.index);
            SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("m_Target");
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_FieldPath");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("m_ParamName");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("m_DoStatic");
            if (list.index == 0)
            {
                property5.boolValue = false;
                property2.objectReferenceValue = null;
            }
            else
            {
                SerializedProperty property6 = this.m_FieldsArray.GetArrayElementAtIndex(list.index - 1);
                property5.boolValue = property6.FindPropertyRelative("m_DoStatic").boolValue;
                property2.objectReferenceValue = property6.FindPropertyRelative("m_Target").objectReferenceValue;
            }
            property3.stringValue = null;
            property4.stringValue = null;
            list.displayAdd = list.serializedProperty.arraySize < 10;
        }

        public GenericMenu BuildPopupList(UnityEngine.Object target, SerializedProperty field)
        {
            GameObject gameObject;
            if (target is Component)
            {
                gameObject = ((Component) target).gameObject;
            }
            else
            {
                gameObject = (GameObject) target;
            }
            string stringValue = field.FindPropertyRelative("m_FieldPath").stringValue;
            GenericMenu menu = new GenericMenu();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate {
                };
            }
            menu.AddItem(this.kNoFieldContent, string.IsNullOrEmpty(stringValue), <>f__am$cache0);
            if (gameObject != null)
            {
                menu.AddSeparator("");
                this.GeneratePopupForType(menu, gameObject, gameObject, field, "", 0);
                Component[] components = gameObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component != null)
                    {
                        this.GeneratePopupForType(menu, component, component, field, "", 0);
                    }
                }
            }
            return menu;
        }

        private static void ClearProperty(object source)
        {
            ((PropertySetter) source).Clear();
        }

        protected virtual void DrawHeader(Rect headerRect)
        {
            headerRect.height = 16f;
            GUI.Label(headerRect, "Parameters");
        }

        private void DrawParam(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty arrayElementAtIndex = this.m_FieldsArray.GetArrayElementAtIndex(index);
            rect.y++;
            Rect[] rowRects = this.GetRowRects(rect);
            Rect position = rowRects[0];
            Rect rect3 = rowRects[1];
            Rect rect4 = rowRects[2];
            Rect rect5 = rowRects[3];
            SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("m_ParamName");
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_Target");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("m_FieldPath");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("m_DoStatic");
            property5.boolValue = EditorGUI.ToggleLeft(rect3, "Static", property5.boolValue);
            EditorGUI.PropertyField(position, property, GUIContent.none);
            if (property5.boolValue)
            {
                SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("m_StaticString");
                property6.stringValue = EditorGUI.TextField(rect5, property6.stringValue);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                GUI.Box(rect4, GUIContent.none);
                EditorGUI.PropertyField(rect4, property3, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    property4.stringValue = null;
                }
                EditorGUI.BeginDisabledGroup(property3.objectReferenceValue == null);
                EditorGUI.BeginProperty(rect5, GUIContent.none, property4);
                StringBuilder builder = new StringBuilder();
                if ((property3.objectReferenceValue == null) || string.IsNullOrEmpty(property4.stringValue))
                {
                    builder.Append("No Field");
                }
                else
                {
                    builder.Append(property3.objectReferenceValue.GetType().Name);
                    if (!string.IsNullOrEmpty(property4.stringValue))
                    {
                        builder.Append(".");
                        builder.Append(property4.stringValue);
                    }
                }
                GUIContent content = new GUIContent(builder.ToString());
                if (GUI.Button(rect5, content, EditorStyles.popup))
                {
                    this.BuildPopupList(property3.objectReferenceValue, arrayElementAtIndex).DropDown(rect5);
                }
                EditorGUI.EndProperty();
                EditorGUI.EndDisabledGroup();
            }
        }

        private void EndDragChild(ReorderableList list)
        {
            this.m_LastSelectedIndex = list.index;
        }

        private void GeneratePopupForType(GenericMenu menu, UnityEngine.Object originalTarget, object target, SerializedProperty fieldProp, string prefix, int depth)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => (x.GetType().Name == "MonoProperty") || (x.GetType().Name == "MonoField");
            }
            MemberInfo[] infoArray = Array.FindAll<MemberInfo>(target.GetType().GetMembers(), <>f__am$cache1);
            foreach (MemberInfo info in infoArray)
            {
                string name = "";
                if (prefix == "")
                {
                    name = info.Name;
                }
                else
                {
                    name = prefix + "/" + info.Name;
                }
                System.Type type = info.GetType();
                type = (info.GetType().Name != "MonoField") ? ((PropertyInfo) info).PropertyType : ((System.Reflection.FieldInfo) info).FieldType;
                if (type.IsPrimitive || (type == typeof(string)))
                {
                    string fp = name.Replace("/", ".");
                    bool on = (fieldProp.FindPropertyRelative("m_Target").objectReferenceValue == originalTarget) && (fieldProp.FindPropertyRelative("m_FieldPath").stringValue == fp);
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new GenericMenu.MenuFunction2(TrackablePropertyDrawer.SetProperty);
                    }
                    menu.AddItem(new GUIContent(originalTarget.GetType().Name + "/" + name), on, <>f__mg$cache0, new PropertySetter(fieldProp, originalTarget, fp));
                }
                else if (((depth <= 1) && ((info.Name != "mesh") || (target.GetType().Name != "MeshFilter"))) && (((info.Name != "material") && (info.Name != "materials")) || !(target is Renderer)))
                {
                    object obj2 = GetValue(info, target);
                    if (obj2 != null)
                    {
                        this.GeneratePopupForType(menu, originalTarget, obj2, fieldProp, name, depth + 1);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.RestoreState(property);
            float height = 0f;
            if (this.m_ReorderableList != null)
            {
                height = this.m_ReorderableList.GetHeight();
            }
            return height;
        }

        private Rect[] GetRowRects(Rect rect)
        {
            Rect[] rectArray = new Rect[4];
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2f;
            Rect rect2 = rect;
            rect2.width *= 0.3f;
            Rect rect3 = rect2;
            rect3.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Rect rect4 = rect;
            rect4.xMin = rect3.xMax + EditorGUIUtility.standardVerticalSpacing;
            Rect rect5 = rect4;
            rect5.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            rectArray[0] = rect2;
            rectArray[1] = rect3;
            rectArray[2] = rect4;
            rectArray[3] = rect5;
            return rectArray;
        }

        private State GetState(SerializedProperty prop)
        {
            State state;
            string propertyPath = prop.propertyPath;
            this.m_States.TryGetValue(propertyPath, out state);
            if (state == null)
            {
                state = new State();
                SerializedProperty elements = prop.FindPropertyRelative("m_Fields");
                state.m_ReorderableList = new ReorderableList(prop.serializedObject, elements, false, true, true, true);
                state.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawHeader);
                state.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawParam);
                state.m_ReorderableList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectParam);
                state.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
                state.m_ReorderableList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddParam);
                state.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
                state.m_ReorderableList.elementHeight = ((EditorGUIUtility.singleLineHeight * 2f) + EditorGUIUtility.standardVerticalSpacing) + 9f;
                state.m_ReorderableList.index = 0;
                this.m_States[propertyPath] = state;
            }
            return state;
        }

        public static object GetValue(MemberInfo m, object v)
        {
            object obj2 = null;
            try
            {
                obj2 = !(m is System.Reflection.FieldInfo) ? ((PropertyInfo) m).GetValue(v, null) : ((System.Reflection.FieldInfo) m).GetValue(v);
            }
            catch (TargetInvocationException)
            {
            }
            catch (TargetParameterCountException)
            {
            }
            return obj2;
        }

        private void OnGUI(Rect position)
        {
            if (((this.m_FieldsArray != null) && this.m_FieldsArray.isArray) && (this.m_ReorderableList != null))
            {
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                this.m_ReorderableList.DoList(position);
                EditorGUI.indentLevel = indentLevel;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            State state = this.RestoreState(property);
            this.OnGUI(position);
            state.lastSelectedIndex = this.m_LastSelectedIndex;
        }

        private void RemoveButton(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            this.m_LastSelectedIndex = list.index;
            list.displayAdd = this.m_FieldsArray.arraySize < 10;
        }

        private State RestoreState(SerializedProperty prop)
        {
            State state = this.GetState(prop);
            this.m_FieldsArray = state.m_ReorderableList.serializedProperty;
            this.m_ReorderableList = state.m_ReorderableList;
            this.m_LastSelectedIndex = state.lastSelectedIndex;
            return state;
        }

        private void SelectParam(ReorderableList list)
        {
            this.m_LastSelectedIndex = list.index;
        }

        private static void SetProperty(object source)
        {
            ((PropertySetter) source).Assign();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertySetter
        {
            private readonly SerializedProperty m_Prop;
            private readonly object m_Target;
            private readonly string m_FieldPath;
            public PropertySetter(SerializedProperty p, object target, string fp)
            {
                this.m_Prop = p;
                this.m_Target = target;
                this.m_FieldPath = fp;
            }

            public void Assign()
            {
                this.m_Prop.FindPropertyRelative("m_Target").objectReferenceValue = (UnityEngine.Object) this.m_Target;
                this.m_Prop.FindPropertyRelative("m_FieldPath").stringValue = this.m_FieldPath;
                this.m_Prop.serializedObject.ApplyModifiedProperties();
            }

            public void Clear()
            {
                this.m_Prop.FindPropertyRelative("m_Target").objectReferenceValue = null;
                this.m_Prop.FindPropertyRelative("m_FieldPath").stringValue = null;
                this.m_Prop.serializedObject.ApplyModifiedProperties();
            }
        }

        protected class State
        {
            public int lastSelectedIndex;
            internal ReorderableList m_ReorderableList;
        }
    }
}


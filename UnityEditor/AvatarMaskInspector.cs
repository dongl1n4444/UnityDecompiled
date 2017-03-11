namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Profiling;

    [CustomEditor(typeof(AvatarMask))]
    internal class AvatarMaskInspector : Editor
    {
        [CompilerGenerated]
        private static Func<char, bool> <>f__am$cache0;
        private SerializedProperty m_AnimationType = null;
        private SerializedProperty m_BodyMask = null;
        private bool m_BodyMaskFoldout = false;
        private bool m_CanImport = true;
        private AnimationClipInfoProperties m_ClipInfo = null;
        private string[] m_HumanTransform = null;
        private NodeInfo[] m_NodeInfos;
        private Avatar m_RefAvatar;
        private ModelImporter m_RefImporter;
        private bool m_ShowBodyMask = true;
        private SerializedProperty m_TransformMask = null;
        private bool m_TransformMaskFoldout = false;
        private string[] m_TransformPaths = null;

        private void CheckChildren(int index, bool value)
        {
            foreach (int num in this.m_NodeInfos[index].m_ChildIndices)
            {
                if (this.m_NodeInfos[num].m_Enabled)
                {
                    this.m_NodeInfos[num].m_Weight.floatValue = !value ? 0f : 1f;
                }
                this.CheckChildren(num, value);
            }
        }

        private void ComputeShownElements()
        {
            for (int i = 0; i < this.m_NodeInfos.Length; i++)
            {
                if (this.m_NodeInfos[i].m_ParentIndex == -1)
                {
                    this.ComputeShownElements(i, true);
                }
            }
        }

        private void ComputeShownElements(int currentIndex, bool show)
        {
            this.m_NodeInfos[currentIndex].m_Show = show;
            bool flag = show && this.m_NodeInfos[currentIndex].m_Expanded;
            foreach (int num in this.m_NodeInfos[currentIndex].m_ChildIndices)
            {
                this.ComputeShownElements(num, flag);
            }
        }

        protected void CopyFromOtherGUI()
        {
            if (this.clipInfo != null)
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.clipInfo.maskSourceProperty, GUIContent.Temp("Source"), new GUILayoutOption[0]);
                AvatarMask objectReferenceValue = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
                if (EditorGUI.EndChangeCheck() && (objectReferenceValue != null))
                {
                    this.UpdateMask(this.clipInfo.maskType);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DeselectAll()
        {
            this.SetAllTransformActive(false);
        }

        private void FillNodeInfos()
        {
            this.m_NodeInfos = new NodeInfo[this.m_TransformMask.arraySize];
            if (this.m_TransformMask.arraySize != 0)
            {
                string[] strArray = new string[this.m_TransformMask.arraySize];
                SerializedProperty arrayElementAtIndex = this.m_TransformMask.GetArrayElementAtIndex(0);
                arrayElementAtIndex.Next(false);
                for (int i = 1; i < this.m_NodeInfos.Length; i++)
                {
                    <FillNodeInfos>c__AnonStorey1 storey = new <FillNodeInfos>c__AnonStorey1();
                    this.m_NodeInfos[i].m_Path = arrayElementAtIndex.FindPropertyRelative("m_Path");
                    this.m_NodeInfos[i].m_Weight = arrayElementAtIndex.FindPropertyRelative("m_Weight");
                    strArray[i] = this.m_NodeInfos[i].m_Path.stringValue;
                    storey.fullPath = strArray[i];
                    if (this.humanTransforms != null)
                    {
                        this.m_NodeInfos[i].m_Enabled = ArrayUtility.FindIndex<string>(this.humanTransforms, new Predicate<string>(storey.<>m__0)) == -1;
                    }
                    else
                    {
                        this.m_NodeInfos[i].m_Enabled = true;
                    }
                    this.m_NodeInfos[i].m_Expanded = true;
                    this.m_NodeInfos[i].m_ParentIndex = -1;
                    this.m_NodeInfos[i].m_ChildIndices = new List<int>();
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = f => f == '/';
                    }
                    this.m_NodeInfos[i].m_Depth = (i != 0) ? Enumerable.Count<char>(storey.fullPath, <>f__am$cache0) : 0;
                    string str = "";
                    int length = storey.fullPath.LastIndexOf('/');
                    if (length > 0)
                    {
                        str = storey.fullPath.Substring(0, length);
                    }
                    length = (length != -1) ? (length + 1) : 0;
                    this.m_NodeInfos[i].m_Name = storey.fullPath.Substring(length);
                    for (int j = 1; j < i; j++)
                    {
                        string str2 = strArray[j];
                        if ((str != "") && (str2 == str))
                        {
                            this.m_NodeInfos[i].m_ParentIndex = j;
                            this.m_NodeInfos[j].m_ChildIndices.Add(i);
                        }
                    }
                    arrayElementAtIndex.Next(false);
                }
            }
        }

        private void ImportAvatarReference()
        {
            EditorGUI.BeginChangeCheck();
            this.m_RefAvatar = EditorGUILayout.ObjectField("Use skeleton from", this.m_RefAvatar, typeof(Avatar), true, new GUILayoutOption[0]) as Avatar;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_RefImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.m_RefAvatar)) as ModelImporter;
            }
            if ((this.m_RefImporter != null) && GUILayout.Button("Import skeleton", new GUILayoutOption[0]))
            {
                AvatarMaskUtility.UpdateTransformMask(this.m_TransformMask, this.m_RefImporter.transformPaths, null);
            }
        }

        private ClipAnimationMaskType IndexToMaskType(int index)
        {
            if (index == 2)
            {
                return ClipAnimationMaskType.None;
            }
            return (ClipAnimationMaskType) index;
        }

        private void InitializeSerializedProperties()
        {
            if (this.clipInfo != null)
            {
                this.m_BodyMask = this.clipInfo.bodyMaskProperty;
                this.m_TransformMask = this.clipInfo.transformMaskProperty;
            }
            else
            {
                this.m_BodyMask = base.serializedObject.FindProperty("m_Mask");
                this.m_TransformMask = base.serializedObject.FindProperty("m_Elements");
            }
            this.FillNodeInfos();
        }

        public bool IsMaskEmpty() => 
            (this.m_NodeInfos.Length == 0);

        public bool IsMaskUpToDate()
        {
            if (this.clipInfo == null)
            {
                return false;
            }
            if (this.m_NodeInfos.Length != this.m_TransformPaths.Length)
            {
                return false;
            }
            if (this.m_TransformMask.arraySize > 0)
            {
                SerializedProperty arrayElementAtIndex = this.m_TransformMask.GetArrayElementAtIndex(0);
                for (int i = 1; i < this.m_NodeInfos.Length; i++)
                {
                    <IsMaskUpToDate>c__AnonStorey0 storey = new <IsMaskUpToDate>c__AnonStorey0 {
                        path = this.m_NodeInfos[i].m_Path.stringValue
                    };
                    if (ArrayUtility.FindIndex<string>(this.m_TransformPaths, new Predicate<string>(storey.<>m__0)) == -1)
                    {
                        return false;
                    }
                    arrayElementAtIndex.Next(false);
                }
            }
            return true;
        }

        private int MaskTypeToIndex(ClipAnimationMaskType maskType)
        {
            if (maskType == ClipAnimationMaskType.None)
            {
                return 2;
            }
            return (int) maskType;
        }

        public void OnBodyInspectorGUI()
        {
            if (this.m_ShowBodyMask)
            {
                bool changed = GUI.changed;
                this.m_BodyMaskFoldout = EditorGUILayout.Foldout(this.m_BodyMaskFoldout, Styles.BodyMask, true);
                GUI.changed = changed;
                if (this.m_BodyMaskFoldout)
                {
                    BodyMaskEditor.Show(this.m_BodyMask, 13);
                }
            }
        }

        private void OnEnable()
        {
            this.InitializeSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            Profiler.BeginSample("AvatarMaskInspector.OnInspectorGUI()");
            if (this.clipInfo == null)
            {
                base.serializedObject.Update();
            }
            bool flag = false;
            if (this.clipInfo != null)
            {
                EditorGUI.BeginChangeCheck();
                int selectedIndex = this.MaskTypeToIndex(this.clipInfo.maskType);
                EditorGUI.showMixedValue = this.clipInfo.maskTypeProperty.hasMultipleDifferentValues;
                selectedIndex = EditorGUILayout.Popup(Styles.MaskDefinition, selectedIndex, Styles.MaskDefinitionOpt, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.clipInfo.maskType = this.IndexToMaskType(selectedIndex);
                    this.UpdateMask(this.clipInfo.maskType);
                }
                flag = this.clipInfo.maskType == ClipAnimationMaskType.CopyFromOther;
            }
            if (flag)
            {
                this.CopyFromOtherGUI();
            }
            bool enabled = GUI.enabled;
            GUI.enabled = !flag;
            EditorGUI.BeginChangeCheck();
            this.OnBodyInspectorGUI();
            this.OnTransformInspectorGUI();
            if ((this.clipInfo != null) && EditorGUI.EndChangeCheck())
            {
                AvatarMask target = base.target as AvatarMask;
                this.clipInfo.MaskFromClip(target);
            }
            GUI.enabled = enabled;
            if (this.clipInfo == null)
            {
                base.serializedObject.ApplyModifiedProperties();
            }
            Profiler.EndSample();
        }

        public void OnTransformInspectorGUI()
        {
            float xmin = 0f;
            float ymin = 0f;
            float a = 0f;
            float ymax = 0f;
            bool changed = GUI.changed;
            this.m_TransformMaskFoldout = EditorGUILayout.Foldout(this.m_TransformMaskFoldout, Styles.TransformMask, true);
            GUI.changed = changed;
            if (this.m_TransformMaskFoldout)
            {
                if (this.canImport)
                {
                    this.ImportAvatarReference();
                }
                if ((this.m_NodeInfos == null) || (this.m_TransformMask.arraySize != this.m_NodeInfos.Length))
                {
                    this.FillNodeInfos();
                }
                if (this.IsMaskEmpty())
                {
                    string str;
                    GUILayout.BeginVertical(new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
                    if (this.animationType == ModelImporterAnimationType.Generic)
                    {
                        str = "No transform mask defined, everything will be imported";
                    }
                    else if (this.animationType == ModelImporterAnimationType.Human)
                    {
                        str = "No transform mask defined, only human curves will be imported";
                    }
                    else
                    {
                        str = "No transform mask defined";
                    }
                    GUILayout.Label(str, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                    if (!this.canImport && (this.clipInfo.maskType == ClipAnimationMaskType.CreateFromThisModel))
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        string text = "Create Mask";
                        if (GUILayout.Button(text, new GUILayoutOption[0]))
                        {
                            this.UpdateMask(this.clipInfo.maskType);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    this.ComputeShownElements();
                    GUILayout.Space(1f);
                    int indentLevel = EditorGUI.indentLevel;
                    int arraySize = this.m_TransformMask.arraySize;
                    for (int i = 1; i < arraySize; i++)
                    {
                        if (this.m_NodeInfos[i].m_Show)
                        {
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            EditorGUI.indentLevel = this.m_NodeInfos[i].m_Depth + 1;
                            EditorGUI.BeginChangeCheck();
                            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                            Rect position = GUILayoutUtility.GetRect((float) 15f, (float) 15f, options);
                            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                            GUILayoutUtility.GetRect((float) 10f, (float) 15f, optionArray2);
                            position.x += 15f;
                            bool enabled = GUI.enabled;
                            GUI.enabled = this.m_NodeInfos[i].m_Enabled;
                            bool flag3 = Event.current.button == 1;
                            bool flag4 = this.m_NodeInfos[i].m_Weight.floatValue > 0f;
                            flag4 = GUI.Toggle(position, flag4, "");
                            GUI.enabled = enabled;
                            if (EditorGUI.EndChangeCheck())
                            {
                                this.m_NodeInfos[i].m_Weight.floatValue = !flag4 ? 0f : 1f;
                                if (!flag3)
                                {
                                    this.CheckChildren(i, flag4);
                                }
                            }
                            if (this.m_NodeInfos[i].m_ChildIndices.Count > 0)
                            {
                                this.m_NodeInfos[i].m_Expanded = EditorGUILayout.Foldout(this.m_NodeInfos[i].m_Expanded, this.m_NodeInfos[i].m_Name, true);
                            }
                            else
                            {
                                EditorGUILayout.LabelField(this.m_NodeInfos[i].m_Name, new GUILayoutOption[0]);
                            }
                            if (i == 1)
                            {
                                ymin = position.yMin;
                                xmin = position.xMin;
                            }
                            else if (i == (arraySize - 1))
                            {
                                ymax = position.yMax;
                            }
                            a = Mathf.Max(a, GUILayoutUtility.GetLastRect().xMax);
                            GUILayout.EndHorizontal();
                        }
                    }
                    EditorGUI.indentLevel = indentLevel;
                }
            }
            Rect rect3 = Rect.MinMaxRect(xmin, ymin, a, ymax);
            if (((Event.current != null) && (Event.current.type == EventType.MouseUp)) && ((Event.current.button == 1) && rect3.Contains(Event.current.mousePosition)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Select all"), false, new GenericMenu.MenuFunction(this.SelectAll));
                menu.AddItem(new GUIContent("Deselect all"), false, new GenericMenu.MenuFunction(this.DeselectAll));
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private void SelectAll()
        {
            this.SetAllTransformActive(true);
        }

        private void SetAllTransformActive(bool active)
        {
            for (int i = 0; i < this.m_NodeInfos.Length; i++)
            {
                if (this.m_NodeInfos[i].m_Enabled)
                {
                    this.m_NodeInfos[i].m_Weight.floatValue = !active ? 0f : 1f;
                }
            }
        }

        private void UpdateMask(ClipAnimationMaskType maskType)
        {
            if (this.clipInfo != null)
            {
                if (maskType == ClipAnimationMaskType.CreateFromThisModel)
                {
                    ModelImporter targetObject = this.clipInfo.maskTypeProperty.serializedObject.targetObject as ModelImporter;
                    AvatarMaskUtility.UpdateTransformMask(this.m_TransformMask, targetObject.transformPaths, this.humanTransforms);
                    this.FillNodeInfos();
                }
                else if (maskType == ClipAnimationMaskType.CopyFromOther)
                {
                    AvatarMask objectReferenceValue = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
                    if (objectReferenceValue != null)
                    {
                        AvatarMask mask = base.target as AvatarMask;
                        mask.Copy(objectReferenceValue);
                        if (this.humanTransforms != null)
                        {
                            AvatarMaskUtility.SetActiveHumanTransforms(mask, this.humanTransforms);
                        }
                        this.clipInfo.MaskToClip(mask);
                        this.FillNodeInfos();
                    }
                }
                else if (maskType == ClipAnimationMaskType.None)
                {
                    AvatarMask mask3 = new AvatarMask();
                    ModelImporter.UpdateTransformMask(mask3, this.clipInfo.transformMaskProperty);
                }
                AvatarMask target = base.target as AvatarMask;
                this.clipInfo.MaskFromClip(target);
            }
        }

        private ModelImporterAnimationType animationType
        {
            get
            {
                if (this.m_AnimationType != null)
                {
                    return (ModelImporterAnimationType) this.m_AnimationType.intValue;
                }
                return ModelImporterAnimationType.None;
            }
        }

        public bool canImport
        {
            get => 
                this.m_CanImport;
            set
            {
                this.m_CanImport = value;
            }
        }

        public AnimationClipInfoProperties clipInfo
        {
            get => 
                this.m_ClipInfo;
            set
            {
                this.m_ClipInfo = value;
                if (this.m_ClipInfo != null)
                {
                    this.m_ClipInfo.MaskFromClip(base.target as AvatarMask);
                    SerializedObject serializedObject = this.m_ClipInfo.maskTypeProperty.serializedObject;
                    this.m_AnimationType = serializedObject.FindProperty("m_AnimationType");
                    ModelImporter targetObject = serializedObject.targetObject as ModelImporter;
                    this.m_TransformPaths = targetObject.transformPaths;
                }
                else
                {
                    this.m_TransformPaths = null;
                    this.m_AnimationType = null;
                }
                this.InitializeSerializedProperties();
            }
        }

        public string[] humanTransforms
        {
            get
            {
                if ((this.animationType == ModelImporterAnimationType.Human) && (this.clipInfo != null))
                {
                    if (this.m_HumanTransform == null)
                    {
                        SerializedObject serializedObject = this.clipInfo.maskTypeProperty.serializedObject;
                        ModelImporter targetObject = serializedObject.targetObject as ModelImporter;
                        this.m_HumanTransform = AvatarMaskUtility.GetAvatarHumanTransform(serializedObject, targetObject.transformPaths);
                    }
                }
                else
                {
                    this.m_HumanTransform = null;
                }
                return this.m_HumanTransform;
            }
        }

        public bool showBody
        {
            get => 
                this.m_ShowBodyMask;
            set
            {
                this.m_ShowBodyMask = value;
            }
        }

        [CompilerGenerated]
        private sealed class <FillNodeInfos>c__AnonStorey1
        {
            internal string fullPath;

            internal bool <>m__0(string s) => 
                (this.fullPath == s);
        }

        [CompilerGenerated]
        private sealed class <IsMaskUpToDate>c__AnonStorey0
        {
            internal string path;

            internal bool <>m__0(string s) => 
                (s == this.path);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NodeInfo
        {
            public bool m_Expanded;
            public bool m_Show;
            public bool m_Enabled;
            public int m_ParentIndex;
            public List<int> m_ChildIndices;
            public int m_Depth;
            public SerializedProperty m_Path;
            public SerializedProperty m_Weight;
            public string m_Name;
        }

        private static class Styles
        {
            public static GUIContent BodyMask = EditorGUIUtility.TextContent("Humanoid|Define which body part are active. Also define which animation curves will be imported for an Animation Clip.");
            public static GUIContent MaskDefinition = EditorGUIUtility.TextContent("Definition|Choose between Create From This Model, Copy From Other Avatar. The first one create a Mask for this file and the second one use a Mask from another file to import animation.");
            public static GUIContent[] MaskDefinitionOpt = new GUIContent[] { EditorGUIUtility.TextContent("Create From This Model|Create a Mask based on the model from this file. For Humanoid rig all the human transform are always imported and converted to muscle curve, thus they cannot be unchecked."), EditorGUIUtility.TextContent("Copy From Other Mask|Copy a Mask from another file to import animation clip."), EditorGUIUtility.TextContent("None | Import Everything") };
            public static GUIContent TransformMask = EditorGUIUtility.TextContent("Transform|Define which transform are active. Also define which animation curves will be imported for an Animation Clip.");
        }
    }
}


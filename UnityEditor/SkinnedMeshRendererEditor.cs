namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(SkinnedMeshRenderer))]
    internal class SkinnedMeshRendererEditor : RendererEditorBase
    {
        private const string kDisplayLightingKey = "SkinnedMeshRendererEditor.Lighting.ShowSettings";
        private SerializedProperty m_AABB;
        private SerializedProperty m_BlendShapeWeights;
        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_DirtyAABB;
        private string[] m_ExcludedProperties;
        private LightingSettingsInspector m_Lighting;
        private SerializedProperty m_Materials;
        private static int s_HandleControlIDHint = typeof(SkinnedMeshRendererEditor).Name.GetHashCode();

        private void InitializeLightingFields()
        {
            this.m_Lighting = new LightingSettingsInspector(base.serializedObject);
            this.m_Lighting.showSettings = EditorPrefs.GetBool("SkinnedMeshRendererEditor.Lighting.ShowSettings", false);
        }

        private void LightingFieldsGUI()
        {
            bool showSettings = this.m_Lighting.showSettings;
            if (this.m_Lighting.Begin())
            {
                base.RenderProbeFields();
                this.m_Lighting.RenderMeshSettings(false);
            }
            this.m_Lighting.End();
            if (this.m_Lighting.showSettings != showSettings)
            {
                EditorPrefs.SetBool("SkinnedMeshRendererEditor.Lighting.ShowSettings", this.m_Lighting.showSettings);
            }
        }

        public void OnBlendShapeUI()
        {
            SkinnedMeshRenderer target = (SkinnedMeshRenderer) base.target;
            int num = (target.sharedMesh != null) ? target.sharedMesh.blendShapeCount : 0;
            if (num != 0)
            {
                GUIContent label = new GUIContent {
                    text = "BlendShapes"
                };
                EditorGUILayout.PropertyField(this.m_BlendShapeWeights, label, false, new GUILayoutOption[0]);
                if (this.m_BlendShapeWeights.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    Mesh sharedMesh = target.sharedMesh;
                    int arraySize = this.m_BlendShapeWeights.arraySize;
                    for (int i = 0; i < num; i++)
                    {
                        label.text = sharedMesh.GetBlendShapeName(i);
                        if (i < arraySize)
                        {
                            EditorGUILayout.PropertyField(this.m_BlendShapeWeights.GetArrayElementAtIndex(i), label, new GUILayoutOption[0]);
                        }
                        else
                        {
                            EditorGUI.BeginChangeCheck();
                            float num4 = EditorGUILayout.FloatField(label, 0f, new GUILayoutOption[0]);
                            if (EditorGUI.EndChangeCheck())
                            {
                                this.m_BlendShapeWeights.arraySize = num;
                                arraySize = num;
                                this.m_BlendShapeWeights.GetArrayElementAtIndex(i).floatValue = num4;
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Materials = base.serializedObject.FindProperty("m_Materials");
            this.m_BlendShapeWeights = base.serializedObject.FindProperty("m_BlendShapeWeights");
            this.m_AABB = base.serializedObject.FindProperty("m_AABB");
            this.m_DirtyAABB = base.serializedObject.FindProperty("m_DirtyAABB");
            this.m_BoundsHandle.SetColor(Handles.s_BoundingBoxHandleColor);
            base.InitializeProbeFields();
            this.InitializeLightingFields();
            List<string> list = new List<string>();
            string[] collection = new string[] { "m_CastShadows", "m_ReceiveShadows", "m_MotionVectors", "m_Materials", "m_BlendShapeWeights", "m_AABB", "m_LightmapParameters" };
            list.AddRange(collection);
            list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
            this.m_ExcludedProperties = list.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.OnBlendShapeUI();
            Editor.DrawPropertiesExcluding(base.serializedObject, this.m_ExcludedProperties);
            UnityEditorInternal.EditMode.DoEditModeInspectorModeButton(UnityEditorInternal.EditMode.SceneViewEditMode.Collider, "Edit Bounds", PrimitiveBoundsHandle.editModeButton, (base.target as SkinnedMeshRenderer).bounds, this);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_AABB, new GUIContent("Bounds"), new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_DirtyAABB.boolValue = false;
            }
            this.LightingFieldsGUI();
            EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            SkinnedMeshRenderer target = (SkinnedMeshRenderer) base.target;
            if (target.updateWhenOffscreen)
            {
                Bounds bounds = target.bounds;
                Vector3 center = bounds.center;
                Vector3 size = bounds.size;
                Handles.DrawWireCube(center, size);
            }
            else
            {
                using (new Handles.DrawingScope(target.actualRootBone.localToWorldMatrix))
                {
                    Bounds localBounds = target.localBounds;
                    this.m_BoundsHandle.center = localBounds.center;
                    this.m_BoundsHandle.size = localBounds.size;
                    this.m_BoundsHandle.handleColor = ((UnityEditorInternal.EditMode.editMode != UnityEditorInternal.EditMode.SceneViewEditMode.Collider) || !UnityEditorInternal.EditMode.IsOwner(this)) ? Color.clear : this.m_BoundsHandle.wireframeColor;
                    EditorGUI.BeginChangeCheck();
                    this.m_BoundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Resize Bounds");
                        target.localBounds = new Bounds(this.m_BoundsHandle.center, this.m_BoundsHandle.size);
                    }
                }
            }
        }
    }
}


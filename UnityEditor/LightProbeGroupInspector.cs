namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(LightProbeGroup))]
    internal class LightProbeGroupInspector : Editor
    {
        private bool m_EditingProbes;
        private LightProbeGroupEditor m_Editor;
        private bool m_ShouldFocus;

        private void EditModeEnded(Editor editor)
        {
            if (editor == this)
            {
                this.EndEditProbes();
            }
        }

        private void EditModeStarted(Editor editor, EditMode.SceneViewEditMode mode)
        {
            if ((editor == this) && (mode == EditMode.SceneViewEditMode.LightProbeGroup))
            {
                this.StartEditProbes();
            }
        }

        private void EndEditProbes()
        {
            if (this.m_EditingProbes)
            {
                this.m_Editor.drawTetrahedra = true;
                this.m_Editor.DeselectProbes();
                this.m_Editor.SetEditing(false);
                this.m_EditingProbes = false;
                Tools.s_Hidden = false;
                SceneView.RepaintAll();
            }
        }

        public bool HasFrameBounds()
        {
            return (this.m_Editor.SelectedCount > 0);
        }

        private void InternalOnSceneView()
        {
            if (EditorGUIUtility.IsGizmosAllowedForObject(base.target))
            {
                if ((SceneView.lastActiveSceneView != null) && this.m_ShouldFocus)
                {
                    this.m_ShouldFocus = false;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                this.m_Editor.PullProbePositions();
                LightProbeGroup target = base.target as LightProbeGroup;
                if (target != null)
                {
                    if (this.m_Editor.OnSceneGUI(target.transform))
                    {
                        this.StartEditProbes();
                    }
                    else
                    {
                        this.EndEditProbes();
                    }
                }
                this.m_Editor.PushProbePositions();
            }
        }

        public void OnDisable()
        {
            this.EndEditProbes();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUIDelegate));
            if (base.target != null)
            {
                this.m_Editor.PushProbePositions();
                this.m_Editor = null;
            }
        }

        public void OnEnable()
        {
            this.m_Editor = new LightProbeGroupEditor(base.target as LightProbeGroup, this);
            this.m_Editor.PullProbePositions();
            this.m_Editor.DeselectProbes();
            this.m_Editor.PushProbePositions();
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUIDelegate));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            EditMode.onEditModeStartDelegate = (EditMode.OnEditModeStartFunc) Delegate.Combine(EditMode.onEditModeStartDelegate, new EditMode.OnEditModeStartFunc(this.EditModeStarted));
            EditMode.onEditModeEndDelegate = (EditMode.OnEditModeStopFunc) Delegate.Combine(EditMode.onEditModeEndDelegate, new EditMode.OnEditModeStopFunc(this.EditModeEnded));
        }

        public Bounds OnGetFrameBounds()
        {
            return this.m_Editor.selectedProbeBounds;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.m_Editor.PullProbePositions();
            EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.LightProbeGroup, "Edit Light Probes", EditorGUIUtility.IconContent("EditCollider"), this.m_Editor.bounds, this);
            GUILayout.Space(3f);
            EditorGUI.BeginDisabledGroup(EditMode.editMode != EditMode.SceneViewEditMode.LightProbeGroup);
            this.m_Editor.drawTetrahedra = EditorGUILayout.Toggle(Styles.showWireframe, this.m_Editor.drawTetrahedra, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(this.m_Editor.SelectedCount == 0);
            Vector3 vector = (this.m_Editor.SelectedCount <= 0) ? Vector3.zero : this.m_Editor.GetSelectedPositions()[0];
            Vector3 vector2 = EditorGUILayout.Vector3Field(Styles.selectedProbePosition, vector, new GUILayoutOption[0]);
            if (vector2 != vector)
            {
                Vector3[] selectedPositions = this.m_Editor.GetSelectedPositions();
                Vector3 vector3 = vector2 - vector;
                for (int i = 0; i < selectedPositions.Length; i++)
                {
                    this.m_Editor.UpdateSelectedPosition(i, selectedPositions[i] + vector3);
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(3f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button(Styles.addProbe, new GUILayoutOption[0]))
            {
                Vector3 zero = Vector3.zero;
                if (SceneView.lastActiveSceneView != null)
                {
                    LightProbeGroup target = base.target as LightProbeGroup;
                    if (target != null)
                    {
                        zero = target.transform.InverseTransformPoint(zero);
                    }
                }
                this.StartEditProbes();
                this.m_Editor.DeselectProbes();
                this.m_Editor.AddProbe(zero);
            }
            if (GUILayout.Button(Styles.deleteSelected, new GUILayoutOption[0]))
            {
                this.StartEditProbes();
                this.m_Editor.RemoveSelectedProbes();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button(Styles.selectAll, new GUILayoutOption[0]))
            {
                this.StartEditProbes();
                this.m_Editor.SelectAllProbes();
            }
            if (GUILayout.Button(Styles.duplicateSelected, new GUILayoutOption[0]))
            {
                this.StartEditProbes();
                this.m_Editor.DuplicateSelectedProbes();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            this.m_Editor.HandleEditMenuHotKeyCommands();
            this.m_Editor.PushProbePositions();
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Editor.MarkTetrahedraDirty();
                SceneView.RepaintAll();
            }
        }

        public void OnSceneGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                this.InternalOnSceneView();
            }
        }

        public void OnSceneGUIDelegate(SceneView sceneView)
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.InternalOnSceneView();
            }
        }

        public void StartEditMode()
        {
            EditMode.ChangeEditMode(EditMode.SceneViewEditMode.LightProbeGroup, this.m_Editor.bounds, this);
        }

        private void StartEditProbes()
        {
            if (!this.m_EditingProbes)
            {
                this.m_EditingProbes = true;
                this.m_Editor.SetEditing(true);
                Tools.s_Hidden = true;
                SceneView.RepaintAll();
            }
        }

        private void UndoRedoPerformed()
        {
            this.m_Editor.PullProbePositions();
            this.m_Editor.MarkTetrahedraDirty();
        }

        private static class Styles
        {
            public static GUIContent addProbe = new GUIContent("Add Probe");
            public static GUIContent deleteSelected = new GUIContent("Delete Selected");
            public static GUIContent duplicateSelected = new GUIContent("Duplicate Selected");
            public static GUIContent selectAll = new GUIContent("Select All");
            public static GUIContent selectedProbePosition = new GUIContent("Selected Probe Position", "The local position of this probe relative to the parent group.");
            public static GUIContent showWireframe = new GUIContent("Show Wireframe", "Show the tetrahedron wireframe visualizing the blending between probes.");
        }
    }
}


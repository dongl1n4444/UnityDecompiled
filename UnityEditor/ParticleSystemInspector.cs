namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(ParticleSystem))]
    internal class ParticleSystemInspector : Editor, ParticleEffectUIOwner
    {
        [CompilerGenerated]
        private static Func<ParticleSystem, bool> <>f__am$cache0;
        private GUIContent closeWindowText = new GUIContent("Close Editor");
        private GUIContent hideWindowText = new GUIContent("Hide Editor");
        private ParticleEffectUI m_ParticleEffectUI;
        private static GUIContent m_PlayBackTitle;
        private GUIContent m_PreviewTitle = new GUIContent("Particle System Curves");
        private GUIContent showWindowText = new GUIContent("Open Editor...");

        private void Clear()
        {
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.Clear();
            }
            this.m_ParticleEffectUI = null;
        }

        public override void DrawPreview(Rect previewArea)
        {
            UnityEngine.Object[] targets = new UnityEngine.Object[] { base.targets[0] };
            ObjectPreview.DrawPreview(this, previewArea, targets);
        }

        public override GUIContent GetPreviewTitle() => 
            this.m_PreviewTitle;

        public override bool HasPreviewGUI() => 
            this.ShouldShowInspector();

        private void HierarchyOrProjectWindowWasChanged()
        {
            if ((base.target != null) && this.ShouldShowInspector())
            {
                this.Init(true);
            }
        }

        private void Init(bool forceInit)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = p => p != null;
            }
            IEnumerable<ParticleSystem> source = Enumerable.Where<ParticleSystem>(base.targets.OfType<ParticleSystem>(), <>f__am$cache0);
            if ((source == null) || !source.Any<ParticleSystem>())
            {
                this.m_ParticleEffectUI = null;
            }
            else if (this.m_ParticleEffectUI == null)
            {
                this.m_ParticleEffectUI = new ParticleEffectUI(this);
                this.m_ParticleEffectUI.InitializeIfNeeded(source);
            }
            else if (forceInit)
            {
                this.m_ParticleEffectUI.InitializeIfNeeded(source);
            }
        }

        public void OnDisable()
        {
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.Clear();
            }
        }

        public void OnEnable()
        {
            EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            this.ShowEdiorButtonGUI();
            if (this.ShouldShowInspector())
            {
                if (this.m_ParticleEffectUI == null)
                {
                    this.Init(true);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
                this.m_ParticleEffectUI.OnGUI();
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            }
            else
            {
                this.Clear();
            }
            EditorGUILayout.EndVertical();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.GetParticleSystemCurveEditor().OnGUI(r);
            }
        }

        public override void OnPreviewSettings()
        {
        }

        public void OnSceneViewGUI(SceneView sceneView)
        {
            if (this.ShouldShowInspector())
            {
                this.Init(false);
                if (this.m_ParticleEffectUI != null)
                {
                    this.m_ParticleEffectUI.OnSceneViewGUI();
                }
            }
        }

        private bool ShouldShowInspector()
        {
            ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
            return (((instance == null) || !instance.IsVisible()) || !this.selectedInParticleSystemWindow);
        }

        private void ShowEdiorButtonGUI()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if ((this.m_ParticleEffectUI == null) || !this.m_ParticleEffectUI.multiEdit)
            {
                bool selectedInParticleSystemWindow = this.selectedInParticleSystemWindow;
                GameObject gameObject = (base.target as ParticleSystem).gameObject;
                GUIContent hideWindowText = null;
                ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
                if (((instance != null) && instance.IsVisible()) && selectedInParticleSystemWindow)
                {
                    if (instance.GetNumTabs() > 1)
                    {
                        hideWindowText = this.hideWindowText;
                    }
                    else
                    {
                        hideWindowText = this.closeWindowText;
                    }
                }
                else
                {
                    hideWindowText = this.showWindowText;
                }
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(110f) };
                if (GUILayout.Button(hideWindowText, EditorStyles.miniButton, options))
                {
                    if (((instance != null) && instance.IsVisible()) && selectedInParticleSystemWindow)
                    {
                        if (!instance.ShowNextTabIfPossible())
                        {
                            instance.Close();
                        }
                    }
                    else
                    {
                        if (!selectedInParticleSystemWindow)
                        {
                            ParticleSystemEditorUtils.lockedParticleSystem = null;
                            Selection.activeGameObject = gameObject;
                        }
                        if (instance != null)
                        {
                            if (!selectedInParticleSystemWindow)
                            {
                                instance.Clear();
                            }
                            instance.Focus();
                        }
                        else
                        {
                            this.Clear();
                            ParticleSystemWindow.CreateWindow();
                            ParticleSystemWindow.GetInstance().customEditor = this;
                            GUIUtility.ExitGUI();
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void UndoRedoPerformed()
        {
            this.Init(true);
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.UndoRedoPerformed();
            }
        }

        public override bool UseDefaultMargins() => 
            false;

        public Editor customEditor =>
            this;

        public static GUIContent playBackTitle
        {
            get
            {
                if (m_PlayBackTitle == null)
                {
                    m_PlayBackTitle = new GUIContent("Particle Effect");
                }
                return m_PlayBackTitle;
            }
        }

        private bool selectedInParticleSystemWindow
        {
            get
            {
                GameObject activeGameObject;
                GameObject gameObject = (base.target as ParticleSystem).gameObject;
                if (ParticleSystemEditorUtils.lockedParticleSystem == null)
                {
                    activeGameObject = Selection.activeGameObject;
                }
                else
                {
                    activeGameObject = ParticleSystemEditorUtils.lockedParticleSystem.gameObject;
                }
                return (activeGameObject == gameObject);
            }
        }
    }
}


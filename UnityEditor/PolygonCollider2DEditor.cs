namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Sprites;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(PolygonCollider2D)), CanEditMultipleObjects]
    internal class PolygonCollider2DEditor : Collider2DEditorBase
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, PolygonCollider2D> <>f__am$cache1;
        private SerializedProperty m_Points;
        private readonly PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();

        private void HandleDragAndDrop(Rect targetRect)
        {
            if (((Event.current.type == EventType.DragPerform) || (Event.current.type == EventType.DragUpdated)) && targetRect.Contains(Event.current.mousePosition))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = obj => (obj is Sprite) || (obj is Texture2D);
                }
                foreach (UnityEngine.Object obj2 in Enumerable.Where<UnityEngine.Object>(DragAndDrop.objectReferences, <>f__am$cache0))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (Event.current.type == EventType.DragPerform)
                    {
                        Sprite sprite = !(obj2 is Sprite) ? UnityEditor.SpriteUtility.TextureToSprite(obj2 as Texture2D) : (obj2 as Sprite);
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = target => target as PolygonCollider2D;
                        }
                        foreach (PolygonCollider2D colliderd in Enumerable.Select<UnityEngine.Object, PolygonCollider2D>(base.targets, <>f__am$cache1))
                        {
                            Vector2[][] vectorArray;
                            UnityEditor.Sprites.SpriteUtility.GenerateOutlineFromSprite(sprite, 0.25f, 200, true, out vectorArray);
                            colliderd.pathCount = vectorArray.Length;
                            for (int i = 0; i < vectorArray.Length; i++)
                            {
                                colliderd.SetPath(i, vectorArray[i]);
                            }
                            this.m_PolyUtility.StopEditing();
                            DragAndDrop.AcceptDrag();
                        }
                    }
                    return;
                }
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        protected override void OnEditEnd()
        {
            this.m_PolyUtility.StopEditing();
        }

        protected override void OnEditStart()
        {
            if (base.target != null)
            {
                this.m_PolyUtility.StartEditing(base.target as Collider2D);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Points = base.serializedObject.FindProperty("m_Points");
            base.m_AutoTiling = base.serializedObject.FindProperty("m_AutoTiling");
            this.m_Points.isExpanded = false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            if (!base.CanEditCollider())
            {
                EditorGUILayout.HelpBox(Collider2DEditorBase.Styles.s_ColliderEditDisableHelp.text, MessageType.Info);
                if (base.editingCollider)
                {
                    UnityEditorInternal.EditMode.QuitEditMode();
                }
            }
            else
            {
                base.BeginColliderInspector();
            }
            base.OnInspectorGUI();
            if (base.targets.Length == 1)
            {
                EditorGUI.BeginDisabledGroup(base.editingCollider);
                EditorGUILayout.PropertyField(this.m_Points, true, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
            }
            base.EndColliderInspector();
            base.FinalizeInspectorGUI();
            EditorGUILayout.EndVertical();
            this.HandleDragAndDrop(GUILayoutUtility.GetLastRect());
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                this.m_PolyUtility.OnSceneGUI();
            }
        }
    }
}


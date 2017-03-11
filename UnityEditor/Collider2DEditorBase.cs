namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects]
    internal abstract class Collider2DEditorBase : ColliderEditorBase
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, Rigidbody2D> <>f__am$cache1;
        protected SerializedProperty m_AutoTiling;
        private static ContactPoint2D[] m_Contacts = new ContactPoint2D[100];
        private Vector2 m_ContactScrollPosition;
        private SerializedProperty m_Density;
        private SerializedProperty m_IsTrigger;
        private SerializedProperty m_Material;
        private SerializedProperty m_Offset;
        private readonly AnimBool m_ShowCompositeRedundants = new AnimBool();
        private readonly AnimBool m_ShowContacts = new AnimBool();
        private readonly AnimBool m_ShowDensity = new AnimBool();
        private readonly AnimBool m_ShowInfo = new AnimBool();
        private SerializedProperty m_UsedByComposite;
        private SerializedProperty m_UsedByEffector;

        protected Collider2DEditorBase()
        {
        }

        protected void BeginColliderInspector()
        {
            base.serializedObject.Update();
            using (new EditorGUI.DisabledScope(base.targets.Length > 1))
            {
                base.InspectorEditButtonGUI();
            }
        }

        protected bool CanEditCollider() => 
            (Enumerable.FirstOrDefault<UnityEngine.Object>(base.targets, delegate (UnityEngine.Object x) {
                SpriteRenderer component = (x as Component).GetComponent<SpriteRenderer>();
                return ((component != null) && (component.drawMode != SpriteDrawMode.Simple)) && this.m_AutoTiling.boolValue;
            }) == 0);

        protected void CheckColliderErrorState()
        {
            switch ((base.target as Collider2D).errorState)
            {
                case ColliderErrorState2D.NoShapes:
                    EditorGUILayout.HelpBox("The collider did not create any collision shapes as they all failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
                    break;

                case ColliderErrorState2D.RemovedShapes:
                    EditorGUILayout.HelpBox("The collider created collision shape(s) but some were removed as they failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
                    break;
            }
        }

        protected void EndColliderInspector()
        {
            base.serializedObject.ApplyModifiedProperties();
        }

        public void FinalizeInspectorGUI()
        {
            this.ShowColliderInfoProperties();
            this.CheckColliderErrorState();
            if (base.targets.Length == 1)
            {
                Collider2D target = base.target as Collider2D;
                if ((target.isActiveAndEnabled && (target.composite == null)) && this.m_UsedByComposite.boolValue)
                {
                    EditorGUILayout.HelpBox("This collider will not function with a composite until there is a CompositeCollider2D on the GameObject that the attached Rigidbody2D is on.", MessageType.Warning);
                }
            }
            Effector2DEditor.CheckEffectorWarnings(base.target as Collider2D);
        }

        private static void FixedEndFadeGroup(float value)
        {
            if ((value != 0f) && (value != 1f))
            {
                EditorGUILayout.EndFadeGroup();
            }
        }

        public override void OnDisable()
        {
            this.m_ShowDensity.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowInfo.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowContacts.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowCompositeRedundants.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            base.OnDisable();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Density = base.serializedObject.FindProperty("m_Density");
            this.m_ShowDensity.value = this.ShouldShowDensity();
            this.m_ShowDensity.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowInfo.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowContacts.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ContactScrollPosition = Vector2.zero;
            this.m_Material = base.serializedObject.FindProperty("m_Material");
            this.m_IsTrigger = base.serializedObject.FindProperty("m_IsTrigger");
            this.m_UsedByEffector = base.serializedObject.FindProperty("m_UsedByEffector");
            this.m_UsedByComposite = base.serializedObject.FindProperty("m_UsedByComposite");
            this.m_Offset = base.serializedObject.FindProperty("m_Offset");
            this.m_AutoTiling = base.serializedObject.FindProperty("m_AutoTiling");
            this.m_ShowCompositeRedundants.value = !this.m_UsedByComposite.boolValue;
            this.m_ShowCompositeRedundants.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        internal override void OnForceReloadInspector()
        {
            base.OnForceReloadInspector();
            if (base.editingCollider)
            {
                UnityEditorInternal.EditMode.QuitEditMode();
            }
        }

        public override void OnInspectorGUI()
        {
            this.m_ShowCompositeRedundants.target = !this.m_UsedByComposite.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowCompositeRedundants.faded))
            {
                this.m_ShowDensity.target = this.ShouldShowDensity();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowDensity.faded))
                {
                    EditorGUILayout.PropertyField(this.m_Density, new GUILayoutOption[0]);
                }
                FixedEndFadeGroup(this.m_ShowDensity.faded);
                EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_UsedByEffector, new GUILayoutOption[0]);
            }
            FixedEndFadeGroup(this.m_ShowCompositeRedundants.faded);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => !(x as Collider2D).compositeCapable;
            }
            if (Enumerable.Where<UnityEngine.Object>(base.targets, <>f__am$cache0).Count<UnityEngine.Object>() == 0)
            {
                EditorGUILayout.PropertyField(this.m_UsedByComposite, new GUILayoutOption[0]);
            }
            if (this.m_AutoTiling != null)
            {
                EditorGUILayout.PropertyField(this.m_AutoTiling, Styles.s_AutoTilingLabel, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_Offset, new GUILayoutOption[0]);
        }

        private bool ShouldShowDensity()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => (x as Collider2D).attachedRigidbody;
            }
            if (Enumerable.Select<UnityEngine.Object, Rigidbody2D>(base.targets, <>f__am$cache1).Distinct<Rigidbody2D>().Count<Rigidbody2D>() > 1)
            {
                return false;
            }
            Rigidbody2D attachedRigidbody = (base.target as Collider2D).attachedRigidbody;
            return (((attachedRigidbody != null) && attachedRigidbody.useAutoMass) && (attachedRigidbody.bodyType == RigidbodyType2D.Dynamic));
        }

        private void ShowColliderInfoProperties()
        {
            this.m_ShowInfo.target = EditorGUILayout.Foldout(this.m_ShowInfo.target, "Info", true);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowInfo.faded))
            {
                if (base.targets.Length == 1)
                {
                    Collider2D collider = base.targets[0] as Collider2D;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Attached Body", collider.attachedRigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
                    EditorGUILayout.FloatField("Friction", collider.friction, new GUILayoutOption[0]);
                    EditorGUILayout.FloatField("Bounciness", collider.bounciness, new GUILayoutOption[0]);
                    EditorGUILayout.FloatField("Shape Count", (float) collider.shapeCount, new GUILayoutOption[0]);
                    if (collider.isActiveAndEnabled)
                    {
                        EditorGUILayout.BoundsField("Bounds", collider.bounds, new GUILayoutOption[0]);
                    }
                    EditorGUI.EndDisabledGroup();
                    this.ShowContacts(collider);
                    base.Repaint();
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot show Info properties when multiple colliders are selected.", MessageType.Info);
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void ShowContacts(Collider2D collider)
        {
            EditorGUI.indentLevel++;
            this.m_ShowContacts.target = EditorGUILayout.Foldout(this.m_ShowContacts.target, "Contacts");
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowContacts.faded))
            {
                int contacts = collider.GetContacts(m_Contacts);
                if (contacts > 0)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(180f) };
                    this.m_ContactScrollPosition = EditorGUILayout.BeginScrollView(this.m_ContactScrollPosition, options);
                    EditorGUI.BeginDisabledGroup(true);
                    for (int i = 0; i < contacts; i++)
                    {
                        ContactPoint2D pointd = m_Contacts[i];
                        EditorGUILayout.HelpBox($"Contact#{i}", MessageType.None);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.Vector2Field("Point", pointd.point, new GUILayoutOption[0]);
                        EditorGUILayout.Vector2Field("Normal", pointd.normal, new GUILayoutOption[0]);
                        EditorGUILayout.Vector2Field("Relative Velocity", pointd.relativeVelocity, new GUILayoutOption[0]);
                        EditorGUILayout.FloatField("Normal Impulse", pointd.normalImpulse, new GUILayoutOption[0]);
                        EditorGUILayout.FloatField("Tangent Impulse", pointd.tangentImpulse, new GUILayoutOption[0]);
                        EditorGUILayout.ObjectField("Collider", pointd.collider, typeof(Collider2D), false, new GUILayoutOption[0]);
                        EditorGUILayout.ObjectField("Rigidbody", pointd.rigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
                        EditorGUILayout.ObjectField("OtherRigidbody", pointd.otherRigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                    }
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.HelpBox("No Contacts", MessageType.Info);
                }
            }
            FixedEndFadeGroup(this.m_ShowContacts.faded);
            EditorGUI.indentLevel--;
        }

        protected class Styles
        {
            public static readonly GUIContent s_AutoTilingLabel = EditorGUIUtility.TextContent("Auto Tiling | When enabled, the collider's shape will update automaticaly based on the SpriteRenderer's tiling properties");
            public static readonly GUIContent s_ColliderEditDisableHelp = EditorGUIUtility.TextContent("Collider cannot be edited because it is driven by SpriteRenderer's tiling properties.");
        }
    }
}


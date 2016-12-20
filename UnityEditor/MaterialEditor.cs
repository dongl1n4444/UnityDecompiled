namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>The Unity Material Editor.</para>
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(Material))]
    public class MaterialEditor : Editor
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <forceVisible>k__BackingField;
        /// <summary>
        /// <para>Useful for indenting shader properties that need the same indent as mini texture field.</para>
        /// </summary>
        public const int kMiniTextureFieldLabelIndentLevel = 2;
        private const float kMiniWarningMessageHeight = 27f;
        private const float kQueuePopupWidth = 100f;
        private const float kSpaceBetweenFlexibleAreaAndField = 5f;
        private const float kSpacingUnderTexture = 6f;
        private const float kWarningMessageHeight = 33f;
        private bool m_CheckSetup;
        private ShaderGUI m_CustomShaderGUI;
        private TextureDimension m_DesiredTexdim;
        private string m_InfoMessage;
        private bool m_InsidePropertiesGUI;
        private bool m_IsVisible;
        private int m_LightMode = 1;
        private Vector2 m_PreviewDir = new Vector2(0f, -20f);
        private PreviewRenderUtility m_PreviewUtility;
        private Color m_PreviousGUIColor;
        private MaterialPropertyBlock m_PropertyBlock;
        private ReflectionProbePicker m_ReflectionProbePicker = new ReflectionProbePicker();
        private Renderer m_RendererForAnimationMode;
        private int m_SelectedMesh;
        private Shader m_Shader;
        private int m_TimeUpdate;
        [NonSerialized]
        private bool m_TriedCreatingCustomGUI;
        private static int s_ControlHash = "EditorTextField".GetHashCode();
        private static readonly GUIContent[] s_LightIcons = new GUIContent[2];
        private static readonly Mesh[] s_Meshes = new Mesh[5];
        private static readonly GUIContent[] s_MeshIcons = new GUIContent[5];
        private static readonly GUIContent s_OffsetText = new GUIContent("Offset");
        private static Mesh s_PlaneMesh;
        private static readonly GUIContent s_TilingText = new GUIContent("Tiling");
        private static readonly GUIContent[] s_TimeIcons = new GUIContent[2];

        /// <summary>
        /// <para>Apply initial MaterialPropertyDrawer values.</para>
        /// </summary>
        /// <param name="material"></param>
        /// <param name="targets"></param>
        public static void ApplyMaterialPropertyDrawers(Material material)
        {
            Object[] targets = new Object[] { material };
            ApplyMaterialPropertyDrawers(targets);
        }

        /// <summary>
        /// <para>Apply initial MaterialPropertyDrawer values.</para>
        /// </summary>
        /// <param name="material"></param>
        /// <param name="targets"></param>
        public static void ApplyMaterialPropertyDrawers(Object[] targets)
        {
            if ((targets != null) && (targets.Length != 0))
            {
                Material material = targets[0] as Material;
                if (material != null)
                {
                    Shader shader = material.shader;
                    MaterialProperty[] materialProperties = GetMaterialProperties(targets);
                    for (int i = 0; i < materialProperties.Length; i++)
                    {
                        MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(shader, materialProperties[i].name);
                        if ((handler != null) && (handler.propertyDrawer != null))
                        {
                            handler.propertyDrawer.Apply(materialProperties[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>Called when the Editor is woken up.</para>
        /// </summary>
        public virtual void Awake()
        {
            this.m_IsVisible = InternalEditorUtility.GetIsInspectorExpanded(base.target);
            if (GetPreviewType(base.target as Material) == PreviewType.Skybox)
            {
                this.m_PreviewDir = new Vector2(0f, 50f);
            }
        }

        public void BeginAnimatedCheck(MaterialProperty prop)
        {
            if (this.m_RendererForAnimationMode != null)
            {
                this.m_PreviousGUIColor = GUI.color;
                if (MaterialAnimationUtility.IsAnimated(prop, this.m_RendererForAnimationMode))
                {
                    GUI.color = AnimationMode.animatedPropertyColor;
                }
            }
        }

        private void CheckSetup()
        {
            if (this.m_CheckSetup && (this.m_Shader != null))
            {
                this.m_CheckSetup = false;
                if ((this.m_CustomShaderGUI == null) && !this.IsMaterialEditor(this.m_Shader.customEditor))
                {
                    object[] args = new object[] { this.m_Shader.name, this.m_Shader.customEditor };
                    Debug.LogWarningFormat("Could not create a custom UI for the shader '{0}'. The shader has the following: 'CustomEditor = {1}'. Does the custom editor specified include its namespace? And does the class either derive from ShaderGUI or MaterialEditor?", args);
                }
            }
        }

        [Obsolete("Use ColorProperty with MaterialProperty instead.")]
        public Color ColorProperty(string propertyName, string label)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyName);
            return this.ColorProperty(materialProperty, label);
        }

        /// <summary>
        /// <para>Draw a property field for a color shader property.</para>
        /// </summary>
        /// <param name="label">Label for the property.</param>
        /// <param name="position"></param>
        /// <param name="prop"></param>
        public Color ColorProperty(MaterialProperty prop, string label)
        {
            return this.ColorPropertyInternal(prop, new GUIContent(label));
        }

        /// <summary>
        /// <para>Draw a property field for a color shader property.</para>
        /// </summary>
        /// <param name="label">Label for the property.</param>
        /// <param name="position"></param>
        /// <param name="prop"></param>
        public Color ColorProperty(Rect position, MaterialProperty prop, string label)
        {
            return this.ColorPropertyInternal(position, prop, new GUIContent(label));
        }

        internal Color ColorPropertyInternal(MaterialProperty prop, GUIContent label)
        {
            Rect position = this.GetPropertyRect(prop, label, true);
            return this.ColorPropertyInternal(position, prop, label);
        }

        internal Color ColorPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            bool hdr = (prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None;
            bool showAlpha = true;
            Color color = EditorGUI.ColorField(position, label, prop.colorValue, true, showAlpha, hdr, null);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                prop.colorValue = color;
            }
            return prop.colorValue;
        }

        private void CreateCustomShaderGUI(Shader shader, string oldEditorName)
        {
            if ((shader == null) || string.IsNullOrEmpty(shader.customEditor))
            {
                this.m_CustomShaderGUI = null;
            }
            else if (oldEditorName != shader.customEditor)
            {
                this.m_CustomShaderGUI = ShaderGUIUtility.CreateShaderGUI(shader.customEditor);
                this.m_CheckSetup = true;
            }
        }

        /// <summary>
        /// <para>Default handling of preview area for materials.</para>
        /// </summary>
        /// <param name="r"></param>
        /// <param name="background"></param>
        public void DefaultPreviewGUI(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Material preview \nnot available");
                }
            }
            else
            {
                this.Init();
                Material target = base.target as Material;
                if (DoesPreviewAllowRotation(GetPreviewType(target)))
                {
                    this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_PreviewUtility.BeginPreview(r, background);
                    this.DoRenderPreview();
                    this.m_PreviewUtility.EndAndDrawPreview(r);
                }
            }
        }

        /// <summary>
        /// <para>Default toolbar for material preview area.</para>
        /// </summary>
        public void DefaultPreviewSettingsGUI()
        {
            if (ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                this.Init();
                Material target = base.target as Material;
                PreviewType previewType = GetPreviewType(target);
                if ((base.targets.Length > 1) || (previewType == PreviewType.Mesh))
                {
                    Rect rect;
                    this.m_TimeUpdate = PreviewGUI.CycleButton(this.m_TimeUpdate, s_TimeIcons);
                    this.m_SelectedMesh = PreviewGUI.CycleButton(this.m_SelectedMesh, s_MeshIcons);
                    this.m_LightMode = PreviewGUI.CycleButton(this.m_LightMode, s_LightIcons);
                    if (this.PreviewSettingsMenuButton(out rect))
                    {
                        PopupWindow.Show(rect, this.m_ReflectionProbePicker);
                    }
                }
            }
        }

        /// <summary>
        /// <para>Handles UI for one shader property ignoring any custom drawers.</para>
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        /// <param name="position"></param>
        public void DefaultShaderProperty(MaterialProperty prop, string label)
        {
            this.DefaultShaderPropertyInternal(prop, new GUIContent(label));
        }

        /// <summary>
        /// <para>Handles UI for one shader property ignoring any custom drawers.</para>
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        /// <param name="position"></param>
        public void DefaultShaderProperty(Rect position, MaterialProperty prop, string label)
        {
            this.DefaultShaderPropertyInternal(position, prop, new GUIContent(label));
        }

        internal void DefaultShaderPropertyInternal(MaterialProperty prop, GUIContent label)
        {
            Rect position = this.GetPropertyRect(prop, label, true);
            this.DefaultShaderPropertyInternal(position, prop, label);
        }

        internal void DefaultShaderPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
        {
            switch (prop.type)
            {
                case MaterialProperty.PropType.Color:
                    this.ColorPropertyInternal(position, prop, label);
                    break;

                case MaterialProperty.PropType.Vector:
                    this.VectorProperty(position, prop, label.text);
                    break;

                case MaterialProperty.PropType.Float:
                    this.FloatPropertyInternal(position, prop, label);
                    break;

                case MaterialProperty.PropType.Range:
                    this.RangePropertyInternal(position, prop, label);
                    break;

                case MaterialProperty.PropType.Texture:
                    this.TextureProperty(position, prop, label.text);
                    break;

                default:
                    GUI.Label(position, string.Concat(new object[] { "Unknown property type: ", prop.name, ": ", (int) prop.type }));
                    break;
            }
        }

        private void DetectShaderChanged()
        {
            Material target = base.target as Material;
            if (target.shader != this.m_Shader)
            {
                string oldEditorName = (this.m_Shader == null) ? string.Empty : this.m_Shader.customEditor;
                this.CreateCustomShaderGUI(target.shader, oldEditorName);
                this.m_Shader = target.shader;
                InspectorWindow.RepaintAllInspectors();
            }
        }

        private static bool DoesPreviewAllowRotation(PreviewType type)
        {
            return (type != PreviewType.Plane);
        }

        internal static int DoIntRangeProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0f;
            int num2 = EditorGUI.IntSlider(position, label, (int) prop.floatValue, (int) prop.rangeLimits.x, (int) prop.rangeLimits.y);
            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = labelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = num2;
            }
            return (int) prop.floatValue;
        }

        internal static float DoPowerRangeProperty(Rect position, MaterialProperty prop, GUIContent label, float power)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0f;
            float num2 = EditorGUI.PowerSlider(position, label, prop.floatValue, prop.rangeLimits.x, prop.rangeLimits.y, power);
            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = labelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = num2;
            }
            return prop.floatValue;
        }

        private void DoRenderPreview()
        {
            if ((this.m_PreviewUtility.m_RenderTexture.width > 0) && (this.m_PreviewUtility.m_RenderTexture.height > 0))
            {
                Color color;
                Material target = base.target as Material;
                PreviewType previewType = GetPreviewType(target);
                this.m_PreviewUtility.m_Camera.transform.position = (Vector3) (-Vector3.forward * 5f);
                this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
                if (this.m_LightMode == 0)
                {
                    this.m_PreviewUtility.m_Light[0].intensity = 1f;
                    this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
                    this.m_PreviewUtility.m_Light[1].intensity = 0f;
                    color = new Color(0.2f, 0.2f, 0.2f, 0f);
                }
                else
                {
                    this.m_PreviewUtility.m_Light[0].intensity = 1f;
                    this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
                    this.m_PreviewUtility.m_Light[1].intensity = 1f;
                    color = new Color(0.2f, 0.2f, 0.2f, 0f);
                }
                InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, color);
                Quaternion identity = Quaternion.identity;
                if (DoesPreviewAllowRotation(previewType))
                {
                    identity = Quaternion.Euler(this.m_PreviewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.m_PreviewDir.x, 0f);
                }
                Mesh mesh = s_Meshes[this.m_SelectedMesh];
                switch (previewType)
                {
                    case PreviewType.Plane:
                        mesh = s_PlaneMesh;
                        break;

                    case PreviewType.Mesh:
                        this.m_PreviewUtility.m_Camera.transform.position = (Vector3) (Quaternion.Inverse(identity) * this.m_PreviewUtility.m_Camera.transform.position);
                        this.m_PreviewUtility.m_Camera.transform.LookAt(Vector3.zero);
                        identity = Quaternion.identity;
                        break;

                    case PreviewType.Skybox:
                        mesh = null;
                        this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.Inverse(identity);
                        this.m_PreviewUtility.m_Camera.fieldOfView = 120f;
                        break;
                }
                if (mesh != null)
                {
                    this.m_PreviewUtility.DrawMesh(mesh, Vector3.zero, identity, target, 0, null, this.m_ReflectionProbePicker.Target);
                }
                bool fog = RenderSettings.fog;
                Unsupported.SetRenderSettingsUseFogNoDirty(false);
                this.m_PreviewUtility.m_Camera.Render();
                if (previewType == PreviewType.Skybox)
                {
                    GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                    InternalEditorUtility.DrawSkyboxMaterial(target, this.m_PreviewUtility.m_Camera);
                    GL.sRGBWrite = false;
                }
                Unsupported.SetRenderSettingsUseFogNoDirty(fog);
                InternalEditorUtility.RemoveCustomLighting();
            }
        }

        public void EndAnimatedCheck()
        {
            if (this.m_RendererForAnimationMode != null)
            {
                GUI.color = this.m_PreviousGUIColor;
            }
        }

        private void ExtraPropertyAfterTexture(Rect r, MaterialProperty property)
        {
            if (((property.type == MaterialProperty.PropType.Float) || (property.type == MaterialProperty.PropType.Color)) && (r.width > EditorGUIUtility.fieldWidth))
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = r.width - EditorGUIUtility.fieldWidth;
                this.ShaderProperty(r, property, " ");
                EditorGUIUtility.labelWidth = labelWidth;
            }
            else
            {
                this.ShaderProperty(r, property, string.Empty);
            }
        }

        [Obsolete("Use FloatProperty with MaterialProperty instead.")]
        public float FloatProperty(string propertyName, string label)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyName);
            return this.FloatProperty(materialProperty, label);
        }

        /// <summary>
        /// <para>Draw a property field for a float shader property.</para>
        /// </summary>
        /// <param name="label">Label for the property.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        public float FloatProperty(MaterialProperty prop, string label)
        {
            return this.FloatPropertyInternal(prop, new GUIContent(label));
        }

        /// <summary>
        /// <para>Draw a property field for a float shader property.</para>
        /// </summary>
        /// <param name="label">Label for the property.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        public float FloatProperty(Rect position, MaterialProperty prop, string label)
        {
            return this.FloatPropertyInternal(position, prop, new GUIContent(label));
        }

        internal float FloatPropertyInternal(MaterialProperty prop, GUIContent label)
        {
            Rect position = this.GetPropertyRect(prop, label, true);
            return this.FloatPropertyInternal(position, prop, label);
        }

        internal float FloatPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float num = EditorGUI.FloatField(position, label, prop.floatValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = num;
            }
            return prop.floatValue;
        }

        private static Renderer GetAssociatedRenderFromInspector()
        {
            if (InspectorWindow.s_CurrentInspectorWindow != null)
            {
                Editor[] activeEditors = InspectorWindow.s_CurrentInspectorWindow.tracker.activeEditors;
                foreach (Editor editor in activeEditors)
                {
                    Renderer target = editor.target as Renderer;
                    if (target != null)
                    {
                        return target;
                    }
                }
            }
            return null;
        }

        [Obsolete("Use GetMaterialProperty instead.")]
        public Color GetColor(string propertyName, out bool hasMixedValue)
        {
            hasMixedValue = false;
            Color color = ((Material) base.targets[0]).GetColor(propertyName);
            for (int i = 1; i < base.targets.Length; i++)
            {
                if (((Material) base.targets[i]).GetColor(propertyName) != color)
                {
                    hasMixedValue = true;
                    return color;
                }
            }
            return color;
        }

        private Rect GetControlRectForSingleLine()
        {
            return EditorGUILayout.GetControlRect(true, 18f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
        }

        /// <summary>
        /// <para>Calculate height needed for the property, ignoring custom drawers.</para>
        /// </summary>
        /// <param name="prop"></param>
        public static float GetDefaultPropertyHeight(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Vector)
            {
                return 32f;
            }
            if (prop.type == MaterialProperty.PropType.Texture)
            {
                return (GetTextureFieldHeight() + 6f);
            }
            return 16f;
        }

        /// <summary>
        /// <para>Utility method for GUI layouting ShaderGUI. Used e.g for the rect after a left aligned Color field.</para>
        /// </summary>
        /// <param name="r">Field Rect.</param>
        /// <returns>
        /// <para>A sub rect of the input Rect.</para>
        /// </returns>
        public static Rect GetFlexibleRectBetweenFieldAndRightEdge(Rect r)
        {
            Rect rectAfterLabelWidth = GetRectAfterLabelWidth(r);
            rectAfterLabelWidth.xMin += EditorGUIUtility.fieldWidth + 5f;
            return rectAfterLabelWidth;
        }

        /// <summary>
        /// <para>Utility method for GUI layouting ShaderGUI.</para>
        /// </summary>
        /// <param name="r">Field Rect.</param>
        /// <returns>
        /// <para>A sub rect of the input Rect.</para>
        /// </returns>
        public static Rect GetFlexibleRectBetweenLabelAndField(Rect r)
        {
            return new Rect(r.x + EditorGUIUtility.labelWidth, r.y, ((r.width - EditorGUIUtility.labelWidth) - EditorGUIUtility.fieldWidth) - 5f, EditorGUIUtility.singleLineHeight);
        }

        [Obsolete("Use GetMaterialProperty instead.")]
        public float GetFloat(string propertyName, out bool hasMixedValue)
        {
            hasMixedValue = false;
            float @float = ((Material) base.targets[0]).GetFloat(propertyName);
            for (int i = 1; i < base.targets.Length; i++)
            {
                if (((Material) base.targets[i]).GetFloat(propertyName) != @float)
                {
                    hasMixedValue = true;
                    return @float;
                }
            }
            return @float;
        }

        private static MaterialGlobalIlluminationFlags GetGlobalIlluminationInt(MaterialGlobalIlluminationFlags flags)
        {
            MaterialGlobalIlluminationFlags none = MaterialGlobalIlluminationFlags.None;
            if ((flags & MaterialGlobalIlluminationFlags.RealtimeEmissive) != MaterialGlobalIlluminationFlags.None)
            {
                return MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            if ((flags & MaterialGlobalIlluminationFlags.BakedEmissive) != MaterialGlobalIlluminationFlags.None)
            {
                none = MaterialGlobalIlluminationFlags.BakedEmissive;
            }
            return none;
        }

        /// <summary>
        /// <para>Utility method for GUI layouting ShaderGUI.</para>
        /// </summary>
        /// <param name="r">Field Rect.</param>
        /// <returns>
        /// <para>A sub rect of the input Rect.</para>
        /// </returns>
        public static Rect GetLeftAlignedFieldRect(Rect r)
        {
            return new Rect(r.x + EditorGUIUtility.labelWidth, r.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
        }

        /// <summary>
        /// <para>Get shader property information of the passed materials.</para>
        /// </summary>
        /// <param name="mats"></param>
        public static MaterialProperty[] GetMaterialProperties(Object[] mats)
        {
            if (mats == null)
            {
                throw new ArgumentNullException("mats");
            }
            if (Array.IndexOf<Object>(mats, null) >= 0)
            {
                throw new ArgumentException("List of materials contains null");
            }
            return ShaderUtil.GetMaterialProperties(mats);
        }

        /// <summary>
        /// <para>Get information about a single shader property.</para>
        /// </summary>
        /// <param name="mats">Selected materials.</param>
        /// <param name="name">Property name.</param>
        /// <param name="propertyIndex">Property index.</param>
        public static MaterialProperty GetMaterialProperty(Object[] mats, int propertyIndex)
        {
            if (mats == null)
            {
                throw new ArgumentNullException("mats");
            }
            if (Array.IndexOf<Object>(mats, null) >= 0)
            {
                throw new ArgumentException("List of materials contains null");
            }
            return ShaderUtil.GetMaterialProperty_Index(mats, propertyIndex);
        }

        /// <summary>
        /// <para>Get information about a single shader property.</para>
        /// </summary>
        /// <param name="mats">Selected materials.</param>
        /// <param name="name">Property name.</param>
        /// <param name="propertyIndex">Property index.</param>
        public static MaterialProperty GetMaterialProperty(Object[] mats, string name)
        {
            if (mats == null)
            {
                throw new ArgumentNullException("mats");
            }
            if (Array.IndexOf<Object>(mats, null) >= 0)
            {
                throw new ArgumentException("List of materials contains null");
            }
            return ShaderUtil.GetMaterialProperty(mats, name);
        }

        private static PreviewType GetPreviewType(Material mat)
        {
            if (mat != null)
            {
                switch (mat.GetTag("PreviewType", false, string.Empty).ToLower())
                {
                    case "plane":
                        return PreviewType.Plane;

                    case "skybox":
                        return PreviewType.Skybox;
                }
                if ((mat.shader != null) && mat.shader.name.Contains("Skybox"))
                {
                    return PreviewType.Skybox;
                }
            }
            return PreviewType.Mesh;
        }

        /// <summary>
        /// <para>Calculate height needed for the property.</para>
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        public float GetPropertyHeight(MaterialProperty prop)
        {
            return this.GetPropertyHeight(prop, prop.displayName);
        }

        /// <summary>
        /// <para>Calculate height needed for the property.</para>
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        public float GetPropertyHeight(MaterialProperty prop, string label)
        {
            float num = 0f;
            MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(((Material) base.target).shader, prop.name);
            if (handler != null)
            {
                if (label == null)
                {
                }
                num = handler.GetPropertyHeight(prop, prop.displayName, this);
                if (handler.propertyDrawer != null)
                {
                    return num;
                }
            }
            return (num + GetDefaultPropertyHeight(prop));
        }

        private Rect GetPropertyRect(MaterialProperty prop, string label, bool ignoreDrawer)
        {
            float height = 0f;
            if (!ignoreDrawer)
            {
                MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(((Material) base.target).shader, prop.name);
                if (handler != null)
                {
                    if (label == null)
                    {
                    }
                    height = handler.GetPropertyHeight(prop, prop.displayName, this);
                    if (handler.propertyDrawer != null)
                    {
                        return EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField, new GUILayoutOption[0]);
                    }
                }
            }
            return EditorGUILayout.GetControlRect(true, height + GetDefaultPropertyHeight(prop), EditorStyles.layerMaskField, new GUILayoutOption[0]);
        }

        private Rect GetPropertyRect(MaterialProperty prop, GUIContent label, bool ignoreDrawer)
        {
            return this.GetPropertyRect(prop, label.text, ignoreDrawer);
        }

        /// <summary>
        /// <para>Utility method for GUI layouting ShaderGUI. This is the rect after the label which can be used for multiple properties. The input rect can be fetched by calling: EditorGUILayout.GetControlRect.</para>
        /// </summary>
        /// <param name="r">Line Rect.</param>
        /// <returns>
        /// <para>A sub rect of the input Rect.</para>
        /// </returns>
        public static Rect GetRectAfterLabelWidth(Rect r)
        {
            return new Rect(r.x + EditorGUIUtility.labelWidth, r.y, r.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        }

        /// <summary>
        /// <para>Utility method for GUI layouting ShaderGUI.</para>
        /// </summary>
        /// <param name="r">Field Rect.</param>
        /// <returns>
        /// <para>A sub rect of the input Rect.</para>
        /// </returns>
        public static Rect GetRightAlignedFieldRect(Rect r)
        {
            return new Rect(r.xMax - EditorGUIUtility.fieldWidth, r.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
        }

        [Obsolete("Use GetMaterialProperty instead.")]
        public Texture GetTexture(string propertyName, out bool hasMixedValue)
        {
            hasMixedValue = false;
            Texture texture = ((Material) base.targets[0]).GetTexture(propertyName);
            for (int i = 1; i < base.targets.Length; i++)
            {
                if (((Material) base.targets[i]).GetTexture(propertyName) != texture)
                {
                    hasMixedValue = true;
                    return texture;
                }
            }
            return texture;
        }

        private static float GetTextureFieldHeight()
        {
            return 64f;
        }

        [Obsolete("Use MaterialProperty instead.")]
        public Vector2 GetTextureOffset(string propertyName, out bool hasMixedValueX, out bool hasMixedValueY)
        {
            hasMixedValueX = false;
            hasMixedValueY = false;
            Vector2 textureOffset = ((Material) base.targets[0]).GetTextureOffset(propertyName);
            for (int i = 1; i < base.targets.Length; i++)
            {
                Vector2 vector2 = ((Material) base.targets[i]).GetTextureOffset(propertyName);
                if (vector2.x != textureOffset.x)
                {
                    hasMixedValueX = true;
                }
                if (vector2.y != textureOffset.y)
                {
                    hasMixedValueY = true;
                }
                if (hasMixedValueX && hasMixedValueY)
                {
                    return textureOffset;
                }
            }
            return textureOffset;
        }

        /// <summary>
        /// <para>Returns the free rect below the label and before the large thumb object field. Is used for e.g. tiling and offset properties.</para>
        /// </summary>
        /// <param name="position">The total rect of the texture property.</param>
        public Rect GetTexturePropertyCustomArea(Rect position)
        {
            EditorGUI.indentLevel++;
            position.height = GetTextureFieldHeight();
            Rect source = position;
            source.yMin += 16f;
            source.xMax -= EditorGUIUtility.fieldWidth + 2f;
            source = EditorGUI.IndentedRect(source);
            EditorGUI.indentLevel--;
            return source;
        }

        [Obsolete("Use MaterialProperty instead.")]
        public Vector2 GetTextureScale(string propertyName, out bool hasMixedValueX, out bool hasMixedValueY)
        {
            hasMixedValueX = false;
            hasMixedValueY = false;
            Vector2 textureScale = ((Material) base.targets[0]).GetTextureScale(propertyName);
            for (int i = 1; i < base.targets.Length; i++)
            {
                Vector2 vector2 = ((Material) base.targets[i]).GetTextureScale(propertyName);
                if (vector2.x != textureScale.x)
                {
                    hasMixedValueX = true;
                }
                if (vector2.y != textureScale.y)
                {
                    hasMixedValueY = true;
                }
                if (hasMixedValueX && hasMixedValueY)
                {
                    return textureScale;
                }
            }
            return textureScale;
        }

        internal static Type GetTextureTypeFromDimension(TextureDimension dim)
        {
            switch (dim)
            {
                case TextureDimension.Any:
                    return typeof(Texture);

                case TextureDimension.Tex2D:
                    return typeof(Texture);

                case TextureDimension.Tex3D:
                    return typeof(Texture3D);

                case TextureDimension.Cube:
                    return typeof(Cubemap);

                case TextureDimension.Tex2DArray:
                    return typeof(Texture2DArray);

                case TextureDimension.CubeArray:
                    return typeof(CubemapArray);
            }
            return null;
        }

        [Obsolete("Use GetMaterialProperty instead.")]
        public Vector4 GetVector(string propertyName, out bool hasMixedValue)
        {
            hasMixedValue = false;
            Vector4 vector = ((Material) base.targets[0]).GetVector(propertyName);
            for (int i = 1; i < base.targets.Length; i++)
            {
                if (((Material) base.targets[i]).GetVector(propertyName) != vector)
                {
                    hasMixedValue = true;
                    return vector;
                }
            }
            return vector;
        }

        internal void HandleRenderer(Renderer r, int materialIndex, Event evt)
        {
            bool flag = false;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    flag = true;
                    break;

                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    flag = true;
                    break;
            }
            if (flag)
            {
                Undo.RecordObject(r, "Assign Material");
                Material[] sharedMaterials = r.sharedMaterials;
                bool alt = evt.alt;
                bool flag3 = (materialIndex >= 0) && (materialIndex < r.sharedMaterials.Length);
                if (!alt && flag3)
                {
                    sharedMaterials[materialIndex] = base.target as Material;
                }
                else
                {
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        sharedMaterials[i] = base.target as Material;
                    }
                }
                r.sharedMaterials = sharedMaterials;
                evt.Use();
            }
        }

        internal void HandleSkybox(GameObject go, Event evt)
        {
            bool flag = go == 0;
            bool flag2 = false;
            if (!flag || (evt.type == EventType.DragExited))
            {
                evt.Use();
            }
            else
            {
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        flag2 = true;
                        break;

                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        flag2 = true;
                        break;
                }
            }
            if (flag2)
            {
                Undo.RecordObject(Object.FindObjectOfType<RenderSettings>(), "Assign Skybox Material");
                RenderSettings.skybox = base.target as Material;
                evt.Use();
            }
        }

        private bool HasMultipleMixedQueueValues()
        {
            int materialRawRenderQueue = ShaderUtil.GetMaterialRawRenderQueue(base.targets[0] as Material);
            for (int i = 1; i < base.targets.Length; i++)
            {
                if (materialRawRenderQueue != ShaderUtil.GetMaterialRawRenderQueue(base.targets[i] as Material))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasMultipleMixedShaderValues()
        {
            Shader shader = (base.targets[0] as Material).shader;
            for (int i = 1; i < base.targets.Length; i++)
            {
                if (shader != (base.targets[i] as Material).shader)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <para>Can this component be Previewed in its current state?</para>
        /// </summary>
        /// <returns>
        /// <para>True if this component can be Previewed in its current state.</para>
        /// </returns>
        public sealed override bool HasPreviewGUI()
        {
            return true;
        }

        /// <summary>
        /// <para>Make a help box with a message and button. Returns true, if button was pressed.</para>
        /// </summary>
        /// <param name="messageContent">The message text.</param>
        /// <param name="buttonContent">The button text.</param>
        /// <returns>
        /// <para>Returns true, if button was pressed.</para>
        /// </returns>
        public bool HelpBoxWithButton(GUIContent messageContent, GUIContent buttonContent)
        {
            Rect position = GUILayoutUtility.GetRect(messageContent, EditorStyles.helpBox);
            GUILayoutUtility.GetRect((float) 1f, (float) 25f);
            position.height += 25f;
            GUI.Label(position, messageContent, EditorStyles.helpBox);
            Rect rect2 = new Rect((position.xMax - 60f) - 4f, (position.yMax - 20f) - 4f, 60f, 20f);
            return GUI.Button(rect2, buttonContent);
        }

        private void Init()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
                EditorUtility.SetCameraAnimateMaterials(this.m_PreviewUtility.m_Camera, true);
            }
            if (s_Meshes[0] == null)
            {
                GameObject obj2 = (GameObject) EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
                obj2.SetActive(false);
                IEnumerator enumerator = obj2.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        MeshFilter component = current.GetComponent<MeshFilter>();
                        switch (current.name)
                        {
                            case "sphere":
                            {
                                s_Meshes[0] = component.sharedMesh;
                                continue;
                            }
                            case "cube":
                            {
                                s_Meshes[1] = component.sharedMesh;
                                continue;
                            }
                            case "cylinder":
                            {
                                s_Meshes[2] = component.sharedMesh;
                                continue;
                            }
                            case "torus":
                            {
                                s_Meshes[3] = component.sharedMesh;
                                continue;
                            }
                        }
                        Debug.Log("Something is wrong, weird object found: " + current.name);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                s_MeshIcons[0] = EditorGUIUtility.IconContent("PreMatSphere");
                s_MeshIcons[1] = EditorGUIUtility.IconContent("PreMatCube");
                s_MeshIcons[2] = EditorGUIUtility.IconContent("PreMatCylinder");
                s_MeshIcons[3] = EditorGUIUtility.IconContent("PreMatTorus");
                s_MeshIcons[4] = EditorGUIUtility.IconContent("PreMatQuad");
                s_LightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
                s_LightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
                s_TimeIcons[0] = EditorGUIUtility.IconContent("PlayButton");
                s_TimeIcons[1] = EditorGUIUtility.IconContent("PauseButton");
                Mesh builtinResource = Resources.GetBuiltinResource(typeof(Mesh), "Quad.fbx") as Mesh;
                s_Meshes[4] = builtinResource;
                s_PlaneMesh = builtinResource;
            }
        }

        private bool IsMaterialEditor(string customEditorName)
        {
            string str = "UnityEditor." + customEditorName;
            foreach (Assembly assembly in EditorAssemblies.loadedAssemblies)
            {
                Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
                foreach (Type type in typesFromAssembly)
                {
                    if ((type.FullName.Equals(customEditorName, StringComparison.Ordinal) || type.FullName.Equals(str, StringComparison.Ordinal)) && typeof(MaterialEditor).IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// <para>This function will draw the UI for the lightmap emission property. (None, Realtime, baked)
        /// 
        /// See Also: MaterialLightmapFlags.</para>
        /// </summary>
        public void LightmapEmissionProperty()
        {
            this.LightmapEmissionProperty(0);
        }

        public void LightmapEmissionProperty(int labelIndent)
        {
            Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
            this.LightmapEmissionProperty(position, labelIndent);
        }

        public void LightmapEmissionProperty(Rect position, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;
            Object[] targets = base.targets;
            Material target = (Material) base.target;
            MaterialGlobalIlluminationFlags globalIlluminationInt = GetGlobalIlluminationInt(target.globalIlluminationFlags);
            bool flag = false;
            for (int i = 1; i < targets.Length; i++)
            {
                Material material2 = (Material) targets[i];
                if (GetGlobalIlluminationInt(material2.globalIlluminationFlags) != globalIlluminationInt)
                {
                    flag = true;
                }
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = flag;
            globalIlluminationInt = (MaterialGlobalIlluminationFlags) EditorGUI.IntPopup(position, Styles.lightmapEmissiveLabel, (int) globalIlluminationInt, Styles.lightmapEmissiveStrings, Styles.lightmapEmissiveValues);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Material material3 in targets)
                {
                    MaterialGlobalIlluminationFlags flags2 = material3.globalIlluminationFlags & ~(MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive);
                    flags2 |= globalIlluminationInt;
                    material3.globalIlluminationFlags = flags2;
                }
            }
            EditorGUI.indentLevel -= labelIndent;
        }

        internal override void OnAssetStoreInspectorGUI()
        {
            this.OnInspectorGUI();
        }

        /// <summary>
        /// <para>Called when the editor is disabled, if overridden please call the base OnDisable() to ensure that the material inspector is set up properly.</para>
        /// </summary>
        public virtual void OnDisable()
        {
            this.m_ReflectionProbePicker.OnDisable();
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        /// <summary>
        /// <para>Called when the editor is enabled, if overridden please call the base OnEnable() to ensure that the material inspector is set up properly.</para>
        /// </summary>
        public virtual void OnEnable()
        {
            this.m_Shader = base.serializedObject.FindProperty("m_Shader").objectReferenceValue as Shader;
            this.CreateCustomShaderGUI(this.m_Shader, "");
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.PropertiesChanged();
            this.m_PropertyBlock = new MaterialPropertyBlock();
            this.m_ReflectionProbePicker.OnEnable();
        }

        internal override void OnHeaderControlsGUI()
        {
            base.serializedObject.Update();
            using (new EditorGUI.DisabledScope(!this.IsEnabled()))
            {
                EditorGUIUtility.labelWidth = 50f;
                this.ShaderPopup("MiniPulldown");
                if (((this.m_Shader != null) && this.HasMultipleMixedShaderValues()) && ((this.m_Shader.hideFlags & HideFlags.DontSave) == HideFlags.None))
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button("Edit...", EditorStyles.miniButton, options))
                    {
                        AssetDatabase.OpenAsset(this.m_Shader);
                    }
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            Rect rect = Editor.DrawHeaderGUI(this, this.targetTitle, !this.forceVisible ? 10f : 0f);
            int controlID = GUIUtility.GetControlID(0xb26e, FocusType.Passive);
            if (!this.forceVisible)
            {
                Rect inspectorTitleBarObjectFoldoutRenderRect = EditorGUI.GetInspectorTitleBarObjectFoldoutRenderRect(rect);
                inspectorTitleBarObjectFoldoutRenderRect.y = rect.yMax - 17f;
                bool isExpanded = EditorGUI.DoObjectFoldout(this.m_IsVisible, rect, inspectorTitleBarObjectFoldoutRenderRect, base.targets, controlID);
                if (isExpanded != this.m_IsVisible)
                {
                    this.m_IsVisible = isExpanded;
                    InternalEditorUtility.SetIsInspectorExpanded(base.target, isExpanded);
                }
            }
        }

        /// <summary>
        /// <para>Implement specific MaterialEditor GUI code here. If you want to simply extend the existing editor call the base OnInspectorGUI () before doing any custom GUI code.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.CheckSetup();
            this.DetectShaderChanged();
            if (((this.isVisible && (this.m_Shader != null)) && !this.HasMultipleMixedShaderValues()) && this.PropertiesGUI())
            {
                this.PropertiesChanged();
            }
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_CustomShaderGUI != null)
            {
                this.m_CustomShaderGUI.OnMaterialInteractivePreviewGUI(this, r, background);
            }
            else
            {
                base.OnInteractivePreviewGUI(r, background);
            }
        }

        /// <summary>
        /// <para>Custom preview for Image component.</para>
        /// </summary>
        /// <param name="r">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_CustomShaderGUI != null)
            {
                this.m_CustomShaderGUI.OnMaterialPreviewGUI(this, r, background);
            }
            else
            {
                this.DefaultPreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.m_CustomShaderGUI != null)
            {
                this.m_CustomShaderGUI.OnMaterialPreviewSettingsGUI(this);
            }
            else
            {
                this.DefaultPreviewSettingsGUI();
            }
        }

        internal void OnSceneDrag(SceneView sceneView)
        {
            Event current = Event.current;
            if (current.type != EventType.Repaint)
            {
                int materialIndex = -1;
                GameObject go = HandleUtility.PickGameObject(current.mousePosition, out materialIndex);
                if (EditorMaterialUtility.IsBackgroundMaterial(base.target as Material))
                {
                    this.HandleSkybox(go, current);
                }
                else if ((go != null) && (go.GetComponent<Renderer>() != null))
                {
                    this.HandleRenderer(go.GetComponent<Renderer>(), materialIndex, current);
                }
            }
        }

        internal void OnSelectedShaderPopup(string command, Shader shader)
        {
            base.serializedObject.Update();
            if (shader != null)
            {
                this.SetShader(shader);
            }
            this.PropertiesChanged();
        }

        public static Renderer PrepareMaterialPropertiesForAnimationMode(MaterialProperty[] properties, bool isMaterialEditable)
        {
            bool flag = AnimationMode.InAnimationMode();
            Renderer associatedRenderFromInspector = GetAssociatedRenderFromInspector();
            if (associatedRenderFromInspector != null)
            {
                ForwardApplyMaterialModification modification = new ForwardApplyMaterialModification(associatedRenderFromInspector, isMaterialEditable);
                MaterialPropertyBlock dest = new MaterialPropertyBlock();
                associatedRenderFromInspector.GetPropertyBlock(dest);
                foreach (MaterialProperty property in properties)
                {
                    property.ReadFromMaterialPropertyBlock(dest);
                    if (flag)
                    {
                        property.applyPropertyCallback = new MaterialProperty.ApplyPropertyCallback(modification.DidModifyAnimationModeMaterialProperty);
                    }
                }
            }
            if (flag)
            {
                return associatedRenderFromInspector;
            }
            return null;
        }

        private bool PreviewSettingsMenuButton(out Rect buttonRect)
        {
            buttonRect = GUILayoutUtility.GetRect(14f, 24f, (float) 14f, (float) 20f);
            Rect position = new Rect(buttonRect.x + ((buttonRect.width - 16f) / 2f), buttonRect.y + ((buttonRect.height - 6f) / 2f), 16f, 6f);
            if (Event.current.type == EventType.Repaint)
            {
                Styles.kReflectionProbePickerStyle.Draw(position, false, false, false, false);
            }
            return EditorGUI.ButtonMouseDown(buttonRect, GUIContent.none, FocusType.Passive, GUIStyle.none);
        }

        /// <summary>
        /// <para>Whenever a material property is changed call this function. This will rebuild the inspector and validate the properties.</para>
        /// </summary>
        public void PropertiesChanged()
        {
            this.m_InfoMessage = null;
            if (base.targets.Length == 1)
            {
                this.m_InfoMessage = PerformanceChecks.CheckMaterial(base.target as Material, EditorUserBuildSettings.activeBuildTarget);
            }
        }

        /// <summary>
        /// <para>Default rendering of shader properties.</para>
        /// </summary>
        /// <param name="props">Array of material properties.</param>
        public void PropertiesDefaultGUI(MaterialProperty[] props)
        {
            this.SetDefaultGUIWidths();
            if (this.m_InfoMessage != null)
            {
                EditorGUILayout.HelpBox(this.m_InfoMessage, MessageType.Info);
            }
            else
            {
                GUIUtility.GetControlID(s_ControlHash, FocusType.Passive, new Rect(0f, 0f, 0f, 0f));
            }
            for (int i = 0; i < props.Length; i++)
            {
                if ((props[i].flags & (MaterialProperty.PropFlags.PerRendererData | MaterialProperty.PropFlags.HideInInspector)) == MaterialProperty.PropFlags.None)
                {
                    float propertyHeight = this.GetPropertyHeight(props[i], props[i].displayName);
                    Rect position = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField, new GUILayoutOption[0]);
                    this.ShaderProperty(position, props[i], props[i].displayName);
                }
            }
            this.RenderQueueField();
        }

        /// <summary>
        /// <para>Render the standard material properties. This method will either render properties using a IShaderGUI instance if found otherwise it uses PropertiesDefaultGUI.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns true if any value was changed.</para>
        /// </returns>
        public bool PropertiesGUI()
        {
            if (this.m_InsidePropertiesGUI)
            {
                Debug.LogWarning("PropertiesGUI() is being called recursively. If you want to render the default gui for shader properties then call PropertiesDefaultGUI() instead");
                return false;
            }
            EditorGUI.BeginChangeCheck();
            MaterialProperty[] materialProperties = GetMaterialProperties(base.targets);
            this.m_RendererForAnimationMode = PrepareMaterialPropertiesForAnimationMode(materialProperties, GUI.enabled);
            bool enabled = GUI.enabled;
            if (this.m_RendererForAnimationMode != null)
            {
                GUI.enabled = true;
            }
            this.m_InsidePropertiesGUI = true;
            try
            {
                if (this.m_CustomShaderGUI != null)
                {
                    this.m_CustomShaderGUI.OnGUI(this, materialProperties);
                }
                else
                {
                    this.PropertiesDefaultGUI(materialProperties);
                }
                Renderer associatedRenderFromInspector = GetAssociatedRenderFromInspector();
                if (associatedRenderFromInspector != null)
                {
                    if (Event.current.type == EventType.Layout)
                    {
                        associatedRenderFromInspector.GetPropertyBlock(this.m_PropertyBlock);
                    }
                    if ((this.m_PropertyBlock != null) && !this.m_PropertyBlock.isEmpty)
                    {
                        EditorGUILayout.HelpBox(Styles.propBlockWarning, MessageType.Warning);
                    }
                }
            }
            catch (Exception)
            {
                GUI.enabled = enabled;
                this.m_InsidePropertiesGUI = false;
                this.m_RendererForAnimationMode = null;
                throw;
            }
            GUI.enabled = enabled;
            this.m_InsidePropertiesGUI = false;
            this.m_RendererForAnimationMode = null;
            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// <para>Draw a range slider for a range shader property.</para>
        /// </summary>
        /// <param name="label">Label for the property.</param>
        /// <param name="prop">The property to edit.</param>
        /// <param name="position">Position and size of the range slider control.</param>
        public float RangeProperty(MaterialProperty prop, string label)
        {
            return this.RangePropertyInternal(prop, new GUIContent(label));
        }

        /// <summary>
        /// <para>Draw a range slider for a range shader property.</para>
        /// </summary>
        /// <param name="label">Label for the property.</param>
        /// <param name="prop">The property to edit.</param>
        /// <param name="position">Position and size of the range slider control.</param>
        public float RangeProperty(Rect position, MaterialProperty prop, string label)
        {
            return this.RangePropertyInternal(position, prop, new GUIContent(label));
        }

        [Obsolete("Use RangeProperty with MaterialProperty instead.")]
        public float RangeProperty(string propertyName, string label, float v2, float v3)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyName);
            return this.RangeProperty(materialProperty, label);
        }

        internal float RangePropertyInternal(MaterialProperty prop, GUIContent label)
        {
            Rect position = this.GetPropertyRect(prop, label, true);
            return this.RangePropertyInternal(position, prop, label);
        }

        internal float RangePropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
        {
            float power = (prop.name != "_Shininess") ? 1f : 5f;
            return DoPowerRangeProperty(position, prop, label, power);
        }

        /// <summary>
        /// <para>Call this when you change a material property. It will add an undo for the action.</para>
        /// </summary>
        /// <param name="label">Undo Label.</param>
        public void RegisterPropertyChangeUndo(string label)
        {
            Undo.RecordObjects(base.targets, "Modify " + label + " of " + this.targetTitle);
        }

        /// <summary>
        /// <para>Display UI for editing material's render queue setting.</para>
        /// </summary>
        /// <param name="r"></param>
        public void RenderQueueField()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
            this.RenderQueueField(controlRectForSingleLine);
        }

        /// <summary>
        /// <para>Display UI for editing material's render queue setting.</para>
        /// </summary>
        /// <param name="r"></param>
        public void RenderQueueField(Rect r)
        {
            EditorGUI.showMixedValue = this.HasMultipleMixedQueueValues();
            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            this.SetDefaultGUIWidths();
            EditorGUIUtility.labelWidth -= 100f;
            Rect position = r;
            position.width -= EditorGUIUtility.fieldWidth + 2f;
            Rect rect2 = r;
            rect2.xMin = rect2.xMax - EditorGUIUtility.fieldWidth;
            Material mat = base.targets[0] as Material;
            int materialRawRenderQueue = ShaderUtil.GetMaterialRawRenderQueue(mat);
            int renderQueue = mat.renderQueue;
            int selectedValue = (Array.IndexOf<int>(Styles.queueValues, materialRawRenderQueue) >= 0) ? materialRawRenderQueue : Styles.kCustomQueueValue;
            int num6 = EditorGUI.IntPopup(position, Styles.queueLabel, selectedValue, Styles.queueNames, Styles.queueValues);
            int num7 = EditorGUI.DelayedIntField(rect2, renderQueue);
            if ((selectedValue != num6) || (renderQueue != num7))
            {
                this.RegisterPropertyChangeUndo("Render Queue");
                int num8 = num7;
                if ((num6 != selectedValue) && (num6 != Styles.kCustomQueueValue))
                {
                    num8 = num6;
                }
                num8 = Mathf.Clamp(num8, -1, 0x1388);
                foreach (Object obj2 in base.targets)
                {
                    ((Material) obj2).renderQueue = num8;
                }
            }
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
            EditorGUI.showMixedValue = false;
        }

        public sealed override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            this.Init();
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            this.DoRenderPreview();
            return this.m_PreviewUtility.EndStaticPreview();
        }

        /// <summary>
        /// <para>Does this edit require to be repainted constantly in its current state?</para>
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return (this.m_TimeUpdate == 1);
        }

        [Obsolete("Use MaterialProperty instead.")]
        public void SetColor(string propertyName, Color value)
        {
            foreach (Material material in base.targets)
            {
                material.SetColor(propertyName, value);
            }
        }

        /// <summary>
        /// <para>Set EditorGUIUtility.fieldWidth and labelWidth to the default values that PropertiesGUI uses.</para>
        /// </summary>
        public void SetDefaultGUIWidths()
        {
            EditorGUIUtility.fieldWidth = 64f;
            EditorGUIUtility.labelWidth = (GUIClip.visibleRect.width - EditorGUIUtility.fieldWidth) - 17f;
        }

        [Obsolete("Use MaterialProperty instead.")]
        public void SetFloat(string propertyName, float value)
        {
            foreach (Material material in base.targets)
            {
                material.SetFloat(propertyName, value);
            }
        }

        /// <summary>
        /// <para>Set the shader of the material.</para>
        /// </summary>
        /// <param name="shader">Shader to set.</param>
        /// <param name="registerUndo">Should undo be registered.</param>
        /// <param name="newShader"></param>
        public void SetShader(Shader shader)
        {
            this.SetShader(shader, true);
        }

        /// <summary>
        /// <para>Set the shader of the material.</para>
        /// </summary>
        /// <param name="shader">Shader to set.</param>
        /// <param name="registerUndo">Should undo be registered.</param>
        /// <param name="newShader"></param>
        public void SetShader(Shader newShader, bool registerUndo)
        {
            bool flag = false;
            ShaderGUI customShaderGUI = this.m_CustomShaderGUI;
            string oldEditorName = (this.m_Shader == null) ? string.Empty : this.m_Shader.customEditor;
            this.CreateCustomShaderGUI(newShader, oldEditorName);
            this.m_Shader = newShader;
            if (customShaderGUI != this.m_CustomShaderGUI)
            {
                flag = true;
            }
            foreach (Material material in base.targets)
            {
                Shader oldShader = material.shader;
                Undo.RecordObject(material, "Assign shader");
                if (this.m_CustomShaderGUI != null)
                {
                    this.m_CustomShaderGUI.AssignNewShaderToMaterial(material, oldShader, newShader);
                }
                else
                {
                    material.shader = newShader;
                }
                EditorMaterialUtility.ResetDefaultTextures(material, false);
                ApplyMaterialPropertyDrawers(material);
            }
            if (flag && (ActiveEditorTracker.sharedTracker != null))
            {
                foreach (InspectorWindow window in InspectorWindow.GetAllInspectorWindows())
                {
                    window.tracker.ForceRebuild();
                }
            }
        }

        [Obsolete("Use MaterialProperty instead.")]
        public void SetTexture(string propertyName, Texture value)
        {
            foreach (Material material in base.targets)
            {
                material.SetTexture(propertyName, value);
            }
        }

        /// <summary>
        /// <para>Set the offset of a given texture property.</para>
        /// </summary>
        /// <param name="propertyName">Name of the texture property that you wish to modify the offset of.</param>
        /// <param name="value">Scale to set.</param>
        /// <param name="coord">Set the x or y component of the offset (0 for x, 1 for y).</param>
        [Obsolete("Use MaterialProperty instead.")]
        public void SetTextureOffset(string propertyName, Vector2 value, int coord)
        {
            foreach (Material material in base.targets)
            {
                Vector2 textureOffset = material.GetTextureOffset(propertyName);
                textureOffset[coord] = value[coord];
                material.SetTextureOffset(propertyName, textureOffset);
            }
        }

        /// <summary>
        /// <para>Set the scale of a given texture property.</para>
        /// </summary>
        /// <param name="propertyName">Name of the texture property that you wish to modify the scale of.</param>
        /// <param name="value">Scale to set.</param>
        /// <param name="coord">Set the x or y component of the scale (0 for x, 1 for y).</param>
        [Obsolete("Use MaterialProperty instead.")]
        public void SetTextureScale(string propertyName, Vector2 value, int coord)
        {
            foreach (Material material in base.targets)
            {
                Vector2 textureScale = material.GetTextureScale(propertyName);
                textureScale[coord] = value[coord];
                material.SetTextureScale(propertyName, textureScale);
            }
        }

        [Obsolete("Use MaterialProperty instead.")]
        public void SetVector(string propertyName, Vector4 value)
        {
            foreach (Material material in base.targets)
            {
                material.SetVector(propertyName, value);
            }
        }

        private void ShaderPopup(GUIStyle style)
        {
            bool enabled = GUI.enabled;
            Rect position = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(new GUILayoutOption[0]), 0xb919, EditorGUIUtility.TempContent("Shader"));
            EditorGUI.showMixedValue = this.HasMultipleMixedShaderValues();
            GUIContent content = EditorGUIUtility.TempContent((this.m_Shader == null) ? "No Shader Selected" : this.m_Shader.name);
            if (EditorGUI.ButtonMouseDown(position, content, FocusType.Keyboard, style))
            {
                EditorGUI.showMixedValue = false;
                Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
                InternalEditorUtility.SetupShaderMenu(base.target as Material);
                EditorUtility.Internal_DisplayPopupMenu(new Rect(vector.x, vector.y, position.width, position.height), "CONTEXT/ShaderPopup", this, 0);
                Event.current.Use();
            }
            EditorGUI.showMixedValue = false;
            GUI.enabled = enabled;
        }

        /// <summary>
        /// <para>Handes UI for one shader property.</para>
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        /// <param name="position"></param>
        public void ShaderProperty(MaterialProperty prop, string label)
        {
            this.ShaderProperty(prop, new GUIContent(label));
        }

        public void ShaderProperty(MaterialProperty prop, GUIContent label)
        {
            this.ShaderProperty(prop, label, 0);
        }

        [Obsolete("Use ShaderProperty that takes MaterialProperty parameter instead.")]
        public void ShaderProperty(Shader shader, int propertyIndex)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyIndex);
            this.ShaderProperty(materialProperty, materialProperty.displayName);
        }

        public void ShaderProperty(MaterialProperty prop, string label, int labelIndent)
        {
            this.ShaderProperty(prop, new GUIContent(label), labelIndent);
        }

        public void ShaderProperty(MaterialProperty prop, GUIContent label, int labelIndent)
        {
            Rect position = this.GetPropertyRect(prop, label, false);
            this.ShaderProperty(position, prop, label, labelIndent);
        }

        /// <summary>
        /// <para>Handes UI for one shader property.</para>
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="label"></param>
        /// <param name="position"></param>
        public void ShaderProperty(Rect position, MaterialProperty prop, string label)
        {
            this.ShaderProperty(position, prop, new GUIContent(label));
        }

        public void ShaderProperty(Rect position, MaterialProperty prop, GUIContent label)
        {
            this.ShaderProperty(position, prop, label, 0);
        }

        public void ShaderProperty(Rect position, MaterialProperty prop, string label, int labelIndent)
        {
            this.ShaderProperty(position, prop, new GUIContent(label), labelIndent);
        }

        public void ShaderProperty(Rect position, MaterialProperty prop, GUIContent label, int labelIndent)
        {
            this.BeginAnimatedCheck(prop);
            EditorGUI.indentLevel += labelIndent;
            this.ShaderPropertyInternal(position, prop, label);
            EditorGUI.indentLevel -= labelIndent;
            this.EndAnimatedCheck();
        }

        private void ShaderPropertyInternal(Rect position, MaterialProperty prop, GUIContent label)
        {
            MaterialPropertyHandler handler = MaterialPropertyHandler.GetHandler(((Material) base.target).shader, prop.name);
            if (handler != null)
            {
                handler.OnGUI(ref position, prop, (label.text == null) ? new GUIContent(prop.displayName) : label, this);
                if (handler.propertyDrawer != null)
                {
                    return;
                }
            }
            this.DefaultShaderPropertyInternal(position, prop, label);
        }

        /// <summary>
        /// <para>Checks if particular property has incorrect type of texture specified by the material, displays appropriate warning and suggests the user to automatically fix the problem.</para>
        /// </summary>
        /// <param name="prop">The texture property to check and display warning for, if necessary.</param>
        public void TextureCompatibilityWarning(MaterialProperty prop)
        {
            if (InternalEditorUtility.BumpMapTextureNeedsFixing(prop) && this.HelpBoxWithButton(EditorGUIUtility.TextContent("This texture is not marked as a normal map"), EditorGUIUtility.TextContent("Fix Now")))
            {
                InternalEditorUtility.FixNormalmapTexture(prop);
            }
        }

        /// <summary>
        /// <para>Draw a property field for a texture shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="scaleOffset">Draw scale / offset.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        /// <param name="tooltip"></param>
        public Texture TextureProperty(MaterialProperty prop, string label)
        {
            bool scaleOffset = (prop.flags & MaterialProperty.PropFlags.NoScaleOffset) == MaterialProperty.PropFlags.None;
            return this.TextureProperty(prop, label, scaleOffset);
        }

        [Obsolete("Use TextureProperty with MaterialProperty instead.")]
        public Texture TextureProperty(string propertyName, string label, ShaderUtil.ShaderPropertyTexDim texDim)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyName);
            return this.TextureProperty(materialProperty, label);
        }

        /// <summary>
        /// <para>Draw a property field for a texture shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="scaleOffset">Draw scale / offset.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        /// <param name="tooltip"></param>
        public Texture TextureProperty(MaterialProperty prop, string label, bool scaleOffset)
        {
            Rect position = this.GetPropertyRect(prop, label, true);
            return this.TextureProperty(position, prop, label, scaleOffset);
        }

        /// <summary>
        /// <para>Draw a property field for a texture shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="scaleOffset">Draw scale / offset.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        /// <param name="tooltip"></param>
        public Texture TextureProperty(Rect position, MaterialProperty prop, string label)
        {
            bool scaleOffset = (prop.flags & MaterialProperty.PropFlags.NoScaleOffset) == MaterialProperty.PropFlags.None;
            return this.TextureProperty(position, prop, label, scaleOffset);
        }

        [Obsolete("Use TextureProperty with MaterialProperty instead.")]
        public Texture TextureProperty(string propertyName, string label, ShaderUtil.ShaderPropertyTexDim texDim, bool scaleOffset)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyName);
            return this.TextureProperty(materialProperty, label, scaleOffset);
        }

        /// <summary>
        /// <para>Draw a property field for a texture shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="scaleOffset">Draw scale / offset.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        /// <param name="tooltip"></param>
        public Texture TextureProperty(Rect position, MaterialProperty prop, string label, bool scaleOffset)
        {
            return this.TextureProperty(position, prop, label, string.Empty, scaleOffset);
        }

        /// <summary>
        /// <para>Draw a property field for a texture shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="scaleOffset">Draw scale / offset.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        /// <param name="tooltip"></param>
        public Texture TextureProperty(Rect position, MaterialProperty prop, string label, string tooltip, bool scaleOffset)
        {
            EditorGUI.PrefixLabel(position, new GUIContent(label, tooltip));
            position.height = GetTextureFieldHeight();
            Rect rect = position;
            rect.xMin = rect.xMax - EditorGUIUtility.fieldWidth;
            Texture texture = this.TexturePropertyBody(rect, prop);
            if (scaleOffset)
            {
                this.TextureScaleOffsetProperty(this.GetTexturePropertyCustomArea(position), prop);
            }
            GUILayout.Space(-6f);
            this.TextureCompatibilityWarning(prop);
            GUILayout.Space(6f);
            return texture;
        }

        private Texture TexturePropertyBody(Rect position, MaterialProperty prop)
        {
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                throw new ArgumentException(string.Format("The MaterialProperty '{0}' should be of type 'Texture' (its type is '{1})'", prop.name, prop.type));
            }
            this.m_DesiredTexdim = prop.textureDimension;
            Type textureTypeFromDimension = GetTextureTypeFromDimension(this.m_DesiredTexdim);
            bool enabled = GUI.enabled;
            EditorGUI.BeginChangeCheck();
            if ((prop.flags & MaterialProperty.PropFlags.PerRendererData) != MaterialProperty.PropFlags.None)
            {
                GUI.enabled = false;
            }
            EditorGUI.showMixedValue = prop.hasMixedValue;
            int id = GUIUtility.GetControlID(0x3042, FocusType.Keyboard, position);
            Texture texture = EditorGUI.DoObjectField(position, position, id, prop.textureValue, textureTypeFromDimension, null, new EditorGUI.ObjectFieldValidator(this.TextureValidator), false) as Texture;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                prop.textureValue = texture;
            }
            GUI.enabled = enabled;
            return prop.textureValue;
        }

        /// <summary>
        /// <para>Draw a property field for a texture shader property that only takes up a single line height.</para>
        /// </summary>
        /// <param name="position">Rect that this control should be rendered in.</param>
        /// <param name="label">Label for the field.</param>
        /// <param name="prop"></param>
        /// <param name="tooltip"></param>
        /// <returns>
        /// <para>Returns total height used by this control.</para>
        /// </returns>
        public Texture TexturePropertyMiniThumbnail(Rect position, MaterialProperty prop, string label, string tooltip)
        {
            Rect rect;
            Rect rect2;
            this.BeginAnimatedCheck(prop);
            EditorGUI.GetRectsForMiniThumbnailField(position, out rect, out rect2);
            EditorGUI.HandlePrefixLabel(position, rect2, new GUIContent(label, tooltip), 0, EditorStyles.label);
            this.EndAnimatedCheck();
            Texture texture = this.TexturePropertyBody(rect, prop);
            Rect rect3 = position;
            rect3.y += position.height;
            rect3.height = 27f;
            this.TextureCompatibilityWarning(prop);
            return texture;
        }

        /// <summary>
        /// <para>Method for showing a texture property control with additional inlined properites.</para>
        /// </summary>
        /// <param name="label">The label used for the texture property.</param>
        /// <param name="textureProp">The texture property.</param>
        /// <param name="extraProperty1">First optional property inlined after the texture property.</param>
        /// <param name="extraProperty2">Second optional property inlined after the extraProperty1.</param>
        /// <returns>
        /// <para>Returns the Rect used.</para>
        /// </returns>
        public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp)
        {
            return this.TexturePropertySingleLine(label, textureProp, null, null);
        }

        /// <summary>
        /// <para>Method for showing a texture property control with additional inlined properites.</para>
        /// </summary>
        /// <param name="label">The label used for the texture property.</param>
        /// <param name="textureProp">The texture property.</param>
        /// <param name="extraProperty1">First optional property inlined after the texture property.</param>
        /// <param name="extraProperty2">Second optional property inlined after the extraProperty1.</param>
        /// <returns>
        /// <para>Returns the Rect used.</para>
        /// </returns>
        public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1)
        {
            return this.TexturePropertySingleLine(label, textureProp, extraProperty1, null);
        }

        /// <summary>
        /// <para>Method for showing a texture property control with additional inlined properites.</para>
        /// </summary>
        /// <param name="label">The label used for the texture property.</param>
        /// <param name="textureProp">The texture property.</param>
        /// <param name="extraProperty1">First optional property inlined after the texture property.</param>
        /// <param name="extraProperty2">Second optional property inlined after the extraProperty1.</param>
        /// <returns>
        /// <para>Returns the Rect used.</para>
        /// </returns>
        public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1, MaterialProperty extraProperty2)
        {
            Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
            this.TexturePropertyMiniThumbnail(controlRectForSingleLine, textureProp, label.text, label.tooltip);
            if ((extraProperty1 != null) || (extraProperty2 != null))
            {
                if ((extraProperty1 == null) || (extraProperty2 == null))
                {
                    MaterialProperty property;
                    if (extraProperty1 != null)
                    {
                        property = extraProperty1;
                    }
                    else
                    {
                        property = extraProperty2;
                    }
                    if (property.type == MaterialProperty.PropType.Color)
                    {
                        this.ExtraPropertyAfterTexture(GetLeftAlignedFieldRect(controlRectForSingleLine), property);
                    }
                    else
                    {
                        this.ExtraPropertyAfterTexture(GetRectAfterLabelWidth(controlRectForSingleLine), property);
                    }
                    return controlRectForSingleLine;
                }
                if (extraProperty1.type == MaterialProperty.PropType.Color)
                {
                    this.ExtraPropertyAfterTexture(GetFlexibleRectBetweenFieldAndRightEdge(controlRectForSingleLine), extraProperty2);
                    this.ExtraPropertyAfterTexture(GetLeftAlignedFieldRect(controlRectForSingleLine), extraProperty1);
                }
                else
                {
                    this.ExtraPropertyAfterTexture(GetRightAlignedFieldRect(controlRectForSingleLine), extraProperty2);
                    this.ExtraPropertyAfterTexture(GetFlexibleRectBetweenLabelAndField(controlRectForSingleLine), extraProperty1);
                }
            }
            return controlRectForSingleLine;
        }

        /// <summary>
        /// <para>Method for showing a compact layout of properties.</para>
        /// </summary>
        /// <param name="label">The label used for the texture property.</param>
        /// <param name="textureProp">The texture property.</param>
        /// <param name="extraProperty1">First extra property inlined after the texture property.</param>
        /// <param name="label2">Label for the second extra property (on a new line and indented).</param>
        /// <param name="extraProperty2">Second property on a new line below the texture.</param>
        /// <returns>
        /// <para>Returns the Rect used.</para>
        /// </returns>
        public Rect TexturePropertyTwoLines(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1, GUIContent label2, MaterialProperty extraProperty2)
        {
            if (extraProperty2 == null)
            {
                return this.TexturePropertySingleLine(label, textureProp, extraProperty1);
            }
            Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
            this.TexturePropertyMiniThumbnail(controlRectForSingleLine, textureProp, label.text, label.tooltip);
            Rect rectAfterLabelWidth = GetRectAfterLabelWidth(controlRectForSingleLine);
            if (extraProperty1.type == MaterialProperty.PropType.Color)
            {
                rectAfterLabelWidth = GetLeftAlignedFieldRect(controlRectForSingleLine);
            }
            this.ExtraPropertyAfterTexture(rectAfterLabelWidth, extraProperty1);
            Rect position = this.GetControlRectForSingleLine();
            this.ShaderProperty(position, extraProperty2, label2.text, 3);
            controlRectForSingleLine.height += position.height;
            return controlRectForSingleLine;
        }

        /// <summary>
        /// <para>Method for showing a texture property control with a HDR color field and its color brightness float field.</para>
        /// </summary>
        /// <param name="label">The label used for the texture property.</param>
        /// <param name="textureProp">The texture property.</param>
        /// <param name="colorProperty">The color property (will be treated as a HDR color).</param>
        /// <param name="hdrConfig">The HDR color configuration used by the HDR Color Picker.</param>
        /// <param name="showAlpha">If false then the alpha channel information will be hidden in the GUI.</param>
        /// <returns>
        /// <para>Return the Rect used.</para>
        /// </returns>
        public Rect TexturePropertyWithHDRColor(GUIContent label, MaterialProperty textureProp, MaterialProperty colorProperty, ColorPickerHDRConfig hdrConfig, bool showAlpha)
        {
            ColorPickerHDRConfig defaultHDRConfig;
            Rect controlRectForSingleLine = this.GetControlRectForSingleLine();
            this.TexturePropertyMiniThumbnail(controlRectForSingleLine, textureProp, label.text, label.tooltip);
            if (colorProperty.type != MaterialProperty.PropType.Color)
            {
                Debug.LogError("Assuming MaterialProperty.PropType.Color (was " + colorProperty.type + ")");
                return controlRectForSingleLine;
            }
            this.BeginAnimatedCheck(colorProperty);
            if (hdrConfig != null)
            {
                defaultHDRConfig = hdrConfig;
            }
            else
            {
                defaultHDRConfig = ColorPicker.defaultHDRConfig;
            }
            Rect leftAlignedFieldRect = GetLeftAlignedFieldRect(controlRectForSingleLine);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = colorProperty.hasMixedValue;
            Color color = EditorGUI.ColorField(leftAlignedFieldRect, GUIContent.none, colorProperty.colorValue, true, showAlpha, true, defaultHDRConfig);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                colorProperty.colorValue = color;
            }
            Rect flexibleRectBetweenFieldAndRightEdge = GetFlexibleRectBetweenFieldAndRightEdge(controlRectForSingleLine);
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = flexibleRectBetweenFieldAndRightEdge.width - EditorGUIUtility.fieldWidth;
            EditorGUI.BeginChangeCheck();
            color = EditorGUI.ColorBrightnessField(flexibleRectBetweenFieldAndRightEdge, GUIContent.Temp(" "), colorProperty.colorValue, defaultHDRConfig.minBrightness, defaultHDRConfig.maxBrightness);
            if (EditorGUI.EndChangeCheck())
            {
                colorProperty.colorValue = color;
            }
            EditorGUIUtility.labelWidth = labelWidth;
            this.EndAnimatedCheck();
            return controlRectForSingleLine;
        }

        public void TextureScaleOffsetProperty(MaterialProperty property)
        {
            Rect position = EditorGUILayout.GetControlRect(true, 32f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
            this.TextureScaleOffsetProperty(position, property, false);
        }

        /// <summary>
        /// <para>Draws tiling and offset properties for a texture.</para>
        /// </summary>
        /// <param name="position">Rect to draw this control in.</param>
        /// <param name="property">Property to draw.</param>
        /// <param name="partOfTexturePropertyControl">If this control should be rendered under large texture property control use 'true'. If this control should be shown seperately use 'false'.</param>
        public float TextureScaleOffsetProperty(Rect position, MaterialProperty property)
        {
            return this.TextureScaleOffsetProperty(position, property, true);
        }

        /// <summary>
        /// <para>TODO.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scaleOffset"></param>
        /// <param name="partOfTexturePropertyControl"></param>
        public static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset)
        {
            return TextureScaleOffsetProperty(position, scaleOffset, 0, false);
        }

        /// <summary>
        /// <para>Draws tiling and offset properties for a texture.</para>
        /// </summary>
        /// <param name="position">Rect to draw this control in.</param>
        /// <param name="property">Property to draw.</param>
        /// <param name="partOfTexturePropertyControl">If this control should be rendered under large texture property control use 'true'. If this control should be shown seperately use 'false'.</param>
        public float TextureScaleOffsetProperty(Rect position, MaterialProperty property, bool partOfTexturePropertyControl)
        {
            this.BeginAnimatedCheck(property);
            EditorGUI.BeginChangeCheck();
            int mixedValueMask = property.mixedValueMask >> 1;
            Vector4 vector = TextureScaleOffsetProperty(position, property.textureScaleAndOffset, mixedValueMask, partOfTexturePropertyControl);
            if (EditorGUI.EndChangeCheck())
            {
                property.textureScaleAndOffset = vector;
            }
            this.EndAnimatedCheck();
            return 32f;
        }

        /// <summary>
        /// <para>TODO.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scaleOffset"></param>
        /// <param name="partOfTexturePropertyControl"></param>
        public static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset, bool partOfTexturePropertyControl)
        {
            return TextureScaleOffsetProperty(position, scaleOffset, 0, partOfTexturePropertyControl);
        }

        internal static Vector4 TextureScaleOffsetProperty(Rect position, Vector4 scaleOffset, int mixedValueMask, bool partOfTexturePropertyControl)
        {
            Vector2 vector = new Vector2(scaleOffset.x, scaleOffset.y);
            Vector2 vector2 = new Vector2(scaleOffset.z, scaleOffset.w);
            float labelWidth = EditorGUIUtility.labelWidth;
            float x = position.x + labelWidth;
            float num3 = position.x + EditorGUI.indent;
            if (partOfTexturePropertyControl)
            {
                labelWidth = 65f;
                x = position.x + labelWidth;
                num3 = position.x;
                position.y = position.yMax - 32f;
            }
            Rect totalPosition = new Rect(num3, position.y, labelWidth, 16f);
            Rect rect2 = new Rect(x, position.y, position.width - labelWidth, 16f);
            EditorGUI.PrefixLabel(totalPosition, s_TilingText);
            vector = EditorGUI.Vector2Field(rect2, GUIContent.none, vector);
            totalPosition.y += 16f;
            rect2.y += 16f;
            EditorGUI.PrefixLabel(totalPosition, s_OffsetText);
            vector2 = EditorGUI.Vector2Field(rect2, GUIContent.none, vector2);
            return new Vector4(vector.x, vector.y, vector2.x, vector2.y);
        }

        private Object TextureValidator(Object[] references, Type objType, SerializedProperty property)
        {
            foreach (Object obj2 in references)
            {
                Texture texture = obj2 as Texture;
                if ((texture != null) && ((texture.dimension == this.m_DesiredTexdim) || (this.m_DesiredTexdim == TextureDimension.Any)))
                {
                    return texture;
                }
            }
            return null;
        }

        public virtual void UndoRedoPerformed()
        {
            if (ActiveEditorTracker.sharedTracker != null)
            {
                ActiveEditorTracker.sharedTracker.ForceRebuild();
            }
            this.PropertiesChanged();
        }

        [Obsolete("Use VectorProperty with MaterialProperty instead.")]
        public Vector4 VectorProperty(string propertyName, string label)
        {
            MaterialProperty materialProperty = GetMaterialProperty(base.targets, propertyName);
            return this.VectorProperty(materialProperty, label);
        }

        /// <summary>
        /// <para>Draw a property field for a vector shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        public Vector4 VectorProperty(MaterialProperty prop, string label)
        {
            Rect position = this.GetPropertyRect(prop, label, true);
            return this.VectorProperty(position, prop, label);
        }

        /// <summary>
        /// <para>Draw a property field for a vector shader property.</para>
        /// </summary>
        /// <param name="label">Label for the field.</param>
        /// <param name="prop"></param>
        /// <param name="position"></param>
        public Vector4 VectorProperty(Rect position, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0f;
            Vector4 vector = EditorGUI.Vector4Field(position, label, prop.vectorValue);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = vector;
            }
            return prop.vectorValue;
        }

        internal bool forceVisible { get; set; }

        /// <summary>
        /// <para>Is the current material expanded.</para>
        /// </summary>
        public bool isVisible
        {
            get
            {
                return (this.forceVisible || this.m_IsVisible);
            }
        }

        private class ForwardApplyMaterialModification
        {
            private bool isMaterialEditable;
            private readonly Renderer renderer;

            public ForwardApplyMaterialModification(Renderer r, bool inIsMaterialEditable)
            {
                this.renderer = r;
                this.isMaterialEditable = inIsMaterialEditable;
            }

            public bool DidModifyAnimationModeMaterialProperty(MaterialProperty property, int changedMask, object previousValue)
            {
                return (MaterialAnimationUtility.ApplyMaterialModificationToAnimationRecording(property, changedMask, this.renderer, previousValue) || !this.isMaterialEditable);
            }
        }

        private enum PreviewType
        {
            Mesh,
            Plane,
            Skybox
        }

        internal class ReflectionProbePicker : PopupWindowContent
        {
            private ReflectionProbe m_SelectedReflectionProbe;

            public override Vector2 GetWindowSize()
            {
                return new Vector2(170f, 48f);
            }

            public void OnDisable()
            {
                SessionState.SetInt("PreviewReflectionProbe", (this.m_SelectedReflectionProbe == null) ? 0 : this.m_SelectedReflectionProbe.GetInstanceID());
            }

            public void OnEnable()
            {
                this.m_SelectedReflectionProbe = EditorUtility.InstanceIDToObject(SessionState.GetInt("PreviewReflectionProbe", 0)) as ReflectionProbe;
            }

            public override void OnGUI(Rect rc)
            {
                EditorGUILayout.LabelField("Select Reflection Probe", EditorStyles.boldLabel, new GUILayoutOption[0]);
                EditorGUILayout.Space();
                this.m_SelectedReflectionProbe = EditorGUILayout.ObjectField("", this.m_SelectedReflectionProbe, typeof(ReflectionProbe), true, new GUILayoutOption[0]) as ReflectionProbe;
            }

            public Transform Target
            {
                get
                {
                    return ((this.m_SelectedReflectionProbe == null) ? null : this.m_SelectedReflectionProbe.transform);
                }
            }
        }

        private static class Styles
        {
            public static int kCustomQueueValue;
            public static readonly GUIStyle kReflectionProbePickerStyle = "PaneOptions";
            public static string lightmapEmissiveLabel;
            public static string[] lightmapEmissiveStrings = new string[] { "None", "Realtime", "Baked" };
            public static int[] lightmapEmissiveValues;
            public static string propBlockWarning;
            public static readonly GUIContent queueLabel;
            public static readonly GUIContent[] queueNames;
            public static readonly int[] queueValues;

            static Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 2;
                lightmapEmissiveValues = numArray1;
                lightmapEmissiveLabel = "Global Illumination";
                propBlockWarning = EditorGUIUtility.TextContent("MaterialPropertyBlock is used to modify these values").text;
                kCustomQueueValue = -2;
                queueLabel = EditorGUIUtility.TextContent("Render Queue");
                queueNames = new GUIContent[] { EditorGUIUtility.TextContent("From Shader"), EditorGUIUtility.TextContent("Geometry|Queue 2000"), EditorGUIUtility.TextContent("AlphaTest|Queue 2450"), EditorGUIUtility.TextContent("Transparent|Queue 3000"), EditorGUIUtility.TextContent("Custom") };
                int[] numArray2 = new int[] { -1, 0x7d0, 0x992, 0xbb8, 0 };
                numArray2[4] = kCustomQueueValue;
                queueValues = numArray2;
            }
        }
    }
}


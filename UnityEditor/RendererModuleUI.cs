namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class RendererModuleUI : ModuleUI
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;
        private const int k_MaxNumMeshes = 4;
        private SerializedProperty m_CameraVelocityScale;
        private SerializedProperty m_CastShadows;
        private SerializedProperty m_LengthScale;
        private SerializedProperty m_Material;
        private SerializedProperty m_MaxParticleSize;
        private SerializedProperty[] m_Meshes;
        private SerializedProperty m_MinParticleSize;
        private SerializedProperty m_NormalDirection;
        private SerializedProperty m_Pivot;
        private RendererEditorBase.Probes m_Probes;
        private SerializedProperty m_ReceiveShadows;
        private SerializedProperty m_RenderAlignment;
        private SerializedProperty m_RenderMode;
        private SerializedProperty[] m_ShownMeshes;
        private SerializedProperty m_SortingFudge;
        private SerializedProperty m_SortingLayerID;
        private SerializedProperty m_SortingOrder;
        private SerializedProperty m_SortMode;
        private SerializedProperty m_TrailMaterial;
        private SerializedProperty m_UseCustomVertexStreams;
        private SerializedProperty m_VelocityScale;
        private SerializedProperty m_VertexStreamMask;
        private static Texts s_Texts;
        private static bool s_VisualizePivot = false;

        public RendererModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ParticleSystemRenderer", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
        {
            this.m_Meshes = new SerializedProperty[4];
            base.m_ToolTip = "Specifies how the particles are rendered.";
        }

        private void DoListOfMeshesGUI()
        {
            base.GUIListOfFloatObjectToggleFields(s_Texts.mesh, this.m_ShownMeshes, null, null, false, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect((float) 0f, (float) 13f);
            position.x = (position.xMax - 24f) - 5f;
            position.width = 12f;
            if ((this.m_ShownMeshes.Length > 1) && ModuleUI.MinusButton(position))
            {
                this.m_ShownMeshes[this.m_ShownMeshes.Length - 1].objectReferenceValue = null;
                List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownMeshes);
                list.RemoveAt(list.Count - 1);
                this.m_ShownMeshes = list.ToArray();
            }
            if (this.m_ShownMeshes.Length < 4)
            {
                position.x += 17f;
                if (ModuleUI.PlusButton(position))
                {
                    List<SerializedProperty> list2 = new List<SerializedProperty>(this.m_ShownMeshes);
                    list2.Add(this.m_Meshes[list2.Count]);
                    this.m_ShownMeshes = list2.ToArray();
                }
            }
        }

        private void DoVertexStreamsGUI(RenderMode renderMode)
        {
            Rect controlRect = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
            GUI.Label(controlRect, s_Texts.streams, ParticleSystemStyles.Get().label);
            int num = 0;
            for (int i = 0; i < s_Texts.vertexStreams.Length; i++)
            {
                if ((this.m_VertexStreamMask.intValue & (((int) 1) << i)) != 0)
                {
                    string str = !(base.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner is ParticleSystemInspector) ? "TEX" : "TEXCOORD";
                    Rect position = new Rect(controlRect.x + EditorGUIUtility.labelWidth, controlRect.y, controlRect.width, controlRect.height);
                    if (s_Texts.vertexStreamIsTexCoord[i])
                    {
                        GUI.Label(position, string.Concat(new object[] { s_Texts.vertexStreams[i], " (", str, num++, ", ", s_Texts.vertexStreamDataTypes[i], ")" }), ParticleSystemStyles.Get().label);
                    }
                    else
                    {
                        GUI.Label(position, s_Texts.vertexStreams[i] + " (" + s_Texts.vertexStreamDataTypes[i] + ")", ParticleSystemStyles.Get().label);
                    }
                    position.x = controlRect.xMax - 12f;
                    controlRect = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
                    if (i == 0)
                    {
                        if (this.m_VertexStreamMask.intValue != ((((int) 1) << s_Texts.vertexStreams.Length) - 1))
                        {
                            position.x -= 2f;
                            position.y -= 2f;
                            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, "OL Plus"))
                            {
                                List<GUIContent> list = new List<GUIContent>();
                                for (int j = 0; j < s_Texts.vertexStreams.Length; j++)
                                {
                                    if ((this.m_VertexStreamMask.intValue & (((int) 1) << j)) == 0)
                                    {
                                        list.Add(new GUIContent(s_Texts.vertexStreams[j]));
                                    }
                                }
                                GenericMenu menu = new GenericMenu();
                                for (int k = 0; k < list.Count; k++)
                                {
                                    if (<>f__mg$cache0 == null)
                                    {
                                        <>f__mg$cache0 = new GenericMenu.MenuFunction2(RendererModuleUI.SelectVertexStreamCallback);
                                    }
                                    menu.AddItem(list[k], false, <>f__mg$cache0, new StreamCallbackData(this.m_VertexStreamMask, list[k].text));
                                }
                                menu.ShowAsContext();
                                Event.current.Use();
                            }
                        }
                    }
                    else if (ModuleUI.MinusButton(position))
                    {
                        this.m_VertexStreamMask.intValue &= ~(((int) 1) << i);
                    }
                }
            }
            string textAndTooltip = "";
            if (this.m_Material != null)
            {
                Material objectReferenceValue = this.m_Material.objectReferenceValue as Material;
                ParticleSystemVertexStreams streams = base.m_ParticleSystemUI.m_ParticleSystem.CheckVertexStreamsMatchShader((ParticleSystemVertexStreams) this.m_VertexStreamMask.intValue, objectReferenceValue);
                if (streams != ParticleSystemVertexStreams.None)
                {
                    textAndTooltip = textAndTooltip + "Vertex streams do not match the shader inputs. Particle systems may not render correctly. Ensure your streams match and are used by the shader.";
                    if ((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None)
                    {
                        textAndTooltip = textAndTooltip + "\n- TANGENT stream does not match.";
                    }
                    if ((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None)
                    {
                        textAndTooltip = textAndTooltip + "\n- COLOR stream does not match.";
                    }
                    if ((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None)
                    {
                        textAndTooltip = textAndTooltip + "\n- TEXCOORD streams do not match.";
                    }
                }
            }
            int maxTexCoordStreams = base.m_ParticleSystemUI.m_ParticleSystem.GetMaxTexCoordStreams();
            if (num > maxTexCoordStreams)
            {
                if (textAndTooltip != "")
                {
                    textAndTooltip = textAndTooltip + "\n\n";
                }
                string str3 = textAndTooltip;
                object[] objArray2 = new object[] { str3, "Only ", maxTexCoordStreams, " TEXCOORD streams are supported." };
                textAndTooltip = string.Concat(objArray2);
            }
            if (renderMode == RenderMode.Mesh)
            {
                ParticleSystemRenderer component = base.m_ParticleSystemUI.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
                Mesh[] meshes = new Mesh[4];
                int num6 = component.GetMeshes(meshes);
                for (int m = 0; m < num6; m++)
                {
                    if (meshes[m].HasChannel(Mesh.InternalShaderChannel.TexCoord2))
                    {
                        if (textAndTooltip != "")
                        {
                            textAndTooltip = textAndTooltip + "\n\n";
                        }
                        textAndTooltip = textAndTooltip + "Meshes may only use a maximum of 2 input UV streams.";
                    }
                }
            }
            if (textAndTooltip != "")
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent(textAndTooltip).text, MessageType.Error, true);
            }
        }

        protected override void Init()
        {
            if (this.m_CastShadows == null)
            {
                this.m_CastShadows = base.GetProperty0("m_CastShadows");
                this.m_ReceiveShadows = base.GetProperty0("m_ReceiveShadows");
                this.m_Material = base.GetProperty0("m_Materials.Array.data[0]");
                this.m_TrailMaterial = base.GetProperty0("m_Materials.Array.data[1]");
                this.m_SortingOrder = base.GetProperty0("m_SortingOrder");
                this.m_SortingLayerID = base.GetProperty0("m_SortingLayerID");
                this.m_RenderMode = base.GetProperty0("m_RenderMode");
                this.m_MinParticleSize = base.GetProperty0("m_MinParticleSize");
                this.m_MaxParticleSize = base.GetProperty0("m_MaxParticleSize");
                this.m_CameraVelocityScale = base.GetProperty0("m_CameraVelocityScale");
                this.m_VelocityScale = base.GetProperty0("m_VelocityScale");
                this.m_LengthScale = base.GetProperty0("m_LengthScale");
                this.m_SortingFudge = base.GetProperty0("m_SortingFudge");
                this.m_SortMode = base.GetProperty0("m_SortMode");
                this.m_NormalDirection = base.GetProperty0("m_NormalDirection");
                this.m_Probes = new RendererEditorBase.Probes();
                this.m_Probes.Initialize(base.serializedObject);
                this.m_RenderAlignment = base.GetProperty0("m_RenderAlignment");
                this.m_Pivot = base.GetProperty0("m_Pivot");
                this.m_Meshes[0] = base.GetProperty0("m_Mesh");
                this.m_Meshes[1] = base.GetProperty0("m_Mesh1");
                this.m_Meshes[2] = base.GetProperty0("m_Mesh2");
                this.m_Meshes[3] = base.GetProperty0("m_Mesh3");
                List<SerializedProperty> list = new List<SerializedProperty>();
                for (int i = 0; i < this.m_Meshes.Length; i++)
                {
                    if ((i == 0) || (this.m_Meshes[i].objectReferenceValue != null))
                    {
                        list.Add(this.m_Meshes[i]);
                    }
                }
                this.m_ShownMeshes = list.ToArray();
                this.m_UseCustomVertexStreams = base.GetProperty0("m_UseCustomVertexStreams");
                this.m_VertexStreamMask = base.GetProperty0("m_VertexStreamMask");
                s_VisualizePivot = EditorPrefs.GetBool("VisualizePivot", false);
            }
        }

        public bool IsMeshEmitter() => 
            ((this.m_RenderMode != null) && (this.m_RenderMode.intValue == 4));

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            RenderMode intValue = (RenderMode) this.m_RenderMode.intValue;
            RenderMode renderMode = (RenderMode) ModuleUI.GUIPopup(s_Texts.renderMode, this.m_RenderMode, s_Texts.particleTypes, new GUILayoutOption[0]);
            switch (renderMode)
            {
                case RenderMode.Mesh:
                    EditorGUI.indentLevel++;
                    this.DoListOfMeshesGUI();
                    EditorGUI.indentLevel--;
                    if ((intValue != RenderMode.Mesh) && (this.m_Meshes[0].objectReferenceInstanceIDValue == 0))
                    {
                        this.m_Meshes[0].objectReferenceValue = Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
                    }
                    break;

                case RenderMode.Stretch3D:
                    EditorGUI.indentLevel++;
                    ModuleUI.GUIFloat(s_Texts.cameraSpeedScale, this.m_CameraVelocityScale, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.speedScale, this.m_VelocityScale, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.lengthScale, this.m_LengthScale, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                    break;
            }
            if (renderMode != RenderMode.None)
            {
                if (renderMode != RenderMode.Mesh)
                {
                    ModuleUI.GUIFloat(s_Texts.normalDirection, this.m_NormalDirection, new GUILayoutOption[0]);
                }
                if (this.m_Material != null)
                {
                    ModuleUI.GUIObject(s_Texts.material, this.m_Material, new GUILayoutOption[0]);
                }
            }
            if (base.m_ParticleSystemUI.m_ParticleSystem.trails.enabled && (this.m_TrailMaterial != null))
            {
                ModuleUI.GUIObject(s_Texts.trailMaterial, this.m_TrailMaterial, new GUILayoutOption[0]);
            }
            if (renderMode != RenderMode.None)
            {
                ModuleUI.GUIPopup(s_Texts.sortMode, this.m_SortMode, s_Texts.sortTypes, new GUILayoutOption[0]);
                ModuleUI.GUIFloat(s_Texts.sortingFudge, this.m_SortingFudge, new GUILayoutOption[0]);
                if (renderMode != RenderMode.Mesh)
                {
                    ModuleUI.GUIFloat(s_Texts.minParticleSize, this.m_MinParticleSize, new GUILayoutOption[0]);
                    ModuleUI.GUIFloat(s_Texts.maxParticleSize, this.m_MaxParticleSize, new GUILayoutOption[0]);
                }
                if (renderMode == RenderMode.Billboard)
                {
                    if (base.m_ParticleSystemUI.m_ParticleSystem.shape.alignToDirection)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            string[] options = new string[] { s_Texts.spaces[2] };
                            ModuleUI.GUIPopup(s_Texts.space, 0, options, new GUILayoutOption[0]);
                        }
                        EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Using Align to Direction in the Shape Module forces the system to be rendered using Local Billboard Alignment.").text, MessageType.Info, true);
                    }
                    else
                    {
                        ModuleUI.GUIPopup(s_Texts.space, this.m_RenderAlignment, s_Texts.spaces, new GUILayoutOption[0]);
                    }
                }
                ModuleUI.GUIVector3Field(s_Texts.pivot, this.m_Pivot, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                s_VisualizePivot = ModuleUI.GUIToggle(s_Texts.visualizePivot, s_VisualizePivot, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool("VisualizePivot", s_VisualizePivot);
                }
                if (ModuleUI.GUIToggle(s_Texts.useCustomVertexStreams, this.m_UseCustomVertexStreams, new GUILayoutOption[0]))
                {
                    this.DoVertexStreamsGUI(renderMode);
                }
                EditorGUILayout.Space();
                ModuleUI.GUIPopup(s_Texts.castShadows, this.m_CastShadows, this.m_CastShadows.enumDisplayNames, new GUILayoutOption[0]);
                using (new EditorGUI.DisabledScope(SceneView.IsUsingDeferredRenderingPath()))
                {
                    ModuleUI.GUIToggle(s_Texts.receiveShadows, this.m_ReceiveShadows, new GUILayoutOption[0]);
                }
                EditorGUILayout.SortingLayerField(s_Texts.sortingLayer, this.m_SortingLayerID, ParticleSystemStyles.Get().popup, ParticleSystemStyles.Get().label);
                ModuleUI.GUIInt(s_Texts.sortingOrder, this.m_SortingOrder, new GUILayoutOption[0]);
            }
            this.m_Probes.OnGUI(null, s.GetComponent<Renderer>(), true);
        }

        [DrawGizmo(GizmoType.Active)]
        private static void RenderPivots(ParticleSystem system, GizmoType gizmoType)
        {
            ParticleSystemRenderer component = system.GetComponent<ParticleSystemRenderer>();
            if (((component != null) && component.enabled) && s_VisualizePivot)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
                int num = system.GetParticles(particles);
                Color color = Gizmos.color;
                Gizmos.color = Color.green;
                Matrix4x4 identity = Matrix4x4.identity;
                if (system.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                {
                    identity = system.GetLocalToWorldMatrix();
                }
                Matrix4x4 matrix = Gizmos.matrix;
                Gizmos.matrix = identity;
                for (int i = 0; i < num; i++)
                {
                    ParticleSystem.Particle particle = particles[i];
                    Vector3 vector = particle.GetCurrentSize3D(system);
                    Gizmos.DrawWireSphere(particle.position, Math.Max(vector.x, Math.Max(vector.y, vector.z)) * 0.05f);
                }
                Gizmos.color = color;
                Gizmos.matrix = matrix;
            }
        }

        private static void SelectVertexStreamCallback(object obj)
        {
            <SelectVertexStreamCallback>c__AnonStorey0 storey = new <SelectVertexStreamCallback>c__AnonStorey0 {
                data = (StreamCallbackData) obj
            };
            storey.data.streamProp.intValue |= ((int) 1) << Array.FindIndex<string>(s_Texts.vertexStreams, new Predicate<string>(storey.<>m__0));
        }

        [CompilerGenerated]
        private sealed class <SelectVertexStreamCallback>c__AnonStorey0
        {
            internal RendererModuleUI.StreamCallbackData data;

            internal bool <>m__0(string item) => 
                (item == this.data.text);
        }

        private enum RenderMode
        {
            Billboard,
            Stretch3D,
            BillboardFixedHorizontal,
            BillboardFixedVertical,
            Mesh,
            None
        }

        private class StreamCallbackData
        {
            public SerializedProperty streamProp;
            public string text;

            public StreamCallbackData(SerializedProperty prop, string t)
            {
                this.streamProp = prop;
                this.text = t;
            }
        }

        private class Texts
        {
            public GUIContent cameraSpeedScale = EditorGUIUtility.TextContent("Camera Scale|How much the camera speed is factored in when determining particle stretching.");
            public GUIContent castShadows = EditorGUIUtility.TextContent("Cast Shadows|Only opaque materials cast shadows");
            public GUIContent lengthScale = EditorGUIUtility.TextContent("Length Scale|Defines the length of the particle compared to its width.");
            public GUIContent material = EditorGUIUtility.TextContent("Material|Defines the material used to render particles.");
            public GUIContent maxParticleSize = EditorGUIUtility.TextContent("Max Particle Size|How large is a particle allowed to be on screen at most? 1 is entire viewport. 0.5 is half viewport.");
            public GUIContent mesh = EditorGUIUtility.TextContent("Mesh|Defines the mesh that will be rendered as particle.");
            public GUIContent minParticleSize = EditorGUIUtility.TextContent("Min Particle Size|How small is a particle allowed to be on screen at least? 1 is entire viewport. 0.5 is half viewport.");
            public GUIContent normalDirection = EditorGUIUtility.TextContent("Normal Direction|Value between 0.0 and 1.0. If 1.0 is used, normals will point towards camera. If 0.0 is used, normals will point out in the corner direction of the particle.");
            public string[] particleTypes = new string[] { "Billboard", "Stretched Billboard", "Horizontal Billboard", "Vertical Billboard", "Mesh", "None" };
            public GUIContent pivot = EditorGUIUtility.TextContent("Pivot|Applies an offset to the pivot of particles, as a multiplier of its size.");
            public GUIContent receiveShadows = EditorGUIUtility.TextContent("Receive Shadows|Only opaque materials receive shadows");
            public GUIContent renderMode = EditorGUIUtility.TextContent("Render Mode|Defines the render mode of the particle renderer.");
            public GUIContent rotation = EditorGUIUtility.TextContent("Rotation|Set whether the rotation of the particles is defined in Screen or World space.");
            public GUIContent sortingFudge = EditorGUIUtility.TextContent("Sorting Fudge|Lower the number and most likely these particles will appear in front of other transparent objects, including other particles.");
            public GUIContent sortingLayer = EditorGUIUtility.TextContent("Sorting Layer");
            public GUIContent sortingOrder = EditorGUIUtility.TextContent("Order in Layer");
            public GUIContent sortMode = EditorGUIUtility.TextContent("Sort Mode|The draw order of particles can be sorted by distance, oldest in front, or youngest in front.");
            public string[] sortTypes = new string[] { "None", "By Distance", "Oldest in Front", "Youngest in Front" };
            public GUIContent space = EditorGUIUtility.TextContent("Billboard Alignment|Specifies if the particles will face the camera, align to world axes, or stay local to the system's transform.");
            public string[] spaces = new string[] { "View", "World", "Local", "Facing" };
            public GUIContent speedScale = EditorGUIUtility.TextContent("Speed Scale|Defines the length of the particle compared to its speed.");
            public GUIContent streams = EditorGUIUtility.TextContent("Vertex Streams|Configure the list of vertex attributes supplied to the vertex shader.");
            public GUIContent trailMaterial = EditorGUIUtility.TextContent("Trail Material|Defines the material used to render particle trails.");
            public GUIContent useCustomVertexStreams = EditorGUIUtility.TextContent("Use Custom Vertex Streams|Choose wheher to send custom particle data to the shader.");
            public string[] vertexStreamDataTypes = new string[] { "float3", "float3", "float4", "fixed4", "float2", "float4", "float4", "float3", "float3", "float3", "float2", "float4", "float4", "float4" };
            public bool[] vertexStreamIsTexCoord = new bool[] { false, false, false, false, true, true, true, true, true, true, true, true, true, true, false, false };
            public string[] vertexStreams = new string[] { "Position", "Normal", "Tangent", "Color", "UV", "UV2BlendAndFrame", "CenterAndVertexID", "Size", "Rotation", "Velocity", "Lifetime", "Custom1", "Custom2", "Random" };
            public GUIContent visualizePivot = EditorGUIUtility.TextContent("Visualize Pivot|Render the pivot positions of the particles.");
        }
    }
}


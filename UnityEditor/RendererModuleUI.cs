namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class RendererModuleUI : ModuleUI
    {
        [CompilerGenerated]
        private static Func<ParticleSystem, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction2 <>f__mg$cache0;
        private const int k_MaxNumMeshes = 4;
        private SerializedProperty m_CameraVelocityScale;
        private SerializedProperty m_CastShadows;
        private bool m_HasColor;
        private bool m_HasTangent;
        private SerializedProperty m_LengthScale;
        private SerializedProperty m_Material;
        private SerializedProperty m_MaxParticleSize;
        private SerializedProperty[] m_Meshes;
        private SerializedProperty m_MinParticleSize;
        private SerializedProperty m_NormalDirection;
        private int m_NumTexCoords;
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
        private int m_TexCoordChannelIndex;
        private SerializedProperty m_TrailMaterial;
        private SerializedProperty m_UseCustomVertexStreams;
        private SerializedProperty m_VelocityScale;
        private SerializedProperty m_VertexStreams;
        private ReorderableList m_VertexStreamsList;
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
            this.m_NumTexCoords = 0;
            this.m_TexCoordChannelIndex = 0;
            this.m_HasTangent = false;
            this.m_HasColor = false;
            this.m_VertexStreamsList.DoLayoutList();
            if (!base.m_ParticleSystemUI.multiEdit)
            {
                string textAndTooltip = "";
                if (this.m_Material != null)
                {
                    Material objectReferenceValue = this.m_Material.objectReferenceValue as Material;
                    int texCoordChannelCount = (this.m_NumTexCoords * 4) + this.m_TexCoordChannelIndex;
                    bool tangentError = false;
                    bool colorError = false;
                    bool uvError = false;
                    if (base.m_ParticleSystemUI.m_ParticleSystems[0].CheckVertexStreamsMatchShader(this.m_HasTangent, this.m_HasColor, texCoordChannelCount, objectReferenceValue, ref tangentError, ref colorError, ref uvError))
                    {
                        textAndTooltip = textAndTooltip + "Vertex streams do not match the shader inputs. Particle systems may not render correctly. Ensure your streams match and are used by the shader.";
                        if (tangentError)
                        {
                            textAndTooltip = textAndTooltip + "\n- TANGENT stream does not match.";
                        }
                        if (colorError)
                        {
                            textAndTooltip = textAndTooltip + "\n- COLOR stream does not match.";
                        }
                        if (uvError)
                        {
                            textAndTooltip = textAndTooltip + "\n- TEXCOORD streams do not match.";
                        }
                    }
                }
                int maxTexCoordStreams = base.m_ParticleSystemUI.m_ParticleSystems[0].GetMaxTexCoordStreams();
                if ((this.m_NumTexCoords > maxTexCoordStreams) || ((this.m_NumTexCoords == maxTexCoordStreams) && (this.m_TexCoordChannelIndex > 0)))
                {
                    if (textAndTooltip != "")
                    {
                        textAndTooltip = textAndTooltip + "\n\n";
                    }
                    string str2 = textAndTooltip;
                    object[] objArray1 = new object[] { str2, "Only ", maxTexCoordStreams, " TEXCOORD streams are supported." };
                    textAndTooltip = string.Concat(objArray1);
                }
                if (renderMode == RenderMode.Mesh)
                {
                    ParticleSystemRenderer component = base.m_ParticleSystemUI.m_ParticleSystems[0].GetComponent<ParticleSystemRenderer>();
                    Mesh[] meshes = new Mesh[4];
                    int num3 = component.GetMeshes(meshes);
                    for (int i = 0; i < num3; i++)
                    {
                        if (meshes[i].HasChannel(Mesh.InternalShaderChannel.TexCoord2))
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
        }

        private void DrawVertexStreamListElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            int intValue = this.m_VertexStreams.GetArrayElementAtIndex(index).intValue;
            string str = !base.isWindowView ? "TEXCOORD" : "TEX";
            int num2 = s_Texts.vertexStreamTexCoordChannels[intValue];
            if (num2 != 0)
            {
                int length = ((this.m_TexCoordChannelIndex + num2) <= 4) ? num2 : (num2 + 1);
                string str2 = s_Texts.channels.Substring(this.m_TexCoordChannelIndex, length);
                GUI.Label(rect, string.Concat(new object[] { s_Texts.vertexStreamsPacked[intValue], " (", str, this.m_NumTexCoords, ".", str2, ")" }), ParticleSystemStyles.Get().label);
                this.m_TexCoordChannelIndex += num2;
                if (this.m_TexCoordChannelIndex >= 4)
                {
                    this.m_TexCoordChannelIndex -= 4;
                    this.m_NumTexCoords++;
                }
            }
            else
            {
                GUI.Label(rect, s_Texts.vertexStreamsPacked[intValue] + " (" + s_Texts.vertexStreamPackedTypes[intValue] + ")", ParticleSystemStyles.Get().label);
                if (s_Texts.vertexStreamsPacked[intValue] == "Tangent")
                {
                    this.m_HasTangent = true;
                }
                if (s_Texts.vertexStreamsPacked[intValue] == "Color")
                {
                    this.m_HasColor = true;
                }
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
                this.m_VertexStreams = base.GetProperty0("m_VertexStreams");
                this.m_VertexStreamsList = new ReorderableList(base.serializedObject, this.m_VertexStreams, true, true, true, true);
                this.m_VertexStreamsList.elementHeight = 16f;
                this.m_VertexStreamsList.headerHeight = 0f;
                this.m_VertexStreamsList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.OnVertexStreamListAddDropdownCallback);
                this.m_VertexStreamsList.onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.OnVertexStreamListCanRemoveCallback);
                this.m_VertexStreamsList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawVertexStreamListElementCallback);
                s_VisualizePivot = EditorPrefs.GetBool("VisualizePivot", false);
            }
        }

        public override void OnInspectorGUI(InitialModuleUI initial)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            EditorGUI.BeginChangeCheck();
            RenderMode renderMode = (RenderMode) ModuleUI.GUIPopup(s_Texts.renderMode, this.m_RenderMode, s_Texts.particleTypes, new GUILayoutOption[0]);
            bool flag = EditorGUI.EndChangeCheck();
            if (!this.m_RenderMode.hasMultipleDifferentValues)
            {
                switch (renderMode)
                {
                    case RenderMode.Mesh:
                        EditorGUI.indentLevel++;
                        this.DoListOfMeshesGUI();
                        EditorGUI.indentLevel--;
                        if ((flag && (this.m_Meshes[0].objectReferenceInstanceIDValue == 0)) && !this.m_Meshes[0].hasMultipleDifferentValues)
                        {
                            this.m_Meshes[0].objectReferenceValue = UnityEngine.Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
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
            }
            if (this.m_TrailMaterial != null)
            {
                ModuleUI.GUIObject(s_Texts.trailMaterial, this.m_TrailMaterial, new GUILayoutOption[0]);
            }
            if (!this.m_RenderMode.hasMultipleDifferentValues && (renderMode != RenderMode.None))
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
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = o => o.shape.alignToDirection;
                    }
                    if (Enumerable.FirstOrDefault<ParticleSystem>(base.m_ParticleSystemUI.m_ParticleSystems, <>f__am$cache0) != null)
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
            List<ParticleSystemRenderer> source = new List<ParticleSystemRenderer>();
            foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
            {
                source.Add(system.GetComponent<ParticleSystemRenderer>());
            }
            this.m_Probes.OnGUI(source.ToArray(), source.FirstOrDefault<ParticleSystemRenderer>(), true);
        }

        public override void OnSceneViewGUI()
        {
            if (s_VisualizePivot)
            {
                Color color = Handles.color;
                Handles.color = Color.green;
                Matrix4x4 matrix = Handles.matrix;
                Vector3[] lineSegments = new Vector3[6];
                foreach (ParticleSystem system in base.m_ParticleSystemUI.m_ParticleSystems)
                {
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
                    int num2 = system.GetParticles(particles);
                    Matrix4x4 identity = Matrix4x4.identity;
                    if (system.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                    {
                        identity = system.GetLocalToWorldMatrix();
                    }
                    Handles.matrix = identity;
                    for (int i = 0; i < num2; i++)
                    {
                        ParticleSystem.Particle particle = particles[i];
                        Vector3 vector = (Vector3) (particle.GetCurrentSize3D(system) * 0.05f);
                        lineSegments[0] = particle.position - ((Vector3) (Vector3.right * vector.x));
                        lineSegments[1] = particle.position + ((Vector3) (Vector3.right * vector.x));
                        lineSegments[2] = particle.position - ((Vector3) (Vector3.up * vector.y));
                        lineSegments[3] = particle.position + ((Vector3) (Vector3.up * vector.y));
                        lineSegments[4] = particle.position - ((Vector3) (Vector3.forward * vector.z));
                        lineSegments[5] = particle.position + ((Vector3) (Vector3.forward * vector.z));
                        Handles.DrawLines(lineSegments);
                    }
                }
                Handles.color = color;
                Handles.matrix = matrix;
            }
        }

        private void OnVertexStreamListAddDropdownCallback(Rect rect, ReorderableList list)
        {
            List<int> list2 = new List<int>();
            for (int i = 0; i < s_Texts.vertexStreamsPacked.Length; i++)
            {
                bool flag = false;
                for (int k = 0; k < this.m_VertexStreams.arraySize; k++)
                {
                    if (this.m_VertexStreams.GetArrayElementAtIndex(k).intValue == i)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list2.Add(i);
                }
            }
            GenericMenu menu = new GenericMenu();
            for (int j = 0; j < list2.Count; j++)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new GenericMenu.MenuFunction2(RendererModuleUI.SelectVertexStreamCallback);
                }
                menu.AddItem(new GUIContent(s_Texts.vertexStreamsMenu[list2[j]]), false, <>f__mg$cache0, new StreamCallbackData(this.m_VertexStreams, list2[j]));
            }
            menu.ShowAsContext();
            Event.current.Use();
        }

        private bool OnVertexStreamListCanRemoveCallback(ReorderableList list)
        {
            SerializedProperty arrayElementAtIndex = this.m_VertexStreams.GetArrayElementAtIndex(list.index);
            return (s_Texts.vertexStreamsPacked[arrayElementAtIndex.intValue] != "Position");
        }

        private static void SelectVertexStreamCallback(object obj)
        {
            StreamCallbackData data = (StreamCallbackData) obj;
            int arraySize = data.streamProp.arraySize;
            data.streamProp.InsertArrayElementAtIndex(arraySize);
            data.streamProp.GetArrayElementAtIndex(arraySize).intValue = data.stream;
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
            public int stream;
            public SerializedProperty streamProp;

            public StreamCallbackData(SerializedProperty prop, int s)
            {
                this.streamProp = prop;
                this.stream = s;
            }
        }

        private class Texts
        {
            public GUIContent cameraSpeedScale = EditorGUIUtility.TextContent("Camera Scale|How much the camera speed is factored in when determining particle stretching.");
            public GUIContent castShadows = EditorGUIUtility.TextContent("Cast Shadows|Only opaque materials cast shadows");
            public string channels = "xyzw|xyz";
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
            public GUIContent trailMaterial = EditorGUIUtility.TextContent("Trail Material|Defines the material used to render particle trails.");
            public GUIContent useCustomVertexStreams = EditorGUIUtility.TextContent("Custom Vertex Streams|Choose whether to send custom particle data to the shader.");
            public string[] vertexStreamPackedTypes = new string[] { "POSITION.xyz", "NORMAL.xyz", "TANGENT.xyzw", "COLOR.xyzw" };
            public string[] vertexStreamsMenu = new string[] { 
                "Position", "Normal", "Tangent", "Color", "UV/UV1", "UV/UV2", "UV/UV3", "UV/UV4", "UV/AnimBlend", "UV/AnimFrame", "Center", "VertexID", "Size/Size.x", "Size/Size.xy", "Size/Size.xyz", "Rotation/Rotation",
                "Rotation/Rotation3D", "Rotation/RotationSpeed", "Rotation/RotationSpeed3D", "Velocity", "Speed", "Lifetime/AgePercent", "Lifetime/InverseStartLifetime", "Random/Stable.x", "Random/Stable.xy", "Random/Stable.xyz", "Random/Stable.xyzw", "Random/Varying.x", "Random/Varying.xy", "Random/Varying.xyz", "Random/Varying.xyzw", "Custom/Custom1.x",
                "Custom/Custom1.xy", "Custom/Custom1.xyz", "Custom/Custom1.xyzw", "Custom/Custom2.x", "Custom/Custom2.xy", "Custom/Custom2.xyz", "Custom/Custom2.xyzw"
            };
            public string[] vertexStreamsPacked = new string[] { 
                "Position", "Normal", "Tangent", "Color", "UV", "UV2", "UV3", "UV4", "AnimBlend", "AnimFrame", "Center", "VertexID", "Size", "Size.xy", "Size.xyz", "Rotation",
                "Rotation3D", "RotationSpeed", "RotationSpeed3D", "Velocity", "Speed", "AgePercent", "InverseStartLifetime", "StableRandom.x", "StableRandom.xy", "StableRandom.xyz", "StableRandom.xyzw", "VariableRandom.x", "VariableRandom.xy", "VariableRandom.xyz", "VariableRandom.xyzw", "Custom1.x",
                "Custom1.xy", "Custom1.xyz", "Custom1.xyzw", "Custom2.x", "Custom2.xy", "Custom2.xyz", "Custom2.xyzw"
            };
            public int[] vertexStreamTexCoordChannels = new int[] { 
                0, 0, 0, 0, 2, 2, 2, 2, 1, 1, 3, 1, 1, 2, 3, 1,
                3, 1, 3, 3, 1, 1, 1, 1, 2, 3, 4, 1, 2, 3, 4, 1,
                2, 3, 4, 1, 2, 3, 4
            };
            public GUIContent visualizePivot = EditorGUIUtility.TextContent("Visualize Pivot|Render the pivot positions of the particles.");
        }
    }
}


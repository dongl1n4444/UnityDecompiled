namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class FrameDebuggerWindow : EditorWindow
    {
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cache0;
        private const int kArraySizeBitMask = 0x3ff;
        private const float kArrayValuePopupBtnWidth = 25f;
        private const float kDetailsMargin = 4f;
        private const string kFloatDetailedFormat = "g7";
        private const string kFloatFormat = "g2";
        private const float kMinDetailsWidth = 200f;
        private const float kMinListWidth = 200f;
        private const float kMinPreviewSize = 64f;
        private const float kMinWindowWidth = 240f;
        private const float kNameFieldWidth = 200f;
        private const int kNeedToRepaintFrames = 4;
        private const float kResizerWidth = 5f;
        private const float kScrollbarWidth = 16f;
        private const float kShaderPropertiesIndention = 15f;
        private const int kShaderTypeBits = 6;
        private const float kValueFieldWidth = 200f;
        private ShowAdditionalInfo m_AdditionalInfo = ShowAdditionalInfo.ShaderProperties;
        private GUIContent[] m_AdditionalInfoGuiContents;
        private AttachProfilerUI m_AttachProfilerUI;
        [NonSerialized]
        private int m_FrameEventsHash;
        [SerializeField]
        private float m_ListWidth = 300f;
        private Material m_Material;
        private int m_PrevEventsCount = 0;
        private int m_PrevEventsLimit = 0;
        public Vector2 m_PreviewDir = new Vector2(120f, -20f);
        private PreviewRenderUtility m_PreviewUtility;
        private int m_RepaintFrames = 4;
        [NonSerialized]
        private float m_RTBlackLevel;
        [NonSerialized]
        private int m_RTChannel;
        [NonSerialized]
        private int m_RTIndex;
        [NonSerialized]
        private float m_RTWhiteLevel = 1f;
        private Vector2 m_ScrollViewShaderProps = Vector2.zero;
        [NonSerialized]
        private FrameDebuggerTreeView m_Tree;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        private Material m_WireMaterial;
        private static Styles ms_Styles;
        private static List<FrameDebuggerWindow> s_FrameDebuggers = new List<FrameDebuggerWindow>();
        public static readonly string[] s_FrameEventTypeNames = new string[] { 
            "Clear (nothing)", "Clear (color)", "Clear (Z)", "Clear (color+Z)", "Clear (stencil)", "Clear (color+stencil)", "Clear (Z+stencil)", "Clear (color+Z+stencil)", "SetRenderTarget", "Resolve Color", "Resolve Depth", "Grab RenderTexture", "Static Batch", "Dynamic Batch", "Draw Mesh", "Draw Dynamic",
            "Draw GL", "GPU Skinning", "Draw Procedural", "Compute Shader", "Plugin Event", "Draw Mesh (instanced)"
        };

        public FrameDebuggerWindow()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, GUIContent>(FrameDebuggerWindow.<m_AdditionalInfoGuiContents>m__0);
            }
            this.m_AdditionalInfoGuiContents = Enumerable.Select<string, GUIContent>(Enum.GetNames(typeof(ShowAdditionalInfo)), <>f__am$cache0).ToArray<GUIContent>();
            this.m_AttachProfilerUI = new AttachProfilerUI();
            base.position = new Rect(50f, 50f, 600f, 350f);
            base.minSize = new Vector2(400f, 200f);
        }

        [CompilerGenerated]
        private static GUIContent <m_AdditionalInfoGuiContents>m__0(string m) => 
            new GUIContent(m);

        internal void ChangeFrameEventLimit(int newLimit)
        {
            if ((newLimit > 0) && (newLimit <= FrameDebuggerUtility.count))
            {
                if ((newLimit != FrameDebuggerUtility.limit) && (newLimit > 0))
                {
                    GameObject frameEventGameObject = FrameDebuggerUtility.GetFrameEventGameObject(newLimit - 1);
                    if (frameEventGameObject != null)
                    {
                        EditorGUIUtility.PingObject(frameEventGameObject);
                    }
                }
                FrameDebuggerUtility.limit = newLimit;
                if (this.m_Tree != null)
                {
                    this.m_Tree.SelectFrameEventIndex(newLimit);
                }
            }
        }

        private void ClickEnableFrameDebugger()
        {
            bool flag = FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled();
            bool flag2 = !flag && this.m_AttachProfilerUI.IsEditor();
            if (!flag2 || FrameDebuggerUtility.locallySupported)
            {
                if (flag2 && (EditorApplication.isPlaying && !EditorApplication.isPaused))
                {
                    EditorApplication.isPaused = true;
                }
                if (!flag)
                {
                    FrameDebuggerUtility.SetEnabled(true, ProfilerDriver.connectedProfiler);
                }
                else
                {
                    FrameDebuggerUtility.SetEnabled(false, FrameDebuggerUtility.GetRemotePlayerGUID());
                }
                if (FrameDebuggerUtility.IsLocalEnabled())
                {
                    GameView view = (GameView) WindowLayout.FindEditorWindowOfType(typeof(GameView));
                    if (view != null)
                    {
                        view.ShowTab();
                    }
                }
                this.m_PrevEventsLimit = FrameDebuggerUtility.limit;
                this.m_PrevEventsCount = FrameDebuggerUtility.count;
            }
        }

        private static void DisableFrameDebugger()
        {
            if (FrameDebuggerUtility.IsLocalEnabled())
            {
                EditorApplication.SetSceneRepaintDirty();
            }
            FrameDebuggerUtility.SetEnabled(false, FrameDebuggerUtility.GetRemotePlayerGUID());
        }

        private void DrawCurrentEvent(Rect rect, FrameDebuggerEvent[] descs)
        {
            int index = FrameDebuggerUtility.limit - 1;
            if ((index >= 0) && (index < descs.Length))
            {
                FrameDebuggerEventData data;
                GUILayout.BeginArea(rect);
                FrameDebuggerEvent event2 = descs[index];
                bool frameEventData = FrameDebuggerUtility.GetFrameEventData(index, out data);
                if (frameEventData)
                {
                    this.DrawRenderTargetControls(data);
                }
                GUILayout.Label($"Event #{index + 1}: {s_FrameEventTypeNames[(int) event2.type]}", EditorStyles.boldLabel, new GUILayoutOption[0]);
                if (FrameDebuggerUtility.IsRemoteEnabled() && FrameDebuggerUtility.receivingRemoteFrameEventData)
                {
                    GUILayout.Label("Receiving frame event data...", new GUILayoutOption[0]);
                }
                else if (frameEventData)
                {
                    if ((data.vertexCount > 0) || (data.indexCount > 0))
                    {
                        this.DrawEventDrawCallInfo(data);
                    }
                    else if (event2.type == FrameEventType.ComputeDispatch)
                    {
                        this.DrawEventComputeDispatchInfo(data);
                    }
                }
                GUILayout.EndArea();
            }
        }

        private void DrawEventComputeDispatchInfo(FrameDebuggerEventData curEventData)
        {
            string str;
            EditorGUILayout.LabelField("Compute Shader", curEventData.csName, new GUILayoutOption[0]);
            if (GUI.Button(GUILayoutUtility.GetLastRect(), GUIContent.none, GUI.skin.label))
            {
                EditorGUIUtility.PingObject(curEventData.csInstanceID);
                Event.current.Use();
            }
            EditorGUILayout.LabelField("Kernel", curEventData.csKernel, new GUILayoutOption[0]);
            if (((curEventData.csThreadGroupsX != 0) || (curEventData.csThreadGroupsY != 0)) || (curEventData.csThreadGroupsZ != 0))
            {
                str = $"{curEventData.csThreadGroupsX}x{curEventData.csThreadGroupsY}x{curEventData.csThreadGroupsZ}";
            }
            else
            {
                str = "indirect dispatch";
            }
            EditorGUILayout.LabelField("Thread Groups", str, new GUILayoutOption[0]);
        }

        private void DrawEventDrawCallInfo(FrameDebuggerEventData curEventData)
        {
            string str = curEventData.shaderName + ", pass #" + curEventData.shaderPassIndex;
            EditorGUILayout.LabelField("Shader", str, new GUILayoutOption[0]);
            if (GUI.Button(GUILayoutUtility.GetLastRect(), Styles.selectShaderTooltip, GUI.skin.label))
            {
                EditorGUIUtility.PingObject(curEventData.shaderInstanceID);
                Event.current.Use();
            }
            if (!string.IsNullOrEmpty(curEventData.shaderKeywords))
            {
                EditorGUILayout.LabelField("Keywords", curEventData.shaderKeywords, new GUILayoutOption[0]);
                if (GUI.Button(GUILayoutUtility.GetLastRect(), Styles.copyToClipboardTooltip, GUI.skin.label))
                {
                    EditorGUIUtility.systemCopyBuffer = str + Environment.NewLine + curEventData.shaderKeywords;
                }
            }
            this.DrawStates(curEventData);
            if (curEventData.batchBreakCause > 1)
            {
                GUILayout.Space(10f);
                GUILayout.Label(Styles.causeOfNewDrawCallLabel, EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUILayout.Label(styles.batchBreakCauses[curEventData.batchBreakCause], EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            GUILayout.Space(15f);
            this.m_AdditionalInfo = (ShowAdditionalInfo) GUILayout.Toolbar((int) this.m_AdditionalInfo, this.m_AdditionalInfoGuiContents, new GUILayoutOption[0]);
            switch (this.m_AdditionalInfo)
            {
                case ShowAdditionalInfo.Preview:
                    if (!this.DrawEventMesh(curEventData))
                    {
                        EditorGUILayout.LabelField("Vertices", curEventData.vertexCount.ToString(), new GUILayoutOption[0]);
                        EditorGUILayout.LabelField("Indices", curEventData.indexCount.ToString(), new GUILayoutOption[0]);
                    }
                    break;

                case ShowAdditionalInfo.ShaderProperties:
                    this.DrawShaderProperties(curEventData.shaderProperties);
                    break;
            }
        }

        private bool DrawEventMesh(FrameDebuggerEventData curEventData)
        {
            Mesh mesh = curEventData.mesh;
            if (mesh == null)
            {
                return false;
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            Rect position = GUILayoutUtility.GetRect((float) 10f, (float) 10f, options);
            if ((position.width >= 64f) && (position.height >= 64f))
            {
                GameObject frameEventGameObject = FrameDebuggerUtility.GetFrameEventGameObject(curEventData.frameEventIndex);
                Rect meshInfoRect = position;
                meshInfoRect.yMin = meshInfoRect.yMax - (EditorGUIUtility.singleLineHeight * 2f);
                Rect rect3 = meshInfoRect;
                meshInfoRect.xMin = meshInfoRect.center.x;
                rect3.xMax = rect3.center.x;
                if (Event.current.type == EventType.MouseDown)
                {
                    if (meshInfoRect.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(mesh);
                        Event.current.Use();
                    }
                    if ((frameEventGameObject != null) && rect3.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(frameEventGameObject.GetInstanceID());
                        Event.current.Use();
                    }
                }
                this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, position);
                if (Event.current.type == EventType.Repaint)
                {
                    int meshSubset = curEventData.meshSubset;
                    this.DrawMeshPreview(curEventData, position, meshInfoRect, mesh, meshSubset);
                    if (frameEventGameObject != null)
                    {
                        EditorGUI.DropShadowLabel(rect3, frameEventGameObject.name);
                    }
                }
            }
            return true;
        }

        private void DrawEventsTree(Rect rect)
        {
            this.m_Tree.OnGUI(rect);
        }

        private void DrawMeshPreview(FrameDebuggerEventData curEventData, Rect previewRect, Rect meshInfoRect, Mesh mesh, int meshSubset)
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility();
                this.m_PreviewUtility.m_CameraFieldOfView = 30f;
            }
            if (this.m_Material == null)
            {
                this.m_Material = EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material;
            }
            if (this.m_WireMaterial == null)
            {
                this.m_WireMaterial = ModelInspector.CreateWireframeMaterial();
            }
            this.m_PreviewUtility.BeginPreview(previewRect, "preBackground");
            ModelInspector.RenderMeshPreview(mesh, this.m_PreviewUtility, this.m_Material, this.m_WireMaterial, this.m_PreviewDir, meshSubset);
            this.m_PreviewUtility.EndAndDrawPreview(previewRect);
            string name = mesh.name;
            if (string.IsNullOrEmpty(name))
            {
                name = "<no name>";
            }
            object[] objArray1 = new object[] { name, " subset ", meshSubset, "\n", curEventData.vertexCount, " verts, ", curEventData.indexCount, " indices" };
            string text = string.Concat(objArray1);
            if (curEventData.instanceCount > 1)
            {
                string str3 = text;
                object[] objArray2 = new object[] { str3, ", ", curEventData.instanceCount, " instances" };
                text = string.Concat(objArray2);
            }
            EditorGUI.DropShadowLabel(meshInfoRect, text);
        }

        private void DrawRenderTargetControls(FrameDebuggerEventData cur)
        {
            if ((cur.rtWidth > 0) && (cur.rtHeight > 0))
            {
                bool flag3;
                bool disabled = (cur.rtFormat == 1) || (cur.rtFormat == 3);
                bool flag2 = cur.rtHasDepthTexture != 0;
                short rtCount = cur.rtCount;
                if (flag2)
                {
                    rtCount = (short) (rtCount + 1);
                }
                EditorGUILayout.LabelField("RenderTarget", cur.rtName, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                using (new EditorGUI.DisabledScope(rtCount <= 1))
                {
                    GUIContent[] displayedOptions = new GUIContent[rtCount];
                    for (int i = 0; i < cur.rtCount; i++)
                    {
                        displayedOptions[i] = Styles.mrtLabels[i];
                    }
                    if (flag2)
                    {
                        displayedOptions[cur.rtCount] = Styles.depthLabel;
                    }
                    int num3 = Mathf.Clamp(this.m_RTIndex, 0, rtCount - 1);
                    flag3 = num3 != this.m_RTIndex;
                    this.m_RTIndex = num3;
                    GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(70f) };
                    this.m_RTIndex = EditorGUILayout.Popup(this.m_RTIndex, displayedOptions, EditorStyles.toolbarPopup, optionArray1);
                }
                GUILayout.Space(10f);
                using (new EditorGUI.DisabledScope(disabled))
                {
                    GUILayout.Label(Styles.channelHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
                    this.m_RTChannel = GUILayout.Toolbar(this.m_RTChannel, Styles.channelLabels, EditorStyles.toolbarButton, new GUILayoutOption[0]);
                }
                GUILayout.Space(10f);
                GUILayout.Label(Styles.levelsHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(200f) };
                EditorGUILayout.MinMaxSlider(ref this.m_RTBlackLevel, ref this.m_RTWhiteLevel, 0f, 1f, options);
                if (EditorGUI.EndChangeCheck() || flag3)
                {
                    Vector4 zero = Vector4.zero;
                    if (this.m_RTChannel == 1)
                    {
                        zero.x = 1f;
                    }
                    else if (this.m_RTChannel == 2)
                    {
                        zero.y = 1f;
                    }
                    else if (this.m_RTChannel == 3)
                    {
                        zero.z = 1f;
                    }
                    else if (this.m_RTChannel == 4)
                    {
                        zero.w = 1f;
                    }
                    else
                    {
                        zero = Vector4.one;
                    }
                    int rTIndex = this.m_RTIndex;
                    if (rTIndex >= cur.rtCount)
                    {
                        rTIndex = -1;
                    }
                    FrameDebuggerUtility.SetRenderTargetDisplayOptions(rTIndex, zero, this.m_RTBlackLevel, this.m_RTWhiteLevel);
                    this.RepaintAllNeededThings();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Label($"{cur.rtWidth}x{cur.rtHeight} {(RenderTextureFormat) cur.rtFormat}", new GUILayoutOption[0]);
                if (cur.rtDim == 4)
                {
                    GUILayout.Label("Rendering into cubemap", new GUILayoutOption[0]);
                }
                if ((cur.rtFormat == 3) && SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9"))
                {
                    EditorGUILayout.HelpBox("Rendering into shadowmap on DX9, can't visualize it in the game view properly", MessageType.Info, true);
                }
            }
        }

        private void DrawShaderProperties(ShaderProperties props)
        {
            this.m_ScrollViewShaderProps = GUILayout.BeginScrollView(this.m_ScrollViewShaderProps, new GUILayoutOption[0]);
            if (props.textures.Count<ShaderTextureInfo>() > 0)
            {
                GUILayout.Label("Textures", EditorStyles.boldLabel, new GUILayoutOption[0]);
                foreach (ShaderTextureInfo info in props.textures)
                {
                    this.OnGUIShaderPropTexture(info);
                }
            }
            if (props.floats.Count<ShaderFloatInfo>() > 0)
            {
                int num3;
                GUILayout.Label("Floats", EditorStyles.boldLabel, new GUILayoutOption[0]);
                for (int i = 0; i < props.floats.Length; i += num3)
                {
                    num3 = (props.floats[i].flags >> 6) & 0x3ff;
                    this.OnGUIShaderPropFloats(props.floats, i, num3);
                }
            }
            if (props.vectors.Count<ShaderVectorInfo>() > 0)
            {
                int num5;
                GUILayout.Label("Vectors", EditorStyles.boldLabel, new GUILayoutOption[0]);
                for (int j = 0; j < props.vectors.Length; j += num5)
                {
                    num5 = (props.vectors[j].flags >> 6) & 0x3ff;
                    this.OnGUIShaderPropVectors(props.vectors, j, num5);
                }
            }
            if (props.matrices.Count<ShaderMatrixInfo>() > 0)
            {
                int num7;
                GUILayout.Label("Matrices", EditorStyles.boldLabel, new GUILayoutOption[0]);
                for (int k = 0; k < props.matrices.Length; k += num7)
                {
                    num7 = (props.matrices[k].flags >> 6) & 0x3ff;
                    this.OnGUIShaderPropMatrices(props.matrices, k, num7);
                }
            }
            if (props.buffers.Count<ShaderBufferInfo>() > 0)
            {
                GUILayout.Label("Buffers", EditorStyles.boldLabel, new GUILayoutOption[0]);
                foreach (ShaderBufferInfo info2 in props.buffers)
                {
                    this.OnGUIShaderPropBuffer(info2);
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawShaderPropertyFlags(int flags)
        {
            string text = string.Empty;
            if ((flags & 2) != 0)
            {
                text = text + 'v';
            }
            if ((flags & 4) != 0)
            {
                text = text + 'f';
            }
            if ((flags & 8) != 0)
            {
                text = text + 'g';
            }
            if ((flags & 0x10) != 0)
            {
                text = text + 'h';
            }
            if ((flags & 0x20) != 0)
            {
                text = text + 'd';
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(20f) };
            GUILayout.Label(text, EditorStyles.miniLabel, options);
        }

        private void DrawStates(FrameDebuggerEventData curEventData)
        {
            FrameDebuggerBlendState blendState = curEventData.blendState;
            FrameDebuggerRasterState rasterState = curEventData.rasterState;
            FrameDebuggerDepthState depthState = curEventData.depthState;
            string str = $"{blendState.srcBlend} {blendState.dstBlend}";
            if ((blendState.srcBlendAlpha != blendState.srcBlend) || (blendState.dstBlendAlpha != blendState.dstBlend))
            {
                str = str + $", {blendState.srcBlendAlpha} {blendState.dstBlendAlpha}";
            }
            EditorGUILayout.LabelField("Blend", str, new GUILayoutOption[0]);
            if ((blendState.blendOp != BlendOp.Add) || (blendState.blendOpAlpha != BlendOp.Add))
            {
                string str2;
                if (blendState.blendOp == blendState.blendOpAlpha)
                {
                    str2 = blendState.blendOp.ToString();
                }
                else
                {
                    str2 = $"{blendState.blendOp}, {blendState.blendOpAlpha}";
                }
                EditorGUILayout.LabelField("BlendOp", str2, new GUILayoutOption[0]);
            }
            if (blendState.writeMask != 15)
            {
                string str3 = "";
                if (blendState.writeMask == 0)
                {
                    str3 = str3 + '0';
                }
                else
                {
                    if ((blendState.writeMask & 2) != 0)
                    {
                        str3 = str3 + 'R';
                    }
                    if ((blendState.writeMask & 4) != 0)
                    {
                        str3 = str3 + 'G';
                    }
                    if ((blendState.writeMask & 8) != 0)
                    {
                        str3 = str3 + 'B';
                    }
                    if ((blendState.writeMask & 1) != 0)
                    {
                        str3 = str3 + 'A';
                    }
                }
                EditorGUILayout.LabelField("ColorMask", str3, new GUILayoutOption[0]);
            }
            EditorGUILayout.LabelField("ZTest", depthState.depthFunc.ToString(), new GUILayoutOption[0]);
            EditorGUILayout.LabelField("ZWrite", (depthState.depthWrite != 0) ? "On" : "Off", new GUILayoutOption[0]);
            EditorGUILayout.LabelField("Cull", rasterState.cullMode.ToString(), new GUILayoutOption[0]);
            if ((rasterState.slopeScaledDepthBias != 0f) || (rasterState.depthBias != 0))
            {
                string str4 = $"{rasterState.slopeScaledDepthBias}, {rasterState.depthBias}";
                EditorGUILayout.LabelField("Offset", str4, new GUILayoutOption[0]);
            }
        }

        private bool DrawToolbar(FrameDebuggerEvent[] descs)
        {
            int num;
            bool flag = false;
            bool flag2 = !this.m_AttachProfilerUI.IsEditor() || FrameDebuggerUtility.locallySupported;
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(!flag2))
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(80f) };
                GUILayout.Toggle(FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled(), styles.recordButton, EditorStyles.toolbarButton, options);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ClickEnableFrameDebugger();
                flag = true;
            }
            this.m_AttachProfilerUI.OnGUILayout(this);
            bool flag3 = FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled();
            if (flag3 && (ProfilerDriver.connectedProfiler != FrameDebuggerUtility.GetRemotePlayerGUID()))
            {
                FrameDebuggerUtility.SetEnabled(false, FrameDebuggerUtility.GetRemotePlayerGUID());
                FrameDebuggerUtility.SetEnabled(true, ProfilerDriver.connectedProfiler);
            }
            GUI.enabled = flag3;
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(FrameDebuggerUtility.count <= 1))
            {
                num = EditorGUILayout.IntSlider(FrameDebuggerUtility.limit, 1, FrameDebuggerUtility.count, new GUILayoutOption[0]);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ChangeFrameEventLimit(num);
            }
            GUILayout.Label(" of " + FrameDebuggerUtility.count, EditorStyles.miniLabel, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(num <= 1))
            {
                if (GUILayout.Button(styles.prevFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    this.ChangeFrameEventLimit(num - 1);
                }
            }
            using (new EditorGUI.DisabledScope(num >= FrameDebuggerUtility.count))
            {
                if (GUILayout.Button(styles.nextFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    this.ChangeFrameEventLimit(num + 1);
                }
                if ((this.m_PrevEventsLimit == this.m_PrevEventsCount) && ((FrameDebuggerUtility.count != this.m_PrevEventsCount) && (FrameDebuggerUtility.limit == this.m_PrevEventsLimit)))
                {
                    this.ChangeFrameEventLimit(FrameDebuggerUtility.count);
                }
                this.m_PrevEventsLimit = FrameDebuggerUtility.limit;
                this.m_PrevEventsCount = FrameDebuggerUtility.count;
            }
            GUILayout.EndHorizontal();
            return flag;
        }

        public void EnableIfNeeded()
        {
            if (!FrameDebuggerUtility.IsLocalEnabled() && !FrameDebuggerUtility.IsRemoteEnabled())
            {
                this.m_RTChannel = 0;
                this.m_RTIndex = 0;
                this.m_RTBlackLevel = 0f;
                this.m_RTWhiteLevel = 1f;
                this.ClickEnableFrameDebugger();
                this.RepaintOnLimitChange();
            }
        }

        internal void OnDidOpenScene()
        {
            DisableFrameDebugger();
        }

        internal void OnDisable()
        {
            if (this.m_WireMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_WireMaterial, true);
            }
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
            s_FrameDebuggers.Remove(this);
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            DisableFrameDebugger();
        }

        internal void OnEnable()
        {
            base.autoRepaintOnSceneChange = true;
            s_FrameDebuggers.Add(this);
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            this.m_RepaintFrames = 4;
        }

        internal void OnGUI()
        {
            FrameDebuggerEvent[] frameEvents = FrameDebuggerUtility.GetFrameEvents();
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            if (this.m_Tree == null)
            {
                this.m_Tree = new FrameDebuggerTreeView(frameEvents, this.m_TreeViewState, this, new Rect());
                this.m_FrameEventsHash = FrameDebuggerUtility.eventsHash;
                this.m_Tree.m_DataSource.SetExpandedWithChildren(this.m_Tree.m_DataSource.root, true);
            }
            if (FrameDebuggerUtility.eventsHash != this.m_FrameEventsHash)
            {
                this.m_Tree.m_DataSource.SetEvents(frameEvents);
                this.m_FrameEventsHash = FrameDebuggerUtility.eventsHash;
            }
            int limit = FrameDebuggerUtility.limit;
            bool flag = this.DrawToolbar(frameEvents);
            if ((!FrameDebuggerUtility.IsLocalEnabled() && !FrameDebuggerUtility.IsRemoteEnabled()) && this.m_AttachProfilerUI.IsEditor())
            {
                GUI.enabled = true;
                if (!FrameDebuggerUtility.locallySupported)
                {
                    EditorGUILayout.HelpBox("Frame Debugger requires multi-threaded renderer. Usually Unity uses that; if it does not, try starting with -force-gfx-mt command line argument.", MessageType.Warning, true);
                }
                EditorGUILayout.HelpBox("Frame Debugger lets you step through draw calls and see how exactly frame is rendered. Click Enable!", MessageType.Info, true);
            }
            else
            {
                float fixedHeight = EditorStyles.toolbar.fixedHeight;
                Rect dragRect = new Rect(this.m_ListWidth, fixedHeight, 5f, base.position.height - fixedHeight);
                dragRect = EditorGUIUtility.HandleHorizontalSplitter(dragRect, base.position.width, 200f, 200f);
                this.m_ListWidth = dragRect.x;
                Rect rect = new Rect(0f, fixedHeight, this.m_ListWidth, base.position.height - fixedHeight);
                Rect rect7 = new Rect(this.m_ListWidth + 4f, fixedHeight + 4f, (base.position.width - this.m_ListWidth) - 8f, (base.position.height - fixedHeight) - 8f);
                this.DrawEventsTree(rect);
                EditorGUIUtility.DrawHorizontalSplitter(dragRect);
                this.DrawCurrentEvent(rect7, frameEvents);
            }
            if (flag || (limit != FrameDebuggerUtility.limit))
            {
                this.RepaintOnLimitChange();
            }
            if (this.m_RepaintFrames > 0)
            {
                this.m_Tree.SelectFrameEventIndex(FrameDebuggerUtility.limit);
                this.RepaintAllNeededThings();
                this.m_RepaintFrames--;
            }
        }

        private void OnGUIShaderPropBuffer(ShaderBufferInfo t)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(15f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
            GUILayout.Label(t.name, EditorStyles.miniLabel, options);
            this.DrawShaderPropertyFlags(t.flags);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
            GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel, optionArray2);
            GUILayout.EndHorizontal();
        }

        private void OnGUIShaderPropFloats(ShaderFloatInfo[] floats, int startIndex, int numValues)
        {
            <OnGUIShaderPropFloats>c__AnonStorey1 storey = new <OnGUIShaderPropFloats>c__AnonStorey1 {
                floats = floats
            };
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(15f);
            ShaderFloatInfo info = storey.floats[startIndex];
            if (numValues == 1)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label(info.name, EditorStyles.miniLabel, options);
                this.DrawShaderPropertyFlags(info.flags);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label(info.value.ToString("g2"), EditorStyles.miniLabel, optionArray2);
                this.ShaderPropertyCopyValueMenu(GUILayoutUtility.GetLastRect(), info.value);
            }
            else
            {
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label($"{info.name} [{numValues}]", EditorStyles.miniLabel, optionArray3);
                this.DrawShaderPropertyFlags(info.flags);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                Rect position = GUILayoutUtility.GetRect(Styles.arrayValuePopupButton, GUI.skin.button, optionArray4);
                position.width = 25f;
                if (GUI.Button(position, Styles.arrayValuePopupButton))
                {
                    ArrayValuePopup.GetValueStringDelegate getValueString = new ArrayValuePopup.GetValueStringDelegate(storey.<>m__0);
                    PopupWindowWithoutFocus.Show(position, new ArrayValuePopup(startIndex, numValues, 100f, getValueString), new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Left });
                }
            }
            GUILayout.EndHorizontal();
        }

        private void OnGUIShaderPropMatrices(ShaderMatrixInfo[] matrices, int startIndex, int numValues)
        {
            <OnGUIShaderPropMatrices>c__AnonStorey3 storey = new <OnGUIShaderPropMatrices>c__AnonStorey3 {
                matrices = matrices
            };
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(15f);
            ShaderMatrixInfo info = storey.matrices[startIndex];
            if (numValues == 1)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label(info.name, EditorStyles.miniLabel, options);
                this.DrawShaderPropertyFlags(info.flags);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label(info.value.ToString("g2"), EditorStyles.miniLabel, optionArray2);
                this.ShaderPropertyCopyValueMenu(GUILayoutUtility.GetLastRect(), info.value);
            }
            else
            {
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label($"{info.name} [{numValues}]", EditorStyles.miniLabel, optionArray3);
                this.DrawShaderPropertyFlags(info.flags);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                Rect position = GUILayoutUtility.GetRect(Styles.arrayValuePopupButton, GUI.skin.button, optionArray4);
                position.width = 25f;
                if (GUI.Button(position, Styles.arrayValuePopupButton))
                {
                    ArrayValuePopup.GetValueStringDelegate getValueString = new ArrayValuePopup.GetValueStringDelegate(storey.<>m__0);
                    PopupWindowWithoutFocus.Show(position, new ArrayValuePopup(startIndex, numValues, 200f, getValueString), new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Left });
                }
            }
            GUILayout.EndHorizontal();
        }

        private void OnGUIShaderPropTexture(ShaderTextureInfo t)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(15f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
            GUILayout.Label(t.name, EditorStyles.miniLabel, options);
            this.DrawShaderPropertyFlags(t.flags);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, optionArray2);
            Event current = Event.current;
            Rect rect2 = position;
            rect2.width = rect2.height;
            if ((t.value != null) && rect2.Contains(current.mousePosition))
            {
                GUI.Label(rect2, GUIContent.Temp(string.Empty, "Ctrl + Click to show preview"));
            }
            if (current.type == EventType.Repaint)
            {
                Rect rect3 = position;
                rect3.xMin += rect2.width;
                if (t.value != null)
                {
                    Texture miniThumbnail = t.value;
                    if (miniThumbnail.dimension != TextureDimension.Tex2D)
                    {
                        miniThumbnail = AssetPreview.GetMiniThumbnail(miniThumbnail);
                    }
                    EditorGUI.DrawPreviewTexture(rect2, miniThumbnail);
                }
                GUI.Label(rect3, (t.value == null) ? t.textureName : t.value.name);
            }
            else if ((current.type == EventType.MouseDown) && position.Contains(current.mousePosition))
            {
                EditorGUI.PingObjectOrShowPreviewOnClick(t.value, position);
                current.Use();
            }
            GUILayout.EndHorizontal();
        }

        private void OnGUIShaderPropVectors(ShaderVectorInfo[] vectors, int startIndex, int numValues)
        {
            <OnGUIShaderPropVectors>c__AnonStorey2 storey = new <OnGUIShaderPropVectors>c__AnonStorey2 {
                vectors = vectors
            };
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(15f);
            ShaderVectorInfo info = storey.vectors[startIndex];
            if (numValues == 1)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label(info.name, EditorStyles.miniLabel, options);
                this.DrawShaderPropertyFlags(info.flags);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label(info.value.ToString("g2"), EditorStyles.miniLabel, optionArray2);
                this.ShaderPropertyCopyValueMenu(GUILayoutUtility.GetLastRect(), info.value);
            }
            else
            {
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                GUILayout.Label($"{info.name} [{numValues}]", EditorStyles.miniLabel, optionArray3);
                this.DrawShaderPropertyFlags(info.flags);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.MinWidth(200f) };
                Rect position = GUILayoutUtility.GetRect(Styles.arrayValuePopupButton, GUI.skin.button, optionArray4);
                position.width = 25f;
                if (GUI.Button(position, Styles.arrayValuePopupButton))
                {
                    ArrayValuePopup.GetValueStringDelegate getValueString = new ArrayValuePopup.GetValueStringDelegate(storey.<>m__0);
                    PopupWindowWithoutFocus.Show(position, new ArrayValuePopup(startIndex, numValues, 200f, getValueString), new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Left });
                }
            }
            GUILayout.EndHorizontal();
        }

        private void OnPlayModeStateChanged()
        {
            this.RepaintOnLimitChange();
        }

        internal static void RepaintAll()
        {
            foreach (FrameDebuggerWindow window in s_FrameDebuggers)
            {
                window.Repaint();
            }
        }

        private void RepaintAllNeededThings()
        {
            EditorApplication.SetSceneRepaintDirty();
            base.Repaint();
        }

        private void RepaintOnLimitChange()
        {
            this.m_RepaintFrames = 4;
            this.RepaintAllNeededThings();
        }

        private void ShaderPropertyCopyValueMenu(Rect valueRect, object value)
        {
            <ShaderPropertyCopyValueMenu>c__AnonStorey0 storey = new <ShaderPropertyCopyValueMenu>c__AnonStorey0 {
                value = value
            };
            Event current = Event.current;
            if ((current.type == EventType.ContextClick) && valueRect.Contains(current.mousePosition))
            {
                current.Use();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Copy value"), false, new GenericMenu.MenuFunction(storey.<>m__0));
                menu.ShowAsContext();
            }
        }

        [UnityEditor.MenuItem("Window/Frame Debugger", false, 0x834)]
        public static FrameDebuggerWindow ShowFrameDebuggerWindow()
        {
            FrameDebuggerWindow window = EditorWindow.GetWindow(typeof(FrameDebuggerWindow)) as FrameDebuggerWindow;
            if (window != null)
            {
                window.titleContent = EditorGUIUtility.TextContent("Frame Debug");
            }
            return window;
        }

        public static Styles styles
        {
            get
            {
                Styles styles;
                if (ms_Styles != null)
                {
                    styles = ms_Styles;
                }
                else
                {
                    styles = ms_Styles = new Styles();
                }
                return styles;
            }
        }

        [CompilerGenerated]
        private sealed class <OnGUIShaderPropFloats>c__AnonStorey1
        {
            internal ShaderFloatInfo[] floats;

            internal string <>m__0(int index, bool highPrecision) => 
                this.floats[index].value.ToString(!highPrecision ? "g2" : "g7");
        }

        [CompilerGenerated]
        private sealed class <OnGUIShaderPropMatrices>c__AnonStorey3
        {
            internal ShaderMatrixInfo[] matrices;

            internal string <>m__0(int index, bool highPrecision) => 
                ('\n' + this.matrices[index].value.ToString(!highPrecision ? "g2" : "g7"));
        }

        [CompilerGenerated]
        private sealed class <OnGUIShaderPropVectors>c__AnonStorey2
        {
            internal ShaderVectorInfo[] vectors;

            internal string <>m__0(int index, bool highPrecision) => 
                this.vectors[index].value.ToString(!highPrecision ? "g2" : "g7");
        }

        [CompilerGenerated]
        private sealed class <ShaderPropertyCopyValueMenu>c__AnonStorey0
        {
            internal object value;

            internal void <>m__0()
            {
                string str = string.Empty;
                if (this.value is Vector4)
                {
                    str = ((Vector4) this.value).ToString("g7");
                }
                else if (this.value is Matrix4x4)
                {
                    str = ((Matrix4x4) this.value).ToString("g7");
                }
                else if (this.value is float)
                {
                    str = ((float) this.value).ToString("g7");
                }
                else
                {
                    str = this.value.ToString();
                }
                EditorGUIUtility.systemCopyBuffer = str;
            }
        }

        private class ArrayValuePopup : PopupWindowContent
        {
            private GetValueStringDelegate GetValueString;
            private int m_NumValues;
            private Vector2 m_ScrollPos = Vector2.zero;
            private int m_StartIndex;
            private static readonly GUIStyle m_Style = EditorStyles.miniLabel;
            private float m_WindowWidth;

            public ArrayValuePopup(int startIndex, int numValues, float windowWidth, GetValueStringDelegate getValueString)
            {
                this.m_StartIndex = startIndex;
                this.m_NumValues = numValues;
                this.m_WindowWidth = windowWidth;
                this.GetValueString = getValueString;
            }

            public override Vector2 GetWindowSize()
            {
                float num = (m_Style.lineHeight + m_Style.padding.vertical) + m_Style.margin.top;
                return new Vector2(this.m_WindowWidth, Math.Min((float) (num * this.m_NumValues), (float) 250f));
            }

            public override void OnGUI(Rect rect)
            {
                this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
                for (int i = 0; i < this.m_NumValues; i++)
                {
                    GUILayout.Label($"[{i}]	{this.GetValueString(this.m_StartIndex + i, false)}", m_Style, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndScrollView();
                Event current = Event.current;
                if ((current.type == EventType.ContextClick) && rect.Contains(current.mousePosition))
                {
                    <OnGUI>c__AnonStorey0 storey = new <OnGUI>c__AnonStorey0();
                    current.Use();
                    storey.allText = string.Empty;
                    for (int j = 0; j < this.m_NumValues; j++)
                    {
                        storey.allText = storey.allText + $"[{j}]	{this.GetValueString(this.m_StartIndex + j, true)}
";
                    }
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy value"), false, new GenericMenu.MenuFunction(storey.<>m__0));
                    menu.ShowAsContext();
                }
            }

            [CompilerGenerated]
            private sealed class <OnGUI>c__AnonStorey0
            {
                internal string allText;

                internal void <>m__0()
                {
                    EditorGUIUtility.systemCopyBuffer = this.allText;
                }
            }

            public delegate string GetValueStringDelegate(int index, bool highPrecision);
        }

        internal class Styles
        {
            public static readonly GUIContent arrayValuePopupButton = new GUIContent("...");
            public readonly string[] batchBreakCauses;
            public static readonly GUIContent causeOfNewDrawCallLabel = EditorGUIUtility.TextContent("Why this draw call can't be batched with the previous one");
            public static readonly GUIContent channelHeader = EditorGUIUtility.TextContent("Channels|Which render target color channels to show");
            public static readonly GUIContent[] channelLabels = new GUIContent[] { EditorGUIUtility.TextContent("All|Show all (RGB) color channels"), EditorGUIUtility.TextContent("R|Show red channel only"), EditorGUIUtility.TextContent("G|Show green channel only"), EditorGUIUtility.TextContent("B|Show blue channel only"), EditorGUIUtility.TextContent("A|Show alpha channel only") };
            public static readonly GUIContent copyToClipboardTooltip = EditorGUIUtility.TextContent("|Click to copy shader and keywords text to clipboard.");
            public static readonly GUIContent depthLabel = EditorGUIUtility.TextContent("Depth|Show depth buffer");
            public GUIStyle entryEven = "OL EntryBackEven";
            public GUIStyle entryOdd = "OL EntryBackOdd";
            public GUIStyle header = "OL title";
            public GUIContent[] headerContent;
            public static readonly GUIContent levelsHeader = EditorGUIUtility.TextContent("Levels|Render target display black/white intensity levels");
            public static readonly GUIContent[] mrtLabels = new GUIContent[] { EditorGUIUtility.TextContent("RT 0|Show render target #0"), EditorGUIUtility.TextContent("RT 1|Show render target #1"), EditorGUIUtility.TextContent("RT 2|Show render target #2"), EditorGUIUtility.TextContent("RT 3|Show render target #3"), EditorGUIUtility.TextContent("RT 4|Show render target #4"), EditorGUIUtility.TextContent("RT 5|Show render target #5"), EditorGUIUtility.TextContent("RT 6|Show render target #6"), EditorGUIUtility.TextContent("RT 7|Show render target #7") };
            public GUIContent nextFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.NextFrame", "|Go one frame forwards"));
            public GUIContent prevFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.PrevFrame", "|Go back one frame"));
            public GUIContent recordButton = new GUIContent(EditorGUIUtility.TextContent("Record|Record profiling information"));
            public GUIStyle rowText = "OL Label";
            public GUIStyle rowTextRight = new GUIStyle("OL Label");
            public static readonly string[] s_ColumnNames = new string[] { "#", "Type", "Vertices", "Indices" };
            public static readonly GUIContent selectShaderTooltip = EditorGUIUtility.TextContent("|Click to select shader");

            public Styles()
            {
                this.rowTextRight.alignment = TextAnchor.MiddleRight;
                this.recordButton.text = "Enable";
                this.recordButton.tooltip = "Enable Frame Debugging";
                this.prevFrame.tooltip = "Previous event";
                this.nextFrame.tooltip = "Next event";
                this.headerContent = new GUIContent[s_ColumnNames.Length];
                for (int i = 0; i < this.headerContent.Length; i++)
                {
                    this.headerContent[i] = EditorGUIUtility.TextContent(s_ColumnNames[i]);
                }
                this.batchBreakCauses = FrameDebuggerUtility.GetBatchBreakCauseStrings();
            }
        }
    }
}


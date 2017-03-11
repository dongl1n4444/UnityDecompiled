namespace UnityEditor.Sprites
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PackerWindow : SpriteUtilityWindow
    {
        [CompilerGenerated]
        private static Action<Rect> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Edge, Edge> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<IGrouping<Edge, Edge>, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<IGrouping<Edge, Edge>, Edge> <>f__am$cache3;
        private string[] m_AtlasNames = s_AtlasNamesEmpty;
        private string[] m_PageNames = s_PageNamesEmpty;
        private int m_SelectedAtlas = 0;
        private int m_SelectedPage = 0;
        private Sprite m_SelectedSprite = null;
        private static string[] s_AtlasNamesEmpty = new string[] { "Sprite atlas cache is empty" };
        private static string[] s_PageNamesEmpty = new string[0];

        private Rect DoToolbarGUI()
        {
            <DoToolbarGUI>c__AnonStorey0 storey = new <DoToolbarGUI>c__AnonStorey0 {
                $this = this
            };
            Rect position = new Rect(0f, 0f, base.position.width, 17f);
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbar.Draw(position, false, false, false, false);
            }
            bool enabled = GUI.enabled;
            GUI.enabled = this.m_AtlasNames.Length > 0;
            position = base.DoAlphaZoomToolbarGUI(position);
            GUI.enabled = enabled;
            Rect drawRect = new Rect(5f, 0f, 0f, 17f);
            position.width -= drawRect.x;
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                drawRect.width = EditorStyles.toolbarButton.CalcSize(PackerWindowStyle.packLabel).x;
                SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref position, new Action<Rect>(storey.<>m__0));
                using (new EditorGUI.DisabledScope(Packer.SelectedPolicy == Packer.kDefaultPolicy))
                {
                    drawRect.x += drawRect.width;
                    drawRect.width = EditorStyles.toolbarButton.CalcSize(PackerWindowStyle.repackLabel).x;
                    SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref position, new Action<Rect>(storey.<>m__1));
                }
            }
            float x = GUI.skin.label.CalcSize(PackerWindowStyle.viewAtlasLabel).x;
            float num2 = ((x + 100f) + 70f) + 100f;
            drawRect.x += 5f;
            position.width -= 5f;
            float width = position.width;
            using (new EditorGUI.DisabledScope(this.m_AtlasNames.Length == 0))
            {
                drawRect.x += drawRect.width;
                drawRect.width = (x / num2) * width;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = delegate (Rect adjustedDrawArea) {
                        GUI.Label(adjustedDrawArea, PackerWindowStyle.viewAtlasLabel);
                    };
                }
                SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref position, <>f__am$cache0);
                EditorGUI.BeginChangeCheck();
                drawRect.x += drawRect.width;
                drawRect.width = (100f / num2) * width;
                SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref position, new Action<Rect>(storey.<>m__2));
                if (EditorGUI.EndChangeCheck())
                {
                    this.RefreshAtlasPageList();
                    this.m_SelectedSprite = null;
                }
                EditorGUI.BeginChangeCheck();
                drawRect.x += drawRect.width;
                drawRect.width = (70f / num2) * width;
                SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref position, new Action<Rect>(storey.<>m__3));
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_SelectedSprite = null;
                }
            }
            EditorGUI.BeginChangeCheck();
            storey.policies = Packer.Policies;
            storey.selectedPolicy = Array.IndexOf<string>(storey.policies, Packer.SelectedPolicy);
            drawRect.x += drawRect.width;
            drawRect.width = (100f / num2) * width;
            SpriteUtilityWindow.DrawToolBarWidget(ref drawRect, ref position, new Action<Rect>(storey.<>m__4));
            if (EditorGUI.EndChangeCheck())
            {
                Packer.SelectedPolicy = storey.policies[storey.selectedPolicy];
            }
            return position;
        }

        protected override void DrawGizmos()
        {
            if ((this.m_SelectedSprite != null) && (base.m_Texture != null))
            {
                Vector2[] spriteUVs = UnityEditor.Sprites.SpriteUtility.GetSpriteUVs(this.m_SelectedSprite, true);
                ushort[] triangles = this.m_SelectedSprite.triangles;
                Edge[] edgeArray = this.FindUniqueEdges(triangles);
                SpriteEditorUtility.BeginLines(new Color(0.3921f, 0.5843f, 0.9294f, 0.75f));
                foreach (Edge edge in edgeArray)
                {
                    this.DrawLineUtility(spriteUVs[edge.v0], spriteUVs[edge.v1]);
                }
                SpriteEditorUtility.EndLines();
            }
        }

        private void DrawLineUtility(Vector2 from, Vector2 to)
        {
            SpriteEditorUtility.DrawLine(new Vector3((from.x * base.m_Texture.width) + (1f / base.m_Zoom), (from.y * base.m_Texture.height) + (1f / base.m_Zoom), 0f), new Vector3((to.x * base.m_Texture.width) + (1f / base.m_Zoom), (to.y * base.m_Texture.height) + (1f / base.m_Zoom), 0f));
        }

        private Edge[] FindUniqueEdges(ushort[] indices)
        {
            Edge[] edgeArray = new Edge[indices.Length];
            int num = indices.Length / 3;
            for (int i = 0; i < num; i++)
            {
                edgeArray[i * 3] = new Edge(indices[i * 3], indices[(i * 3) + 1]);
                edgeArray[(i * 3) + 1] = new Edge(indices[(i * 3) + 1], indices[(i * 3) + 2]);
                edgeArray[(i * 3) + 2] = new Edge(indices[(i * 3) + 2], indices[i * 3]);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => x;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => x.Count<Edge>() == 1;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = x => x.First<Edge>();
            }
            return Enumerable.Select<IGrouping<Edge, Edge>, Edge>(Enumerable.Where<IGrouping<Edge, Edge>>(Enumerable.GroupBy<Edge, Edge>(edgeArray, <>f__am$cache1), <>f__am$cache2), <>f__am$cache3).ToArray<Edge>();
        }

        private void OnAtlasNameListChanged()
        {
            if (this.m_AtlasNames.Length > 0)
            {
                string[] atlasNames = Packer.atlasNames;
                string str = this.m_AtlasNames[this.m_SelectedAtlas];
                string str2 = (atlasNames.Length > this.m_SelectedAtlas) ? atlasNames[this.m_SelectedAtlas] : null;
                if (str.Equals(str2))
                {
                    this.RefreshAtlasNameList();
                    this.RefreshAtlasPageList();
                    this.m_SelectedSprite = null;
                    return;
                }
            }
            this.Reset();
        }

        private void OnEnable()
        {
            base.minSize = new Vector2(400f, 256f);
            base.titleContent = PackerWindowStyle.windowTitle;
            this.Reset();
        }

        public void OnGUI()
        {
            if (this.ValidateIsPackingEnabled())
            {
                Matrix4x4 matrix = Handles.matrix;
                base.InitStyles();
                this.RefreshState();
                Rect rect = this.DoToolbarGUI();
                if (base.m_Texture != null)
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    base.m_TextureViewRect = new Rect(0f, rect.yMax, base.position.width - 16f, (base.position.height - 16f) - rect.height);
                    GUILayout.FlexibleSpace();
                    base.DoTextureGUI();
                    string text = string.Format("{1}x{2}, {0}", TextureUtil.GetTextureFormatString(base.m_Texture.format), base.m_Texture.width, base.m_Texture.height);
                    EditorGUI.DropShadowLabel(new Rect(this.m_TextureViewRect.x, this.m_TextureViewRect.y + 10f, this.m_TextureViewRect.width, 20f), text);
                    EditorGUILayout.EndHorizontal();
                    Handles.matrix = matrix;
                }
            }
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject != null)
            {
                Sprite activeObject = Selection.activeObject as Sprite;
                if (activeObject != this.m_SelectedSprite)
                {
                    if (activeObject != null)
                    {
                        <OnSelectionChange>c__AnonStorey1 storey = new <OnSelectionChange>c__AnonStorey1();
                        Packer.GetAtlasDataForSprite(activeObject, out storey.selAtlasName, out storey.selAtlasTexture);
                        int num = this.m_AtlasNames.ToList<string>().FindIndex(new Predicate<string>(storey.<>m__0));
                        if (num == -1)
                        {
                            return;
                        }
                        int num2 = Packer.GetTexturesForAtlas(storey.selAtlasName).ToList<Texture2D>().FindIndex(new Predicate<Texture2D>(storey.<>m__1));
                        if (num2 == -1)
                        {
                            return;
                        }
                        this.m_SelectedAtlas = num;
                        this.m_SelectedPage = num2;
                        this.RefreshAtlasPageList();
                    }
                    this.m_SelectedSprite = activeObject;
                    base.Repaint();
                }
            }
        }

        private void RefreshAtlasNameList()
        {
            this.m_AtlasNames = Packer.atlasNames;
            if (this.m_SelectedAtlas >= this.m_AtlasNames.Length)
            {
                this.m_SelectedAtlas = 0;
            }
        }

        private void RefreshAtlasPageList()
        {
            if (this.m_AtlasNames.Length > 0)
            {
                string atlasName = this.m_AtlasNames[this.m_SelectedAtlas];
                Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(atlasName);
                this.m_PageNames = new string[texturesForAtlas.Length];
                for (int i = 0; i < texturesForAtlas.Length; i++)
                {
                    this.m_PageNames[i] = string.Format(PackerWindowStyle.pageContentLabel.text, i + 1);
                }
            }
            else
            {
                this.m_PageNames = s_PageNamesEmpty;
            }
            if (this.m_SelectedPage >= this.m_PageNames.Length)
            {
                this.m_SelectedPage = 0;
            }
        }

        private void RefreshState()
        {
            string[] atlasNames = Packer.atlasNames;
            if (!atlasNames.SequenceEqual<string>(this.m_AtlasNames))
            {
                if (atlasNames.Length == 0)
                {
                    this.Reset();
                    return;
                }
                this.OnAtlasNameListChanged();
            }
            if (this.m_AtlasNames.Length == 0)
            {
                base.SetNewTexture(null);
            }
            else
            {
                if (this.m_SelectedAtlas >= this.m_AtlasNames.Length)
                {
                    this.m_SelectedAtlas = 0;
                }
                string atlasName = this.m_AtlasNames[this.m_SelectedAtlas];
                Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(atlasName);
                if (this.m_SelectedPage >= texturesForAtlas.Length)
                {
                    this.m_SelectedPage = 0;
                }
                base.SetNewTexture(texturesForAtlas[this.m_SelectedPage]);
                Texture2D[] alphaTexturesForAtlas = Packer.GetAlphaTexturesForAtlas(atlasName);
                Texture2D alphaTexture = (this.m_SelectedPage >= alphaTexturesForAtlas.Length) ? null : alphaTexturesForAtlas[this.m_SelectedPage];
                base.SetAlphaTextureOverride(alphaTexture);
            }
        }

        private void Reset()
        {
            this.RefreshAtlasNameList();
            this.RefreshAtlasPageList();
            this.m_SelectedAtlas = 0;
            this.m_SelectedPage = 0;
            this.m_SelectedSprite = null;
        }

        private bool ValidateIsPackingEnabled()
        {
            if (EditorSettings.spritePackerMode == SpritePackerMode.Disabled)
            {
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label(PackerWindowStyle.packingDisabledLabel, new GUILayoutOption[0]);
                if (GUILayout.Button(PackerWindowStyle.openProjectSettingButton, new GUILayoutOption[0]))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
                }
                EditorGUILayout.EndVertical();
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <DoToolbarGUI>c__AnonStorey0
        {
            internal PackerWindow $this;
            internal string[] policies;
            internal int selectedPolicy;

            internal void <>m__0(Rect adjustedDrawRect)
            {
                if (GUI.Button(adjustedDrawRect, PackerWindow.PackerWindowStyle.packLabel, EditorStyles.toolbarButton))
                {
                    Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
                    this.$this.m_SelectedSprite = null;
                    this.$this.RefreshAtlasPageList();
                    this.$this.RefreshState();
                }
            }

            internal void <>m__1(Rect adjustedDrawRect)
            {
                if (GUI.Button(adjustedDrawRect, PackerWindow.PackerWindowStyle.repackLabel, EditorStyles.toolbarButton))
                {
                    Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.ForceRegroup);
                    this.$this.m_SelectedSprite = null;
                    this.$this.RefreshAtlasPageList();
                    this.$this.RefreshState();
                }
            }

            internal void <>m__2(Rect adjustedDrawArea)
            {
                this.$this.m_SelectedAtlas = EditorGUI.Popup(adjustedDrawArea, this.$this.m_SelectedAtlas, this.$this.m_AtlasNames, EditorStyles.toolbarPopup);
            }

            internal void <>m__3(Rect adjustedDrawArea)
            {
                this.$this.m_SelectedPage = EditorGUI.Popup(adjustedDrawArea, this.$this.m_SelectedPage, this.$this.m_PageNames, EditorStyles.toolbarPopup);
            }

            internal void <>m__4(Rect adjustedDrawArea)
            {
                this.selectedPolicy = EditorGUI.Popup(adjustedDrawArea, this.selectedPolicy, this.policies, EditorStyles.toolbarPopup);
            }
        }

        [CompilerGenerated]
        private sealed class <OnSelectionChange>c__AnonStorey1
        {
            internal string selAtlasName;
            internal Texture2D selAtlasTexture;

            internal bool <>m__0(string s) => 
                (this.selAtlasName == s);

            internal bool <>m__1(Texture2D t) => 
                (this.selAtlasTexture == t);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Edge
        {
            public ushort v0;
            public ushort v1;
            public Edge(ushort a, ushort b)
            {
                this.v0 = a;
                this.v1 = b;
            }

            public override bool Equals(object obj)
            {
                PackerWindow.Edge edge = (PackerWindow.Edge) obj;
                return (((this.v0 == edge.v0) && (this.v1 == edge.v1)) || ((this.v0 == edge.v1) && (this.v1 == edge.v0)));
            }

            public override int GetHashCode()
            {
                int num = (this.v1 << 0x10) | this.v0;
                return (((this.v0 << 0x10) | this.v1) ^ num.GetHashCode());
            }
        }

        private class PackerWindowStyle
        {
            public static readonly GUIContent openProjectSettingButton = EditorGUIUtility.TextContent("Open Project Editor Settings");
            public static readonly GUIContent packingDisabledLabel = EditorGUIUtility.TextContent("Sprite packing is disabled. Enable it in Edit > Project Settings > Editor.");
            public static readonly GUIContent packLabel = EditorGUIUtility.TextContent("Pack");
            public static readonly GUIContent pageContentLabel = EditorGUIUtility.TextContent("Page {0}");
            public static readonly GUIContent repackLabel = EditorGUIUtility.TextContent("Repack");
            public static readonly GUIContent viewAtlasLabel = EditorGUIUtility.TextContent("View Atlas:");
            public static readonly GUIContent windowTitle = EditorGUIUtility.TextContent("Sprite Packer");
        }
    }
}


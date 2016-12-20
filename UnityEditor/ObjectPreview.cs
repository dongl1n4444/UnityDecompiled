namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Base Class to derive from when creating Custom Previews.</para>
    /// </summary>
    public class ObjectPreview : IPreviewable
    {
        private const int kGridSpacing = 10;
        private const int kGridTargetCount = 0x19;
        private const int kPreviewLabelHeight = 12;
        private const int kPreviewLabelPadding = 5;
        private const int kPreviewMinSize = 0x37;
        protected int m_ReferenceTargetIndex;
        protected Object[] m_Targets;
        private static Styles s_Styles;

        private static float AbsRatioDiff(float x, float y)
        {
            return Mathf.Max((float) (x / y), (float) (y / x));
        }

        /// <summary>
        /// <para>This is the first entry point for Preview Drawing.</para>
        /// </summary>
        /// <param name="previewArea">The available area to draw the preview.</param>
        public void DrawPreview(Rect previewArea)
        {
            DrawPreview(this, previewArea, this.m_Targets);
        }

        internal static void DrawPreview(IPreviewable defaultPreview, Rect previewArea, Object[] targets)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            string infoString = string.Empty;
            Event current = Event.current;
            if (targets.Length > 1)
            {
                Rect rect = new RectOffset(0x10, 0x10, 20, 0x19).Remove(previewArea);
                int num = Mathf.Max(1, Mathf.FloorToInt((rect.height + 10f) / 77f));
                int num2 = Mathf.Max(1, Mathf.FloorToInt((rect.width + 10f) / 65f));
                int num3 = num * num2;
                int minimumNr = Mathf.Min(targets.Length, 0x19);
                bool flag = true;
                int[] numArray = new int[] { num2, num };
                if (minimumNr < num3)
                {
                    numArray = GetGridDivision(rect, minimumNr, 12);
                    flag = false;
                }
                int num5 = Mathf.Min(numArray[0] * numArray[1], targets.Length);
                rect.width += 10f;
                rect.height += 10f;
                Vector2 vector = new Vector2((float) Mathf.FloorToInt((rect.width / ((float) numArray[0])) - 10f), (float) Mathf.FloorToInt((rect.height / ((float) numArray[1])) - 10f));
                float a = Mathf.Min(vector.x, vector.y - 12f);
                if (flag)
                {
                    a = Mathf.Min(a, 55f);
                }
                bool flag2 = (((current.type == EventType.MouseDown) && (current.button == 0)) && (current.clickCount == 2)) && previewArea.Contains(current.mousePosition);
                defaultPreview.ResetTarget();
                for (int i = 0; i < num5; i++)
                {
                    Rect position = new Rect(rect.x + (((i % numArray[0]) * rect.width) / ((float) numArray[0])), rect.y + (((i / numArray[0]) * rect.height) / ((float) numArray[1])), vector.x, vector.y);
                    if (flag2 && position.Contains(Event.current.mousePosition))
                    {
                        Selection.objects = new Object[] { defaultPreview.target };
                    }
                    position.height -= 12f;
                    Rect rect3 = new Rect(position.x + ((position.width - a) * 0.5f), position.y + ((position.height - a) * 0.5f), a, a);
                    GUI.BeginGroup(rect3);
                    Editor.m_AllowMultiObjectAccess = false;
                    defaultPreview.OnInteractivePreviewGUI(new Rect(0f, 0f, a, a), s_Styles.preBackgroundSolid);
                    Editor.m_AllowMultiObjectAccess = true;
                    GUI.EndGroup();
                    position.y = rect3.yMax;
                    position.height = 16f;
                    GUI.Label(position, targets[i].name, s_Styles.previewMiniLabel);
                    defaultPreview.MoveNextTarget();
                }
                defaultPreview.ResetTarget();
                if (Event.current.type == EventType.Repaint)
                {
                    infoString = string.Format("Previewing {0} of {1} Objects", num5, targets.Length);
                }
            }
            else
            {
                defaultPreview.OnInteractivePreviewGUI(previewArea, s_Styles.preBackground);
                if (Event.current.type == EventType.Repaint)
                {
                    infoString = defaultPreview.GetInfoString();
                    if (infoString != string.Empty)
                    {
                        infoString = infoString.Replace("\n", "   ");
                        infoString = string.Format("{0}\n{1}", defaultPreview.target.name, infoString);
                    }
                }
            }
            if ((Event.current.type == EventType.Repaint) && (infoString != string.Empty))
            {
                float height = s_Styles.dropShadowLabelStyle.CalcHeight(GUIContent.Temp(infoString), previewArea.width);
                EditorGUI.DropShadowLabel(new Rect(previewArea.x, (previewArea.yMax - height) - 5f, previewArea.width, height), infoString);
            }
        }

        private static int[] GetGridDivision(Rect rect, int minimumNr, int labelHeight)
        {
            float num = Mathf.Sqrt((rect.width * rect.height) / ((float) minimumNr));
            int num2 = Mathf.FloorToInt(rect.width / num);
            int num3 = Mathf.FloorToInt(rect.height / (num + labelHeight));
            while ((num2 * num3) < minimumNr)
            {
                float num4 = AbsRatioDiff(((float) (num2 + 1)) / rect.width, ((float) num3) / (rect.height - (num3 * labelHeight)));
                float num5 = AbsRatioDiff(((float) num2) / rect.width, ((float) (num3 + 1)) / (rect.height - ((num3 + 1) * labelHeight)));
                if (num4 < num5)
                {
                    num2++;
                    if ((num2 * num3) > minimumNr)
                    {
                        num3 = Mathf.CeilToInt(((float) minimumNr) / ((float) num2));
                    }
                }
                else
                {
                    num3++;
                    if ((num2 * num3) > minimumNr)
                    {
                        num2 = Mathf.CeilToInt(((float) minimumNr) / ((float) num3));
                    }
                }
            }
            return new int[] { num2, num3 };
        }

        /// <summary>
        /// <para>Implement this method to show object information on top of the object preview.</para>
        /// </summary>
        public virtual string GetInfoString()
        {
            return "";
        }

        /// <summary>
        /// <para>Override this method if you want to change the label of the Preview area.</para>
        /// </summary>
        public virtual GUIContent GetPreviewTitle()
        {
            GUIContent content = new GUIContent();
            if (this.m_Targets.Length == 1)
            {
                content.text = this.target.name;
                return content;
            }
            content.text = this.m_Targets.Length + " ";
            if (this.target is MonoBehaviour)
            {
                content.text = content.text + MonoScript.FromMonoBehaviour(this.target as MonoBehaviour).GetClass().Name;
            }
            else
            {
                content.text = content.text + ObjectNames.NicifyVariableName(ObjectNames.GetClassName(this.target));
            }
            content.text = content.text + "s";
            return content;
        }

        /// <summary>
        /// <para>Can this component be Previewed in its current state?</para>
        /// </summary>
        /// <returns>
        /// <para>True if this component can be Previewed in its current state.</para>
        /// </returns>
        public virtual bool HasPreviewGUI()
        {
            return false;
        }

        /// <summary>
        /// <para>Called when the Preview gets created with the objects being previewed.</para>
        /// </summary>
        /// <param name="targets">The objects being previewed.</param>
        public virtual void Initialize(Object[] targets)
        {
            this.m_ReferenceTargetIndex = 0;
            this.m_Targets = targets;
        }

        /// <summary>
        /// <para>Called to iterate through the targets, this will be used when previewing more than one target.</para>
        /// </summary>
        /// <returns>
        /// <para>True if there is another target available.</para>
        /// </returns>
        public virtual bool MoveNextTarget()
        {
            this.m_ReferenceTargetIndex++;
            return (this.m_ReferenceTargetIndex < (this.m_Targets.Length - 1));
        }

        /// <summary>
        /// <para>Implement to create your own interactive custom preview. Interactive custom previews are used in the preview area of the inspector and the object selector.</para>
        /// </summary>
        /// <param name="r">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public virtual void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.OnPreviewGUI(r, background);
        }

        /// <summary>
        /// <para>Implement to create your own custom preview for the preview area of the inspector, primary editor headers and the object selector.</para>
        /// </summary>
        /// <param name="r">Rectangle in which to draw the preview.</param>
        /// <param name="background">Background image.</param>
        public virtual void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(r, false, false, false, false);
            }
        }

        /// <summary>
        /// <para>Override this method if you want to show custom controls in the preview header.</para>
        /// </summary>
        public virtual void OnPreviewSettings()
        {
        }

        public virtual void ReloadPreviewInstances()
        {
        }

        /// <summary>
        /// <para>Called to Reset the target before iterating through them.</para>
        /// </summary>
        public virtual void ResetTarget()
        {
            this.m_ReferenceTargetIndex = 0;
        }

        /// <summary>
        /// <para>The object currently being previewed.</para>
        /// </summary>
        public virtual Object target
        {
            get
            {
                return this.m_Targets[this.m_ReferenceTargetIndex];
            }
        }

        private class Styles
        {
            public GUIStyle dropShadowLabelStyle = new GUIStyle("PreOverlayLabel");
            public GUIStyle preBackground = "preBackground";
            public GUIStyle preBackgroundSolid = new GUIStyle("preBackground");
            public GUIStyle previewMiniLabel = new GUIStyle(EditorStyles.whiteMiniLabel);

            public Styles()
            {
                this.preBackgroundSolid.overflow = this.preBackgroundSolid.border;
                this.previewMiniLabel.alignment = TextAnchor.UpperCenter;
            }
        }
    }
}


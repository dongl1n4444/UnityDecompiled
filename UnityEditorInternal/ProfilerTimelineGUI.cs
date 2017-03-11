namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class ProfilerTimelineGUI
    {
        private float animationTime = 1f;
        private List<GroupInfo> groups;
        private const float kGroupHeight = 20f;
        private const float kLineHeight = 16f;
        private const float kSmallWidth = 7f;
        private const float kTextFadeOutWidth = 20f;
        private const float kTextFadeStartWidth = 50f;
        private const float kTextLongWidth = 200f;
        private double lastScrollUpdate = 0.0;
        private float m_SelectedDur = 0f;
        private int m_SelectedFrameId = -1;
        private int m_SelectedInstanceId = -1;
        private string m_SelectedName = string.Empty;
        private Rect m_SelectedRect = Rect.zero;
        private int m_SelectedThreadId = 0;
        private float m_SelectedTime = 0f;
        private float m_SelectedY = 0f;
        [NonSerialized]
        private ZoomableArea m_TimeArea;
        private IProfilerWindowController m_Window;
        private static Styles ms_Styles;

        public ProfilerTimelineGUI(IProfilerWindowController window)
        {
            this.m_Window = window;
            this.groups = new List<GroupInfo>();
        }

        private void CalculateBars(Rect r, int frameIndex, float time)
        {
            ProfilerFrameDataIterator iterator = new ProfilerFrameDataIterator();
            int groupCount = iterator.GetGroupCount(frameIndex);
            float num2 = 0f;
            iterator.SetRoot(frameIndex, 0);
            int threadCount = iterator.GetThreadCount(frameIndex);
            <CalculateBars>c__AnonStorey1 storey = new <CalculateBars>c__AnonStorey1 {
                i = 0
            };
            while (storey.i < threadCount)
            {
                <CalculateBars>c__AnonStorey0 storey2 = new <CalculateBars>c__AnonStorey0 {
                    <>f__ref$1 = storey
                };
                iterator.SetRoot(frameIndex, storey.i);
                storey2.groupname = iterator.GetGroupName();
                GroupInfo item = this.groups.Find(new Predicate<GroupInfo>(storey2.<>m__0));
                if (item == null)
                {
                    item = new GroupInfo {
                        name = storey2.groupname,
                        height = 20f,
                        expanded = false,
                        threads = new List<ThreadInfo>()
                    };
                    this.groups.Add(item);
                    if ((storey2.groupname == "") || (storey2.groupname == "Unity Job System"))
                    {
                        item.expanded = true;
                    }
                }
                ThreadInfo info2 = item.threads.Find(new Predicate<ThreadInfo>(storey2.<>m__1));
                if (info2 == null)
                {
                    info2 = new ThreadInfo {
                        name = iterator.GetThreadName(),
                        height = 0f
                    };
                    info2.weight = info2.desiredWeight = !item.expanded ? ((float) 0) : ((float) 1);
                    info2.threadIndex = storey.i;
                    item.threads.Add(info2);
                }
                if (info2.weight != info2.desiredWeight)
                {
                    info2.weight = (info2.desiredWeight * time) + ((1f - info2.desiredWeight) * (1f - time));
                }
                num2 += info2.weight;
                storey.i++;
            }
            float num5 = 20f * groupCount;
            float num6 = r.height - num5;
            float num7 = num6 / (num2 + 2f);
            foreach (GroupInfo info3 in this.groups)
            {
                foreach (ThreadInfo info4 in info3.threads)
                {
                    info4.height = num7 * info4.weight;
                }
            }
            this.groups[0].expanded = true;
            this.groups[0].height = 0f;
            this.groups[0].threads[0].height = 3f * num7;
        }

        private void ClearSelection()
        {
            this.m_Window.ClearSelectedPropertyPath();
            this.m_SelectedFrameId = -1;
            this.m_SelectedThreadId = 0;
            this.m_SelectedInstanceId = -1;
            this.m_SelectedTime = 0f;
            this.m_SelectedDur = 0f;
            this.m_SelectedY = 0f;
            this.m_SelectedName = string.Empty;
            this.m_SelectedRect = Rect.zero;
            Event.current.Use();
        }

        public void DoGUI(int frameIndex, float width, float ypos, float height, bool detailView)
        {
            Rect position = new Rect(0f, ypos - 1f, width, height + 1f);
            float num = 169f;
            if (Event.current.type == EventType.Repaint)
            {
                styles.profilerGraphBackground.Draw(position, false, false, false, false);
                EditorStyles.toolbar.Draw(new Rect(0f, (ypos + height) - 15f, num, 15f), false, false, false, false);
            }
            bool flag = false;
            if (this.m_TimeArea == null)
            {
                flag = true;
                this.m_TimeArea = new ZoomableArea();
                this.m_TimeArea.hRangeLocked = false;
                this.m_TimeArea.vRangeLocked = true;
                this.m_TimeArea.hSlider = true;
                this.m_TimeArea.vSlider = false;
                this.m_TimeArea.scaleWithWindow = true;
                this.m_TimeArea.rect = new Rect((position.x + num) - 1f, position.y, position.width - num, position.height);
                this.m_TimeArea.margin = 10f;
            }
            ProfilerFrameDataIterator iterator = new ProfilerFrameDataIterator();
            iterator.SetRoot(frameIndex, 0);
            this.m_TimeArea.hBaseRangeMin = 0f;
            this.m_TimeArea.hBaseRangeMax = iterator.frameTimeMS;
            if (flag)
            {
                this.PerformFrameSelected(iterator.frameTimeMS);
            }
            this.m_TimeArea.rect = new Rect(position.x + num, position.y, position.width - num, position.height);
            this.m_TimeArea.BeginViewGUI();
            this.m_TimeArea.EndViewGUI();
            position = this.m_TimeArea.drawRect;
            this.CalculateBars(position, frameIndex, this.animationTime);
            this.DrawBars(position, frameIndex);
            GUI.BeginClip(this.m_TimeArea.drawRect);
            position.x = 0f;
            position.y = 0f;
            bool enabled = GUI.enabled;
            GUI.enabled = false;
            ProfilerFrameDataIterator iterator2 = new ProfilerFrameDataIterator();
            int threadCount = iterator2.GetThreadCount(frameIndex);
            int previousFrameIndex = ProfilerDriver.GetPreviousFrameIndex(frameIndex);
            if (previousFrameIndex != -1)
            {
                iterator2.SetRoot(previousFrameIndex, 0);
                this.DoProfilerFrame(previousFrameIndex, position, true, threadCount, -iterator2.frameTimeMS, detailView);
            }
            int nextFrameIndex = ProfilerDriver.GetNextFrameIndex(frameIndex);
            if (nextFrameIndex != -1)
            {
                iterator2.SetRoot(frameIndex, 0);
                this.DoProfilerFrame(nextFrameIndex, position, true, threadCount, iterator2.frameTimeMS, detailView);
            }
            GUI.enabled = enabled;
            threadCount = 0;
            this.DoProfilerFrame(frameIndex, position, false, threadCount, 0f, detailView);
            GUI.EndClip();
        }

        private void DoProfilerFrame(int frameIndex, Rect fullRect, bool ghost, int threadCount, float offset, bool detailView)
        {
            ProfilerFrameDataIterator iter = new ProfilerFrameDataIterator();
            int num = iter.GetThreadCount(frameIndex);
            if (!ghost || (num == threadCount))
            {
                iter.SetRoot(frameIndex, 0);
                if (!ghost)
                {
                    this.DrawGrid(fullRect, threadCount, iter.frameTimeMS);
                    this.HandleFrameSelected(iter.frameTimeMS);
                }
                float y = fullRect.y;
                foreach (GroupInfo info in this.groups)
                {
                    Rect r = fullRect;
                    bool expanded = info.expanded;
                    if (expanded)
                    {
                        y += info.height;
                    }
                    float num3 = y;
                    int count = info.threads.Count;
                    foreach (ThreadInfo info2 in info.threads)
                    {
                        iter.SetRoot(frameIndex, info2.threadIndex);
                        r.y = y;
                        r.height = !expanded ? Math.Max((float) ((info.height / ((float) count)) - 1f), (float) 2f) : info2.height;
                        if (detailView)
                        {
                            this.DrawProfilingDataDetailNative(r, frameIndex, info2.threadIndex, offset);
                        }
                        else
                        {
                            this.DrawProfilingData(iter, r, frameIndex, info2.threadIndex, offset, ghost, expanded);
                        }
                        y += r.height;
                    }
                    if (!expanded)
                    {
                        y = num3 + info.height;
                    }
                }
                if (((this.m_SelectedName.Length > 0) && (this.m_SelectedFrameId == frameIndex)) && !ghost)
                {
                    GUIContent content = new GUIContent(string.Format((this.m_SelectedDur < 1.0) ? "{0}\n{1:f3}ms" : "{0}\n{1:f2}ms", this.m_SelectedName, this.m_SelectedDur));
                    GUIStyle tooltip = styles.tooltip;
                    Vector2 vector = tooltip.CalcSize(content);
                    float x = this.m_TimeArea.TimeToPixel(this.m_SelectedTime + (this.m_SelectedDur * 0.5f), this.m_SelectedRect);
                    if (x > this.m_SelectedRect.xMax)
                    {
                        x = this.m_SelectedRect.xMax - 20f;
                    }
                    if (x < this.m_SelectedRect.x)
                    {
                        x = this.m_SelectedRect.x + 20f;
                    }
                    Rect position = new Rect(x - 32f, this.m_SelectedY, 50f, 7f);
                    Rect rect3 = new Rect(x, this.m_SelectedY + 6f, vector.x, vector.y);
                    if (rect3.xMax > (this.m_SelectedRect.xMax + 20f))
                    {
                        rect3.x = (this.m_SelectedRect.xMax - rect3.width) + 20f;
                    }
                    if (rect3.xMin < (this.m_SelectedRect.xMin + 30f))
                    {
                        rect3.x = this.m_SelectedRect.xMin + 30f;
                    }
                    GUI.Label(position, GUIContent.none, styles.tooltipArrow);
                    GUI.Label(rect3, content, tooltip);
                }
            }
        }

        private bool DrawBar(Rect r, float y, float height, string name, bool group, bool expanded, bool indent)
        {
            Rect position = new Rect(r.x - 170f, y, 170f, height);
            Rect rect2 = new Rect(r.x, y, r.width, height);
            if (Event.current.type == EventType.Repaint)
            {
                styles.rightPane.Draw(rect2, false, false, false, false);
                bool flag = height < 10f;
                bool flag2 = height < 25f;
                GUIContent content = (!group && !flag) ? GUIContent.Temp(name) : GUIContent.none;
                if (flag2)
                {
                    RectOffset padding = styles.leftPane.padding;
                    padding.top -= ((int) (25f - height)) / 2;
                }
                if (indent)
                {
                    RectOffset offset2 = styles.leftPane.padding;
                    offset2.left += 10;
                }
                styles.leftPane.Draw(position, content, false, false, false, false);
                if (indent)
                {
                    RectOffset offset3 = styles.leftPane.padding;
                    offset3.left -= 10;
                }
                if (flag2)
                {
                    RectOffset offset4 = styles.leftPane.padding;
                    offset4.top += ((int) (25f - height)) / 2;
                }
            }
            if (group)
            {
                position.width--;
                position.xMin++;
                return GUI.Toggle(position, expanded, GUIContent.Temp(name), styles.foldout);
            }
            return false;
        }

        private void DrawBars(Rect r, int frameIndex)
        {
            float y = r.y;
            foreach (GroupInfo info in this.groups)
            {
                bool flag = info.name == "";
                if (!flag)
                {
                    float height = info.height;
                    bool expanded = info.expanded;
                    info.expanded = this.DrawBar(r, y, height, info.name, true, expanded, false);
                    if (info.expanded != expanded)
                    {
                        this.animationTime = 0f;
                        this.lastScrollUpdate = EditorApplication.timeSinceStartup;
                        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedFoldout));
                        foreach (ThreadInfo info2 in info.threads)
                        {
                            info2.desiredWeight = !info.expanded ? 0f : 1f;
                        }
                    }
                    y += height;
                }
                foreach (ThreadInfo info3 in info.threads)
                {
                    float num3 = info3.height;
                    if (num3 != 0f)
                    {
                        this.DrawBar(r, y, num3, info3.name, false, true, !flag);
                    }
                    y += num3;
                }
            }
        }

        private void DrawGrid(Rect r, int threadCount, float frameTime)
        {
            if (Event.current.type == EventType.Repaint)
            {
                float num;
                float num3;
                float num2 = 16.66667f;
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                GL.Color(new Color(1f, 1f, 1f, 0.2f));
                for (num3 = num2; num3 <= frameTime; num3 += num2)
                {
                    num = this.m_TimeArea.TimeToPixel(num3, r);
                    GL.Vertex3(num, r.y, 0f);
                    GL.Vertex3(num, r.y + r.height, 0f);
                }
                GL.Color(new Color(1f, 1f, 1f, 0.8f));
                num = this.m_TimeArea.TimeToPixel(0f, r);
                GL.Vertex3(num, r.y, 0f);
                GL.Vertex3(num, r.y + r.height, 0f);
                num = this.m_TimeArea.TimeToPixel(frameTime, r);
                GL.Vertex3(num, r.y, 0f);
                GL.Vertex3(num, r.y + r.height, 0f);
                GL.End();
                GUI.color = new Color(1f, 1f, 1f, 0.4f);
                for (num3 = 0f; num3 <= frameTime; num3 += num2)
                {
                    Chart.DoLabel(this.m_TimeArea.TimeToPixel(num3, r) + 2f, r.yMax - 12f, $"{num3:f1}ms", 0f);
                }
                GUI.color = new Color(1f, 1f, 1f, 1f);
                num3 = frameTime;
                Chart.DoLabel(this.m_TimeArea.TimeToPixel(num3, r) + 2f, r.yMax - 12f, $"{num3:f1}ms ({1000f / num3:f0}FPS)", 0f);
            }
        }

        private void DrawProfilingData(ProfilerFrameDataIterator iter, Rect r, int frameIndex, int threadIndex, float timeOffset, bool ghost, bool includeSubSamples)
        {
            float num = !ghost ? 7f : 21f;
            string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            Color[] colors = ProfilerColors.colors;
            bool flag = false;
            float num2 = -1f;
            float num3 = -1f;
            float y = -1f;
            int size = 0;
            float num6 = !includeSubSamples ? r.height : 16f;
            float num7 = !includeSubSamples ? ((float) 0) : ((float) 1);
            float height = num6 - (2f * num7);
            r.height -= num7;
            GUI.BeginGroup(r);
            float num9 = r.y;
            float num10 = 0f;
            r.y = num10;
            r.x = num10;
            bool singleClick = (Event.current.clickCount == 1) && (Event.current.type == EventType.MouseDown);
            bool doubleClick = (Event.current.clickCount == 2) && (Event.current.type == EventType.MouseDown);
            Rect shownArea = this.m_TimeArea.shownArea;
            float rectWidthDivShownWidth = r.width / shownArea.width;
            float x = r.x;
            float shownX = shownArea.x;
            bool enterChildren = true;
            while (iter.Next(enterChildren))
            {
                enterChildren = includeSubSamples;
                float time = iter.startTimeMS + timeOffset;
                float durationMS = iter.durationMS;
                float num16 = Mathf.Max(durationMS, 0.0003f);
                float num17 = TimeToPixelCached(time, rectWidthDivShownWidth, shownX, x);
                float num18 = TimeToPixelCached(time + num16, rectWidthDivShownWidth, shownX, x) - 1f;
                float width = num18 - num17;
                if ((num17 > (r.x + r.width)) || (num18 < r.x))
                {
                    enterChildren = false;
                }
                else
                {
                    float num20 = iter.depth - 1;
                    float num21 = r.y + (num20 * num6);
                    if (flag)
                    {
                        bool flag5 = false;
                        if (width >= num)
                        {
                            flag5 = true;
                        }
                        if (y != num21)
                        {
                            flag5 = true;
                        }
                        if ((num17 - num3) > 6f)
                        {
                            flag5 = true;
                        }
                        if (flag5)
                        {
                            this.DrawSmallGroup(num2, num3, y, height, size);
                            flag = false;
                        }
                    }
                    if (width < num)
                    {
                        enterChildren = false;
                        if (!flag)
                        {
                            flag = true;
                            y = num21;
                            num2 = num17;
                            size = 0;
                        }
                        num3 = num18;
                        size++;
                        continue;
                    }
                    int instanceId = iter.instanceId;
                    string path = iter.path;
                    bool flag6 = (path == selectedPropertyPath) && !ghost;
                    if (this.m_SelectedInstanceId >= 0)
                    {
                        flag6 &= instanceId == this.m_SelectedInstanceId;
                    }
                    flag6 &= threadIndex == this.m_SelectedThreadId;
                    Color white = Color.white;
                    Color color4 = colors[iter.group % colors.Length];
                    color4.a = !flag6 ? 0.75f : 1f;
                    if (ghost)
                    {
                        color4.a = 0.4f;
                        white.a = 0.5f;
                    }
                    string name = iter.name;
                    if ((width < 20f) || !includeSubSamples)
                    {
                        name = string.Empty;
                    }
                    else
                    {
                        if ((width < 50f) && !flag6)
                        {
                            white.a *= (width - 20f) / 30f;
                        }
                        if (width > 200f)
                        {
                            name = name + $" ({durationMS:f2}ms)";
                        }
                    }
                    GUI.color = color4;
                    GUI.contentColor = white;
                    Rect position = new Rect(num17, num21, width, height);
                    GUI.Label(position, name, styles.bar);
                    if ((singleClick || doubleClick) && (position.Contains(Event.current.mousePosition) && includeSubSamples))
                    {
                        this.m_Window.SetSelectedPropertyPath(path);
                        this.m_SelectedFrameId = frameIndex;
                        this.m_SelectedThreadId = threadIndex;
                        this.m_SelectedInstanceId = instanceId;
                        this.m_SelectedName = iter.name;
                        this.m_SelectedTime = time;
                        this.m_SelectedDur = durationMS;
                        this.m_SelectedRect = r;
                        this.m_SelectedY = (num9 + num21) + num6;
                        this.UpdateSelectedObject(singleClick, doubleClick);
                        Event.current.Use();
                    }
                    flag = false;
                }
            }
            if (flag)
            {
                this.DrawSmallGroup(num2, num3, y, height, size);
            }
            GUI.color = color;
            GUI.contentColor = contentColor;
            if ((Event.current.type == EventType.MouseDown) && r.Contains(Event.current.mousePosition))
            {
                this.ClearSelection();
            }
            GUI.EndGroup();
        }

        private void DrawProfilingDataDetailNative(Rect r, int frameIndex, int threadIndex, float timeOffset)
        {
            bool singleClick = (Event.current.clickCount == 1) && (Event.current.type == EventType.MouseDown);
            bool doubleClick = (Event.current.clickCount == 2) && (Event.current.type == EventType.MouseDown);
            bool flag3 = r.Contains(Event.current.mousePosition);
            GUI.BeginGroup(r);
            ProfilingDataDrawNativeInfo d = new ProfilingDataDrawNativeInfo();
            d.Reset();
            d.trySelect = (!singleClick && !doubleClick) ? 0 : 1;
            d.frameIndex = frameIndex;
            d.threadIndex = threadIndex;
            d.timeOffset = timeOffset;
            d.threadRect = r;
            d.shownAreaRect = this.m_TimeArea.shownArea;
            d.mousePos = Event.current.mousePosition;
            d.profilerColors = ProfilerColors.colors;
            d.nativeAllocationColor = ProfilerColors.nativeAllocation;
            d.ghostAlpha = 0.3f;
            d.nonSelectedAlpha = 0.75f;
            d.guiStyle = styles.bar.m_Ptr;
            d.lineHeight = 16f;
            d.textFadeOutWidth = 20f;
            d.textFadeStartWidth = 50f;
            ProfilerDraw.DrawNative(ref d);
            if (singleClick || doubleClick)
            {
                if (d.out_SelectedPath.Length > 0)
                {
                    this.m_Window.SetSelectedPropertyPath(d.out_SelectedPath);
                    this.m_SelectedFrameId = frameIndex;
                    this.m_SelectedThreadId = threadIndex;
                    this.m_SelectedInstanceId = d.out_SelectedInstanceId;
                    this.m_SelectedTime = d.out_SelectedTime;
                    this.m_SelectedDur = d.out_SelectedDur;
                    this.m_SelectedY = r.y + d.out_SelectedY;
                    this.m_SelectedName = d.out_SelectedName;
                    this.m_SelectedRect = r;
                    this.UpdateSelectedObject(singleClick, doubleClick);
                    Event.current.Use();
                }
                else if (flag3)
                {
                    this.ClearSelection();
                }
            }
            GUI.EndGroup();
        }

        private void DrawSmallGroup(float x1, float x2, float y, float height, int size)
        {
            if ((x2 - x1) >= 1f)
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                GUI.contentColor = Color.white;
                GUIContent none = GUIContent.none;
                if ((x2 - x1) > 20f)
                {
                    none = new GUIContent(size + " items");
                }
                GUI.Label(new Rect(x1, y, x2 - x1, height), none, styles.bar);
            }
        }

        private void HandleFrameSelected(float frameMS)
        {
            Event current = Event.current;
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && (current.commandName == "FrameSelected"))
            {
                if (current.type == EventType.ExecuteCommand)
                {
                    this.PerformFrameSelected(frameMS);
                }
                current.Use();
            }
        }

        private void PerformFrameSelected(float frameMS)
        {
            float selectedTime = this.m_SelectedTime;
            float selectedDur = this.m_SelectedDur;
            if ((this.m_SelectedInstanceId < 0) || (selectedDur <= 0f))
            {
                selectedTime = 0f;
                selectedDur = frameMS;
            }
            this.m_TimeArea.SetShownHRangeInsideMargins(selectedTime - (selectedDur * 0.2f), selectedTime + (selectedDur * 1.2f));
        }

        private static float TimeToPixelCached(float time, float rectWidthDivShownWidth, float shownX, float rectX) => 
            (((time - shownX) * rectWidthDivShownWidth) + rectX);

        private void UpdateAnimatedFoldout()
        {
            double num = EditorApplication.timeSinceStartup - this.lastScrollUpdate;
            this.animationTime = Math.Min((float) 1f, (float) (this.animationTime + ((float) num)));
            this.m_Window.Repaint();
            if (this.animationTime == 1f)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedFoldout));
            }
        }

        private void UpdateSelectedObject(bool singleClick, bool doubleClick)
        {
            UnityEngine.Object gameObject = EditorUtility.InstanceIDToObject(this.m_SelectedInstanceId);
            if (gameObject is Component)
            {
                gameObject = ((Component) gameObject).gameObject;
            }
            if (gameObject != null)
            {
                if (singleClick)
                {
                    EditorGUIUtility.PingObject(gameObject.GetInstanceID());
                }
                else if (doubleClick)
                {
                    Selection.objects = new List<UnityEngine.Object> { gameObject }.ToArray();
                }
            }
        }

        private static Styles styles
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
        private sealed class <CalculateBars>c__AnonStorey0
        {
            internal ProfilerTimelineGUI.<CalculateBars>c__AnonStorey1 <>f__ref$1;
            internal string groupname;

            internal bool <>m__0(ProfilerTimelineGUI.GroupInfo g) => 
                (g.name == this.groupname);

            internal bool <>m__1(ProfilerTimelineGUI.ThreadInfo t) => 
                (t.threadIndex == this.<>f__ref$1.i);
        }

        [CompilerGenerated]
        private sealed class <CalculateBars>c__AnonStorey1
        {
            internal int i;
        }

        internal class GroupInfo
        {
            public bool expanded;
            public float height;
            public string name;
            public List<ProfilerTimelineGUI.ThreadInfo> threads;
        }

        internal class Styles
        {
            public GUIStyle background = "OL Box";
            public GUIStyle bar = "ProfilerTimelineBar";
            public GUIStyle foldout = "ProfilerTimelineFoldout";
            public GUIStyle leftPane = "ProfilerTimelineLeftPane";
            public GUIStyle profilerGraphBackground = new GUIStyle("ProfilerScrollviewBackground");
            public GUIStyle rightPane = "ProfilerRightPane";
            public GUIStyle tooltip = "AnimationEventTooltip";
            public GUIStyle tooltipArrow = "AnimationEventTooltipArrow";

            internal Styles()
            {
                Texture2D whiteTexture = EditorGUIUtility.whiteTexture;
                this.bar.active.background = whiteTexture;
                this.bar.hover.background = whiteTexture;
                this.bar.normal.background = whiteTexture;
                Color black = Color.black;
                this.bar.active.textColor = black;
                this.bar.hover.textColor = black;
                this.bar.normal.textColor = black;
                this.profilerGraphBackground.overflow.left = -169;
                this.leftPane.padding.left = 15;
            }
        }

        internal class ThreadInfo
        {
            public float desiredWeight;
            public float height;
            public string name;
            public int threadIndex;
            public float weight;
        }
    }
}


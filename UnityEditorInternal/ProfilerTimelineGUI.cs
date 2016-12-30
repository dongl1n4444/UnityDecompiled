namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class ProfilerTimelineGUI
    {
        [CompilerGenerated]
        private static Func<GroupInfo, bool> <>f__am$cache0;
        private float animationTime = 1f;
        private List<GroupInfo> groups;
        private const float kGroupHeight = 20f;
        private const float kLineHeight = 16f;
        private const float kSmallWidth = 7f;
        private const float kTextFadeOutWidth = 20f;
        private const float kTextFadeStartWidth = 50f;
        private const float kTextLongWidth = 200f;
        private double lastScrollUpdate = 0.0;
        private string m_LocalizedString_Instances;
        private string m_LocalizedString_Total;
        private SelectedEntryInfo m_SelectedEntry = new SelectedEntryInfo();
        private float m_SelectedThreadY = 0f;
        [NonSerialized]
        private ZoomableArea m_TimeArea;
        private IProfilerWindowController m_Window;
        private static Styles ms_Styles;

        public ProfilerTimelineGUI(IProfilerWindowController window)
        {
            this.m_Window = window;
            GroupInfo[] collection = new GroupInfo[3];
            GroupInfo info = new GroupInfo {
                name = "",
                height = 20f,
                expanded = true,
                threads = new List<ThreadInfo>()
            };
            collection[0] = info;
            info = new GroupInfo {
                name = "Unity Job System",
                height = 20f,
                expanded = true,
                threads = new List<ThreadInfo>()
            };
            collection[1] = info;
            info = new GroupInfo {
                name = "Loading",
                height = 20f,
                expanded = false,
                threads = new List<ThreadInfo>()
            };
            collection[2] = info;
            this.groups = new List<GroupInfo>(collection);
            this.m_LocalizedString_Total = LocalizationDatabase.GetLocalizedString("Total");
            this.m_LocalizedString_Instances = LocalizationDatabase.GetLocalizedString("Instances");
        }

        private void CalculateBars(Rect r, int frameIndex, float time)
        {
            ProfilerFrameDataIterator iterator = new ProfilerFrameDataIterator();
            float num = 0f;
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
                num += info2.weight;
                storey.i++;
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = group => group.threads.Count > 1;
            }
            int num4 = Enumerable.Count<GroupInfo>(this.groups, <>f__am$cache0);
            float num5 = 20f * num4;
            float num6 = r.height - num5;
            float num7 = num6 / (num + 1f);
            foreach (GroupInfo info3 in this.groups)
            {
                foreach (ThreadInfo info4 in info3.threads)
                {
                    info4.height = num7 * info4.weight;
                }
            }
            this.groups[0].expanded = true;
            this.groups[0].height = 0f;
            this.groups[0].threads[0].height = 2f * num7;
        }

        private void ClearSelection()
        {
            this.m_Window.ClearSelectedPropertyPath();
            this.m_SelectedEntry.Reset();
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
            if (flag)
            {
                NativeProfilerTimeline_InitializeArgs args = new NativeProfilerTimeline_InitializeArgs();
                args.Reset();
                args.profilerColors = ProfilerColors.colors;
                args.nativeAllocationColor = ProfilerColors.nativeAllocation;
                args.ghostAlpha = 0.3f;
                args.nonSelectedAlpha = 0.75f;
                args.guiStyle = styles.bar.m_Ptr;
                args.lineHeight = 16f;
                args.textFadeOutWidth = 20f;
                args.textFadeStartWidth = 50f;
                NativeProfilerTimeline.Initialize(ref args);
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
            this.DoSelectionTooltip(frameIndex, this.m_TimeArea.drawRect, detailView);
        }

        private void DoNativeProfilerTimeline(Rect r, int frameIndex, int threadIndex, float timeOffset, bool ghost)
        {
            Rect position = r;
            float topMargin = Math.Min((float) (position.height * 0.25f), (float) 1f);
            float num2 = topMargin + 1f;
            position.y += topMargin;
            position.height -= num2;
            GUI.BeginGroup(position);
            Rect threadRect = position;
            threadRect.x = 0f;
            threadRect.y = 0f;
            if (Event.current.type == EventType.Repaint)
            {
                this.DrawNativeProfilerTimeline(threadRect, frameIndex, threadIndex, timeOffset, ghost);
            }
            else if ((Event.current.type == EventType.MouseDown) && !ghost)
            {
                this.HandleNativeProfilerTimelineInput(threadRect, frameIndex, threadIndex, timeOffset, topMargin);
            }
            GUI.EndGroup();
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
                            this.DoNativeProfilerTimeline(r, frameIndex, info2.threadIndex, offset, ghost);
                        }
                        else
                        {
                            this.DrawProfilingData(iter, r, frameIndex, info2.threadIndex, offset, ghost, expanded);
                        }
                        if ((this.m_SelectedEntry.IsValid() && (this.m_SelectedEntry.frameId == frameIndex)) && (this.m_SelectedEntry.threadId == info2.threadIndex))
                        {
                            this.m_SelectedThreadY = y;
                        }
                        y += r.height;
                    }
                    if (!expanded)
                    {
                        y = num3 + info.height;
                    }
                }
            }
        }

        private void DoSelectionTooltip(int frameIndex, Rect fullRect, bool detailView)
        {
            if (this.m_SelectedEntry.IsValid() && (this.m_SelectedEntry.frameId == frameIndex))
            {
                string str = string.Format((this.m_SelectedEntry.duration < 1.0) ? "{0:f3}ms" : "{0:f2}ms", this.m_SelectedEntry.duration);
                StringBuilder builder = new StringBuilder();
                builder.Append($"{this.m_SelectedEntry.name}
{str}");
                if (this.m_SelectedEntry.instanceCount > 1)
                {
                    string str2 = string.Format((this.m_SelectedEntry.totalDuration < 1.0) ? "{0:f3}ms" : "{0:f2}ms", this.m_SelectedEntry.totalDuration);
                    builder.Append($"
{this.m_LocalizedString_Total}: {str2} ({this.m_SelectedEntry.instanceCount} {this.m_LocalizedString_Instances})");
                }
                if (this.m_SelectedEntry.metaData.Length > 0)
                {
                    builder.Append($"
{this.m_SelectedEntry.metaData}");
                }
                if (this.m_SelectedEntry.allocationInfo.Length > 0)
                {
                    builder.Append($"
{this.m_SelectedEntry.allocationInfo}");
                }
                float y = (fullRect.y + this.m_SelectedThreadY) + this.m_SelectedEntry.relativeYPos;
                GUIContent content = new GUIContent(builder.ToString());
                GUIStyle tooltip = styles.tooltip;
                Vector2 vector = tooltip.CalcSize(content);
                float x = this.m_TimeArea.TimeToPixel(this.m_SelectedEntry.time + (this.m_SelectedEntry.duration * 0.5f), fullRect);
                Rect position = new Rect(x - 32f, y, 64f, 6f);
                Rect rect2 = new Rect(x, y + 6f, vector.x, vector.y);
                if (rect2.xMax > (fullRect.xMax + 16f))
                {
                    rect2.x = (fullRect.xMax - rect2.width) + 16f;
                }
                if (position.xMax > (fullRect.xMax + 20f))
                {
                    position.x = (fullRect.xMax - position.width) + 20f;
                }
                if (rect2.xMin < (fullRect.xMin + 30f))
                {
                    rect2.x = fullRect.xMin + 30f;
                }
                if (position.xMin < (fullRect.xMin - 20f))
                {
                    position.x = fullRect.xMin - 20f;
                }
                float num3 = (16f + rect2.height) + (2f * position.height);
                bool flag = (((y + vector.y) + 6f) > fullRect.yMax) && ((rect2.y - num3) > 0f);
                if (flag)
                {
                    rect2.y -= num3;
                    position.y -= 16f + (2f * position.height);
                }
                GUI.BeginClip(position);
                Matrix4x4 matrix = GUI.matrix;
                if (flag)
                {
                    GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(position.width * 0.5f, position.height));
                }
                GUI.Label(new Rect(0f, 0f, position.width, position.height), GUIContent.none, styles.tooltipArrow);
                GUI.matrix = matrix;
                GUI.EndClip();
                GUI.Label(rect2, content, tooltip);
            }
        }

        private bool DrawBar(Rect r, float y, float height, string name, bool group, bool expanded, bool indent)
        {
            Rect position = new Rect(r.x - 170f, y, 170f, height);
            Rect rect2 = new Rect(r.x, y, r.width, height);
            if (Event.current.type == EventType.Repaint)
            {
                styles.rightPane.Draw(rect2, false, false, false, false);
                bool flag = height < 25f;
                GUIContent content = GUIContent.Temp(name);
                if (flag)
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
                if (flag)
                {
                    RectOffset offset4 = styles.leftPane.padding;
                    offset4.top += ((int) (25f - height)) / 2;
                }
            }
            if (group)
            {
                position.width--;
                position.xMin++;
                return GUI.Toggle(position, expanded, GUIContent.none, styles.foldout);
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

        private void DrawNativeProfilerTimeline(Rect threadRect, int frameIndex, int threadIndex, float timeOffset, bool ghost)
        {
            bool flag = (this.m_SelectedEntry.threadId == threadIndex) && (this.m_SelectedEntry.frameId == frameIndex);
            NativeProfilerTimeline_DrawArgs args = new NativeProfilerTimeline_DrawArgs();
            args.Reset();
            args.frameIndex = frameIndex;
            args.threadIndex = threadIndex;
            args.timeOffset = timeOffset;
            args.threadRect = threadRect;
            args.shownAreaRect = this.m_TimeArea.shownArea;
            args.selectedEntryIndex = !flag ? -1 : this.m_SelectedEntry.nativeIndex;
            args.mousedOverEntryIndex = -1;
            NativeProfilerTimeline.Draw(ref args);
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
            float num9 = 0f;
            r.y = num9;
            r.x = num9;
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
                float num15 = Mathf.Max(durationMS, 0.0003f);
                float num16 = TimeToPixelCached(time, rectWidthDivShownWidth, shownX, x);
                float num17 = TimeToPixelCached(time + num15, rectWidthDivShownWidth, shownX, x) - 1f;
                float width = num17 - num16;
                if ((num16 > (r.x + r.width)) || (num17 < r.x))
                {
                    enterChildren = false;
                }
                else
                {
                    float num19 = iter.depth - 1;
                    float num20 = r.y + (num19 * num6);
                    if (flag)
                    {
                        bool flag5 = false;
                        if (width >= num)
                        {
                            flag5 = true;
                        }
                        if (y != num20)
                        {
                            flag5 = true;
                        }
                        if ((num16 - num3) > 6f)
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
                            y = num20;
                            num2 = num16;
                            size = 0;
                        }
                        num3 = num17;
                        size++;
                        continue;
                    }
                    int instanceId = iter.instanceId;
                    string path = iter.path;
                    bool flag6 = (path == selectedPropertyPath) && !ghost;
                    if (this.m_SelectedEntry.instanceId >= 0)
                    {
                        flag6 &= instanceId == this.m_SelectedEntry.instanceId;
                    }
                    flag6 &= threadIndex == this.m_SelectedEntry.threadId;
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
                    Rect position = new Rect(num16, num20, width, height);
                    GUI.Label(position, name, styles.bar);
                    if ((singleClick || doubleClick) && (position.Contains(Event.current.mousePosition) && includeSubSamples))
                    {
                        this.m_Window.SetSelectedPropertyPath(path);
                        this.m_SelectedEntry.Reset();
                        this.m_SelectedEntry.frameId = frameIndex;
                        this.m_SelectedEntry.threadId = threadIndex;
                        this.m_SelectedEntry.instanceId = instanceId;
                        this.m_SelectedEntry.name = iter.name;
                        if (iter.extraTooltipInfo != null)
                        {
                            this.m_SelectedEntry.metaData = iter.extraTooltipInfo;
                        }
                        this.m_SelectedEntry.time = time;
                        this.m_SelectedEntry.duration = durationMS;
                        this.m_SelectedEntry.relativeYPos = num20 + num6;
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
                Event.current.Use();
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

        private void HandleNativeProfilerTimelineInput(Rect threadRect, int frameIndex, int threadIndex, float timeOffset, float topMargin)
        {
            if (threadRect.Contains(Event.current.mousePosition))
            {
                bool singleClick = (Event.current.clickCount == 1) && (Event.current.type == EventType.MouseDown);
                bool doubleClick = (Event.current.clickCount == 2) && (Event.current.type == EventType.MouseDown);
                bool flag4 = (singleClick || doubleClick) && (Event.current.button == 0);
                if (flag4)
                {
                    NativeProfilerTimeline_GetEntryAtPositionArgs args = new NativeProfilerTimeline_GetEntryAtPositionArgs();
                    args.Reset();
                    args.frameIndex = frameIndex;
                    args.threadIndex = threadIndex;
                    args.timeOffset = timeOffset;
                    args.threadRect = threadRect;
                    args.shownAreaRect = this.m_TimeArea.shownArea;
                    args.position = Event.current.mousePosition;
                    NativeProfilerTimeline.GetEntryAtPosition(ref args);
                    int nativeIndex = args.out_EntryIndex;
                    if (nativeIndex != -1)
                    {
                        if (!this.m_SelectedEntry.Equals(frameIndex, threadIndex, nativeIndex))
                        {
                            NativeProfilerTimeline_GetEntryTimingInfoArgs args2 = new NativeProfilerTimeline_GetEntryTimingInfoArgs();
                            args2.Reset();
                            args2.frameIndex = frameIndex;
                            args2.threadIndex = threadIndex;
                            args2.entryIndex = nativeIndex;
                            args2.calculateFrameData = true;
                            NativeProfilerTimeline.GetEntryTimingInfo(ref args2);
                            NativeProfilerTimeline_GetEntryInstanceInfoArgs args3 = new NativeProfilerTimeline_GetEntryInstanceInfoArgs();
                            args3.Reset();
                            args3.frameIndex = frameIndex;
                            args3.threadIndex = threadIndex;
                            args3.entryIndex = nativeIndex;
                            NativeProfilerTimeline.GetEntryInstanceInfo(ref args3);
                            this.m_Window.SetSelectedPropertyPath(args3.out_Path);
                            this.m_SelectedEntry.Reset();
                            this.m_SelectedEntry.frameId = frameIndex;
                            this.m_SelectedEntry.threadId = threadIndex;
                            this.m_SelectedEntry.nativeIndex = nativeIndex;
                            this.m_SelectedEntry.instanceId = args3.out_Id;
                            this.m_SelectedEntry.time = args2.out_LocalStartTime;
                            this.m_SelectedEntry.duration = args2.out_Duration;
                            this.m_SelectedEntry.totalDuration = args2.out_TotalDurationForFrame;
                            this.m_SelectedEntry.instanceCount = args2.out_InstanceCountForFrame;
                            this.m_SelectedEntry.relativeYPos = args.out_EntryYMaxPos + topMargin;
                            this.m_SelectedEntry.name = args.out_EntryName;
                            this.m_SelectedEntry.allocationInfo = args3.out_AllocationInfo;
                            this.m_SelectedEntry.metaData = args3.out_MetaData;
                        }
                        Event.current.Use();
                        this.UpdateSelectedObject(singleClick, doubleClick);
                    }
                    else if (flag4)
                    {
                        this.ClearSelection();
                        Event.current.Use();
                    }
                }
            }
        }

        private void PerformFrameSelected(float frameMS)
        {
            float time = this.m_SelectedEntry.time;
            float duration = this.m_SelectedEntry.duration;
            if ((this.m_SelectedEntry.instanceId < 0) || (duration <= 0f))
            {
                time = 0f;
                duration = frameMS;
            }
            this.m_TimeArea.SetShownHRangeInsideMargins(time - (duration * 0.2f), time + (duration * 1.2f));
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
            Object gameObject = EditorUtility.InstanceIDToObject(this.m_SelectedEntry.instanceId);
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
                    Selection.objects = new List<Object> { gameObject }.ToArray();
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

        private class EntryInfo
        {
            public float duration = 0f;
            public int frameId = -1;
            public string name = string.Empty;
            public int nativeIndex = -1;
            public float relativeYPos = 0f;
            public int threadId = -1;
            public float time = 0f;

            public bool Equals(int frameId, int threadId, int nativeIndex) => 
                (((frameId == this.frameId) && (threadId == this.threadId)) && (nativeIndex == this.nativeIndex));

            public bool IsValid() => 
                (this.name.Length > 0);

            public virtual void Reset()
            {
                this.frameId = -1;
                this.threadId = -1;
                this.nativeIndex = -1;
                this.relativeYPos = 0f;
                this.time = 0f;
                this.duration = 0f;
                this.name = string.Empty;
            }
        }

        internal class GroupInfo
        {
            public bool expanded;
            public float height;
            public string name;
            public List<ProfilerTimelineGUI.ThreadInfo> threads;
        }

        private class SelectedEntryInfo : ProfilerTimelineGUI.EntryInfo
        {
            public string allocationInfo = string.Empty;
            public int instanceCount = -1;
            public int instanceId = -1;
            public string metaData = string.Empty;
            public float totalDuration = -1f;

            public override void Reset()
            {
                base.Reset();
                this.instanceId = -1;
                this.metaData = string.Empty;
                this.totalDuration = -1f;
                this.instanceCount = -1;
                this.allocationInfo = string.Empty;
            }
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


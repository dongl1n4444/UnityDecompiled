namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SplitView : View, ICleanuppable, IDropArea
    {
        public int controlID = 0;
        internal const float kGrabDist = 5f;
        private const float kMaxViewDropZoneThickness = 300f;
        private const float kMinViewDropDestinationThickness = 100f;
        private const float kRootDropDestinationThickness = 200f;
        private const float kRootDropZoneOffset = 50f;
        private const float kRootDropZoneThickness = 70f;
        private static float[] s_DragPos;
        private static float[] s_StartDragPos;
        private SplitterState splitState = null;
        public bool vertical = false;

        public override void AddChild(View child, int idx)
        {
            base.AddChild(child, idx);
            this.ChildrenMinMaxChanged();
            this.splitState = null;
        }

        private void CalcRoomForRect(Rect[] sources, Rect r)
        {
            float num8;
            float num = !this.vertical ? r.x : r.y;
            float num2 = num + (!this.vertical ? r.width : r.height);
            float num3 = (num + num2) * 0.5f;
            int index = 0;
            while (index < sources.Length)
            {
                float num5 = !this.vertical ? (sources[index].x + (sources[index].width * 0.5f)) : (sources[index].y + (sources[index].height * 0.5f));
                if (num5 > num3)
                {
                    break;
                }
                index++;
            }
            float num6 = num;
            for (int i = index - 1; i >= 0; i--)
            {
                if (this.vertical)
                {
                    sources[i].yMax = num6;
                    if (sources[i].height >= base.children[i].minSize.y)
                    {
                        break;
                    }
                    num8 = sources[i].yMax - base.children[i].minSize.y;
                    sources[i].yMin = num8;
                    num6 = num8;
                }
                else
                {
                    sources[i].xMax = num6;
                    if (sources[i].width >= base.children[i].minSize.x)
                    {
                        break;
                    }
                    num8 = sources[i].xMax - base.children[i].minSize.x;
                    sources[i].xMin = num8;
                    num6 = num8;
                }
            }
            if (num6 < 0f)
            {
                float num9 = -num6;
                for (int k = 0; k < (index - 1); k++)
                {
                    if (this.vertical)
                    {
                        sources[k].y += num9;
                    }
                    else
                    {
                        sources[k].x += num9;
                    }
                }
                num2 += num9;
            }
            num6 = num2;
            for (int j = index; j < sources.Length; j++)
            {
                if (this.vertical)
                {
                    float yMax = sources[j].yMax;
                    sources[j].yMin = num6;
                    sources[j].yMax = yMax;
                    if (sources[j].height >= base.children[j].minSize.y)
                    {
                        break;
                    }
                    num8 = sources[j].yMin + base.children[j].minSize.y;
                    sources[j].yMax = num8;
                    num6 = num8;
                }
                else
                {
                    float xMax = sources[j].xMax;
                    sources[j].xMin = num6;
                    sources[j].xMax = xMax;
                    if (sources[j].width >= base.children[j].minSize.x)
                    {
                        break;
                    }
                    num8 = sources[j].xMin + base.children[j].minSize.x;
                    sources[j].xMax = num8;
                    num6 = num8;
                }
            }
            float num14 = !this.vertical ? base.position.width : base.position.height;
            if (num6 > num14)
            {
                float num15 = num14 - num6;
                for (int m = 0; m < (index - 1); m++)
                {
                    if (this.vertical)
                    {
                        sources[m].y += num15;
                    }
                    else
                    {
                        sources[m].x += num15;
                    }
                }
                num2 += num15;
            }
        }

        protected override void ChildrenMinMaxChanged()
        {
            Vector2 zero = Vector2.zero;
            Vector2 max = Vector2.zero;
            if (this.vertical)
            {
                foreach (View view in base.children)
                {
                    zero.x = Mathf.Max(view.minSize.x, zero.x);
                    max.x = Mathf.Max(view.maxSize.x, max.x);
                    zero.y += view.minSize.y;
                    max.y += view.maxSize.y;
                }
            }
            else
            {
                foreach (View view2 in base.children)
                {
                    zero.x += view2.minSize.x;
                    max.x += view2.maxSize.x;
                    zero.y = Mathf.Max(view2.minSize.y, zero.y);
                    max.y = Mathf.Max(view2.maxSize.y, max.y);
                }
            }
            this.splitState = null;
            base.SetMinMaxSizes(zero, max);
        }

        public void Cleanup()
        {
            SplitView parent = base.parent as SplitView;
            if ((base.children.Length == 1) && (parent != null))
            {
                View child = base.children[0];
                child.position = base.position;
                if (base.parent != null)
                {
                    base.parent.AddChild(child, base.parent.IndexOfChild(this));
                    base.parent.RemoveChild(this);
                    if (parent != null)
                    {
                        parent.Cleanup();
                    }
                    child.position = base.position;
                    if (!Unsupported.IsDestroyScriptableObject(this))
                    {
                        Object.DestroyImmediate(this);
                    }
                    return;
                }
                if (child is SplitView)
                {
                    this.RemoveChild(child);
                    base.window.rootView = child;
                    child.position = new Rect(0f, 0f, child.window.position.width, base.window.position.height);
                    child.Reflow();
                    if (!Unsupported.IsDestroyScriptableObject(this))
                    {
                        Object.DestroyImmediate(this);
                    }
                    return;
                }
            }
            if (parent != null)
            {
                parent.Cleanup();
                parent = base.parent as SplitView;
                if ((parent != null) && (parent.vertical == this.vertical))
                {
                    int index = new List<View>(base.parent.children).IndexOf(this);
                    foreach (View view3 in base.children)
                    {
                        parent.AddChild(view3, index++);
                        view3.position = new Rect(base.position.x + view3.position.x, base.position.y + view3.position.y, view3.position.width, view3.position.height);
                    }
                }
            }
            if (base.children.Length == 0)
            {
                if ((base.parent == null) && (base.window != null))
                {
                    base.window.Close();
                }
                else
                {
                    ICleanuppable cleanuppable = base.parent as ICleanuppable;
                    if (base.parent is SplitView)
                    {
                        ((SplitView) base.parent).RemoveChildNice(this);
                        if (!Unsupported.IsDestroyScriptableObject(this))
                        {
                            Object.DestroyImmediate(this, true);
                        }
                    }
                    cleanuppable.Cleanup();
                }
            }
            else
            {
                this.splitState = null;
                this.Reflow();
            }
        }

        public DropInfo DragOver(EditorWindow w, Vector2 mouseScreenPosition)
        {
            for (int i = 0; i < base.children.Length; i++)
            {
                View view = base.children[i];
                if ((view != DockArea.s_IgnoreDockingForView) && !(view is SplitView))
                {
                    ViewEdge none = ViewEdge.None;
                    Rect screenPosition = view.screenPosition;
                    Rect rect = this.RectFromEdge(screenPosition, ViewEdge.Bottom, screenPosition.height - 39f, 0f);
                    float thickness = Mathf.Min(Mathf.Round(rect.width / 3f), 300f);
                    float num3 = Mathf.Min(Mathf.Round(rect.height / 3f), 300f);
                    Rect rect3 = this.RectFromEdge(rect, ViewEdge.Left, thickness, 0f);
                    Rect rect4 = this.RectFromEdge(rect, ViewEdge.Right, thickness, 0f);
                    Rect rect5 = this.RectFromEdge(rect, ViewEdge.Bottom, num3, 0f);
                    Rect rect6 = this.RectFromEdge(rect, ViewEdge.Top, num3, 0f);
                    if (rect3.Contains(mouseScreenPosition))
                    {
                        none |= ViewEdge.Left;
                    }
                    if (rect4.Contains(mouseScreenPosition))
                    {
                        none |= ViewEdge.Right;
                    }
                    if (rect5.Contains(mouseScreenPosition))
                    {
                        none |= ViewEdge.Bottom;
                    }
                    if (rect6.Contains(mouseScreenPosition))
                    {
                        none |= ViewEdge.Top;
                    }
                    Vector2 zero = Vector2.zero;
                    Vector2 vector2 = Vector2.zero;
                    ViewEdge bottom = none;
                    ViewEdge left = none;
                    switch (none)
                    {
                        case ViewEdge.BottomLeft:
                            bottom = ViewEdge.Bottom;
                            left = ViewEdge.Left;
                            zero = new Vector2(rect.x, rect.yMax) - mouseScreenPosition;
                            vector2 = new Vector2(-thickness, num3);
                            break;

                        case ViewEdge.Before:
                            bottom = ViewEdge.Left;
                            left = ViewEdge.Top;
                            zero = new Vector2(rect.x, rect.y) - mouseScreenPosition;
                            vector2 = new Vector2(-thickness, -num3);
                            break;

                        case ViewEdge.After:
                            bottom = ViewEdge.Right;
                            left = ViewEdge.Bottom;
                            zero = new Vector2(rect.xMax, rect.yMax) - mouseScreenPosition;
                            vector2 = new Vector2(thickness, num3);
                            break;

                        case ViewEdge.TopRight:
                            bottom = ViewEdge.Top;
                            left = ViewEdge.Right;
                            zero = new Vector2(rect.xMax, rect.y) - mouseScreenPosition;
                            vector2 = new Vector2(thickness, -num3);
                            break;
                    }
                    none = (((zero.x * vector2.y) - (zero.y * vector2.x)) >= 0f) ? left : bottom;
                    if (none != ViewEdge.None)
                    {
                        float num4 = Mathf.Max(Mathf.Round((((none & ViewEdge.FitsHorizontal) == ViewEdge.None) ? screenPosition.height : screenPosition.width) / 3f), 100f);
                        return new DropInfo(this) { 
                            userData = new ExtraDropInfo(false, none, i),
                            type = DropInfo.Type.Pane,
                            rect = this.RectFromEdge(screenPosition, none, num4, 0f)
                        };
                    }
                }
            }
            if (base.screenPosition.Contains(mouseScreenPosition) && !(base.parent is SplitView))
            {
                return new DropInfo(null);
            }
            return null;
        }

        public DropInfo DragOverRootView(Vector2 mouseScreenPosition)
        {
            if ((base.children.Length == 1) && (DockArea.s_IgnoreDockingForView == base.children[0]))
            {
                return null;
            }
            DropInfo info1 = this.RootViewDropZone(ViewEdge.Bottom, mouseScreenPosition, base.screenPosition);
            if (info1 != null)
            {
                return info1;
            }
            DropInfo info2 = this.RootViewDropZone(ViewEdge.Top, mouseScreenPosition, base.screenPosition);
            if (info2 != null)
            {
                return info2;
            }
            DropInfo info3 = this.RootViewDropZone(ViewEdge.Left, mouseScreenPosition, base.screenPosition);
            if (info3 != null)
            {
                return info3;
            }
            return this.RootViewDropZone(ViewEdge.Right, mouseScreenPosition, base.screenPosition);
        }

        private void MakeRoomForRect(Rect r)
        {
            Rect[] sources = new Rect[base.children.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i] = base.children[i].position;
            }
            this.CalcRoomForRect(sources, r);
            for (int j = 0; j < sources.Length; j++)
            {
                base.children[j].position = sources[j];
            }
        }

        public bool PerformDrop(EditorWindow dropWindow, DropInfo dropInfo, Vector2 screenPos)
        {
            ExtraDropInfo userData = dropInfo.userData as ExtraDropInfo;
            bool rootWindow = userData.rootWindow;
            ViewEdge edge = userData.edge;
            int index = userData.index;
            Rect r = dropInfo.rect;
            bool flag2 = (edge & ViewEdge.Before) != ViewEdge.None;
            bool flag3 = (edge & ViewEdge.FitsVertical) != ViewEdge.None;
            SplitView view = null;
            if ((this.vertical == flag3) || (base.children.Length < 2))
            {
                if (!flag2)
                {
                    if (rootWindow)
                    {
                        index = base.children.Length;
                    }
                    else
                    {
                        index++;
                    }
                }
                view = this;
            }
            else if (rootWindow)
            {
                SplitView view2 = ScriptableObject.CreateInstance<SplitView>();
                view2.position = base.position;
                if (base.window.rootView == this)
                {
                    base.window.rootView = view2;
                }
                else
                {
                    base.parent.AddChild(view2, base.parent.IndexOfChild(this));
                }
                view2.AddChild(this);
                base.position = new Rect(Vector2.zero, base.position.size);
                index = !flag2 ? 1 : 0;
                view = view2;
            }
            else
            {
                SplitView view3 = ScriptableObject.CreateInstance<SplitView>();
                view3.AddChild(base.children[index]);
                this.AddChild(view3, index);
                view3.position = view3.children[0].position;
                view3.children[0].position = new Rect(Vector2.zero, view3.position.size);
                index = !flag2 ? 1 : 0;
                view = view3;
            }
            r.position -= base.screenPosition.position;
            DockArea child = ScriptableObject.CreateInstance<DockArea>();
            view.vertical = flag3;
            view.MakeRoomForRect(r);
            view.AddChild(child, index);
            child.position = r;
            DockArea.s_OriginalDragSource.RemoveTab(dropWindow);
            dropWindow.m_Parent = child;
            child.AddTab(dropWindow);
            this.Reflow();
            RecalcMinMaxAndReflowAll(this);
            child.MakeVistaDWMHappyDance();
            return true;
        }

        private void PlaceView(int i, float pos, float size)
        {
            float y = Mathf.Round(pos);
            if (this.vertical)
            {
                base.children[i].position = new Rect(0f, y, base.position.width, Mathf.Round(pos + size) - y);
            }
            else
            {
                base.children[i].position = new Rect(y, 0f, Mathf.Round(pos + size) - y, base.position.height);
            }
        }

        private static string PosVals(float[] posVals)
        {
            string str = "[";
            foreach (float num in posVals)
            {
                string str2 = str;
                object[] objArray1 = new object[] { str2, "", num, ", " };
                str = string.Concat(objArray1);
            }
            return (str + "]");
        }

        private static void RecalcMinMaxAndReflowAll(SplitView start)
        {
            SplitView node = start;
            SplitView parent = start;
            do
            {
                node = parent;
                parent = node.parent as SplitView;
            }
            while (parent != null);
            RecalcMinMaxRecurse(node);
            ReflowRecurse(node);
        }

        private static void RecalcMinMaxRecurse(SplitView node)
        {
            foreach (View view in node.children)
            {
                SplitView view2 = view as SplitView;
                if (view2 != null)
                {
                    RecalcMinMaxRecurse(view2);
                }
            }
            node.ChildrenMinMaxChanged();
        }

        private Rect RectFromEdge(Rect rect, ViewEdge edge, float thickness, float offset)
        {
            switch (edge)
            {
                case ViewEdge.Left:
                    return new Rect(rect.x - offset, rect.y, thickness, rect.height);

                case ViewEdge.Bottom:
                    return new Rect(rect.x, (rect.yMax - thickness) + offset, rect.width, thickness);

                case ViewEdge.Top:
                    return new Rect(rect.x, rect.y - offset, rect.width, thickness);

                case ViewEdge.Right:
                    return new Rect((rect.xMax - thickness) + offset, rect.y, thickness, rect.height);
            }
            throw new ArgumentException("Specify exactly one edge");
        }

        internal override void Reflow()
        {
            this.SetupSplitter();
            for (int i = 0; i < (base.children.Length - 1); i++)
            {
                this.splitState.DoSplitter(i, i + 1, 0);
            }
            this.splitState.RelativeToRealSizes(!this.vertical ? ((int) base.position.width) : ((int) base.position.height));
            this.SetupRectsFromSplitter();
        }

        private static void ReflowRecurse(SplitView node)
        {
            node.Reflow();
            foreach (View view in node.children)
            {
                SplitView view2 = view as SplitView;
                if (view2 != null)
                {
                    RecalcMinMaxRecurse(view2);
                }
            }
        }

        public override void RemoveChild(View child)
        {
            this.splitState = null;
            base.RemoveChild(child);
        }

        public void RemoveChildNice(View child)
        {
            if (base.children.Length != 1)
            {
                int num = base.IndexOfChild(child);
                float t = 0f;
                if (num == 0)
                {
                    t = 0f;
                }
                else if (num == (base.children.Length - 1))
                {
                    t = 1f;
                }
                else
                {
                    t = 0.5f;
                }
                t = !this.vertical ? Mathf.Lerp(child.position.xMin, child.position.xMax, t) : Mathf.Lerp(child.position.yMin, child.position.yMax, t);
                if (num > 0)
                {
                    View view = base.children[num - 1];
                    Rect position = view.position;
                    if (this.vertical)
                    {
                        position.yMax = t;
                    }
                    else
                    {
                        position.xMax = t;
                    }
                    view.position = position;
                    if (view is SplitView)
                    {
                        ((SplitView) view).Reflow();
                    }
                }
                if (num < (base.children.Length - 1))
                {
                    View view2 = base.children[num + 1];
                    Rect rect6 = view2.position;
                    if (this.vertical)
                    {
                        view2.position = new Rect(rect6.x, t, rect6.width, rect6.yMax - t);
                    }
                    else
                    {
                        view2.position = new Rect(t, rect6.y, rect6.xMax - t, rect6.height);
                    }
                    if (view2 is SplitView)
                    {
                        ((SplitView) view2).Reflow();
                    }
                }
            }
            this.RemoveChild(child);
        }

        private DropInfo RootViewDropZone(ViewEdge edge, Vector2 mousePos, Rect screenRect)
        {
            float offset = ((edge & ViewEdge.FitsVertical) == ViewEdge.None) ? 50f : 70f;
            if (!this.RectFromEdge(screenRect, edge, 70f, offset).Contains(mousePos))
            {
                return null;
            }
            return new DropInfo(this) { 
                type = DropInfo.Type.Pane,
                userData = new ExtraDropInfo(true, edge, 0),
                rect = this.RectFromEdge(screenRect, edge, 200f, 0f)
            };
        }

        protected override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            this.Reflow();
        }

        private void SetupRectsFromSplitter()
        {
            if (base.children.Length != 0)
            {
                int num = 0;
                int num2 = 0;
                foreach (int num3 in this.splitState.realSizes)
                {
                    num2 += num3;
                }
                float num5 = 1f;
                if (num2 > (!this.vertical ? base.position.width : base.position.height))
                {
                    num5 = (!this.vertical ? base.position.width : base.position.height) / ((float) num2);
                }
                SavedGUIState state = SavedGUIState.Create();
                for (int i = 0; i < base.children.Length; i++)
                {
                    int num7 = (int) Mathf.Round(this.splitState.realSizes[i] * num5);
                    if (this.vertical)
                    {
                        base.children[i].position = new Rect(0f, (float) num, base.position.width, (float) num7);
                    }
                    else
                    {
                        base.children[i].position = new Rect((float) num, 0f, (float) num7, base.position.height);
                    }
                    num += num7;
                }
                state.ApplyAndForget();
            }
        }

        private void SetupSplitter()
        {
            int[] realSizes = new int[base.children.Length];
            int[] minSizes = new int[base.children.Length];
            for (int i = 0; i < base.children.Length; i++)
            {
                View view = base.children[i];
                realSizes[i] = !this.vertical ? ((int) view.position.width) : ((int) view.position.height);
                minSizes[i] = !this.vertical ? ((int) view.minSize.x) : ((int) view.minSize.y);
            }
            this.splitState = new SplitterState(realSizes, minSizes, null);
            this.splitState.splitSize = 10;
        }

        public void SplitGUI(Event evt)
        {
            if (this.splitState == null)
            {
                this.SetupSplitter();
            }
            SplitView parent = base.parent as SplitView;
            if (parent != null)
            {
                Event event2 = new Event(evt);
                event2.mousePosition += new Vector2(base.position.x, base.position.y);
                parent.SplitGUI(event2);
                if (event2.type == EventType.Used)
                {
                    evt.Use();
                }
            }
            float num = !this.vertical ? evt.mousePosition.x : evt.mousePosition.y;
            int controlID = GUIUtility.GetControlID(0x857b3, FocusType.Passive);
            this.controlID = controlID;
            EventType typeForControl = evt.GetTypeForControl(controlID);
            if (typeForControl == EventType.MouseDown)
            {
                if (base.children.Length != 1)
                {
                    int num3 = !this.vertical ? ((int) base.children[0].position.x) : ((int) base.children[0].position.y);
                    for (int i = 0; i < (base.children.Length - 1); i++)
                    {
                        if (i >= this.splitState.realSizes.Length)
                        {
                            DockArea current = GUIView.current as DockArea;
                            string str = "Non-dock area " + GUIView.current.GetType();
                            if (((current != null) && (current.m_Selected < current.m_Panes.Count)) && (current.m_Panes[current.m_Selected] != null))
                            {
                                str = current.m_Panes[current.m_Selected].GetType().ToString();
                            }
                            if (Unsupported.IsDeveloperBuild())
                            {
                                Debug.LogError(string.Concat(new object[] { "Real sizes out of bounds for: ", str, " index: ", i, " RealSizes: ", this.splitState.realSizes.Length }));
                            }
                            this.SetupSplitter();
                        }
                        Rect rect5 = !this.vertical ? new Rect((float) ((num3 + this.splitState.realSizes[i]) - (this.splitState.splitSize / 2)), base.children[0].position.y, (float) this.splitState.splitSize, base.children[0].position.height) : new Rect(base.children[0].position.x, (float) ((num3 + this.splitState.realSizes[i]) - (this.splitState.splitSize / 2)), base.children[0].position.width, (float) this.splitState.splitSize);
                        if (rect5.Contains(evt.mousePosition))
                        {
                            this.splitState.splitterInitialOffset = (int) num;
                            this.splitState.currentActiveSplitter = i;
                            GUIUtility.hotControl = controlID;
                            evt.Use();
                            break;
                        }
                        num3 += this.splitState.realSizes[i];
                    }
                }
            }
            else if (typeForControl == EventType.MouseDrag)
            {
                if (((base.children.Length > 1) && (GUIUtility.hotControl == controlID)) && (this.splitState.currentActiveSplitter >= 0))
                {
                    int diff = ((int) num) - this.splitState.splitterInitialOffset;
                    if (diff != 0)
                    {
                        this.splitState.splitterInitialOffset = (int) num;
                        this.splitState.DoSplitter(this.splitState.currentActiveSplitter, this.splitState.currentActiveSplitter + 1, diff);
                    }
                    this.SetupRectsFromSplitter();
                    evt.Use();
                }
            }
            else if ((typeForControl == EventType.MouseUp) && (GUIUtility.hotControl == controlID))
            {
                GUIUtility.hotControl = 0;
            }
        }

        public override string ToString()
        {
            return (!this.vertical ? "SplitView (horiz)" : "SplitView (vert)");
        }

        internal class ExtraDropInfo
        {
            public SplitView.ViewEdge edge;
            public int index;
            public bool rootWindow;

            public ExtraDropInfo(bool rootWindow, SplitView.ViewEdge edge, int index)
            {
                this.rootWindow = rootWindow;
                this.edge = edge;
                this.index = index;
            }
        }

        [Flags]
        internal enum ViewEdge
        {
            After = 10,
            Before = 5,
            Bottom = 2,
            BottomLeft = 3,
            BottomRight = 10,
            FitsHorizontal = 9,
            FitsVertical = 6,
            Left = 1,
            None = 0,
            Right = 8,
            Top = 4,
            TopLeft = 5,
            TopRight = 12
        }
    }
}


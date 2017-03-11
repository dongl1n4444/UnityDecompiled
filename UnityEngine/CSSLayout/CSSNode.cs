namespace UnityEngine.CSSLayout
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class CSSNode : IEnumerable<CSSNode>, IEnumerable
    {
        private List<CSSNode> _children;
        private CSSMeasureFunc _cssMeasureFunc;
        private IntPtr _cssNode;
        private object _data;
        private MeasureFunction _measureFunction;
        private WeakReference _parent;

        public CSSNode()
        {
            CSSLogger.Initialize();
            this._cssNode = Native.CSSNodeNew();
            if (this._cssNode == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to allocate native memory");
            }
        }

        public void CalculateLayout()
        {
            Native.CSSNodeCalculateLayout(this._cssNode, float.NaN, float.NaN, Native.CSSNodeStyleGetDirection(this._cssNode));
        }

        public void Clear()
        {
            if (this._children != null)
            {
                while (this._children.Count > 0)
                {
                    this.RemoveAt(this._children.Count - 1);
                }
            }
        }

        public void CopyStyle(CSSNode srcNode)
        {
            Native.CSSNodeCopyStyle(this._cssNode, srcNode._cssNode);
        }

        ~CSSNode()
        {
            Native.CSSNodeFree(this._cssNode);
        }

        public float GetBorder(CSSEdge edge) => 
            Native.CSSNodeStyleGetBorder(this._cssNode, edge);

        public IEnumerator<CSSNode> GetEnumerator() => 
            ((this._children == null) ? Enumerable.Empty<CSSNode>().GetEnumerator() : this._children.GetEnumerator());

        public static int GetInstanceCount() => 
            Native.CSSNodeGetInstanceCount();

        public float GetMargin(CSSEdge edge) => 
            Native.CSSNodeStyleGetMargin(this._cssNode, edge);

        public float GetPadding(CSSEdge edge) => 
            Native.CSSNodeStyleGetPadding(this._cssNode, edge);

        public float GetPosition(CSSEdge edge) => 
            Native.CSSNodeStyleGetPosition(this._cssNode, edge);

        public int IndexOf(CSSNode node) => 
            ((this._children == null) ? -1 : this._children.IndexOf(node));

        public void Insert(int index, CSSNode node)
        {
            if (this._children == null)
            {
                this._children = new List<CSSNode>(4);
            }
            this._children.Insert(index, node);
            node._parent = new WeakReference(this);
            Native.CSSNodeInsertChild(this._cssNode, node._cssNode, (uint) index);
        }

        public static bool IsExperimentalFeatureEnabled(CSSExperimentalFeature feature) => 
            Native.CSSLayoutIsExperimentalFeatureEnabled(feature);

        public virtual void MarkDirty()
        {
            Native.CSSNodeMarkDirty(this._cssNode);
        }

        public void MarkHasNewLayout()
        {
            Native.CSSNodeSetHasNewLayout(this._cssNode, true);
        }

        public void MarkLayoutSeen()
        {
            Native.CSSNodeSetHasNewLayout(this._cssNode, false);
        }

        private CSSSize MeasureInternal(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode)
        {
            if (this._measureFunction == null)
            {
                throw new InvalidOperationException("Measure function is not defined.");
            }
            long measureOutput = this._measureFunction(this, width, widthMode, height, heightMode);
            return new CSSSize { 
                width = MeasureOutput.GetWidth(measureOutput),
                height = MeasureOutput.GetHeight(measureOutput)
            };
        }

        public string Print() => 
            this.Print(CSSPrintOptions.Children | CSSPrintOptions.Style | CSSPrintOptions.Layout);

        public string Print(CSSPrintOptions options)
        {
            <Print>c__AnonStorey0 storey = new <Print>c__AnonStorey0 {
                sb = new StringBuilder()
            };
            CSSLogger.Func logger = CSSLogger.Logger;
            CSSLogger.Logger = new CSSLogger.Func(storey.<>m__0);
            Native.CSSNodePrint(this._cssNode, options);
            CSSLogger.Logger = logger;
            return storey.sb.ToString();
        }

        public void RemoveAt(int index)
        {
            CSSNode node = this._children[index];
            node._parent = null;
            this._children.RemoveAt(index);
            Native.CSSNodeRemoveChild(this._cssNode, node._cssNode);
        }

        public void Reset()
        {
            this._measureFunction = null;
            this._data = null;
            Native.CSSNodeReset(this._cssNode);
        }

        public void SetBorder(CSSEdge edge, float border)
        {
            Native.CSSNodeStyleSetBorder(this._cssNode, edge, border);
        }

        public static void SetExperimentalFeatureEnabled(CSSExperimentalFeature feature, bool enabled)
        {
            Native.CSSLayoutSetExperimentalFeatureEnabled(feature, enabled);
        }

        public void SetMargin(CSSEdge edge, float value)
        {
            Native.CSSNodeStyleSetMargin(this._cssNode, edge, value);
        }

        public void SetMeasureFunction(MeasureFunction measureFunction)
        {
            this._measureFunction = measureFunction;
            this._cssMeasureFunc = (measureFunction == null) ? null : new CSSMeasureFunc(this.MeasureInternal);
            Native.CSSNodeSetMeasureFunc(this._cssNode, this._cssMeasureFunc);
        }

        public void SetPadding(CSSEdge edge, float padding)
        {
            Native.CSSNodeStyleSetPadding(this._cssNode, edge, padding);
        }

        public void SetPosition(CSSEdge edge, float position)
        {
            Native.CSSNodeStyleSetPosition(this._cssNode, edge, position);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            ((this._children == null) ? Enumerable.Empty<CSSNode>().GetEnumerator() : this._children.GetEnumerator());

        public bool ValuesEqual(float f1, float f2)
        {
            if (float.IsNaN(f1) || float.IsNaN(f2))
            {
                return (float.IsNaN(f1) && float.IsNaN(f2));
            }
            return (Math.Abs((float) (f2 - f1)) < float.Epsilon);
        }

        public CSSAlign AlignContent
        {
            get => 
                Native.CSSNodeStyleGetAlignContent(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetAlignContent(this._cssNode, value);
            }
        }

        public CSSAlign AlignItems
        {
            get => 
                Native.CSSNodeStyleGetAlignItems(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetAlignItems(this._cssNode, value);
            }
        }

        public CSSAlign AlignSelf
        {
            get => 
                Native.CSSNodeStyleGetAlignSelf(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetAlignSelf(this._cssNode, value);
            }
        }

        public float AspectRatio
        {
            get => 
                Native.CSSNodeStyleGetAspectRatio(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetAspectRatio(this._cssNode, value);
            }
        }

        public int Count =>
            ((this._children == null) ? 0 : this._children.Count);

        public object Data
        {
            get => 
                this._data;
            set
            {
                this._data = value;
            }
        }

        public float Flex
        {
            set
            {
                Native.CSSNodeStyleSetFlex(this._cssNode, value);
            }
        }

        public float FlexBasis
        {
            get => 
                Native.CSSNodeStyleGetFlexBasis(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetFlexBasis(this._cssNode, value);
            }
        }

        public CSSFlexDirection FlexDirection
        {
            get => 
                Native.CSSNodeStyleGetFlexDirection(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetFlexDirection(this._cssNode, value);
            }
        }

        public float FlexGrow
        {
            get => 
                Native.CSSNodeStyleGetFlexGrow(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetFlexGrow(this._cssNode, value);
            }
        }

        public float FlexShrink
        {
            get => 
                Native.CSSNodeStyleGetFlexShrink(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetFlexShrink(this._cssNode, value);
            }
        }

        public bool HasNewLayout =>
            Native.CSSNodeGetHasNewLayout(this._cssNode);

        public float Height
        {
            get => 
                Native.CSSNodeStyleGetHeight(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetHeight(this._cssNode, value);
            }
        }

        public bool IsDirty =>
            Native.CSSNodeIsDirty(this._cssNode);

        public bool IsMeasureDefined =>
            (this._measureFunction != null);

        public CSSNode this[int index] =>
            this._children[index];

        public CSSJustify JustifyContent
        {
            get => 
                Native.CSSNodeStyleGetJustifyContent(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetJustifyContent(this._cssNode, value);
            }
        }

        public CSSDirection LayoutDirection =>
            Native.CSSNodeLayoutGetDirection(this._cssNode);

        public float LayoutHeight =>
            Native.CSSNodeLayoutGetHeight(this._cssNode);

        public float LayoutWidth =>
            Native.CSSNodeLayoutGetWidth(this._cssNode);

        public float LayoutX =>
            Native.CSSNodeLayoutGetLeft(this._cssNode);

        public float LayoutY =>
            Native.CSSNodeLayoutGetTop(this._cssNode);

        public float MaxHeight
        {
            get => 
                Native.CSSNodeStyleGetMaxHeight(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetMaxHeight(this._cssNode, value);
            }
        }

        public float MaxWidth
        {
            get => 
                Native.CSSNodeStyleGetMaxWidth(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetMaxWidth(this._cssNode, value);
            }
        }

        public float MinHeight
        {
            get => 
                Native.CSSNodeStyleGetMinHeight(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetMinHeight(this._cssNode, value);
            }
        }

        public float MinWidth
        {
            get => 
                Native.CSSNodeStyleGetMinWidth(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetMinWidth(this._cssNode, value);
            }
        }

        public CSSOverflow Overflow
        {
            get => 
                Native.CSSNodeStyleGetOverflow(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetOverflow(this._cssNode, value);
            }
        }

        public CSSNode Parent =>
            ((this._parent == null) ? null : (this._parent.Target as CSSNode));

        public CSSPositionType PositionType
        {
            get => 
                Native.CSSNodeStyleGetPositionType(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetPositionType(this._cssNode, value);
            }
        }

        public CSSDirection StyleDirection
        {
            get => 
                Native.CSSNodeStyleGetDirection(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetDirection(this._cssNode, value);
            }
        }

        public float Width
        {
            get => 
                Native.CSSNodeStyleGetWidth(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetWidth(this._cssNode, value);
            }
        }

        public CSSWrap Wrap
        {
            get => 
                Native.CSSNodeStyleGetFlexWrap(this._cssNode);
            set
            {
                Native.CSSNodeStyleSetFlexWrap(this._cssNode, value);
            }
        }

        [CompilerGenerated]
        private sealed class <Print>c__AnonStorey0
        {
            internal StringBuilder sb;

            internal void <>m__0(CSSLogLevel level, string message)
            {
                this.sb.Append(message);
            }
        }
    }
}


namespace UnityEngine.CSSLayout
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal static class Native
    {
        private const string DllName = "CSSLayout";
        private static Dictionary<IntPtr, WeakReference> s_MeasureFunctions = new Dictionary<IntPtr, WeakReference>();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CSSLayoutIsExperimentalFeatureEnabled(CSSExperimentalFeature feature);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSLayoutSetExperimentalFeatureEnabled(CSSExperimentalFeature feature, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeCalculateLayout(IntPtr node, float availableWidth, float availableHeight, CSSDirection parentDirection);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint CSSNodeChildCount(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeCopyStyle(IntPtr dstNode, IntPtr srcNode);
        public static void CSSNodeFree(IntPtr cssNode)
        {
            if (cssNode != IntPtr.Zero)
            {
                CSSNodeSetMeasureFunc(cssNode, null);
                CSSNodeFreeInternal(cssNode);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CSSNodeFreeInternal(IntPtr cssNode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr CSSNodeGetChild(IntPtr node, uint index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr CSSNodeGetContext(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CSSNodeGetHasNewLayout(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int CSSNodeGetInstanceCount();
        public static CSSMeasureFunc CSSNodeGetMeasureFunc(IntPtr node)
        {
            WeakReference reference = null;
            if (s_MeasureFunctions.TryGetValue(node, out reference) && reference.IsAlive)
            {
                return (reference.Target as CSSMeasureFunc);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeInit(IntPtr cssNode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeInsertChild(IntPtr node, IntPtr child, uint index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CSSNodeIsDirty(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeLayoutGetBottom(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSDirection CSSNodeLayoutGetDirection(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeLayoutGetHeight(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeLayoutGetLeft(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeLayoutGetRight(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeLayoutGetTop(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeLayoutGetWidth(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeMarkDirty(IntPtr node);
        [RequiredByNativeCode]
        public static void CSSNodeMeasureInvoke(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode, IntPtr returnValueAddress)
        {
            CSSMeasureFunc func = CSSNodeGetMeasureFunc(node);
            if (func != null)
            {
                returnValueAddress[0] = (IntPtr) func(node, width, widthMode, height, heightMode);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr CSSNodeNew();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodePrint(IntPtr node, CSSPrintOptions options);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeRemoveChild(IntPtr node, IntPtr child);
        public static void CSSNodeReset(IntPtr cssNode)
        {
            CSSNodeSetMeasureFunc(cssNode, null);
            CSSNodeResetInternal(cssNode);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CSSNodeResetInternal(IntPtr cssNode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeSetContext(IntPtr node, IntPtr context);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeSetHasNewLayout(IntPtr node, bool hasNewLayout);
        public static void CSSNodeSetMeasureFunc(IntPtr node, CSSMeasureFunc measureFunc)
        {
            if (measureFunc != null)
            {
                s_MeasureFunctions[node] = new WeakReference(measureFunc);
                CSSLayoutCallbacks.RegisterWrapper(node);
            }
            else if (s_MeasureFunctions.ContainsKey(node))
            {
                s_MeasureFunctions.Remove(node);
                CSSLayoutCallbacks.UnegisterWrapper(node);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSAlign CSSNodeStyleGetAlignContent(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSAlign CSSNodeStyleGetAlignItems(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSAlign CSSNodeStyleGetAlignSelf(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetAspectRatio(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetBorder(IntPtr node, CSSEdge edge);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSDirection CSSNodeStyleGetDirection(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetFlexBasis(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSFlexDirection CSSNodeStyleGetFlexDirection(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetFlexGrow(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetFlexShrink(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSWrap CSSNodeStyleGetFlexWrap(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetHeight(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSJustify CSSNodeStyleGetJustifyContent(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetMargin(IntPtr node, CSSEdge edge);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetMaxHeight(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetMaxWidth(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetMinHeight(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetMinWidth(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSOverflow CSSNodeStyleGetOverflow(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetPadding(IntPtr node, CSSEdge edge);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetPosition(IntPtr node, CSSEdge edge);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern CSSPositionType CSSNodeStyleGetPositionType(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float CSSNodeStyleGetWidth(IntPtr node);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetAlignContent(IntPtr node, CSSAlign alignContent);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetAlignItems(IntPtr node, CSSAlign alignItems);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetAlignSelf(IntPtr node, CSSAlign alignSelf);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetAspectRatio(IntPtr node, float aspectRatio);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetBorder(IntPtr node, CSSEdge edge, float border);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetDirection(IntPtr node, CSSDirection direction);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetFlex(IntPtr node, float flex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetFlexBasis(IntPtr node, float flexBasis);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetFlexDirection(IntPtr node, CSSFlexDirection flexDirection);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetFlexGrow(IntPtr node, float flexGrow);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetFlexShrink(IntPtr node, float flexShrink);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetFlexWrap(IntPtr node, CSSWrap flexWrap);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetHeight(IntPtr node, float height);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetJustifyContent(IntPtr node, CSSJustify justifyContent);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetMargin(IntPtr node, CSSEdge edge, float margin);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetMaxHeight(IntPtr node, float maxHeight);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetMaxWidth(IntPtr node, float maxWidth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetMinHeight(IntPtr node, float minHeight);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetMinWidth(IntPtr node, float minWidth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetOverflow(IntPtr node, CSSOverflow flexWrap);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetPadding(IntPtr node, CSSEdge edge, float padding);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetPosition(IntPtr node, CSSEdge edge, float position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetPositionType(IntPtr node, CSSPositionType positionType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CSSNodeStyleSetWidth(IntPtr node, float width);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CSSValueIsUndefined(float value);
    }
}


namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class TransformManipulator
    {
        private static EventType s_EventTypeBefore = EventType.Ignore;
        private static int s_HotControl = 0;
        private static bool s_LockHandle = false;
        private static TransformData[] s_MouseDownState = null;
        private static Vector3 s_StartHandlePosition = Vector3.zero;
        private static Vector3 s_StartLocalHandleOffset = Vector3.zero;

        private static void BeginEventCheck()
        {
            s_EventTypeBefore = Event.current.GetTypeForControl(s_HotControl);
        }

        public static void BeginManipulationHandling(bool lockHandleWhileDragging)
        {
            BeginEventCheck();
            s_LockHandle = lockHandleWhileDragging;
        }

        public static void DebugAlignment(Quaternion targetRotation)
        {
            if (s_MouseDownState != null)
            {
                for (int i = 0; i < s_MouseDownState.Length; i++)
                {
                    s_MouseDownState[i].DebugAlignment(targetRotation);
                }
            }
        }

        private static EventType EndEventCheck()
        {
            EventType type = (s_EventTypeBefore == Event.current.GetTypeForControl(s_HotControl)) ? EventType.Ignore : s_EventTypeBefore;
            s_EventTypeBefore = EventType.Ignore;
            switch (type)
            {
                case EventType.MouseDown:
                    s_HotControl = GUIUtility.hotControl;
                    return type;

                case EventType.MouseUp:
                    s_HotControl = 0;
                    return type;
            }
            return type;
        }

        public static EventType EndManipulationHandling()
        {
            EventType type = EndEventCheck();
            if (type == EventType.MouseDown)
            {
                RecordMouseDownState(Selection.transforms);
                s_StartHandlePosition = Tools.handlePosition;
                s_StartLocalHandleOffset = Tools.localHandleOffset;
                if (s_LockHandle)
                {
                    Tools.LockHandlePosition();
                }
                Tools.LockHandleRectRotation();
                return type;
            }
            if ((s_MouseDownState != null) && ((type == EventType.MouseUp) || (GUIUtility.hotControl != s_HotControl)))
            {
                s_MouseDownState = null;
                if (s_LockHandle)
                {
                    Tools.UnlockHandlePosition();
                }
                Tools.UnlockHandleRectRotation();
                ManipulationToolUtility.DisableMinDragDifference();
            }
            return type;
        }

        private static void RecordMouseDownState(Transform[] transforms)
        {
            s_MouseDownState = new TransformData[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                s_MouseDownState[i] = TransformData.GetData(transforms[i]);
            }
        }

        private static void SetLocalHandleOffsetScaleDelta(Vector3 scaleDelta, Quaternion pivotRotation)
        {
            Quaternion quaternion = Quaternion.Inverse(Tools.handleRotation) * pivotRotation;
            Tools.localHandleOffset = Vector3.Scale(Vector3.Scale(s_StartLocalHandleOffset, (Vector3) (quaternion * scaleDelta)), (Vector3) (quaternion * Vector3.one));
        }

        public static void SetPositionDelta(Vector3 positionDelta)
        {
            if (s_MouseDownState != null)
            {
                for (int i = 0; i < s_MouseDownState.Length; i++)
                {
                    TransformData data = s_MouseDownState[i];
                    Undo.RecordObject((data.rectTransform == null) ? data.transform : data.rectTransform, "Move");
                }
                for (int j = 0; j < s_MouseDownState.Length; j++)
                {
                    s_MouseDownState[j].SetPositionDelta(positionDelta);
                }
            }
        }

        public static void SetResizeDelta(Vector3 scaleDelta, Vector3 pivotPosition, Quaternion pivotRotation)
        {
            if (s_MouseDownState != null)
            {
                SetLocalHandleOffsetScaleDelta(scaleDelta, pivotRotation);
                for (int i = 0; i < s_MouseDownState.Length; i++)
                {
                    TransformData data = s_MouseDownState[i];
                    if (data.rectTransform != null)
                    {
                        Undo.RecordObject(data.rectTransform, "Resize");
                    }
                    else
                    {
                        SpriteRenderer component = data.transform.GetComponent<SpriteRenderer>();
                        if ((component != null) && (component.drawMode != SpriteDrawMode.Simple))
                        {
                            UnityEngine.Object[] objectsToUndo = new UnityEngine.Object[] { component, data.transform };
                            Undo.RecordObjects(objectsToUndo, "Resize");
                        }
                        else
                        {
                            Undo.RecordObject(data.transform, "Resize");
                        }
                    }
                }
                for (int j = 0; j < s_MouseDownState.Length; j++)
                {
                    s_MouseDownState[j].SetScaleDelta(scaleDelta, pivotPosition, pivotRotation, true);
                }
            }
        }

        public static void SetScaleDelta(Vector3 scaleDelta, Quaternion pivotRotation)
        {
            if (s_MouseDownState != null)
            {
                SetLocalHandleOffsetScaleDelta(scaleDelta, pivotRotation);
                for (int i = 0; i < s_MouseDownState.Length; i++)
                {
                    TransformData data = s_MouseDownState[i];
                    Undo.RecordObject(data.transform, "Scale");
                }
                Vector3 handlePosition = Tools.handlePosition;
                for (int j = 0; j < s_MouseDownState.Length; j++)
                {
                    if (Tools.pivotMode == PivotMode.Pivot)
                    {
                        handlePosition = s_MouseDownState[j].position;
                    }
                    if (individualSpace)
                    {
                        pivotRotation = s_MouseDownState[j].rotation;
                    }
                    s_MouseDownState[j].SetScaleDelta(scaleDelta, handlePosition, pivotRotation, false);
                }
            }
        }

        public static bool active =>
            (s_MouseDownState != null);

        public static bool individualSpace =>
            ((Tools.pivotRotation == PivotRotation.Local) && (Tools.pivotMode == PivotMode.Pivot));

        public static Vector3 mouseDownHandlePosition =>
            s_StartHandlePosition;

        [StructLayout(LayoutKind.Sequential)]
        private struct TransformData
        {
            public static Quaternion[] s_Alignments;
            public Transform transform;
            public Vector3 position;
            public Vector3 localPosition;
            public Quaternion rotation;
            public Vector3 scale;
            public RectTransform rectTransform;
            public Rect rect;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public static TransformManipulator.TransformData GetData(Transform t)
            {
                TransformManipulator.TransformData data = new TransformManipulator.TransformData();
                data.SetupTransformValues(t);
                data.rectTransform = t.GetComponent<RectTransform>();
                if (data.rectTransform != null)
                {
                    data.sizeDelta = data.rectTransform.sizeDelta;
                    data.rect = data.rectTransform.rect;
                    data.anchoredPosition = data.rectTransform.anchoredPosition;
                    return data;
                }
                SpriteRenderer component = t.GetComponent<SpriteRenderer>();
                if ((component != null) && (component.drawMode != SpriteDrawMode.Simple))
                {
                    data.sizeDelta = component.size;
                }
                return data;
            }

            private Quaternion GetRefAlignment(Quaternion targetRotation, Quaternion ownRotation)
            {
                float negativeInfinity = float.NegativeInfinity;
                Quaternion identity = Quaternion.identity;
                for (int i = 0; i < s_Alignments.Length; i++)
                {
                    float[] values = new float[] { Mathf.Abs(Vector3.Dot((Vector3) (targetRotation * Vector3.right), (Vector3) ((ownRotation * s_Alignments[i]) * Vector3.right))), Mathf.Abs(Vector3.Dot((Vector3) (targetRotation * Vector3.up), (Vector3) ((ownRotation * s_Alignments[i]) * Vector3.up))), Mathf.Abs(Vector3.Dot((Vector3) (targetRotation * Vector3.forward), (Vector3) ((ownRotation * s_Alignments[i]) * Vector3.forward))) };
                    float num3 = Mathf.Min(values);
                    if (num3 > negativeInfinity)
                    {
                        negativeInfinity = num3;
                        identity = s_Alignments[i];
                    }
                }
                return identity;
            }

            private void SetupTransformValues(Transform t)
            {
                this.transform = t;
                this.position = t.position;
                this.localPosition = t.localPosition;
                this.rotation = t.rotation;
                this.scale = t.localScale;
            }

            private void SetScaleValue(Vector3 scale)
            {
                this.transform.localScale = scale;
            }

            public void SetScaleDelta(Vector3 scaleDelta, Vector3 scalePivot, Quaternion scaleRotation, bool preferRectResize)
            {
                this.SetPosition(((Vector3) (scaleRotation * Vector3.Scale((Vector3) (Quaternion.Inverse(scaleRotation) * (this.position - scalePivot)), scaleDelta))) + scalePivot);
                Vector3 minDragDifference = ManipulationToolUtility.minDragDifference;
                if (this.transform.parent != null)
                {
                    minDragDifference.x /= this.transform.parent.lossyScale.x;
                    minDragDifference.y /= this.transform.parent.lossyScale.y;
                    minDragDifference.z /= this.transform.parent.lossyScale.z;
                }
                Quaternion ownRotation = (!Tools.rectBlueprintMode || !InternalEditorUtility.SupportsRectLayout(this.transform)) ? this.rotation : this.transform.parent.rotation;
                Quaternion refAlignment = this.GetRefAlignment(scaleRotation, ownRotation);
                scaleDelta = (Vector3) (refAlignment * scaleDelta);
                scaleDelta = Vector3.Scale(scaleDelta, (Vector3) (refAlignment * Vector3.one));
                if (preferRectResize)
                {
                    if (this.rectTransform != null)
                    {
                        Vector2 vector5 = (this.sizeDelta + Vector2.Scale(this.rect.size, scaleDelta)) - this.rect.size;
                        vector5.x = MathUtils.RoundBasedOnMinimumDifference(vector5.x, minDragDifference.x);
                        vector5.y = MathUtils.RoundBasedOnMinimumDifference(vector5.y, minDragDifference.y);
                        this.rectTransform.sizeDelta = vector5;
                        if (this.rectTransform.drivenByObject != null)
                        {
                            RectTransform.SendReapplyDrivenProperties(this.rectTransform);
                        }
                        return;
                    }
                    SpriteRenderer component = this.transform.GetComponent<SpriteRenderer>();
                    if ((component != null) && (component.drawMode != SpriteDrawMode.Simple))
                    {
                        component.size = Vector2.Scale(this.sizeDelta, scaleDelta);
                        return;
                    }
                }
                this.SetScaleValue(Vector3.Scale(this.scale, scaleDelta));
            }

            private void SetPosition(Vector3 newPosition)
            {
                this.SetPositionDelta(newPosition - this.position);
            }

            public void SetPositionDelta(Vector3 positionDelta)
            {
                Vector3 vector = positionDelta;
                Vector3 minDragDifference = ManipulationToolUtility.minDragDifference;
                if (this.transform.parent != null)
                {
                    vector = this.transform.parent.InverseTransformVector(vector);
                    minDragDifference.x /= this.transform.parent.lossyScale.x;
                    minDragDifference.y /= this.transform.parent.lossyScale.y;
                    minDragDifference.z /= this.transform.parent.lossyScale.z;
                }
                bool flag = Mathf.Approximately(vector.x, 0f);
                bool flag2 = Mathf.Approximately(vector.y, 0f);
                bool flag3 = Mathf.Approximately(vector.z, 0f);
                if (this.rectTransform == null)
                {
                    Vector3 vector6 = this.localPosition + vector;
                    vector6.x = !flag ? MathUtils.RoundBasedOnMinimumDifference(vector6.x, minDragDifference.x) : this.localPosition.x;
                    vector6.y = !flag2 ? MathUtils.RoundBasedOnMinimumDifference(vector6.y, minDragDifference.y) : this.localPosition.y;
                    vector6.z = !flag3 ? MathUtils.RoundBasedOnMinimumDifference(vector6.z, minDragDifference.z) : this.localPosition.z;
                    this.transform.localPosition = vector6;
                }
                else
                {
                    Vector3 vector7 = this.localPosition + vector;
                    vector7.z = !flag3 ? MathUtils.RoundBasedOnMinimumDifference(vector7.z, minDragDifference.z) : this.localPosition.z;
                    this.transform.localPosition = vector7;
                    Vector2 vector8 = this.anchoredPosition + vector;
                    vector8.x = !flag ? MathUtils.RoundBasedOnMinimumDifference(vector8.x, minDragDifference.x) : this.anchoredPosition.x;
                    vector8.y = !flag2 ? MathUtils.RoundBasedOnMinimumDifference(vector8.y, minDragDifference.y) : this.anchoredPosition.y;
                    this.rectTransform.anchoredPosition = vector8;
                    if (this.rectTransform.drivenByObject != null)
                    {
                        RectTransform.SendReapplyDrivenProperties(this.rectTransform);
                    }
                }
            }

            public void DebugAlignment(Quaternion targetRotation)
            {
                Quaternion identity = Quaternion.identity;
                if (!TransformManipulator.individualSpace)
                {
                    identity = this.GetRefAlignment(targetRotation, this.rotation);
                }
                Vector3 position = this.transform.position;
                float num = HandleUtility.GetHandleSize(position) * 0.25f;
                Color color = Handles.color;
                Handles.color = Color.red;
                Vector3 vector = (Vector3) (((this.rotation * identity) * Vector3.right) * num);
                Handles.DrawLine(position - vector, position + vector);
                Handles.color = Color.green;
                vector = (Vector3) (((this.rotation * identity) * Vector3.up) * num);
                Handles.DrawLine(position - vector, position + vector);
                Handles.color = Color.blue;
                vector = (Vector3) (((this.rotation * identity) * Vector3.forward) * num);
                Handles.DrawLine(position - vector, position + vector);
                Handles.color = color;
            }

            static TransformData()
            {
                s_Alignments = new Quaternion[] { Quaternion.LookRotation(Vector3.right, Vector3.up), Quaternion.LookRotation(Vector3.right, Vector3.forward), Quaternion.LookRotation(Vector3.up, Vector3.forward), Quaternion.LookRotation(Vector3.up, Vector3.right), Quaternion.LookRotation(Vector3.forward, Vector3.right), Quaternion.LookRotation(Vector3.forward, Vector3.up) };
            }
        }
    }
}


namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CurveMenuManager
    {
        private CurveUpdater updater;

        public CurveMenuManager(CurveUpdater updater)
        {
            this.updater = updater;
        }

        public void AddTangentMenuItems(GenericMenu menu, List<KeyIdentifier> keyList)
        {
            bool flag = keyList.Count > 0;
            bool on = flag;
            bool flag3 = flag;
            bool flag4 = flag;
            bool flag5 = flag;
            bool flag6 = flag;
            bool flag7 = flag;
            bool flag8 = flag;
            bool flag9 = flag;
            bool flag10 = flag;
            bool flag11 = flag;
            bool flag12 = flag;
            foreach (KeyIdentifier identifier in keyList)
            {
                Keyframe key = identifier.keyframe;
                AnimationUtility.TangentMode keyLeftTangentMode = AnimationUtility.GetKeyLeftTangentMode(key);
                AnimationUtility.TangentMode keyRightTangentMode = AnimationUtility.GetKeyRightTangentMode(key);
                bool keyBroken = AnimationUtility.GetKeyBroken(key);
                if ((keyLeftTangentMode != AnimationUtility.TangentMode.ClampedAuto) || (keyRightTangentMode != AnimationUtility.TangentMode.ClampedAuto))
                {
                    on = false;
                }
                if ((keyLeftTangentMode != AnimationUtility.TangentMode.Auto) || (keyRightTangentMode != AnimationUtility.TangentMode.Auto))
                {
                    flag3 = false;
                }
                if ((keyBroken || (keyLeftTangentMode != AnimationUtility.TangentMode.Free)) || (keyRightTangentMode != AnimationUtility.TangentMode.Free))
                {
                    flag4 = false;
                }
                if ((keyBroken || (keyLeftTangentMode != AnimationUtility.TangentMode.Free)) || (((key.inTangent != 0f) || (keyRightTangentMode != AnimationUtility.TangentMode.Free)) || (key.outTangent != 0f)))
                {
                    flag5 = false;
                }
                if (!keyBroken)
                {
                    flag6 = false;
                }
                if (!keyBroken || (keyLeftTangentMode != AnimationUtility.TangentMode.Free))
                {
                    flag7 = false;
                }
                if (!keyBroken || (keyLeftTangentMode != AnimationUtility.TangentMode.Linear))
                {
                    flag8 = false;
                }
                if (!keyBroken || (keyLeftTangentMode != AnimationUtility.TangentMode.Constant))
                {
                    flag9 = false;
                }
                if (!keyBroken || (keyRightTangentMode != AnimationUtility.TangentMode.Free))
                {
                    flag10 = false;
                }
                if (!keyBroken || (keyRightTangentMode != AnimationUtility.TangentMode.Linear))
                {
                    flag11 = false;
                }
                if (!keyBroken || (keyRightTangentMode != AnimationUtility.TangentMode.Constant))
                {
                    flag12 = false;
                }
            }
            if (flag)
            {
                menu.AddItem(EditorGUIUtility.TextContent("Clamped Auto"), on, new GenericMenu.MenuFunction2(this.SetClampedAuto), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Auto"), flag3, new GenericMenu.MenuFunction2(this.SetAuto), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Free Smooth"), flag4, new GenericMenu.MenuFunction2(this.SetEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Flat"), flag5, new GenericMenu.MenuFunction2(this.SetFlat), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Broken"), flag6, new GenericMenu.MenuFunction2(this.SetBroken), keyList);
                menu.AddSeparator("");
                menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Free"), flag7, new GenericMenu.MenuFunction2(this.SetLeftEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Linear"), flag8, new GenericMenu.MenuFunction2(this.SetLeftLinear), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Constant"), flag9, new GenericMenu.MenuFunction2(this.SetLeftConstant), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Free"), flag10, new GenericMenu.MenuFunction2(this.SetRightEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Linear"), flag11, new GenericMenu.MenuFunction2(this.SetRightLinear), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Constant"), flag12, new GenericMenu.MenuFunction2(this.SetRightConstant), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Free"), flag10 && flag7, new GenericMenu.MenuFunction2(this.SetBothEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Linear"), flag11 && flag8, new GenericMenu.MenuFunction2(this.SetBothLinear), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Constant"), flag12 && flag9, new GenericMenu.MenuFunction2(this.SetBothConstant), keyList);
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Clamped Auto"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Auto"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Free Smooth"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Flat"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Broken"));
                menu.AddSeparator("");
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Free"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Linear"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Constant"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Free"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Linear"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Constant"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Free"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Linear"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Constant"));
            }
        }

        public void Flatten(List<KeyIdentifier> keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            foreach (KeyIdentifier identifier in keysToSet)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                key.inTangent = 0f;
                key.outTangent = 0f;
                curve.MoveKey(identifier.key, key);
                AnimationUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.curveId, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            this.updater.UpdateCurves(list, "Set Tangents");
        }

        public void SetAuto(object keysToSet)
        {
            this.SetBoth(AnimationUtility.TangentMode.Auto, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBoth(AnimationUtility.TangentMode mode, List<KeyIdentifier> keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            foreach (KeyIdentifier identifier in keysToSet)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                AnimationUtility.SetKeyBroken(ref key, false);
                AnimationUtility.SetKeyRightTangentMode(ref key, mode);
                AnimationUtility.SetKeyLeftTangentMode(ref key, mode);
                if (mode == AnimationUtility.TangentMode.Free)
                {
                    float num = CurveUtility.CalculateSmoothTangent(key);
                    key.inTangent = num;
                    key.outTangent = num;
                }
                curve.MoveKey(identifier.key, key);
                AnimationUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.curveId, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            this.updater.UpdateCurves(list, "Set Tangents");
        }

        public void SetBothConstant(object keysToSet)
        {
            this.SetTangent(2, AnimationUtility.TangentMode.Constant, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBothEditable(object keysToSet)
        {
            this.SetTangent(2, AnimationUtility.TangentMode.Free, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBothLinear(object keysToSet)
        {
            this.SetTangent(2, AnimationUtility.TangentMode.Linear, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBroken(object _keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            List<KeyIdentifier> list2 = (List<KeyIdentifier>) _keysToSet;
            foreach (KeyIdentifier identifier in list2)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                AnimationUtility.SetKeyBroken(ref key, true);
                if ((AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.ClampedAuto) || (AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Auto))
                {
                    AnimationUtility.SetKeyRightTangentMode(ref key, AnimationUtility.TangentMode.Free);
                }
                if ((AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.ClampedAuto) || (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Auto))
                {
                    AnimationUtility.SetKeyLeftTangentMode(ref key, AnimationUtility.TangentMode.Free);
                }
                curve.MoveKey(identifier.key, key);
                AnimationUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.curveId, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            this.updater.UpdateCurves(list, "Set Tangents");
        }

        public void SetClampedAuto(object keysToSet)
        {
            this.SetBoth(AnimationUtility.TangentMode.ClampedAuto, (List<KeyIdentifier>) keysToSet);
        }

        public void SetEditable(object keysToSet)
        {
            this.SetBoth(AnimationUtility.TangentMode.Free, (List<KeyIdentifier>) keysToSet);
        }

        public void SetFlat(object keysToSet)
        {
            this.SetBoth(AnimationUtility.TangentMode.Free, (List<KeyIdentifier>) keysToSet);
            this.Flatten((List<KeyIdentifier>) keysToSet);
        }

        public void SetLeftConstant(object keysToSet)
        {
            this.SetTangent(0, AnimationUtility.TangentMode.Constant, (List<KeyIdentifier>) keysToSet);
        }

        public void SetLeftEditable(object keysToSet)
        {
            this.SetTangent(0, AnimationUtility.TangentMode.Free, (List<KeyIdentifier>) keysToSet);
        }

        public void SetLeftLinear(object keysToSet)
        {
            this.SetTangent(0, AnimationUtility.TangentMode.Linear, (List<KeyIdentifier>) keysToSet);
        }

        public void SetRightConstant(object keysToSet)
        {
            this.SetTangent(1, AnimationUtility.TangentMode.Constant, (List<KeyIdentifier>) keysToSet);
        }

        public void SetRightEditable(object keysToSet)
        {
            this.SetTangent(1, AnimationUtility.TangentMode.Free, (List<KeyIdentifier>) keysToSet);
        }

        public void SetRightLinear(object keysToSet)
        {
            this.SetTangent(1, AnimationUtility.TangentMode.Linear, (List<KeyIdentifier>) keysToSet);
        }

        public void SetTangent(int leftRight, AnimationUtility.TangentMode mode, List<KeyIdentifier> keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            foreach (KeyIdentifier identifier in keysToSet)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                AnimationUtility.SetKeyBroken(ref key, true);
                if (leftRight == 2)
                {
                    AnimationUtility.SetKeyLeftTangentMode(ref key, mode);
                    AnimationUtility.SetKeyRightTangentMode(ref key, mode);
                }
                else if (leftRight == 0)
                {
                    AnimationUtility.SetKeyLeftTangentMode(ref key, mode);
                    if ((AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.ClampedAuto) || (AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Auto))
                    {
                        AnimationUtility.SetKeyRightTangentMode(ref key, AnimationUtility.TangentMode.Free);
                    }
                }
                else
                {
                    AnimationUtility.SetKeyRightTangentMode(ref key, mode);
                    if ((AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.ClampedAuto) || (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Auto))
                    {
                        AnimationUtility.SetKeyLeftTangentMode(ref key, AnimationUtility.TangentMode.Free);
                    }
                }
                if ((mode == AnimationUtility.TangentMode.Constant) && ((leftRight == 0) || (leftRight == 2)))
                {
                    key.inTangent = float.PositiveInfinity;
                }
                if ((mode == AnimationUtility.TangentMode.Constant) && ((leftRight == 1) || (leftRight == 2)))
                {
                    key.outTangent = float.PositiveInfinity;
                }
                curve.MoveKey(identifier.key, key);
                AnimationUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.curveId, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            this.updater.UpdateCurves(list, "Set Tangents");
        }
    }
}


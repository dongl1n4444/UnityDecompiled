namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal static class CurveUtility
    {
        private static Texture2D iconCurve;
        private static Texture2D iconKey;
        private static Texture2D iconNone;

        public static float CalculateSmoothTangent(Keyframe key)
        {
            if (key.inTangent == float.PositiveInfinity)
            {
                key.inTangent = 0f;
            }
            if (key.outTangent == float.PositiveInfinity)
            {
                key.outTangent = 0f;
            }
            return ((key.outTangent + key.inTangent) * 0.5f);
        }

        public static Color GetBalancedColor(Color c)
        {
            return new Color(0.15f + (0.75f * c.r), 0.2f + (0.6f * c.g), 0.1f + (0.9f * c.b));
        }

        public static string GetClipName(AnimationClip clip)
        {
            if (clip == null)
            {
                return "[No Clip]";
            }
            string name = clip.name;
            if ((clip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                name = name + " (Read-Only)";
            }
            return name;
        }

        public static Texture2D GetIconCurve()
        {
            if (iconCurve == null)
            {
                iconCurve = EditorGUIUtility.LoadIcon("animationanimated");
            }
            return iconCurve;
        }

        public static Texture2D GetIconKey()
        {
            if (iconKey == null)
            {
                iconKey = EditorGUIUtility.LoadIcon("animationkeyframe");
            }
            return iconKey;
        }

        public static int GetPathAndTypeID(string path, Type type)
        {
            return ((path.GetHashCode() * 0x1b) ^ type.GetHashCode());
        }

        public static Color GetPropertyColor(string name)
        {
            Color white = Color.white;
            int num = 0;
            if (name.StartsWith("m_LocalPosition"))
            {
                num = 1;
            }
            if (name.StartsWith("localEulerAngles"))
            {
                num = 2;
            }
            if (name.StartsWith("m_LocalScale"))
            {
                num = 3;
            }
            if (num == 1)
            {
                if (name.EndsWith(".x"))
                {
                    white = Handles.xAxisColor;
                }
                else if (name.EndsWith(".y"))
                {
                    white = Handles.yAxisColor;
                }
                else if (name.EndsWith(".z"))
                {
                    white = Handles.zAxisColor;
                }
            }
            else if (num == 2)
            {
                if (name.EndsWith(".x"))
                {
                    white = (Color) AnimEditor.kEulerXColor;
                }
                else if (name.EndsWith(".y"))
                {
                    white = (Color) AnimEditor.kEulerYColor;
                }
                else if (name.EndsWith(".z"))
                {
                    white = (Color) AnimEditor.kEulerZColor;
                }
            }
            else if (num == 3)
            {
                if (name.EndsWith(".x"))
                {
                    white = GetBalancedColor(new Color(0.7f, 0.4f, 0.4f));
                }
                else if (name.EndsWith(".y"))
                {
                    white = GetBalancedColor(new Color(0.4f, 0.7f, 0.4f));
                }
                else if (name.EndsWith(".z"))
                {
                    white = GetBalancedColor(new Color(0.4f, 0.4f, 0.7f));
                }
            }
            else if (name.EndsWith(".x"))
            {
                white = Handles.xAxisColor;
            }
            else if (name.EndsWith(".y"))
            {
                white = Handles.yAxisColor;
            }
            else if (name.EndsWith(".z"))
            {
                white = Handles.zAxisColor;
            }
            else if (name.EndsWith(".w"))
            {
                white = new Color(1f, 0.5f, 0f);
            }
            else if (name.EndsWith(".r"))
            {
                white = GetBalancedColor(Color.red);
            }
            else if (name.EndsWith(".g"))
            {
                white = GetBalancedColor(Color.green);
            }
            else if (name.EndsWith(".b"))
            {
                white = GetBalancedColor(Color.blue);
            }
            else if (name.EndsWith(".a"))
            {
                white = GetBalancedColor(Color.yellow);
            }
            else if (name.EndsWith(".width"))
            {
                white = GetBalancedColor(Color.blue);
            }
            else if (name.EndsWith(".height"))
            {
                white = GetBalancedColor(Color.yellow);
            }
            else
            {
                float f = 6.283185f * (name.GetHashCode() % 0x3e8);
                f -= Mathf.Floor(f);
                white = GetBalancedColor(Color.HSVToRGB(f, 1f, 1f));
            }
            white.a = 1f;
            return white;
        }

        public static bool HaveKeysInRange(AnimationCurve curve, float beginTime, float endTime)
        {
            for (int i = curve.length - 1; i >= 0; i--)
            {
                Keyframe keyframe = curve[i];
                if (keyframe.time >= beginTime)
                {
                    Keyframe keyframe2 = curve[i];
                    if (keyframe2.time < endTime)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void RemoveKeysInRange(AnimationCurve curve, float beginTime, float endTime)
        {
            for (int i = curve.length - 1; i >= 0; i--)
            {
                Keyframe keyframe = curve[i];
                if (keyframe.time >= beginTime)
                {
                    Keyframe keyframe2 = curve[i];
                    if (keyframe2.time < endTime)
                    {
                        curve.RemoveKey(i);
                    }
                }
            }
        }

        public static void SetKeyModeFromContext(AnimationCurve curve, int keyIndex)
        {
            Keyframe key = curve[keyIndex];
            bool broken = false;
            bool flag2 = false;
            if (keyIndex > 0)
            {
                if (AnimationUtility.GetKeyBroken(curve[keyIndex - 1]))
                {
                    broken = true;
                }
                if (AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]) == AnimationUtility.TangentMode.ClampedAuto)
                {
                    flag2 = true;
                }
            }
            if (keyIndex < (curve.length - 1))
            {
                if (AnimationUtility.GetKeyBroken(curve[keyIndex + 1]))
                {
                    broken = true;
                }
                if (AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]) == AnimationUtility.TangentMode.ClampedAuto)
                {
                    flag2 = true;
                }
            }
            AnimationUtility.SetKeyBroken(ref key, broken);
            if (broken && !flag2)
            {
                if (keyIndex > 0)
                {
                    AnimationUtility.SetKeyLeftTangentMode(ref key, AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]));
                }
                if (keyIndex < (curve.length - 1))
                {
                    AnimationUtility.SetKeyRightTangentMode(ref key, AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]));
                }
            }
            else
            {
                AnimationUtility.TangentMode free = AnimationUtility.TangentMode.Free;
                if (((keyIndex == 0) || (AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]) == AnimationUtility.TangentMode.ClampedAuto)) && ((keyIndex == (curve.length - 1)) || (AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]) == AnimationUtility.TangentMode.ClampedAuto)))
                {
                    free = AnimationUtility.TangentMode.ClampedAuto;
                }
                else if (((keyIndex == 0) || (AnimationUtility.GetKeyRightTangentMode(curve[keyIndex - 1]) == AnimationUtility.TangentMode.Auto)) && ((keyIndex == (curve.length - 1)) || (AnimationUtility.GetKeyLeftTangentMode(curve[keyIndex + 1]) == AnimationUtility.TangentMode.Auto)))
                {
                    free = AnimationUtility.TangentMode.Auto;
                }
                AnimationUtility.SetKeyLeftTangentMode(ref key, free);
                AnimationUtility.SetKeyRightTangentMode(ref key, free);
            }
            curve.MoveKey(keyIndex, key);
        }
    }
}


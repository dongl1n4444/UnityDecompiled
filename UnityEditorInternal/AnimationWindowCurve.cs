namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowCurve : IComparable<AnimationWindowCurve>
    {
        [CompilerGenerated]
        private static Comparison<AnimationWindowKeyframe> <>f__am$cache0;
        private EditorCurveBinding m_Binding;
        private AnimationClip m_Clip;
        public List<AnimationWindowKeyframe> m_Keyframes;
        private AnimationWindowSelectionItem m_SelectionBinding;
        private Type m_ValueType;
        public const float timeEpsilon = 1E-05f;

        public AnimationWindowCurve(AnimationClip clip, EditorCurveBinding binding, Type valueType)
        {
            binding = RotationCurveInterpolation.RemapAnimationBindingForRotationCurves(binding, clip);
            this.m_Binding = binding;
            this.m_ValueType = valueType;
            this.m_Clip = clip;
            this.LoadKeyframes(clip);
        }

        public void AddKeyframe(AnimationWindowKeyframe key, AnimationKeyTime keyTime)
        {
            this.RemoveKeyframe(keyTime);
            this.m_Keyframes.Add(key);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (a, b) => a.time.CompareTo(b.time);
            }
            this.m_Keyframes.Sort(<>f__am$cache0);
        }

        public int CompareTo(AnimationWindowCurve obj)
        {
            bool flag = this.path.Equals(obj.path);
            bool flag2 = obj.type == this.type;
            if (flag || (this.depth == obj.depth))
            {
                bool flag3 = ((this.type == typeof(Transform)) && (obj.type == typeof(Transform))) && flag;
                bool flag4 = ((this.type == typeof(Transform)) || (obj.type == typeof(Transform))) && flag;
                if (flag3)
                {
                    string nicePropertyGroupDisplayName = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(this.propertyName));
                    string str6 = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(obj.propertyName));
                    if (nicePropertyGroupDisplayName.Contains("Position") && str6.Contains("Rotation"))
                    {
                        return -1;
                    }
                    if (nicePropertyGroupDisplayName.Contains("Rotation") && str6.Contains("Position"))
                    {
                        return 1;
                    }
                }
                else if (flag4)
                {
                    if (this.type == typeof(Transform))
                    {
                        return -1;
                    }
                    return 1;
                }
                if (flag && flag2)
                {
                    int componentIndex = AnimationWindowUtility.GetComponentIndex(obj.propertyName);
                    int num6 = AnimationWindowUtility.GetComponentIndex(this.propertyName);
                    if (((componentIndex != -1) && (num6 != -1)) && (this.propertyName.Substring(0, this.propertyName.Length - 2) == obj.propertyName.Substring(0, obj.propertyName.Length - 2)))
                    {
                        return (num6 - componentIndex);
                    }
                }
                return (this.path + this.type + this.propertyName).CompareTo(obj.path + obj.type + obj.propertyName);
            }
            int num = Math.Min(this.path.Length, obj.path.Length);
            int startIndex = 0;
            int num3 = 0;
            while (num3 < num)
            {
                if (this.path[num3] != obj.path[num3])
                {
                    break;
                }
                if (this.path[num3] == '/')
                {
                    startIndex = num3 + 1;
                }
                num3++;
            }
            if (num3 == num)
            {
                startIndex = num;
            }
            string str = this.path.Substring(startIndex);
            string str2 = obj.path.Substring(startIndex);
            if (string.IsNullOrEmpty(str))
            {
                return -1;
            }
            if (string.IsNullOrEmpty(str2))
            {
                return 1;
            }
            Regex regex = new Regex(@"^[^\/]*\/");
            Match match = regex.Match(str);
            string str3 = !match.Success ? str : match.Value.Substring(0, match.Value.Length - 1);
            Match match2 = regex.Match(str2);
            string strB = !match2.Success ? str2 : match2.Value.Substring(0, match2.Value.Length - 1);
            return str3.CompareTo(strB);
        }

        public object Evaluate(float time)
        {
            if (this.m_Keyframes.Count != 0)
            {
                AnimationWindowKeyframe keyframe = this.m_Keyframes[0];
                if (time <= keyframe.time)
                {
                    return keyframe.value;
                }
                AnimationWindowKeyframe keyframe2 = this.m_Keyframes[this.m_Keyframes.Count - 1];
                if (time >= keyframe2.time)
                {
                    return keyframe2.value;
                }
                AnimationWindowKeyframe keyframe3 = keyframe;
                for (int i = 1; i < this.m_Keyframes.Count; i++)
                {
                    AnimationWindowKeyframe keyframe4 = this.m_Keyframes[i];
                    if ((keyframe3.time < time) && (keyframe4.time >= time))
                    {
                        if (this.isPPtrCurve)
                        {
                            return keyframe3.value;
                        }
                        Keyframe keyframe5 = new Keyframe(keyframe3.time, (float) keyframe3.value, keyframe3.m_InTangent, keyframe3.m_OutTangent);
                        Keyframe keyframe6 = new Keyframe(keyframe4.time, (float) keyframe4.value, keyframe4.m_InTangent, keyframe4.m_OutTangent);
                        AnimationCurve curve = new AnimationCurve();
                        curve.keys = new Keyframe[] { keyframe5, keyframe6 };
                        return curve.Evaluate(time);
                    }
                    keyframe3 = keyframe4;
                }
            }
            return null;
        }

        public AnimationWindowKeyframe FindKeyAtTime(AnimationKeyTime keyTime)
        {
            int keyframeIndex = this.GetKeyframeIndex(keyTime);
            if (keyframeIndex == -1)
            {
                return null;
            }
            return this.m_Keyframes[keyframeIndex];
        }

        public override int GetHashCode()
        {
            int num = (this.clip != null) ? this.clip.GetInstanceID() : 0;
            return (((this.selectionID * 0x16a95) ^ (num * 0x4c93)) ^ this.binding.GetHashCode());
        }

        public int GetKeyframeIndex(AnimationKeyTime time)
        {
            for (int i = 0; i < this.m_Keyframes.Count; i++)
            {
                if (time.ContainsTime(this.m_Keyframes[i].time))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool HasKeyframe(AnimationKeyTime time)
        {
            return (this.GetKeyframeIndex(time) != -1);
        }

        public void LoadKeyframes(AnimationClip clip)
        {
            this.m_Keyframes = new List<AnimationWindowKeyframe>();
            if (!this.m_Binding.isPPtrCurve)
            {
                AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, this.binding);
                for (int i = 0; (editorCurve != null) && (i < editorCurve.length); i++)
                {
                    this.m_Keyframes.Add(new AnimationWindowKeyframe(this, editorCurve[i]));
                }
            }
            else
            {
                ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(clip, this.binding);
                for (int j = 0; (objectReferenceCurve != null) && (j < objectReferenceCurve.Length); j++)
                {
                    this.m_Keyframes.Add(new AnimationWindowKeyframe(this, objectReferenceCurve[j]));
                }
            }
        }

        public void RemoveKeyframe(AnimationKeyTime time)
        {
            for (int i = this.m_Keyframes.Count - 1; i >= 0; i--)
            {
                if (time.ContainsTime(this.m_Keyframes[i].time))
                {
                    this.m_Keyframes.RemoveAt(i);
                }
            }
        }

        public void RemoveKeysAtRange(float startTime, float endTime)
        {
            for (int i = this.m_Keyframes.Count - 1; i >= 0; i--)
            {
                if (Mathf.Approximately(endTime, this.m_Keyframes[i].time) || ((this.m_Keyframes[i].time > startTime) && (this.m_Keyframes[i].time < endTime)))
                {
                    this.m_Keyframes.RemoveAt(i);
                }
            }
        }

        public AnimationCurve ToAnimationCurve()
        {
            int count = this.m_Keyframes.Count;
            AnimationCurve curve = new AnimationCurve();
            List<Keyframe> list = new List<Keyframe>();
            float minValue = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                if (Mathf.Abs((float) (this.m_Keyframes[i].time - minValue)) > 1E-05f)
                {
                    Keyframe item = new Keyframe(this.m_Keyframes[i].time, (float) this.m_Keyframes[i].value, this.m_Keyframes[i].m_InTangent, this.m_Keyframes[i].m_OutTangent) {
                        tangentMode = this.m_Keyframes[i].m_TangentMode
                    };
                    list.Add(item);
                    minValue = this.m_Keyframes[i].time;
                }
            }
            curve.keys = list.ToArray();
            return curve;
        }

        public ObjectReferenceKeyframe[] ToObjectCurve()
        {
            int count = this.m_Keyframes.Count;
            List<ObjectReferenceKeyframe> list = new List<ObjectReferenceKeyframe>();
            float minValue = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                if (Mathf.Abs((float) (this.m_Keyframes[i].time - minValue)) > 1E-05f)
                {
                    ObjectReferenceKeyframe item = new ObjectReferenceKeyframe {
                        time = this.m_Keyframes[i].time,
                        value = (Object) this.m_Keyframes[i].value
                    };
                    minValue = item.time;
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        public bool animationIsEditable
        {
            get
            {
                return ((this.m_SelectionBinding == null) || this.m_SelectionBinding.animationIsEditable);
            }
        }

        public EditorCurveBinding binding
        {
            get
            {
                return this.m_Binding;
            }
        }

        public AnimationClip clip
        {
            get
            {
                return this.m_Clip;
            }
        }

        public bool clipIsEditable
        {
            get
            {
                return ((this.m_SelectionBinding == null) || this.m_SelectionBinding.clipIsEditable);
            }
        }

        public int depth
        {
            get
            {
                return ((this.path.Length <= 0) ? 0 : this.path.Split(new char[] { '/' }).Length);
            }
        }

        public bool isPhantom
        {
            get
            {
                return this.m_Binding.isPhantom;
            }
        }

        public bool isPPtrCurve
        {
            get
            {
                return this.m_Binding.isPPtrCurve;
            }
        }

        public int length
        {
            get
            {
                return this.m_Keyframes.Count;
            }
        }

        public string path
        {
            get
            {
                return this.m_Binding.path;
            }
        }

        public string propertyName
        {
            get
            {
                return this.m_Binding.propertyName;
            }
        }

        public GameObject rootGameObject
        {
            get
            {
                return ((this.m_SelectionBinding == null) ? null : this.m_SelectionBinding.rootGameObject);
            }
        }

        public ScriptableObject scriptableObject
        {
            get
            {
                return ((this.m_SelectionBinding == null) ? null : this.m_SelectionBinding.scriptableObject);
            }
        }

        public AnimationWindowSelectionItem selectionBinding
        {
            get
            {
                return this.m_SelectionBinding;
            }
            set
            {
                this.m_SelectionBinding = value;
            }
        }

        public int selectionID
        {
            get
            {
                return ((this.m_SelectionBinding == null) ? 0 : this.m_SelectionBinding.id);
            }
        }

        public float timeOffset
        {
            get
            {
                return ((this.m_SelectionBinding == null) ? 0f : this.m_SelectionBinding.timeOffset);
            }
        }

        public Type type
        {
            get
            {
                return this.m_Binding.type;
            }
        }

        public Type valueType
        {
            get
            {
                return this.m_ValueType;
            }
        }
    }
}


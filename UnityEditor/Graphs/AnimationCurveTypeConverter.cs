namespace UnityEditor.Graphs
{
    using System;
    using System.CodeDom;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;
    using UnityEngine;

    public class AnimationCurveTypeConverter : GraphsTypeConverter
    {
        private System.Type m_Type;

        public AnimationCurveTypeConverter(System.Type type)
        {
            this.m_Type = type;
        }

        private object AnimationCurveToCodeExpression(object value)
        {
            AnimationCurve curve = (AnimationCurve) value;
            StringBuilder builder = new StringBuilder();
            foreach (Keyframe keyframe in curve.keys)
            {
                if (builder.Length != 0)
                {
                    builder.Append(',');
                }
                object[] args = new object[] { InvStr(keyframe.time), InvStr(keyframe.value), InvStr(keyframe.inTangent), InvStr(keyframe.outTangent) };
                builder.AppendFormat("new UnityEngine.Keyframe({0}F, {1}F, {2}F, {3}F)", args);
            }
            return new CodeSnippetExpression(string.Format("new {0}(new UnityEngine.Keyframe[] {{{1}}}) {{postWrapMode = UnityEngine.WrapMode.{2}, preWrapMode = UnityEngine.WrapMode.{3}}}", new object[] { this.m_Type.FullName, builder, curve.postWrapMode, curve.preWrapMode }));
        }

        private static string AnimationCurveToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            AnimationCurve curve = (AnimationCurve) value;
            StringBuilder builder = new StringBuilder();
            EnumConverter converter = new EnumConverter(typeof(WrapMode));
            builder.AppendFormat("{0}\n{1}", converter.ConvertToString(curve.postWrapMode), converter.ConvertToString(curve.preWrapMode));
            foreach (Keyframe keyframe in curve.keys)
            {
                object[] args = new object[] { InvStr(keyframe.inTangent), InvStr(keyframe.outTangent), InvStr(keyframe.time), InvStr(keyframe.value) };
                builder.AppendFormat("\n{0}, {1}, {2}, {3}", args);
            }
            return builder.ToString();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            char[] separator = new char[] { '\n' };
            string[] strArray = ((string) value).Split(separator);
            Keyframe[] keys = new Keyframe[strArray.Length - 2];
            for (int i = 0; i < keys.Length; i++)
            {
                char[] chArray2 = new char[] { ',' };
                string[] keyvals = strArray[i + 2].Split(chArray2);
                keys[i] = new Keyframe(ParseKeyVal(keyvals, Val.Time), ParseKeyVal(keyvals, Val.Value), ParseKeyVal(keyvals, Val.InTangent), ParseKeyVal(keyvals, Val.OutTangent));
            }
            AnimationCurve curve = new AnimationCurve(keys);
            EnumConverter converter = new EnumConverter(typeof(WrapMode));
            curve.postWrapMode = (WrapMode) converter.ConvertFromString(strArray[0]);
            curve.preWrapMode = (WrapMode) converter.ConvertFromString(strArray[1]);
            return curve;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(CodeExpression))
            {
                return this.AnimationCurveToCodeExpression(value);
            }
            if (destinationType != typeof(string))
            {
                throw new ArgumentException("Can't convert to '" + destinationType.Name + "'");
            }
            return AnimationCurveToString(value);
        }

        private static string InvStr(float val)
        {
            return val.ToString(CultureInfo.InvariantCulture);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value != null)
            {
                if (value.GetType() == this.m_Type)
                {
                    return true;
                }
                if (value.GetType() != typeof(string))
                {
                    return false;
                }
                char[] separator = new char[] { '\n' };
                string[] strArray = ((string) value).Split(separator);
                if (strArray.Length < 2)
                {
                    return false;
                }
                EnumConverter converter = new EnumConverter(typeof(WrapMode));
                if (!converter.IsValid(strArray[0]))
                {
                    return false;
                }
                if (!converter.IsValid(strArray[1]))
                {
                    return false;
                }
                for (int i = 2; i < strArray.Length; i++)
                {
                    char[] chArray2 = new char[] { ',' };
                    string[] strArray2 = strArray[i].Split(chArray2);
                    if (strArray2.Length != 4)
                    {
                        return false;
                    }
                    foreach (string str in strArray2)
                    {
                        float num3;
                        if (!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out num3))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static float ParseKeyVal(string[] keyvals, Val val)
        {
            return float.Parse(keyvals[(int) val], CultureInfo.InvariantCulture);
        }

        private enum Val
        {
            InTangent,
            OutTangent,
            Time,
            Value
        }
    }
}


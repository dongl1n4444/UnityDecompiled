using System;
using System.CodeDom;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class AnimationCurveTypeConverter : GraphsTypeConverter
	{
		private enum Val
		{
			InTangent,
			OutTangent,
			Time,
			Value
		}

		private Type m_Type;

		public AnimationCurveTypeConverter(Type type)
		{
			this.m_Type = type;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			object result;
			if (destinationType == typeof(CodeExpression))
			{
				result = this.AnimationCurveToCodeExpression(value);
			}
			else
			{
				if (destinationType != typeof(string))
				{
					throw new ArgumentException("Can't convert to '" + destinationType.Name + "'");
				}
				result = AnimationCurveTypeConverter.AnimationCurveToString(value);
			}
			return result;
		}

		private object AnimationCurveToCodeExpression(object value)
		{
			AnimationCurve animationCurve = (AnimationCurve)value;
			StringBuilder stringBuilder = new StringBuilder();
			Keyframe[] keys = animationCurve.keys;
			for (int i = 0; i < keys.Length; i++)
			{
				Keyframe keyframe = keys[i];
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.AppendFormat("new UnityEngine.Keyframe({0}F, {1}F, {2}F, {3}F)", new object[]
				{
					AnimationCurveTypeConverter.InvStr(keyframe.time),
					AnimationCurveTypeConverter.InvStr(keyframe.value),
					AnimationCurveTypeConverter.InvStr(keyframe.inTangent),
					AnimationCurveTypeConverter.InvStr(keyframe.outTangent)
				});
			}
			return new CodeSnippetExpression(string.Format("new {0}(new UnityEngine.Keyframe[] {{{1}}}) {{postWrapMode = UnityEngine.WrapMode.{2}, preWrapMode = UnityEngine.WrapMode.{3}}}", new object[]
			{
				this.m_Type.FullName,
				stringBuilder,
				animationCurve.postWrapMode,
				animationCurve.preWrapMode
			}));
		}

		private static string AnimationCurveToString(object value)
		{
			string result;
			if (value == null)
			{
				result = string.Empty;
			}
			else
			{
				AnimationCurve animationCurve = (AnimationCurve)value;
				StringBuilder stringBuilder = new StringBuilder();
				EnumConverter enumConverter = new EnumConverter(typeof(WrapMode));
				stringBuilder.AppendFormat("{0}\n{1}", enumConverter.ConvertToString(animationCurve.postWrapMode), enumConverter.ConvertToString(animationCurve.preWrapMode));
				Keyframe[] keys = animationCurve.keys;
				for (int i = 0; i < keys.Length; i++)
				{
					Keyframe keyframe = keys[i];
					stringBuilder.AppendFormat("\n{0}, {1}, {2}, {3}", new object[]
					{
						AnimationCurveTypeConverter.InvStr(keyframe.inTangent),
						AnimationCurveTypeConverter.InvStr(keyframe.outTangent),
						AnimationCurveTypeConverter.InvStr(keyframe.time),
						AnimationCurveTypeConverter.InvStr(keyframe.value)
					});
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private static string InvStr(float val)
		{
			return val.ToString(CultureInfo.InvariantCulture);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string[] array = ((string)value).Split(new char[]
			{
				'\n'
			});
			Keyframe[] array2 = new Keyframe[array.Length - 2];
			for (int i = 0; i < array2.Length; i++)
			{
				string[] keyvals = array[i + 2].Split(new char[]
				{
					','
				});
				array2[i] = new Keyframe(AnimationCurveTypeConverter.ParseKeyVal(keyvals, AnimationCurveTypeConverter.Val.Time), AnimationCurveTypeConverter.ParseKeyVal(keyvals, AnimationCurveTypeConverter.Val.Value), AnimationCurveTypeConverter.ParseKeyVal(keyvals, AnimationCurveTypeConverter.Val.InTangent), AnimationCurveTypeConverter.ParseKeyVal(keyvals, AnimationCurveTypeConverter.Val.OutTangent));
			}
			AnimationCurve animationCurve = new AnimationCurve(array2);
			EnumConverter enumConverter = new EnumConverter(typeof(WrapMode));
			animationCurve.postWrapMode = (WrapMode)enumConverter.ConvertFromString(array[0]);
			animationCurve.preWrapMode = (WrapMode)enumConverter.ConvertFromString(array[1]);
			return animationCurve;
		}

		private static float ParseKeyVal(string[] keyvals, AnimationCurveTypeConverter.Val val)
		{
			return float.Parse(keyvals[(int)val], CultureInfo.InvariantCulture);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			bool result;
			if (value == null)
			{
				result = true;
			}
			else if (value.GetType() == this.m_Type)
			{
				result = true;
			}
			else if (value.GetType() != typeof(string))
			{
				result = false;
			}
			else
			{
				string[] array = ((string)value).Split(new char[]
				{
					'\n'
				});
				if (array.Length < 2)
				{
					result = false;
				}
				else
				{
					EnumConverter enumConverter = new EnumConverter(typeof(WrapMode));
					if (!enumConverter.IsValid(array[0]))
					{
						result = false;
					}
					else if (!enumConverter.IsValid(array[1]))
					{
						result = false;
					}
					else
					{
						for (int i = 2; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length != 4)
							{
								result = false;
								return result;
							}
							string[] array3 = array2;
							for (int j = 0; j < array3.Length; j++)
							{
								string s = array3[j];
								float num;
								if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
								{
									result = false;
									return result;
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}
	}
}

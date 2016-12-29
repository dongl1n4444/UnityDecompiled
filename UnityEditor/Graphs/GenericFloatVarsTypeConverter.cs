namespace UnityEditor.Graphs
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class GenericFloatVarsTypeConverter : GraphsTypeConverter
    {
        private Type m_Type;
        private string[] m_VarNames;

        public GenericFloatVarsTypeConverter(Type type, params string[] varNames)
        {
            this.m_Type = type;
            this.m_VarNames = varNames;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            char[] separator = new char[] { ',' };
            string[] strArray = ((string) value).Split(separator);
            object[] args = new object[this.m_VarNames.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = float.Parse(strArray[i], CultureInfo.InvariantCulture);
            }
            return Activator.CreateInstance(this.m_Type, args);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(CodeExpression))
            {
                return this.ConvertToCodeExpression(value);
            }
            return this.ConvertToString(value);
        }

        private object ConvertToCodeExpression(object target)
        {
            CodePrimitiveExpression[] parameters = new CodePrimitiveExpression[this.m_VarNames.Length];
            int num = 0;
            foreach (float num2 in this.GetAllValues(target))
            {
                parameters[num++] = new CodePrimitiveExpression(num2);
            }
            return new CodeObjectCreateExpression(this.m_Type, parameters);
        }

        private object ConvertToString(object target)
        {
            string str = string.Empty;
            foreach (float num in this.GetAllValues(target))
            {
                if (str.Length != 0)
                {
                    str = str + ",";
                }
                str = str + num.ToString(CultureInfo.InvariantCulture);
            }
            return str;
        }

        private IEnumerable<float> GetAllValues(object target)
        {
            <GetAllValues>c__AnonStorey0 storey = new <GetAllValues>c__AnonStorey0 {
                target = target,
                $this = this
            };
            return Enumerable.Select<string, float>(this.m_VarNames, new Func<string, float>(storey, (IntPtr) this.<>m__0));
        }

        private float GetVariableValue(string varName, object target)
        {
            FieldInfo field = this.m_Type.GetField(varName);
            if (field != null)
            {
                return (float) field.GetValue(target);
            }
            return (float) this.m_Type.GetProperty(varName).GetValue(target, null);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            <IsValid>c__AnonStorey1 storey = new <IsValid>c__AnonStorey1();
            if (value.GetType() == this.m_Type)
            {
                return true;
            }
            if (value.GetType() != typeof(string))
            {
                return false;
            }
            char[] separator = new char[] { ',' };
            string[] strArray = ((string) value).Split(separator);
            if (strArray.Length != this.m_VarNames.Length)
            {
                return false;
            }
            return Enumerable.All<string>(strArray, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        [CompilerGenerated]
        private sealed class <GetAllValues>c__AnonStorey0
        {
            internal GenericFloatVarsTypeConverter $this;
            internal object target;

            internal float <>m__0(string varName) => 
                this.$this.GetVariableValue(varName, this.target);
        }

        [CompilerGenerated]
        private sealed class <IsValid>c__AnonStorey1
        {
            internal float dummy;

            internal bool <>m__0(string t) => 
                float.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out this.dummy);
        }
    }
}


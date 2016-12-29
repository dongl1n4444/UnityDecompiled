namespace UnityScript.TypeSystem
{
    using Boo.Lang;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Services;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Serializable]
    public class UnityScriptEntityFormatter : EntityFormatter
    {
        public string FormatCallableType(ICallableType type)
        {
            $FormatCallableType$locals$274 s$ = new $FormatCallableType$locals$274 {
                $signature = type.GetSignature()
            };
            string str = Builtins.join(new $FormatCallableType$335(this, s$), ", ");
            string str2 = this.FormatType(s$.$signature.get_ReturnType());
            return new StringBuilder("function(").Append(str).Append("): ").Append(str2).ToString();
        }

        public override string FormatGenericArguments(IEnumerable<string> genericArgs) => 
            new StringBuilder(".<").Append(Builtins.join(genericArgs, ", ")).Append(">").ToString();

        public override string FormatType(IType type)
        {
            IType type4;
            int num;
            IType type2 = type;
            if (type2 is IArrayType)
            {
                IArrayType type3;
                IArrayType type1 = type3 = type2;
                if (1 != 0)
                {
                    IType type5 = type4 = type3.get_ElementType();
                    if (1 != 0)
                    {
                        int num1 = num = type3.get_Rank();
                    }
                }
            }
            return ((1 == 0) ? (!(type2 is ICallableType) ? base.FormatType(type) : this.FormatCallableType((ICallableType) type)) : new StringBuilder().Append(this.FormatType(type4)).Append("[").Append("," * (num - 1)).Append("]").ToString());
        }

        [Serializable, CompilerGenerated]
        internal sealed class $FormatCallableType$335 : GenericGenerator<string>
        {
            internal UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$339;
            internal UnityScriptEntityFormatter $this$338;

            public $FormatCallableType$335(UnityScriptEntityFormatter $this$338, UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$339)
            {
                this.$this$338 = $this$338;
                this.$$locals$339 = $$locals$339;
            }

            public override IEnumerator<string> GetEnumerator() => 
                new Enumerator(this.$this$338, this.$$locals$339);

            [Serializable]
            internal class Enumerator : IEnumerator<string>, IDisposable, ICloneable
            {
                protected string $$current;
                protected IEnumerator<IParameter> $$enumerator;
                internal UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$337;
                internal UnityScriptEntityFormatter $this$336;

                public Enumerator(UnityScriptEntityFormatter $this$336, UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$337)
                {
                    this.$this$336 = $this$336;
                    this.$$locals$337 = $$locals$337;
                    this.Reset();
                }

                public override object Clone() => 
                    this.MemberwiseClone();

                public override void Dispose()
                {
                    this.$$enumerator.Dispose();
                }

                public override bool MoveNext() => 
                    this.$$enumerator.MoveNext();

                public override void Reset()
                {
                    this.$$enumerator = this.$$locals$337.$signature.get_Parameters().GetEnumerator();
                }

                public override string Current =>
                    this.$$current;

                public override object System.Collections.IEnumerator.Current =>
                    this.$$current;
            }
        }

        [Serializable]
        internal class $FormatCallableType$locals$274
        {
            internal CallableSignature $signature;
        }
    }
}


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
            string str = Builtins.join(new $FormatCallableType$335(s$, this), ", ");
            string str2 = this.FormatType(s$.$signature.ReturnType);
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
                IArrayType type1 = type3 = (IArrayType) type2;
                if (1 != 0)
                {
                    IType type5 = type4 = type3.ElementType;
                    if (1 != 0)
                    {
                        int num1 = num = type3.Rank;
                    }
                }
            }
            return ((1 == 0) ? (!(type2 is ICallableType) ? base.FormatType(type) : this.FormatCallableType((ICallableType) type)) : new StringBuilder().Append(this.FormatType(type4)).Append("[").Append("," * (num - 1)).Append("]").ToString());
        }

        [Serializable, CompilerGenerated]
        internal sealed class $FormatCallableType$335 : GenericGenerator<string>
        {
            internal UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$338;
            internal UnityScriptEntityFormatter $this$339;

            public $FormatCallableType$335(UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$338, UnityScriptEntityFormatter $this$339)
            {
                this.$$locals$338 = $$locals$338;
                this.$this$339 = $this$339;
            }

            public override IEnumerator<string> GetEnumerator() => 
                new Enumerator(this.$$locals$338, this.$this$339);

            [Serializable]
            internal class Enumerator : IEnumerator<string>, IDisposable, ICloneable
            {
                protected string $$current;
                protected IEnumerator<IParameter> $$enumerator;
                internal UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$336;
                internal UnityScriptEntityFormatter $this$337;

                public Enumerator(UnityScriptEntityFormatter.$FormatCallableType$locals$274 $$locals$336, UnityScriptEntityFormatter $this$337)
                {
                    this.$$locals$336 = $$locals$336;
                    this.$this$337 = $this$337;
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
                    this.$$enumerator = this.$$locals$336.$signature.Parameters.GetEnumerator();
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


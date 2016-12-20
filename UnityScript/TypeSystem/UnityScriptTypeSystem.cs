namespace UnityScript.TypeSystem
{
    using Boo.Lang;
    using Boo.Lang.Compiler.TypeSystem;
    using System;
    using UnityScript;
    using UnityScript.Lang;

    [Serializable]
    public class UnityScriptTypeSystem : TypeSystemServices
    {
        protected IType _AbstractGenerator;
        protected IType _ScriptBaseType;
        [NonSerialized]
        public static readonly BuiltinFunction UnityScriptEval = new BuiltinFunction("eval", 6);
        [NonSerialized]
        public static readonly BuiltinFunction UnityScriptTypeof = new BuiltinFunction("typeof", 6);

        public UnityScriptTypeSystem()
        {
            this._ScriptBaseType = this.Map(this.UnityScriptParameters.ScriptBaseType);
            this._AbstractGenerator = this.Map(typeof(AbstractGenerator));
        }

        public override bool CanBeReachedByPromotion(IType expected, IType actual)
        {
            return (!base.CanBeReachedByPromotion(expected, actual) ? (!expected.get_IsEnum() ? (actual.get_IsEnum() && this.IsIntegerNumber(expected)) : this.IsIntegerNumber(actual)) : true);
        }

        public bool IsGenerator(IMethod method)
        {
            IType type = method.get_ReturnType();
            return ((type != base.IEnumeratorType) ? type.IsSubclassOf(this._AbstractGenerator) : true);
        }

        public bool IsScriptType(IType type)
        {
            return type.IsSubclassOf(this._ScriptBaseType);
        }

        public override void PrepareBuiltinFunctions()
        {
            base.PrepareBuiltinFunctions();
            this.AddBuiltin(UnityScriptEval);
            this.AddBuiltin(UnityScriptTypeof);
        }

        protected override void PreparePrimitives()
        {
            this.AddPrimitiveType("void", base.VoidType);
            this.AddPrimitiveType("boolean", base.BoolType);
            this.AddPrimitiveType("char", base.CharType);
            this.AddPrimitiveType("Date", base.DateTimeType);
            this.AddPrimitiveType("String", base.StringType);
            this.AddPrimitiveType("Object", base.ObjectType);
            this.AddPrimitiveType("Regex", base.RegexType);
            this.AddPrimitiveType("sbyte", base.SByteType);
            this.AddPrimitiveType("byte", base.ByteType);
            this.AddPrimitiveType("short", base.ShortType);
            this.AddPrimitiveType("ushort", base.UShortType);
            this.AddPrimitiveType("int", base.IntType);
            this.AddPrimitiveType("uint", base.UIntType);
            this.AddPrimitiveType("long", base.LongType);
            this.AddPrimitiveType("ulong", base.ULongType);
            this.AddPrimitiveType("Number", base.DoubleType);
            this.AddPrimitiveType("float", base.SingleType);
            this.AddPrimitiveType("double", base.DoubleType);
            this.AddPrimitiveType("Function", base.ICallableType);
            this.AddPrimitiveType("Array", this.Map(typeof(Array)));
        }

        public IType ScriptBaseType
        {
            get
            {
                return this._ScriptBaseType;
            }
        }

        public UnityScriptCompilerParameters UnityScriptParameters
        {
            get
            {
                return (UnityScriptCompilerParameters) this.get_Context().get_Parameters();
            }
        }
    }
}


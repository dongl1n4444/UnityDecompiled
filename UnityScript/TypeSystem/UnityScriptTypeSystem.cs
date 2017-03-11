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
        public static readonly BuiltinFunction UnityScriptEval = new BuiltinFunction("eval", BuiltinFunctionType.Custom);
        [NonSerialized]
        public static readonly BuiltinFunction UnityScriptTypeof = new BuiltinFunction("typeof", BuiltinFunctionType.Custom);

        public UnityScriptTypeSystem()
        {
            this._ScriptBaseType = this.Map(this.UnityScriptParameters.ScriptBaseType);
            this._AbstractGenerator = this.Map(typeof(AbstractGenerator));
        }

        public override bool CanBeReachedByPromotion(IType expected, IType actual) => 
            (!base.CanBeReachedByPromotion(expected, actual) ? (!expected.IsEnum ? (actual.IsEnum && this.IsIntegerNumber(expected)) : this.IsIntegerNumber(actual)) : true);

        public bool IsGenerator(IMethod method)
        {
            IType returnType = method.ReturnType;
            return ((returnType != base.IEnumeratorType) ? returnType.IsSubclassOf(this._AbstractGenerator) : true);
        }

        public bool IsScriptType(IType type) => 
            type.IsSubclassOf(this._ScriptBaseType);

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
            this.AddPrimitiveType("Array", this.Map(typeof(UnityScript.Lang.Array)));
        }

        public IType ScriptBaseType =>
            this._ScriptBaseType;

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) this.Context.Parameters);
    }
}


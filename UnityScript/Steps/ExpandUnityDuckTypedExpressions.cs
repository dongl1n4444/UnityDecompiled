namespace UnityScript.Steps
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using System;
    using UnityScript;
    using UnityScript.Core;
    using UnityScript.Lang;
    using UnityScript.TypeSystem;

    [Serializable]
    public class ExpandUnityDuckTypedExpressions : ExpandDuckTypedExpressions
    {
        private bool _expando;
        private IMethod UnityRuntimeServices_GetProperty;
        private IMethod UnityRuntimeServices_Invoke;

        public override void ExpandQuackInvocation(MethodInvocationExpression node)
        {
            this.ExpandQuackInvocation(node, this.UnityRuntimeServices_Invoke);
            node.Arguments.Add(this.CodeBuilder.CreateTypeofExpression(this.UnityScriptTypeSystem.ScriptBaseType));
        }

        public override IMethod GetGetPropertyMethod() => 
            this.UnityRuntimeServices_GetProperty;

        public override IMethod GetSetPropertyMethod() => 
            (this._expando ? this.ResolveUnityRuntimeMethod("SetProperty") : base.GetSetPropertyMethod());

        public override void Initialize(CompilerContext context)
        {
            base.Initialize(context);
            this.UnityRuntimeServices_Invoke = this.ResolveUnityRuntimeMethod("Invoke");
            this.UnityRuntimeServices_GetProperty = this.ResolveUnityRuntimeMethod("GetProperty");
        }

        public override void OnModule(Module module)
        {
            if (!this.UnityScriptParameters.Expando)
            {
            }
            this._expando = Pragmas.IsEnabledOn(module, "expando");
            bool strict = this.Parameters.Strict;
            try
            {
                this.Parameters.Strict = Pragmas.IsEnabledOn(module, "strict");
                base.OnModule(module);
            }
            finally
            {
                this.Parameters.Strict = strict;
            }
        }

        public IMethod ResolveUnityRuntimeMethod(string methodName) => 
            this.NameResolutionService.ResolveMethod(this.UnityRuntimeServicesType, methodName);

        public IType UnityRuntimeServicesType =>
            this.TypeSystemServices.Map(typeof(UnityRuntimeServices));

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) this.Parameters);

        public UnityScript.TypeSystem.UnityScriptTypeSystem UnityScriptTypeSystem =>
            ((UnityScript.TypeSystem.UnityScriptTypeSystem) this.TypeSystemServices);
    }
}


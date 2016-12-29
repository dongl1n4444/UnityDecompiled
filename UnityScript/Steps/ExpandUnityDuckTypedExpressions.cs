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
            node.get_Arguments().Add(this.get_CodeBuilder().CreateTypeofExpression(this.UnityScriptTypeSystem.ScriptBaseType));
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
            bool flag = this.get_Parameters().get_Strict();
            try
            {
                this.get_Parameters().set_Strict(Pragmas.IsEnabledOn(module, "strict"));
                base.OnModule(module);
            }
            finally
            {
                this.get_Parameters().set_Strict(flag);
            }
        }

        public IMethod ResolveUnityRuntimeMethod(string methodName) => 
            this.get_NameResolutionService().ResolveMethod(this.UnityRuntimeServicesType, methodName);

        public IType UnityRuntimeServicesType =>
            this.get_TypeSystemServices().Map(typeof(UnityRuntimeServices));

        public UnityScriptCompilerParameters UnityScriptParameters =>
            ((UnityScriptCompilerParameters) this.get_Parameters());

        public UnityScript.TypeSystem.UnityScriptTypeSystem UnityScriptTypeSystem =>
            ((UnityScript.TypeSystem.UnityScriptTypeSystem) this.get_TypeSystemServices());
    }
}


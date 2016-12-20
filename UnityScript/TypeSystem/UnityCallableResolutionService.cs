namespace UnityScript.TypeSystem
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using System;

    [Serializable]
    public class UnityCallableResolutionService : CallableResolutionService
    {
        public override bool CheckVarArgsParameter(IParameter[] parameters, ExpressionCollection args)
        {
            return (!UnityCallableResolutionServiceModule.IsArrayArgumentExplicitlyProvided(parameters, args) ? base.CheckVarArgsParameter(parameters, args) : true);
        }

        public override bool ShouldExpandArgs(IMethod method, ExpressionCollection args)
        {
            return (!UnityCallableResolutionServiceModule.IsArrayArgumentExplicitlyProvided(method.GetParameters(), args) ? base.ShouldExpandArgs(method, args) : false);
        }
    }
}


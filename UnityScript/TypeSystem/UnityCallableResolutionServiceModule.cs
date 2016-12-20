namespace UnityScript.TypeSystem
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Core;
    using Boo.Lang.Runtime;
    using System;
    using System.Runtime.CompilerServices;

    [CompilerGlobalScope]
    public sealed class UnityCallableResolutionServiceModule
    {
        private UnityCallableResolutionServiceModule()
        {
        }

        public static bool IsArrayArgumentExplicitlyProvided(IParameter[] parameters, ExpressionCollection args)
        {
            IType expressionType = TypeSystemServices.GetExpressionType(args.get_Item(-1));
            IType rhs = parameters[parameters.Length + -1].get_Type();
            if (!RuntimeServices.EqualityOperator(expressionType, rhs))
            {
            }
            return ((parameters.Length == args.Count) ? RuntimeServices.EqualityOperator(expressionType, EmptyArrayType.Default) : false);
        }
    }
}


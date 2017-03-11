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
            IType expressionType = TypeSystemServices.GetExpressionType(args[-1]);
            IType type = parameters[parameters.Length + -1].Type;
            if (!RuntimeServices.EqualityOperator(expressionType, type))
            {
            }
            return ((parameters.Length == args.Count) ? RuntimeServices.EqualityOperator(expressionType, EmptyArrayType.Default) : false);
        }
    }
}


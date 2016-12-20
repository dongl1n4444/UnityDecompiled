namespace UnityScript.Steps
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Runtime;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    [CompilerGlobalScope]
    public sealed class UtilitiesModule
    {
        private UtilitiesModule()
        {
        }

        public static IConstructor ConstructorTakingNArgumentsFor(IType type, int arguments)
        {
            foreach (IConstructor constructor in TypeSystemExtensions.GetConstructors(type))
            {
                if (constructor.GetParameters().Length == arguments)
                {
                    return constructor;
                }
            }
            throw new Exception(new StringBuilder("no constructor in ").Append(type).Append(" taking ").Append(arguments).Append(" arguments").ToString());
        }

        public static ClassDefinition GetScriptClass(CompilerContext context)
        {
            object obj1 = context.get_Item("ScriptClass");
            if (!(obj1 is ClassDefinition))
            {
            }
            return (ClassDefinition) RuntimeServices.Coerce(obj1, typeof(ClassDefinition));
        }

        public static bool IsPossibleStartCoroutineInvocationForm(MethodInvocationExpression node)
        {
            MethodInvocationExpression expression = node;
            if (expression is Node)
            {
                Node node2;
                MethodInvocationExpression expression1 = node2 = expression;
                if ((1 != 0) && ((node2.get_ParentNode() is ExpressionStatement) || (node2.get_ParentNode() is YieldStatement)))
                {
                }
            }
            return IsRhsOfAssignment(node);
        }

        public static bool IsRhsOfAssignment(Expression node)
        {
            Expression expression2;
            Node node2 = node.get_ParentNode();
            if (node2 is BinaryExpression)
            {
                BinaryExpression expression;
                BinaryExpression expression1 = expression = node2;
                if ((1 != 0) && (expression.get_Operator() == 15))
                {
                    Expression expression3 = expression2 = expression.get_Right();
                }
            }
            return ((1 != 0) && (expression2 == node));
        }

        public static void SetScriptClass(CompilerContext context, ClassDefinition klass)
        {
            context.set_Item("ScriptClass", klass);
        }
    }
}


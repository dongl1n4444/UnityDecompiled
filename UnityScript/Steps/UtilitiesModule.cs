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
            foreach (IConstructor constructor in type.GetConstructors())
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
            object obj1 = context["ScriptClass"];
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
                if ((1 != 0) && ((node2.ParentNode is ExpressionStatement) || (node2.ParentNode is YieldStatement)))
                {
                }
            }
            return IsRhsOfAssignment(node);
        }

        public static bool IsRhsOfAssignment(Expression node)
        {
            Expression expression2;
            Node parentNode = node.ParentNode;
            if (parentNode is BinaryExpression)
            {
                BinaryExpression expression;
                BinaryExpression expression1 = expression = (BinaryExpression) parentNode;
                if ((1 != 0) && (expression.Operator == BinaryOperatorType.Assign))
                {
                    Expression expression3 = expression2 = expression.Right;
                }
            }
            return ((1 != 0) && (expression2 == node));
        }

        public static void SetScriptClass(CompilerContext context, ClassDefinition klass)
        {
            context["ScriptClass"] = klass;
        }
    }
}


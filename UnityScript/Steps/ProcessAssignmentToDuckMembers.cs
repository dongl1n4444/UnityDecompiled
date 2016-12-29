namespace UnityScript.Steps
{
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Runtime;
    using CompilerGenerated;
    using System;
    using UnityScript.Lang;

    [Serializable]
    public class ProcessAssignmentToDuckMembers : ProcessAssignmentsToSpecialMembers
    {
        protected IMethod _propagateChanges;
        protected IConstructor _sliceValueTypeChangeConstructor;
        protected IConstructor _valueTypeChangeConstructor;
        protected IType _valueTypeChangeType;

        public MethodInvocationExpression CreateConstructorInvocation(IConstructor ctor, params Expression[] args)
        {
            MethodInvocationExpression expression = this.get_CodeBuilder().CreateConstructorInvocation(ctor);
            int index = 0;
            Expression[] expressionArray = args;
            int length = expressionArray.Length;
            while (index < length)
            {
                expression.get_Arguments().Add(expressionArray[index]);
                index++;
            }
            return expression;
        }

        public override void Initialize(CompilerContext context)
        {
            base.Initialize(context);
            Type type = typeof(UnityRuntimeServices.MemberValueTypeChange);
            this._valueTypeChangeConstructor = this.get_TypeSystemServices().Map(type.GetConstructors()[0]);
            this._valueTypeChangeType = this.get_TypeSystemServices().Map(typeof(UnityRuntimeServices.ValueTypeChange));
            Type type2 = typeof(UnityRuntimeServices.SliceValueTypeChange);
            this._sliceValueTypeChangeConstructor = this.get_TypeSystemServices().Map(type2.GetConstructors()[0]);
            this._propagateChanges = this.get_TypeSystemServices().Map(new __ProcessAssignmentToDuckMembers_Initialize$callable0$25_95__(UnityRuntimeServices.PropagateValueTypeChanges).Method);
        }

        public bool IsFieldReference(Node node) => 
            (0x40 == TypeSystemServices.GetEntity(node).get_EntityType());

        public override bool IsReadOnlyMember(MemberReferenceExpression node) => 
            (!this.IsSpecialMemberTarget(node) ? base.IsReadOnlyMember(node) : false);

        public override bool IsSpecialMemberTarget(Expression node) => 
            this.get_TypeSystemServices().IsQuackBuiltin(node);

        public override void PropagateChanges(MethodInvocationExpression eval, List chain)
        {
            ExpressionCollection expressions = new ExpressionCollection();
            foreach (object local1 in chain.Reversed)
            {
                if (!(local1 is ProcessAssignmentsToSpecialMembers.ChainItem))
                {
                }
                ProcessAssignmentsToSpecialMembers.ChainItem item = (ProcessAssignmentsToSpecialMembers.ChainItem) RuntimeServices.Coerce(local1, typeof(ProcessAssignmentsToSpecialMembers.ChainItem));
                if (item.Container is MethodInvocationExpression)
                {
                    break;
                }
                if (item.Container is SlicingExpression)
                {
                    SlicingExpression expression = item.Container;
                    Expression[] expressionArray1 = new Expression[] { expression.get_Target().CloneNode(), expression.get_Indices().get_Item(0).get_Begin().CloneNode(), this.get_CodeBuilder().CreateReference(item.Local) };
                    expressions.Add(this.CreateConstructorInvocation(this._sliceValueTypeChangeConstructor, expressionArray1));
                    break;
                }
                MemberReferenceExpression container = item.Container;
                Expression[] args = new Expression[] { container.get_Target().CloneNode(), this.get_CodeBuilder().CreateStringLiteral(container.get_Name()), this.get_CodeBuilder().CreateReference(item.Local) };
                expressions.Add(this.CreateConstructorInvocation(this._valueTypeChangeConstructor, args));
            }
            MethodInvocationExpression expression3 = this.get_CodeBuilder().CreateMethodInvocation(this._propagateChanges);
            IArrayType type = this._valueTypeChangeType.MakeArrayType(1);
            expression3.get_Arguments().Add(this.get_CodeBuilder().CreateArray(type, expressions));
            eval.get_Arguments().Add(expression3);
        }

        public override List WalkMemberChain(MemberReferenceExpression memberRef)
        {
            List list = new List();
            while (true)
            {
                MemberReferenceExpression node = memberRef.get_Target() as MemberReferenceExpression;
                if ((node == null) || (this.IsSpecialMemberTarget(node) && this.IsReadOnlyMember(node)))
                {
                    this.get_Warnings().Add(CompilerWarningFactory.AssignmentToTemporary(memberRef));
                    return null;
                }
                if (this.IsSpecialMemberTarget(node) && !this.IsFieldReference(node))
                {
                    list.Insert(0, new ProcessAssignmentsToSpecialMembers.ChainItem(node));
                }
                if ((node.get_Target() is MethodInvocationExpression) || (node.get_Target() is SlicingExpression))
                {
                    list.Insert(0, new ProcessAssignmentsToSpecialMembers.ChainItem(node.get_Target()));
                    return list;
                }
                if (this.IsTerminalReferenceNode(node.get_Target()))
                {
                    return list;
                }
                memberRef = node;
            }
        }
    }
}


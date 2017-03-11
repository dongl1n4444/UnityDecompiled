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
            MethodInvocationExpression expression = this.CodeBuilder.CreateConstructorInvocation(ctor);
            int index = 0;
            Expression[] expressionArray = args;
            int length = expressionArray.Length;
            while (index < length)
            {
                expression.Arguments.Add(expressionArray[index]);
                index++;
            }
            return expression;
        }

        public override void Initialize(CompilerContext context)
        {
            base.Initialize(context);
            Type type = typeof(UnityRuntimeServices.MemberValueTypeChange);
            this._valueTypeChangeConstructor = this.TypeSystemServices.Map(type.GetConstructors()[0]);
            this._valueTypeChangeType = this.TypeSystemServices.Map(typeof(UnityRuntimeServices.ValueTypeChange));
            Type type2 = typeof(UnityRuntimeServices.SliceValueTypeChange);
            this._sliceValueTypeChangeConstructor = this.TypeSystemServices.Map(type2.GetConstructors()[0]);
            this._propagateChanges = this.TypeSystemServices.Map(new __ProcessAssignmentToDuckMembers_Initialize$callable0$25_95__(UnityRuntimeServices.PropagateValueTypeChanges).Method);
        }

        public bool IsFieldReference(Node node) => 
            (EntityType.Field == TypeSystemServices.GetEntity(node).EntityType);

        public override bool IsReadOnlyMember(MemberReferenceExpression node) => 
            (!this.IsSpecialMemberTarget(node) ? base.IsReadOnlyMember(node) : false);

        public override bool IsSpecialMemberTarget(Expression node) => 
            this.TypeSystemServices.IsQuackBuiltin(node);

        public override void PropagateChanges(MethodInvocationExpression eval, List chain)
        {
            ExpressionCollection items = new ExpressionCollection();
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
                    SlicingExpression expression = (SlicingExpression) item.Container;
                    Expression[] expressionArray1 = new Expression[] { expression.Target.CloneNode(), expression.Indices[0].Begin.CloneNode(), this.CodeBuilder.CreateReference(item.Local) };
                    items.Add(this.CreateConstructorInvocation(this._sliceValueTypeChangeConstructor, expressionArray1));
                    break;
                }
                MemberReferenceExpression container = (MemberReferenceExpression) item.Container;
                Expression[] args = new Expression[] { container.Target.CloneNode(), this.CodeBuilder.CreateStringLiteral(container.Name), this.CodeBuilder.CreateReference(item.Local) };
                items.Add(this.CreateConstructorInvocation(this._valueTypeChangeConstructor, args));
            }
            MethodInvocationExpression expression3 = this.CodeBuilder.CreateMethodInvocation(this._propagateChanges);
            IArrayType arrayType = this._valueTypeChangeType.MakeArrayType(1);
            expression3.Arguments.Add(this.CodeBuilder.CreateArray(arrayType, items));
            eval.Arguments.Add(expression3);
        }

        public override List WalkMemberChain(MemberReferenceExpression memberRef)
        {
            List list = new List();
            while (true)
            {
                MemberReferenceExpression target = memberRef.Target as MemberReferenceExpression;
                if ((target == null) || (this.IsSpecialMemberTarget(target) && this.IsReadOnlyMember(target)))
                {
                    this.Warnings.Add(CompilerWarningFactory.AssignmentToTemporary(memberRef));
                    return null;
                }
                if (this.IsSpecialMemberTarget(target) && !this.IsFieldReference(target))
                {
                    list.Insert(0, new ProcessAssignmentsToSpecialMembers.ChainItem(target));
                }
                if ((target.Target is MethodInvocationExpression) || (target.Target is SlicingExpression))
                {
                    list.Insert(0, new ProcessAssignmentsToSpecialMembers.ChainItem(target.Target));
                    return list;
                }
                if (this.IsTerminalReferenceNode(target.Target))
                {
                    return list;
                }
                memberRef = target;
            }
        }
    }
}

